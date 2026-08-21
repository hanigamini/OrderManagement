# سیستم مدیریت سفارشات - Microservices

اسکلت اولیه (scaffold) پروژه بر اساس سناریوی چالش، با Clean Architecture برای هر سه سرویس.

## نکته درباره‌ی نسخه C#

.NET 8 از C# 12 و .NET 9 از C# 13 استفاده می‌کند - نسخه‌ای به اسم "C# 10" مربوط به .NET 6 است.
در تمام پروژه‌ها `TargetFramework=net9.0` و `LangVersion=latest` تنظیم شده تا در Visual Studio 2026
جدیدترین قابلیت‌های زبان (primary constructors، collection expressions، required members و ...) در دسترس باشد.
اگر ترجیح می‌دهید روی .NET 8 کار کنید، فقط کافیست در `Directory.Build.props` مقدار `net9.0` را به `net8.0` تغییر دهید.

## پیش‌نیازها

1. Visual Studio 2026 با Workload های **ASP.NET and web development**
2. .NET 9 SDK (از dotnet.microsoft.com)
3. Docker Desktop (برای PostgreSQL, RabbitMQ, Keycloak)

## مراحل راه‌اندازی

### ۱. بالا آوردن زیرساخت با Docker

```bash
docker compose up -d
```

این دستور سه کانتینر بالا می‌آورد:
- **PostgreSQL** روی پورت 5432 (سه دیتابیس customerdb/orderdb/paymentdb به‌صورت خودکار ساخته می‌شوند)
- **RabbitMQ** روی پورت 5672 و پنل مدیریت روی http://localhost:15672 (guest/guest)
- **Keycloak** روی http://localhost:8080 (admin/admin)

### ۲. باز کردن Solution در Visual Studio 2026

فایل `OrderManagement.sln` را باز کنید. سه پروژه Api (`Customer.Api`, `Order.Api`, `Payment.Api`)
هرکدام باید روی پورت جداگانه اجرا شوند - در Solution Explorer روی Solution راست‌کلیک کنید،
"Configure Startup Projects" را انتخاب کرده و هر سه Api را به‌صورت Multiple startup projects فعال کنید.

### ۳. اجرای Migration های EF Core

برای هر سرویس، از Package Manager Console (با انتخاب Default project مربوطه):

```powershell
Add-Migration InitialCreate -Project Customer.Infrastructure -StartupProject Customer.Api
Update-Database -Project Customer.Infrastructure -StartupProject Customer.Api

Add-Migration InitialCreate -Project Order.Infrastructure -StartupProject Order.Api
Update-Database -Project Order.Infrastructure -StartupProject Order.Api

Add-Migration InitialCreate -Project Payment.Infrastructure -StartupProject Payment.Api
Update-Database -Project Payment.Infrastructure -StartupProject Payment.Api
```

یا از CLI (بعد از `dotnet tool install --global dotnet-ef`):

```bash
dotnet ef migrations add InitialCreate -p src/Services/Customer/Customer.Infrastructure -s src/Services/Customer/Customer.Api
dotnet ef database update -p src/Services/Customer/Customer.Infrastructure -s src/Services/Customer/Customer.Api
```
(همین کار را برای Order و Payment هم تکرار کنید.)

### ۴. اجرا و تست

هر سه سرویس Swagger UI دارند (در حالت Development):
- Customer.Api → `https://localhost:xxxx/swagger`
- Order.Api → `https://localhost:xxxx/swagger`
- Payment.Api → `https://localhost:xxxx/swagger`

**سناریوی تست:**
1. با `POST /api/customers` یک مشتری بسازید.
2. با `POST /api/orders` یک سفارش برای همان مشتری ثبت کنید (customerId دقیقاً باید برابر شناسه‌ی مشتری باشد چون در سند نمونه به‌صورت string فرض شده).
3. Order.Service به‌صورت خودکار `OrderCreatedEvent` را به RabbitMQ می‌فرستد.
4. Payment.Service این پیام را مصرف کرده، پرداخت را (به‌صورت شبیه‌سازی‌شده) موفق می‌کند و `PaymentProcessedEvent` را منتشر می‌کند.
5. Order.Service این پیام را مصرف کرده و وضعیت سفارش را به `Paid` تغییر می‌دهد.
6. با `GET /api/orders?customerId=...` وضعیت را بررسی کنید - باید `Paid` باشد.

پنل RabbitMQ (http://localhost:15672) را هم باز نگه دارید تا صف‌ها و پیام‌های رد و بدل شده را ببینید.

### ۵. Keycloak (اختیاری برای شروع)

خطوط `AddKeycloakWebApiAuthentication` و `UseAuthentication/UseAuthorization` در هر `Program.cs`
کامنت شده‌اند تا در قدم اول بتوانید بدون درگیری با Realm/Client، سرویس‌ها را تست کنید.
وقتی آماده بودید:
1. وارد http://localhost:8080 شوید و یک Realm به اسم `order-management` بسازید.
2. برای هر سرویس یک Client (`customer-service`, `order-service`, `payment-service`) بسازید.
3. کامنت‌های مربوط به Keycloak را در `Program.cs` هر سرویس بردارید.

## ساختار پروژه

```
src/
  BuildingBlocks/
    Shared.Contracts/        # Integration Events مشترک بین سرویس‌ها
  Services/
    Customer/
      Customer.Domain/       # Entity ها و قوانین دامنه
      Customer.Application/  # CQRS (Commands/Queries) + Interface ها
      Customer.Infrastructure/ # EF Core + PostgreSQL
      Customer.Api/           # Controllers + Program.cs
    Order/        (همان ساختار + MassTransit Publisher/Consumer)
    Payment/      (همان ساختار + MassTransit Consumer/Publisher)
```

## نکاتی که باید خودتان تکمیل کنید (فراتر از اسکلت اولیه)

- Validation ورودی (مثلاً با FluentValidation)
- مدیریت خطا و Middleware مرکزی (Exception Handling)
- Health Checks برای هر سرویس (`AddHealthChecks`)
- Dockerfile برای هر Api جهت containerize کردن کامل (فعلاً فقط زیرساخت‌ها Docker هستند)
- API Gateway (اختیاری، مثلاً YARP) در صورت نیاز به یک نقطه ورود واحد
- تست‌های واحد برای Application Layer (Command/Query Handler ها)
