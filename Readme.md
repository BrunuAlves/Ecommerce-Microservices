# 🛒 Ecommerce Microservices

Sistema de e-commerce construído com arquitetura de microserviços utilizando .NET 9, Docker, RabbitMQ, JWT e API Gateway com Ocelot. Cada serviço é independente, escalável e seguro, com comunicação assíncrona entre vendas e estoque.

---

## 📦 Microserviços

| Serviço        | Descrição                                                                 |
|----------------|---------------------------------------------------------------------------|
| **AuthService**    | Autenticação de usuários com JWT e perfis (`buyer`, `seller`)             |
| **StockService**   | Cadastro e gerenciamento de produtos e estoque                            |
| **SalesService**   | Criação de pedidos com validação de estoque e envio de eventos via RabbitMQ |
| **APIGateway**     | Roteamento centralizado com autenticação e autorização via JWT            |
| **RabbitMQ**       | Broker de mensagens para comunicação entre serviços                       |

---

## 📚 Rotas dos microserviços

### 🔑 AuthService

| Método | Rota           | Descrição                                 | Acesso        |
|--------|----------------|-------------------------------------------|---------------|
| POST   | `/api/auth/login`  | Autentica usuário e retorna JWT           | Público

---

### 📦 StockService

| Método | Rota                       | Descrição                     | Acesso        |
|--------|----------------------------|-------------------------------|---------------|
| GET    | `/api/products`      | Lista todos os produtos       | Público
| POST   | `/api/products`      | Cadastra novo produto         | Privado
| GET    | `/api/products/{id}` | Consulta produto por ID       | Público
| PUT    | `/api/products/{id}` | Editar produto      | Privado
| DELETE | `/api/products/{id}` | Deletar produto       | Privado
| GET    | `/api/products/{id}/availability` | Consulta estoque do produto       | Público
| GET    | `/api/products/myproducts` | Consulta produtos do vendedor       | Privado

---

### 🛒 SalesService

| Método | Rota                   | Descrição                          | Acesso        |
|--------|------------------------|------------------------------------|---------------|
| GET    | `/api/orders`        | Lista pedidos                      | Privado
| POST   | `/api/orders`        | Cria pedido (valida estoque)       | Privado
| GET    | `/api/orders/{id}`   | Consulta pedido por ID             | Privado

---

## 🔐 Autenticação

- Login via `POST /auth/login`
- Retorna token JWT com perfil (`buyer` ou `seller`)
- Token deve ser enviado no header:  
  `Authorization: Bearer {token}`

---

## 🧪 Testes

Os testes unitários estão organizados por microserviço. Para executá-los:

```bash
dotnet test SalesService.Tests
dotnet test StockService.Tests
```

---

## 🗃️ Migrations com Entity Framework

Para aplicar ou atualizar o banco de dados com migrations:
- Criar uma nova migration (exemplo para StockService):

```bash
cd StockService
dotnet ef migrations add InitialCreate
```

- Aplicar a migration no banco de dados:
```bash
dotnet ef database update
```

Certifique-se de que o pacote Microsoft.EntityFrameworkCore.Tools está instalado e que o DbContext está configurado corretamente.

---

## 🛠 Tecnologias utilizadas

- ASP.NET Core 9
- Docker & Docker Compose
- RabbitMQ
- Ocelot API Gateway
- JWT Authentication
- Swagger
- FluentValidation
- xUnit
- Entity Framework Core

---

## 📈 Escalabilidade

O sistema foi projetado para permitir fácil adição de novos microserviços, como:
- PaymentService
- AnalyticsService
- NotificationService
Cada serviço pode ser containerizado e integrado ao gateway com autenticação e rotas específicas, mantendo a arquitetura desacoplada e extensível.

---

## 📁 Estrutura de pastas

```
Ecommerce-Microservices/
├── AuthService/
├── StockService/
├── SalesService/
├── APIGateway/
├── docker-compose.yml
└── README.md
```

---

## 🚀 Como rodar localmente

```bash
git clone https://github.com/BrunuAlves/Ecommerce-Microservices.git
cd Ecommerce-Microservices
docker-compose up --build
```

Acesse o gateway em http://localhost/swagger

---

## 👤 Autor
Bruno Alves
Desenvolvedor .NET | Microserviços | Arquitetura distribuída