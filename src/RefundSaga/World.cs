namespace RefundSaga;

// Wires the services and delivers one message at a time.
public class World
{
    private readonly Bus _bus = new();
    private readonly OrderSaga _saga;
    private readonly Inventory _inventory;

    public World(params Gateway[] refundScript)
    {
        Store = new InMemoryOrderStore();
        Payments = new Payments(_bus, refundScript);
        _inventory = new Inventory(_bus, new()
        {
            ["shirt"] = 5,
            ["sneakers"] = 0
        });
        _saga = new OrderSaga(Store, _bus);
    }

    public IOrderStore Store { get; }
    public Payments Payments { get; }

    public void Start(string orderId, string sku) =>
        _saga.Start(orderId, sku, 49.90m);

    public bool Step()
    {
        if (!_bus.TryNext(out var message)) return false;

        switch (message)
        {
            case ChargePayment m: Payments.Handle(m); break;
            case RefundPayment m: Payments.Handle(m); break;
            case ReserveStock m: _inventory.Handle(m); break;
            case PaymentSucceeded m: _saga.On(m); break;
            case StockReserved m: _saga.On(m); break;
            case StockRejected m: _saga.On(m); break;
            case RefundSucceeded m: _saga.On(m); break;
            case RefundFailed m: _saga.On(m); break;
        }
        return true;
    }

    public void Drain()
    {
        while (Step()) { }
    }
}
