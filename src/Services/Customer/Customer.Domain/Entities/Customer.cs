namespace Customer.Domain.Entities;

// Domain Model اصلی این سرویس (DDD)
public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }

    private Customer() { } // برای EF Core

    public Customer(string fullName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("نام مشتری نمی‌تواند خالی باشد.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("ایمیل نمی‌تواند خالی باشد.", nameof(email));

        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
