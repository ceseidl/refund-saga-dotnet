namespace RefundSaga;

public class OrderSaga(IOrderStore store, Bus bus)
{
    private const int MaxRefundAttempts = 3;

    public void Start(string orderId, string sku, decimal amount)
    {
        store.Save(new OrderState
        {
            OrderId = orderId,
            Sku = sku,
            Amount = amount
        });
        bus.Send(new ChargePayment(orderId, amount));
    }

    public void On(PaymentSucceeded e)
    {
        var order = store.Get(e.OrderId);
        if (order.Payment == PaymentStatus.Paid) return;

        order.Payment = PaymentStatus.Paid;
        order.PaymentId = e.PaymentId;
        store.Save(order);
        bus.Send(new ReserveStock(order.OrderId, order.Sku));
    }

    public void On(StockReserved e)
    {
        var order = store.Get(e.OrderId);
        order.Stock = StockStatus.Reserved;
        store.Save(order);
    }

    public void On(StockRejected e)
    {
        var order = store.Get(e.OrderId);
        order.Stock = StockStatus.Rejected;
        order.Refund = RefundStatus.Pending;
        order.RefundKey = $"refund-{order.OrderId}";
        store.Save(order);
        SendRefund(order);
    }

    public void On(RefundSucceeded e)
    {
        var order = store.Get(e.OrderId);
        order.Refund = RefundStatus.Confirmed;
        store.Save(order);
    }

    public void On(RefundFailed e)
    {
        var order = store.Get(e.OrderId);
        if (order.Refund != RefundStatus.Pending) return;

        if (order.RefundAttempts >= MaxRefundAttempts)
        {
            order.Refund = RefundStatus.ManualReview;
            store.Save(order);
            return;
        }
        SendRefund(order);
    }

    // Same RefundKey on each attempt: no double refund.
    private void SendRefund(OrderState order)
    {
        order.RefundAttempts++;
        store.Save(order);
        bus.Send(new RefundPayment(
            order.OrderId, order.PaymentId!, order.RefundKey!));
    }
}
