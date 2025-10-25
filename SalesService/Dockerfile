# SalesService/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os csproj necessários para aproveitar cache
#COPY ./Common/Common.csproj ./Common/
COPY ./SalesService/SalesService.csproj ./SalesService/

# Restaura somente o projeto (usa project references)
RUN dotnet restore ./SalesService/SalesService.csproj

# Copia todo o repositório (context = .)
COPY . .

# Publica o projeto
WORKDIR /src/SalesService
RUN dotnet publish "SalesService.csproj" -c Release -o /app/publish

# Stage runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SalesService.dll"]
