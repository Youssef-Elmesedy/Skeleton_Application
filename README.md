# 🛒 Skeleton E-Commerce API

ASP.NET Core 10 REST API following **Clean Architecture** (Domain → Application → Infrastructure → Presentation).

---

## 🏗️ Architecture

```
Skeleton.Domain          ← Entities, Business Rules, Interfaces
Skeleton.Application     ← CQRS (Commands/Queries), DTOs, Services, Mappings
Skeleton.Infrastructure  ← EF Core, Repositories, JWT, Auth Services
Skeleton                 ← Controllers, Middleware, Program.cs
```

---

## 🔐 Authentication & Authorization

### JWT Setup
Configure in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey":        "YourProductionSecretKey32CharsMin!",
    "Issuer":           "SkeletonEcommerceApi",
    "Audience":         "SkeletonEcommerceClients",
    "ExpiresInMinutes": "60"
  }
}
```

### Roles & Permissions

| Endpoint Group       | Admin | Employee | Customer           |
|----------------------|-------|----------|--------------------|
| **Products** — Read  | ✅    | ✅       | ✅ (anonymous)     |
| **Products** — Write | ✅    | ❌       | ❌                 |
| **Categories** — Read| ✅    | ✅       | ✅ (anonymous)     |
| **Categories** — Write| ✅   | ❌       | ❌                 |
| **Customers** — View | ✅    | ✅       | Own only           |
| **Customers** — Create| ✅   | ✅       | ❌                 |
| **Customers** — Update/Delete| ✅ | ❌  | ❌                 |
| **Cart**             | ✅    | ✅       | Own only           |
| **Orders** — View    | ✅    | ✅       | Own only           |
| **Orders** — Create  | ✅    | ✅       | Own only           |
| **Orders** — Status  | ✅    | ✅       | ❌                 |
| **Orders** — Cancel  | ✅    | ✅       | Own only           |
| **Discounts** — CRUD | ✅    | ❌       | ❌                 |
| **Discounts** — Validate| ✅ | ✅       | ✅                 |
| **Payments** — Process| ✅   | ✅       | ❌                 |
| **Payments** — Refund| ✅    | ❌       | ❌                 |
| **Customer Account** — View| ✅ | ✅   | Own only           |
| **Customer Account** — Debit/Payment| ✅ | ✅ | ❌         |
| **Reviews** — View   | ✅    | ✅       | ✅ (anonymous)     |
| **Reviews** — Create | ❌    | ❌       | ✅                 |
| **Reviews** — Delete | ✅    | ❌       | ❌                 |

---

## ⚡ Rate Limiting

| Policy        | Limit         | Applied To                        |
|---------------|---------------|-----------------------------------|
| **Global**    | 100 req/min   | All endpoints (per IP)            |
| **AuthPolicy**| 10 req/min    | `/api/auth/login` & `/register`   |
| **WritePolicy**| 30 req/min   | All POST / PUT / PATCH / DELETE   |

---

## 💰 CustomerAccount vs Discount Clarification

| Concept | Entity | Purpose |
|---------|--------|---------|
| **Product Discount** | `Product.Discount` (decimal) | Fixed price reduction on the product itself (e.g. sale price) |
| **Coupon Discount** | `Discount` entity | Promo codes (e.g. `SUMMER20`) applied at order checkout |
| **Customer Account Debit** | `CustomerAccount.AddDebit()` | Customer purchases on credit — balance increases |
| **Customer Account Payment** | `CustomerAccount.AddPayment()` | Customer pays off debt — balance decreases |

---

## 📦 Key Endpoints

### Auth
```
POST /api/auth/login     → LoginDto         → LoginResponseDto (JWT token)
POST /api/auth/register  → RegisterDto      → LoginResponseDto (JWT token)
```

### Customer Account
```
GET  /api/customeraccounts/{customerId}          → balance + transaction history
POST /api/customeraccounts/{customerId}/debit    → add debt (Admin/Employee only)
POST /api/customeraccounts/{customerId}/payment  → record payment (Admin/Employee only)
```

### Products
```
GET  /api/products               → all products (anonymous)
GET  /api/products/paged         → paginated (anonymous)
GET  /api/products/{id}          → by ID (anonymous)
GET  /api/products/search        → by name (anonymous)
GET  /api/products/by-category/{id}        → by category
GET  /api/products/grouped-by-category     → grouped + paginated
POST /api/products               → create (Admin only)
PUT  /api/products               → update (Admin only)
DELETE /api/products/{id}        → delete (Admin only)
```

### Orders
```
GET  /api/orders                          → all (Admin/Employee)
GET  /api/orders/paged                    → paginated
GET  /api/orders/{id}                     → by ID
GET  /api/orders/by-customer/{customerId} → customer orders
POST /api/orders/{customerId}             → create from cart
PATCH /api/orders/{id}/status            → update status (Admin/Employee)
DELETE /api/orders/{id}/cancel           → cancel order
```

---

## 🗄️ Database Setup

```bash
# In Package Manager Console (Skeleton.Infrastructure as startup)
Add-Migration Add_AppUser_And_Auth
Update-Database
```

The migration `20260313000000_Add_AppUser_And_Auth` creates the `AppUsers` table.

---

## 🚀 Running Locally

1. Set connection string in `appsettings.DataBase.json`
2. Change `JwtSettings.SecretKey` in `appsettings.json` to a strong secret (≥32 chars)
3. Run migrations
4. `dotnet run`
5. Open `https://localhost:{port}/swagger`
6. Click **Authorize** → enter `Bearer {your_token}`

---

## 📁 New Files Added in This Update

```
Skeleton.Domain/
  Entities/AppUser.cs                          ← Auth user entity
  Eunm/UserRole.cs                             ← Admin / Employee / Customer

Skeleton.Application/
  Feature/Auth/AuthDto/AuthDtos.cs             ← Login/Register DTOs
  Feature/CustomerAccount/
    AccountDto/CustomerAccountDtos.cs          ← Account + Transaction DTOs
    Commands/AddDebit/AddDebitCommand.cs       ← CQRS Command + Handler + Validator
    Commands/AddPayment/AddPaymentCommand.cs   ← CQRS Command + Handler + Validator
    Queries/GetAccount/GetAccountQuery.cs      ← CQRS Query + Handler
    Mapping/CustomerAccountMapping.cs          ← Mapster config
  Services/Interfaces/
    IAuthService.cs
    IJwtService.cs
    ICustomerAccountService.cs
  Interfaces/Queries/
    ICustomerAccountQueryRepository.cs

Skeleton.Infrastructure/
  Services/JwtService.cs                       ← JWT token generation
  Services/AuthService.cs                      ← Login + Register logic
  Implementation/Queries/
    CustomerAccountQueryRepository.cs          ← Balance + transaction history queries
  Persistence/Configurations/AppUserConfiguration.cs
  Migrations/20260313000000_Add_AppUser_And_Auth.cs

Skeleton/
  Controllers/AuthController.cs                ← POST /api/auth/login + /register
  Controllers/CustomerAccountsController.cs    ← Account management endpoints
  Middleware/GlobalExceptionMiddleware.cs      ← Updated: handles BusinessException + 429
```
