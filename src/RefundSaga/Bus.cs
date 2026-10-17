namespace RefundSaga;

// A queue standing in for RabbitMQ: send now, handle later.
public class Bus
{
    private readonly Queue<object> _queue = new();

    public void Send(object message) => _queue.Enqueue(message);

    public bool TryNext(out object message) =>
        _queue.TryDequeue(out message!);
}
