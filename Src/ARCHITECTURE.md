# 🏗️ Skeleton E-Commerce — Architecture Reference

## Solution Structure

```
Skeleton.Domain          ← Pure domain: Entities, Enums, Exceptions, Business Rules, Interfaces
Skeleton.Application     ← Use cases: CQRS, DTOs, Service Interfaces, Validators, Mappings
Skeleton.Infrastructure  ← Implementations: EF Core, Repositories, Services, Gateways, Hubs
Skeleton                 ← Presentation: Controllers, Middleware, Program.cs
```

---

## Dependency Flow

```
Skeleton (API)
    ↓
Skeleton.Application       ← ONLY depends on Domain
    ↓
Skeleton.Domain            ← No external dependencies (pure C#)
    ↑
Skeleton.Infrastructure    ← Implements Application interfaces, depends on Domain + Application
```

**Rule**: Nothing in Domain references Application or Infrastructure.
**Rule**: Application references only Domain (no EF Core, no HTTP clients).

---

## CQRS Pattern

Every feature follows:
```
Feature/
  Commands/
    CreateX/
      CreateXCommand.cs       ← record + IRequest<Result<T>>
      CreateXCommandHandler.cs← IRequestHandler
      CreateXValidator.cs     ← AbstractValidator<CreateXCommand>
  Queries/
    GetX/
      GetXQuery.cs
      GetXQueryHandler.cs
  XDto/
    XDtos.cs                  ← Response + Input records
  Mapping/
    XMapping.cs               ← Mapster config
```

---

## 💳 Custom Payment Flow

```
Client                    API                       Gateway
  │                        │                           │
  │  POST /payments/initiate│                           │
  │────────────────────────►│  CreatePaymentRecord()    │
  │                        │──────────────────────────►│
  │◄────────────────────────│  GatewayReferenceCode     │
  │                        │                           │
  │  [User pays on gateway] │                           │
  │                        │                           │
  │  POST /payments/confirm │                           │
  │────────────────────────►│  ConfirmAsync()           │
  │                        │──────────────────────────►│
  │                        │◄──────────────────────────│
  │◄────────────────────────│  PaymentResponseDto       │
  │                        │                           │
  │                        │◄── Webhook (async) ───────│
  │                        │  HandleWebhookAsync()      │
  │◄── SignalR notification │                           │
```

### Supported Gateways

| Gateway | Type | Notes |
|---------|------|-------|
| `Cash` | Instant | Auto-confirms. No external call. |
| `Stripe` | Card | Uses PaymentIntent. Replace stub with Stripe.net SDK. |
| `Fawry` | Kiosk (Egypt) | Generates ref code. Customer pays at Fawry outlet. |
| `BankTransfer` | Manual | Admin confirms after bank receipt. |
| `Installment` | Deferred | Splits amount across N months. |

Adding a new gateway:
1. Create `MyGatewayService : IPaymentGatewayService` in `Skeleton.Infrastructure/Payment/Gateways/`
2. Register in `ServiceRegistration.cs` as `IPaymentGatewayService`
3. Add value to `PaymentGateway` enum

---

## 🔔 Notification Architecture

```
Event occurs (order/payment)
        │
   INotificationService.CreateAsync()
        │
   ┌────┴────────────────────┐
   │  Save to DB             │  ← Persistent history
   │  SignalR push (real-time)│  ← Instant to connected clients
   │  (Email via Hangfire)   │  ← Async background job
   └─────────────────────────┘
```

### SignalR Events
| Event | Payload |
|-------|---------|
| `ReceiveNotification` | `NotificationResponseDto` |
| `NotificationMarkedRead` | `Guid` (notification ID) |
| `BroadcastAlert` | `{ title, message }` |

---

## 🔄 Background Jobs (Hangfire)

| Job ID | Schedule | What it does |
|--------|----------|-------------|
| `expire-stale-payments` | Every hour | Marks pending payments > 30 min as expired |
| `retry-failed-payments` | Every 5 min | Retries failed payments (max 3 attempts) |
| `check-low-stock` | Daily 2 AM | Sends low-stock alerts to admins |
| `cleanup-old-notifications` | Daily 3 AM | Deletes read notifications older than 90 days |
| `weekly-sales-report` | Monday 8 AM | Emails weekly revenue summary to admins |

Dashboard: `GET /hangfire`

---

## ⚡ Performance Strategy

| Layer | Technique |
|-------|-----------|
| **API** | ETag + 304 Not Modified (`PerformanceMiddleware`) |
| **API** | Response compression (`AddResponseCompression`) |
| **Cache L1** | In-memory (`IMemoryCache`) — sub-millisecond |
| **Cache L2** | Redis (`IDistributedCache`) — shared across nodes |
| **Cache Strategy** | `ICacheService.GetOrSetAsync()` — auto fallback L2→L1 |
| **DB** | `AsNoTracking()` on all read queries |
| **DB** | Projection to DTO in SQL (no entity loading) |
| **DB** | Indexes on: `Status`, `CustomerId`, `GatewayTransactionId` |
| **SignalR** | Redis backplane for horizontal scaling |
| **Jobs** | Hangfire fire-and-forget for emails (non-blocking) |

---

## 🛡️ Rate Limiting (3 layers)

```
Request
   │
   ├── Custom Sliding Window (RateLimitingMiddleware)
   │      100 req/min per IP (global)
   │      Returns X-RateLimit-* headers
   │
   ├── ASP.NET Named Policies (UseRateLimiter)
   │      AuthPolicy: 10 req/min  → /api/auth/*
   │      WritePolicy: 30 req/min → POST/PUT/DELETE
   │
   └── Controller
```

---

## 🌍 Localization

| Header | Language |
|--------|----------|
| `Accept-Language: en` | English (default) |
| `Accept-Language: ar` | Arabic |

Resources: `Skeleton/Resources/Messages.{culture}.resx`

Keys follow format: `{Module}.{Action}` e.g. `Auth.InvalidCredentials`, `Employee.NotFound`

---

## 🗄️ Database Migrations

| Migration | Description |
|-----------|-------------|
| `20260311_Intital_Data_Base_v1` | All original tables |
| `20260313_Add_AppUser_And_Auth` | AppUsers table |
| `20260314_Add_Employee_EmailVerification_PasswordReset` | Employees + AppUser new columns |
| `20260315_Add_Notifications_And_Payment_Flow` | Notifications + Payment extended columns |

Run: `Update-Database` in Package Manager Console (set `Skeleton.Infrastructure` as startup project).
