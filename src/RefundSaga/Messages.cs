namespace RefundSaga;

// Commands: ask a service to do something.
public record ChargePayment(string OrderId, decimal Amount);
public record ReserveStock(string OrderId, string Sku);
public record RefundPayment(
    string OrderId, string PaymentId, string RefundKey);

// Events: a service reports what it has already committed.
public record PaymentSucceeded(string OrderId, string PaymentId);
public record StockReserved(string OrderId);
public record StockRejected(string OrderId);
public record RefundSucceeded(string OrderId);
public record RefundFailed(string OrderId, string Reason);
