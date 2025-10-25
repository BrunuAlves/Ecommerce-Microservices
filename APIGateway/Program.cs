using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Carrega configuração de rotas
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 🔐 Autenticação JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://authservice:8080";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "AuthService",
            ValidateAudience = true,
            ValidAudience = "microservices",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "profile"
        };
    });

builder.Services.AddAuthorization();

// 🔁 Ocelot + Consul
builder.Services.AddOcelot(builder.Configuration).AddConsul();

// 📄 Swagger manual
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//Mostrar Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/sales/swagger/v1/swagger.json", "Sales Service v1");
        c.SwaggerEndpoint("/stock/swagger/v1/swagger.json", "Stock Service v1");
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service v1");
    });

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

await app.UseOcelot();

app.Run();