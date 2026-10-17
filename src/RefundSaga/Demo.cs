namespace RefundSaga;

public static class Demo
{
    private const string Sold = "sneakers"; // stock = 0

    public static void Run()
    {
        Show("1 stock ok", Order("shirt"));
        Show("2 sold out", SoldOut());
        Show("3 fails once", Order(Sold, Gateway.Down));
        Show("4 answer lost", Order(Sold, Gateway.LostResponse));
        Show("5 gateway down", Order(
            Sold, Gateway.Down, Gateway.Down, Gateway.Down));
    }

    private static World Order(string sku, params Gateway[] g)
    {
        var world = new World(g);
        world.Start("A-1", sku);
        world.Drain();
        return world;
    }

    // Stops once the refund is requested, before any answer.
    private static World SoldOut()
    {
        var world = new World();
        world.Start("A-1", Sold);
        while (world.Store.Get("A-1").Refund
               != RefundStatus.Pending)
            world.Step();

        Show("2 refund requested", world);
        world.Drain();
        return world;
    }

    private static void Show(string title, World world)
    {
        var o = world.Store.Get("A-1");
        Console.WriteLine($"[{title}]");
        Console.WriteLine(
            $"  payment={o.Payment} stock={o.Stock} " +
            $"refund={o.Refund}");
        Console.WriteLine(
            $"  attempts={o.RefundAttempts} " +
            $"refunds executed={world.Payments.RefundsExecuted}");
    }
}
