
# Messaging Queue Processor (.NET 9)

A simulated A2P (Application-to-Person) messaging platform built with .NET 9.  
Supports message queuing, processing, and monitoring via RESTful APIs and a simple dashboard.

---

##  Features Implemented

- Simulated in-memory message queue (producer-consumer pattern)
- Asynchronous processing using Task Parallel Library
- Message types supported: SMS, Email, Push Notification
- SQLite database with Dapper for persistence
- External API simulation with random delays and failures
- Retry logic for failed messages (up to 3 attempts)
- Dead Letter Queue for unrecoverable failures
- Serilog structured logging
- Real-time metrics tracking:
  - Queue Depth
  - Throughput (messages per 10 seconds)
  - Error Rate
- REST API for message submission, status check, and statistics
- Razor Pages Web UI for monitoring (queue, throughput, error rate)
- FluentValidation-based input validation
- Clean DI setup, layered architecture, and scoped/reusable services

---
##  Improvements With More Time

- Add Docker support for containerized deployment.
- Use Redis or RabbitMQ for a real message broker.
- Add authentication/authorization for API and dashboard.
- Add real external service integration (e.g., Twilio or SendGrid).
- Implement horizontal scalability via worker instances.

---

##  Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2022+ or VS Code
- SQLite (no installation needed; uses local `messages.db` file)

---
## Optional Components Implemented

The following optional components were implemented beyond the core requirements:

- **Recovery mechanism for failed messages**  
  Unprocessed messages (with statuses `Pending`, `Processing`, or `Retried`) are reloaded from the database and re-enqueued on application startup.

- **Simple dashboard page showing system status**  
  A Razor Pages-based frontend provides a dashboard UI showing:
  - Real-time queue depth
  - Throughput over the last 10 seconds
  - Error rate as a doughnut chart
  - Paginated list of all messages
  - A separate statistics page to filter grouped counts by message status

- **Retry policies with exponential backoff**  
  Messages that fail to send are retried with an increasing delay based on exponential backoff (`2^RetryCount` seconds), up to 3 attempts.

- **Dead letter queue for permanently failed messages**  
  Messages that exceed the retry limit are marked with a `DeadLetter` status and excluded from further processing.


---

## Bonus Points Implemented

- **Implementation of optional components beyond the minimum requirement**
   You implemented 4 optional components (Recovery, Retry with Backoff, Dead Letter Queue, UI Dashboard).

- **Message Prioritization**  
  Users can assign a priority (`High`, `Medium`, `Low`) to each message. The in-memory queue respects priority by processing high-priority messages first.

- **Simple Web UI for Monitoring**  
  A web-based frontend allows real-time observation of processing metrics and system behavior.
 ---
##  How to Run

```bash
# 1. Build and run the API
cd MessagingProcessor.API
dotnet run

# 2. Build and run the WebUI (Dashboard)
cd ../MessagingProcessor.WebUI
dotnet run

# 3. Open in browser:
# API: https://localhost:7263/swagger
```

---

##  API Endpoints

| Endpoint                         | Method | Description                             |
|----------------------------------|--------|-----------------------------------------|
| `/api/message`                   | POST   | Submit a new message                    |
| `/api/message/all`               | GET    | Retrieve all messages                   |
| `/api/message/statistics`        | GET    | Count of messages by type/status        |
| `/api/metrics/summary`           | GET    | Metrics: queue depth, throughput, error rate |

Example payload for submitting a message:

```json
{
  "type": "SMS",
  "recipient": "+1234567890",
  "content": "Hello world",
  "priority": "High"
}
```

---


##  Validation Rules

Implemented using FluentValidation:

- **SMS**: Recipient must be digits or start with `+`
- **Email**: Must follow email format
- **Push Notification**: Must have a valid device token format

Invalid requests return `400 Bad Request` with error details.

---

##  Testing

Unit tests written using **xUnit** and cover:

Tests are located in MessagingProcessor.Tests

- Message submission
- Retry/dead-letter logic
- Metrics updating
- Message recovery from database

Run tests with:

```bash
dotnet test
```

---

##  Assumptions

- SQLite is sufficient for simulating persistence and durability.
- Messages are processed sequentially with throttling, and no multithreading beyond async/await was required.
- Retry strategy is exponential backoff, max 3 retries.
- Prioritization uses a Priority field (1 = High, 2 = Medium, 3 = Low) and affects dequeue order.

---

##  Architecture

- Clean layered structure: Controller → Service → Repository
- Dapper for fast and simple DB access
- Serilog for structured logging (console and file)
- Scoped services and metrics tracking
- Separate Razor Pages UI project consuming API via `fetch()`
- Configuration via `appsettings.json` (e.g., `ApiBaseUrl`)

---

##  Author

Created as part of a .NET Developer Assessment.  
**Author**: Diellza Muçaj 
**Email**: muqajdiellza9@gmail.com 
**GitHub**: https://github.com/DiellzaMuqaj

