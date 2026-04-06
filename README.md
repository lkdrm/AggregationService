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
## 🧠 Mandatory Design Decisions

### Why I Chose This Orchestration Approach

I chose a **pre-aggregation pattern via a background `BackgroundService`** combined with a **CQRS read model** rather than live fan-out on every request.

The Pricing service has a simulated latency of 500–800 ms and a 25% failure rate. If every API call triggered three parallel downstream requests, end-users would experience high tail latency and frequent degraded responses. By pre-aggregating data at startup (and in a real system — periodically), the API can serve requests from a local SQL read model in single-digit milliseconds regardless of downstream health.

MediatR was chosen to enforce the CQRS boundary cleanly: queries go through a typed pipeline with cross-cutting behaviors (logging, timing) without polluting the handler logic.

### Trade-offs

| Decision | Benefit | Cost |
|---|---|---|
| Pre-aggregated read model | Fast, consistent reads | Data can be slightly stale (eventual consistency) |
| In-memory EF Core DB | Zero infrastructure, easy local dev | Lost on restart; cannot scale horizontally |
| Background sync runs once at startup | Simple, no scheduler dependency | No periodic refresh; stale after first sync |
| Polly retry (5×) inside PricingClient | Hides transient failures automatically | Can add up to ~5s extra latency per product before fallback |
| Null fallback for missing price | API stays available during pricing outages | Clients receive incomplete data without explicit warning |

### What Would Change Under 10x Load

- **Replace in-memory DB with a real persistent store** (PostgreSQL / SQL Server) behind a connection pool — in-memory EF Core is single-process and loses state on restart.
- **Add a distributed cache** (Redis) in front of the repository to absorb read spikes without hitting the DB on every request.
- **Introduce periodic background refresh** (e.g., every 30 s) instead of a single startup sync, keeping the read model current under load.
- **Scale the API horizontally** (multiple Docker containers / Kubernetes pods) behind a load balancer — stateless design makes this straightforward.
- **Add OpenTelemetry** with distributed tracing (traces, metrics, spans) so bottlenecks across replicas are visible.
- **Rate-limit incoming requests** at the gateway level (e.g., YARP, API Management, or NGINX) to protect downstream mocks from thundering-herd scenarios.

### What I Intentionally Simplified

- **Mock HTTP clients** — `ProductClient`, `PricingClient`, and `StockClient` are in-process dictionaries rather than real HTTP calls, so no network is required to run the project.
- **In-memory database** — EF Core `UseInMemoryDatabase` avoids the need for a running SQL engine; a real deployment would use a persistent store with migrations.
- **Single startup sync** — the `BackgroundService` aggregates data once on startup instead of running on a schedule; a production version would use a `PeriodicTimer`.
- **Security (design only)** — no authentication or authorization middleware is wired up. The intended approach would be OAuth 2.0 / JWT bearer tokens, API-key rate limiting, and a WAF at the edge.
- **No distributed tracing** — correlation IDs are propagated via `CorrelationIdMiddleware`, but there is no OpenTelemetry / Jaeger / Zipkin integration.

---

## 💥 Failure Scenarios

### Scenario 1 — Pricing Service is Unavailable

**What happens:** The Pricing service throws exceptions on every call (e.g., 100% failure rate or network partition).

**How it is handled:**
1. Polly's `WaitAndRetryAsync` policy retries up to **5 times** with a 100 ms back-off, writing each attempt to the console.
2. A **5-second `CancellationTokenSource` timeout** wraps the entire pricing call in `ProductService`, so retries cannot run indefinitely.
3. On final failure the `catch` block logs a `LogWarning` and returns **`null` as a fallback price** — the rest of the aggregated product (name, image, stock) is still persisted and served.
4. Clients receive a valid `200 OK` response with `"price": null` instead of an error, keeping the service partially available.

### Scenario 2 — Product ID Not Found in the Read Model

**What happens:** A client requests `GET /api/products/999`, but product `999` was never synced into the read model (e.g., it doesn't exist in the mock data).

**How it is handled:**
1. `GetAggregatedProductHandler` calls `_productRepository.GetByIdAsync(id)`, which returns `null`.
2. The handler throws a `KeyNotFoundException` with a descriptive message.
3. `GlobalExceptionHandler` (registered via `IExceptionHandler`) catches the exception, maps it to **HTTP 404**, and writes a structured `ProblemDetails` JSON body that includes the `correlationId` from `HttpContext.Items` for easy log correlation.
4. The client receives a consistent error envelope rather than an unformatted 500.

---

## 🐳 Getting Started (Docker)
The easiest way to run the application is via Docker. No local .NET SDK is required.

Build the Docker Image:

Bash
docker build -t aggregation-api .
Run the Container:

Bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name my-api aggregation-api

## Access the API:
Open your browser and navigate to the Swagger UI:
👉 http://localhost:8080/swagger

Or if its local:
👉 http://localhost:XXXX/swagger

Where XXXX u can get from running console

## 🧪 Testing
The project includes comprehensive unit tests focusing on business logic and handler behaviors.
To run the tests locally:

Bash
dotnet test