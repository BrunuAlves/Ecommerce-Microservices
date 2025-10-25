using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StockService.Data;
using StockService.Repositories;
using StockService.Services;
using StockService.Validators;
using StockService.Models;
using StockService.Mappings;

var builder = WebApplication.CreateBuilder(args);

// 🌍 Ambiente atual
Console.WriteLine($"🌍 Ambiente atual: {builder.Environment.EnvironmentName}");

// 📦 Banco de dados
builder.Services.AddDbContext<StockDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("StockConnection")));

// 🔁 Repositórios e serviços
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddHostedService<RabbitMQConsumer>(); // Serviço de consumo RabbitMQ
builder.Services.AddSingleton<RabbitMQPublisher>(); // Serviço de publicação RabbitMQ

// 📖 Mapeamentos com AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ✅ Validação com FluentValidation
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();

// ✅ Controllers
builder.Services.AddControllers();

// 📚 Swagger + segurança
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock Service", Version = "v1" });

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Service v1");
        c.RoutePrefix = string.Empty;
    });
}

// 🔀 Rotas e controllers
app.MapControllers();

// 🛠️ Log de inicialização
Console.WriteLine("✅ Stock Service iniciado com sucesso!");

app.Run();