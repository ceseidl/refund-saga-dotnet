namespace RefundSaga;

public class Inventory(Bus bus, Dictionary<string, int> stock)
{
    public void Handle(ReserveStock r)
    {
        if (stock.GetValueOrDefault(r.Sku) > 0)
        {
            stock[r.Sku]--;
            bus.Send(new StockReserved(r.OrderId));
        }
        else
        {
            bus.Send(new StockRejected(r.OrderId));
        }
    }
}
