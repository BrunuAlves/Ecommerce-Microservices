using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesService.Models.DTOs;
using SalesService.Repositories;
using System.Text;
using System.Text.Json;

namespace SalesService.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMQConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<RabbitMQConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var settings = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = settings["HostName"],
                UserName = settings["Username"],
                Password = settings["Password"],
                Port = settings.GetValue<int>("Port")
            };

            // Retry com backoff exponencial
            const int maxRetries = 10;
            const int delaySeconds = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                    _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

                    _logger.LogInformation($"✅ Conectado ao RabbitMQ na tentativa {attempt}.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"⚠️ Tentativa {attempt}: falha ao conectar ao RabbitMQ: {ex.Message}");
                    if (attempt == maxRetries) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _channel.QueueDeclareAsync(
                queue: "order_confirmed",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<RabbitMQPublisher>();
                var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderEvent>(json);
                    if (orderEvent == null)
                    {
                        _logger.LogWarning("❌ Mensagem recebida inválida ou nula.");
                        return;
                    }

                    _logger.LogInformation($"📦 Pedido confirmado: ID [{orderEvent.Id}] com {orderEvent.Items.Length} item(ns).");
                    await ProcessOrderAsync(orderEvent, repo, _logger);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Erro ao processar pedido: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "order_confirmed",
                autoAck: true,
                consumer: consumer
            );

            await Task.Delay(-1, stoppingToken);
        }

        private static async Task ProcessOrderAsync(OrderEvent order, IOrderRepository repo, ILogger logger)
        {
            var updated = await repo.UpdateStatusAsync(order.Id, "Confirmado");

            if (updated)
                logger?.LogInformation($"✅ Pedido [{order.Id}] atualizado para status 'Confirmado'.");
            else
                logger?.LogWarning($"⚠️ Pedido [{order.Id}] não encontrado para atualização de status.");
        }
    }
}