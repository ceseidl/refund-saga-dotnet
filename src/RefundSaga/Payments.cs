namespace RefundSaga;

public enum Gateway { Ok, Down, LostResponse }

// Down: the refund never happens. LostResponse: it happens,
// but the answer never arrives. The script gives one per call.
public class Payments(Bus bus, params Gateway[] refundScript)
{
    private readonly Queue<Gateway> _script = new(refundScript);
    private readonly Dictionary<string, string> _refunds = new();

    public int RefundsExecuted => _refunds.Count;

    public void Handle(ChargePayment c) =>
        bus.Send(new PaymentSucceeded(
            c.OrderId, $"pay-{c.OrderId}"));

    public void Handle(RefundPayment r)
    {
        if (_refunds.ContainsKey(r.RefundKey))
        {
            bus.Send(new RefundSucceeded(r.OrderId));
            return;
        }

        var outcome = _script.Count > 0
            ? _script.Dequeue()
            : Gateway.Ok;

        if (outcome == Gateway.Down)
        {
            bus.Send(new RefundFailed(r.OrderId, "gateway down"));
            return;
        }

        _refunds[r.RefundKey] = r.PaymentId;
        object reply = outcome == Gateway.LostResponse
            ? new RefundFailed(r.OrderId, "timeout")
            : new RefundSucceeded(r.OrderId);
        bus.Send(reply);
    }
}
