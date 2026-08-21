namespace Customer.Application.DTOs;

public record CustomerDto(Guid Id, string FullName, string Email, string PhoneNumber);
