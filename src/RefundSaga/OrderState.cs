namespace RefundSaga;

public enum PaymentStatus { Pending, Paid }
public enum StockStatus { Pending, Reserved, Rejected }
public enum RefundStatus
{
    None, Pending, Confirmed, ManualReview
}

// One row per order: which steps passed and what comes next.
public class OrderState
{
    public required string OrderId { get; init; }
    public required string Sku { get; init; }
    public required decimal Amount { get; init; }
    public PaymentStatus Payment { get; set; }
    public StockStatus Stock { get; set; }
    public RefundStatus Refund { get; set; }
    public string? PaymentId { get; set; }
    public string? RefundKey { get; set; }
    public int RefundAttempts { get; set; }
}

public interface IOrderStore
{
    OrderState Get(string orderId);
    void Save(OrderState state);
}

// Stands in for the real table (PostgreSQL in the article).
public class InMemoryOrderStore : IOrderStore
{
    private readonly Dictionary<string, OrderState> _rows = new();

    public OrderState Get(string orderId) => _rows[orderId];

    public void Save(OrderState state) =>
        _rows[state.OrderId] = state;
}
