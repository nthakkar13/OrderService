The OrderService is a transactional microservice built on .NET 10 using Clean Architecture to process and retrieve customer orders.

🏗️ Architecture Decisions
Structure: Clean Architecture is used (Domain, Application, Infrastructure, API) to enforce strict separation of concerns and maximize testability.

Workflow: The Strategy Pattern (IOrderProcessingStrategy) manages status-specific behavior (retries, delays, logging). This adheres to the OCP (Open/Closed Principle).

Request Handling: MediatR (ISender) implements CQRS (Command Query Responsibility Segregation) to decouple HTTP controllers from the business logic handlers.

Resilience: Polly provides exponential backoff and status-specific retry policies for HTTP calls to the NotificationService.

🔗 Integration Points
Notification Service

Technology: HTTP POST via Typed HttpClient.

Responsibility: Receives order status updates; calls are protected by Polly policies.

Kafka

Technology: Confluent.Kafka (IMessageProducer).

Responsibility: Publishes events (orders.created.<status>) for asynchronous downstream services.

Redis

Technology: StackExchange.Redis (ICacheService).

Responsibility: Implements a cache (in-memory or external) for read operations (GET /orders/{id}) with 5-minute expiry.

📝 Assumptions Made
Database: Persistent storage (e.g., SQL DB) is assumed but mocked/simplified in the code for focus.

External Endpoints: The Redis server runs on localhost:6379 and Kafka runs on localhost:9092 (as configured in appsettings.json).

Notifications: Transient HTTP failures are simulated within the MockNotificationController to test the Polly retry logic.

🚀 How to Run Locally
Prerequisites
.NET 8+ SDK

Docker (for running Redis).

Steps
Start Infrastructure: Run the Redis container using Docker.

Bash

docker compose up -d redis
Configure & Build: Ensure appsettings.json points to the correct localhost ports for Redis and Kafka.

Run API:

Bash

cd src/OrderService
dotnet run 
Test: Access the service at http://localhost:(the configured port) and use Swagger UI (/swagger) to verify functionality.
