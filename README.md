# Refund Saga in .NET: when the payment passed and the order failed

English | [Português](README.pt-BR.md)

[![CI](https://github.com/ceseidl/refund-saga-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/ceseidl/refund-saga-dotnet/actions/workflows/ci.yml) [![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> **Quick start**

```bash
dotnet run --project src/RefundSaga
```

Needs only the .NET 10 SDK. Nothing else to install: the broker, the payment gateway and the database are simulated in memory.

A runnable .NET 10 example of a **saga that handles a partial failure**: the payment is charged, the stock is rejected, and the saga has to refund. The refund is treated as new work that can fail too: it has its own status, its own retries with a stable idempotency key, and a "manual review" end state. A small console demo runs five scenarios.

## Projects

```
src/
└── RefundSaga/
    ├── Messages.cs     # commands and result events
    ├── OrderState.cs   # statuses, the order row and its store
    ├── OrderSaga.cs    # reacts to events, decides the next action
    ├── Payments.cs     # charge and idempotent refund (can fail)
    ├── Inventory.cs    # reserves stock or rejects
    ├── Bus.cs          # in-memory queue standing in for RabbitMQ
    ├── World.cs        # wires the services, delivers messages
    ├── Demo.cs         # the five scenarios
    └── Program.cs      # entry point
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Run

```bash
dotnet run --project src/RefundSaga
```

## Expected output

```
[1 stock ok]
  payment=Paid stock=Reserved refund=None
  attempts=0 refunds executed=0
[2 refund requested]
  payment=Paid stock=Rejected refund=Pending
  attempts=1 refunds executed=0
[2 sold out]
  payment=Paid stock=Rejected refund=Confirmed
  attempts=1 refunds executed=1
[3 fails once]
  payment=Paid stock=Rejected refund=Confirmed
  attempts=2 refunds executed=1
[4 answer lost]
  payment=Paid stock=Rejected refund=Confirmed
  attempts=2 refunds executed=1
[5 gateway down]
  payment=Paid stock=Rejected refund=ManualReview
  attempts=3 refunds executed=0
```

- `1 stock ok`: the happy path, no refund.
- `2 refund requested`: the refund was only requested. The status is `Pending`, not `Confirmed`, and nothing was refunded yet.
- `2 sold out`: the gateway answered, so the refund is `Confirmed`.
- `3 fails once`: the first attempt fails (gateway down); the retry uses the same key and works.
- `4 answer lost`: the refund happened, but the answer was lost. The retry carries the same key, so the gateway does not refund again (`refunds executed=1`).
- `5 gateway down`: three failed attempts end in `ManualReview`, keeping the payment id for a person to handle.

## Things worth knowing

- **Local transactions only.** Each service commits its own work; there is no global rollback. Undoing a step means a new action (the refund).
- **The saga state is persisted before the next command is sent.** Here it is an in-memory dictionary; a real system would use a table and handle duplicate messages with an inbox or the same idempotency idea.
- **The retry limit is a constant.** Production code would add a delay between attempts (exponential backoff) and a circuit breaker.
- **Broker and gateway are simulated.** The `Bus` delivers in order and never loses a message; a real broker needs acknowledgements, retries and a dead-letter queue.

## Links

- [Saga pattern (Microsoft Azure Architecture Center)](https://learn.microsoft.com/azure/architecture/patterns/saga)
- [Compensating Transaction pattern (Microsoft Azure Architecture Center)](https://learn.microsoft.com/azure/architecture/patterns/compensating-transaction)
- [Idempotent requests (Stripe API)](https://docs.stripe.com/api/idempotent_requests)

## License

[MIT](LICENSE). Author: Carlos Eduardo Seidl.
