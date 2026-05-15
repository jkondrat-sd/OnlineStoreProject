namespace OrderService.Domain.ValueObjects;

public enum OrderStatus
{
    New,        // только что создан
    Pending,    // ожидает оплаты
    Paid,       // оплачен
    Cancelled   // отменен (неуспех оплаты)
}