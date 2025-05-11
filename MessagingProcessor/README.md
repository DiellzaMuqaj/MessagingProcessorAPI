
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
- SQLite (no installation needed; uses local `.db` file)

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
# Dashboard: https://localhost:7185
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

##  Dashboard Overview

Built using Razor Pages and Bootstrap. Shows:

-  Real-time queue depth
-  Messages processed in the last 10 seconds
-  Current error rate
-  Message table with status and retry count
-  Form to send new messages

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
**GitHub**: [https://github.com/yourname]

