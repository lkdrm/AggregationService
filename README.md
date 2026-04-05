# 📦 Aggregation Service

A high-performance, resilient .NET API built to aggregate product data (details, pricing, and stock availability) from multiple external sources. 

This project demonstrates modern backend engineering practices, focusing on **Clean Architecture**, **CQRS**, and **Fault Tolerance**.

## 🚀 Key Features & Architecture

* **Clean Architecture:** Strict separation of concerns (Domain, Application, Infrastructure, UI/API).
* **CQRS Pattern (MediatR):** Read and write operations are strictly separated. The API reads instantly from an optimized SQL Read Model.
* **Background Processing:** A hosted `BackgroundService` asynchronously fetches data from external APIs and updates the Read Model without blocking user requests.
* **Resilience & Fallbacks:** Handles external service failures gracefully. If the Pricing Service goes down, the application logs the error, applies a fallback strategy (null pricing), and continues to serve available data.
* **Centralized Exception Handling:** Uses .NET `IExceptionHandler` and Problem Details for consistent API error responses.
* **Dockerized:** Fully containerized using a multi-stage Docker build for optimized image size and seamless deployment.

## 🛠️ Tech Stack

* **Framework:** .NET 10.0 / ASP.NET Core
* **Architecture:** Clean Architecture, CQRS
* **Libraries:** MediatR, Entity Framework Core (In-Memory/SQL)
* **DevOps:** Docker
* **Testing:** xUnit, Moq, FluentAssertions

## 📂 Project Structure

```text
AggregationService/
├── AggregationService.Domain/        # Enterprise logic, Entities, Value Objects
├── AggregationService.Application/   # Use Cases, CQRS Handlers, Interfaces
├── AggregationService.Infrastructure/# External API Clients, Background Services
├── AggregationService.Sql/           # EF Core DbContext, Repositories, Migrations
└── AggregationService/               # API Controllers, Middleware, DI Setup
```
## 🐳 Getting Started (Docker)
The easiest way to run the application is via Docker. No local .NET SDK is required.

Build the Docker Image:

Bash
docker build -t aggregation-api .
Run the Container:

Bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name my-api aggregation-api
Access the API:
Open your browser and navigate to the Swagger UI:
👉 http://localhost:8080/swagger

##🧪 Testing
The project includes comprehensive unit tests focusing on business logic and handler behaviors.
To run the tests locally:

Bash
dotnet test