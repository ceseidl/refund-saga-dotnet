# Saga com Estorno em .NET: o pagamento passou e o pedido falhou

[English](README.md) | Português

[![CI](https://github.com/ceseidl/refund-saga-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/ceseidl/refund-saga-dotnet/actions/workflows/ci.yml) [![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> **Início rápido**

```bash
dotnet run --project src/RefundSaga
```

Precisa só do SDK do .NET 10. Não há nada para instalar: o broker, o gateway de pagamento e o banco são simulados em memória.

Exemplo executável em .NET 10 de uma **saga que trata uma falha parcial**: o pagamento é cobrado, o estoque é rejeitado e a saga precisa estornar. O estorno é tratado como um novo trabalho que também pode falhar: tem status próprio, tentativas com uma chave de idempotência estável e um estado final de "revisão manual". Uma demonstração no console roda cinco cenários.

## Projetos

```
src/
└── RefundSaga/
    ├── Messages.cs     # comandos e eventos de resultado
    ├── OrderState.cs   # status, a linha do pedido e seu repositório
    ├── OrderSaga.cs    # reage aos eventos, decide a próxima ação
    ├── Payments.cs     # cobrança e estorno idempotente (pode falhar)
    ├── Inventory.cs    # reserva o estoque ou rejeita
    ├── Bus.cs          # fila em memória no lugar do RabbitMQ
    ├── World.cs        # liga os serviços e entrega as mensagens
    ├── Demo.cs         # os cinco cenários
    └── Program.cs      # ponto de entrada
```

## Requisitos

- [SDK do .NET 10](https://dotnet.microsoft.com/download)

## Como rodar

```bash
dotnet run --project src/RefundSaga
```

## Saída esperada

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

- `1 stock ok`: o caminho feliz, sem estorno.
- `2 refund requested`: o estorno só foi solicitado. O status é `Pending`, não `Confirmed`, e nada foi devolvido ainda.
- `2 sold out`: o gateway respondeu, então o estorno está `Confirmed`.
- `3 fails once`: a primeira tentativa falha (gateway fora); a nova tentativa usa a mesma chave e funciona.
- `4 answer lost`: o estorno aconteceu, mas a resposta se perdeu. A nova tentativa leva a mesma chave, então o gateway não estorna de novo (`refunds executed=1`).
- `5 gateway down`: três tentativas falhas terminam em `ManualReview`, guardando o id do pagamento para uma pessoa tratar.

## O que vale saber

- **Só transações locais.** Cada serviço confirma o seu trabalho; não existe rollback global. Desfazer um passo é uma nova ação (o estorno).
- **O estado da saga é gravado antes de enviar o próximo comando.** Aqui é um dicionário em memória; um sistema real usaria uma tabela e trataria mensagens duplicadas com uma caixa de entrada (inbox) ou a mesma ideia de idempotência.
- **O limite de tentativas é uma constante.** Em produção entram um intervalo entre as tentativas (backoff exponencial) e um circuit breaker.
- **Broker e gateway são simulados.** O `Bus` entrega em ordem e nunca perde mensagem; um broker real exige confirmações (ack), reenvio e uma fila de mensagens mortas.

## Links

- [Saga pattern (Microsoft Azure Architecture Center)](https://learn.microsoft.com/azure/architecture/patterns/saga)
- [Compensating Transaction pattern (Microsoft Azure Architecture Center)](https://learn.microsoft.com/azure/architecture/patterns/compensating-transaction)
- [Idempotent requests (Stripe API)](https://docs.stripe.com/api/idempotent_requests)

## Licença

[MIT](LICENSE). Autor: Carlos Eduardo Seidl.
