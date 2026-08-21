# سیستم مدیریت سفارشات - Microservices

اسکلت اولیه (scaffold) پروژه بر اساس سناریوی چالش، با Clean Architecture برای هر سه سرویس.

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
