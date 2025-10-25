# APIGateway/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os csproj necessários para aproveitar cache
COPY ./APIGateway/APIGateway.csproj ./APIGateway/

# Restaura somente o projeto (usa project references)
RUN dotnet restore ./APIGateway/APIGateway.csproj

# Copia todo o repositório (context = .)
COPY . .

# Publica o projeto
WORKDIR /src/APIGateway
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish

# Stage runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.dll"]
