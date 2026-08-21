namespace Order.Application.DTOs;

public record OrderItemDto(string ProductId, int Quantity);

public record CreateOrderItemDto(string ProductId, int Quantity, decimal UnitPrice);

public record OrderDto(Guid OrderId, string Status, decimal TotalAmount);
