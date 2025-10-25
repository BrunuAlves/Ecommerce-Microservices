using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

// 🌍 Ambiente atual
Console.WriteLine($"🌍 Ambiente atual: {builder.Environment.EnvironmentName}");

// 🔁 Serviço de tokens JWT
builder.Services.AddSingleton<TokenService>();

// ✅ Controllers
builder.Services.AddControllers();

// 📚 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🚀 Inicializa o app
var app = builder.Build();

// 🧪 Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service v1");
        c.RoutePrefix = string.Empty;
    });
}

// 🔀 Rotas e controllers
app.MapControllers();

// 🛠️ Log de inicialização
Console.WriteLine("✅ Auth Service running...");

app.Run();