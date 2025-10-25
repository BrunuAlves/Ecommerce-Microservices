# StockService/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os csproj necessários para aproveitar cache
#COPY ./Common/Common.csproj ./Common/
COPY ./StockService/StockService.csproj ./StockService/

# Restaura somente o projeto (usa project references)
RUN dotnet restore ./StockService/StockService.csproj

# Copia todo o repositório (context = .)
COPY . .

# Publica o projeto
WORKDIR /src/StockService
RUN dotnet publish "StockService.csproj" -c Release -o /app/publish

# Stage runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StockService.dll"]
