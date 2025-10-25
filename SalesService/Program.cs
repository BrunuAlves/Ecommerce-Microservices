using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using SalesService.Data;
using SalesService.Repositories;
using SalesService.Services;
using SalesService.Models.DTOs;
using SalesService.Validators;
using SalesService.Mappings;

var builder = WebApplication.CreateBuilder(args);

// 🌍 Ambiente atual
Console.WriteLine($"🌍 Ambiente atual: {builder.Environment.EnvironmentName}");

// 📦 Banco de dados
builder.Services.AddDbContext<SalesDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SalesConnection")));

// 🔁 Repositórios e serviços
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddHttpClient<InventoryClient>(client =>
{
    client.BaseAddress = new Uri("http://stockservice:8080");
});

builder.Services.AddHostedService<RabbitMQConsumer>(); // Serviço de consumo RabbitMQ
builder.Services.AddSingleton<RabbitMQPublisher>();// Serviço de publicação RabbitMQ

// 📖 Mapeamentos com AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ✅ Validação com FluentValidation
builder.Services.AddScoped<IOrderValidator, OrderValidator>();
builder.Services.AddScoped<IValidator<OrderCreateDto>, OrderCreateValidator>();

// ✅ Controllers
builder.Services.AddControllers();

// 📚 Swagger + segurança
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sales Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 🚀 Inicializa o app
var app = builder.Build();

// 🧪 Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales Service v1");
        c.RoutePrefix = string.Empty;
    });
}
// 🔀 Rotas e controllers
app.MapControllers();

// 🛠️ Log de inicialização
Console.WriteLine("✅ Sales Service iniciado com sucesso!");

app.Run();