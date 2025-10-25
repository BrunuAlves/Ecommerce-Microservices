using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using SalesService.Models.DTOs;

namespace SalesService.Services
{
    public class RabbitMQPublisher : IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            var rabbitMQSettings = configuration.GetSection("RabbitMQ");

            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings["HostName"],
                UserName = rabbitMQSettings["Username"],
                Password = rabbitMQSettings["Password"],
                Port = rabbitMQSettings.GetValue<int>("Port")
            };

            // Cria conexão e canal na inicialização
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task PublishOrderCreatedAsync(OrderEvent orderEvent)
        {
            // Garante que a fila existe
            await _channel.QueueDeclareAsync(
                queue: "order_created",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Serializa o evento e publica
            var json = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "order_created",
                body: body
            );
            
        }

        // Implementa IDisposable para fechar conexões
        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}
