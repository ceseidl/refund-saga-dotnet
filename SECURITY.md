# Security Policy / Política de Segurança

[English](#english) | [Português](#português)

## English

`refund-saga-dotnet` is **educational code** that accompanies an article. It is not meant to run in production, and only the latest commit on `main` is maintained.

### Reporting a vulnerability

Please **do not open a public issue** for a security problem.

Use GitHub's private channel instead: go to the **Security** tab of this repository and choose **Report a vulnerability** (<https://github.com/ceseidl/refund-saga-dotnet/security/advisories/new>). Only the maintainer can see the report.

Please include what you found, how to reproduce it and, if possible, a suggested fix. This is a personal project, so the response is best effort: I will try to acknowledge the report within 7 days.

### What is expected, not a vulnerability

These are deliberate simplifications of a didactic example (the README lists the main ones):

- The broker, the payment gateway and the database are simulated in memory (no network, no authentication), to keep the example small. A real system would use a real broker, a real gateway and a durable store for the saga state.

Vulnerable dependencies are tracked with Dependabot; a pull request that bumps a package is welcome.

## Português

O `refund-saga-dotnet` é **código didático** que acompanha um artigo. Não foi feito para rodar em produção, e só o último commit da `main` é mantido.

### Como reportar uma vulnerabilidade

Por favor, **não abra uma issue pública** para um problema de segurança.

Use o canal privado do GitHub: na aba **Security** deste repositório, escolha **Report a vulnerability** (<https://github.com/ceseidl/refund-saga-dotnet/security/advisories/new>). Só o mantenedor vê o relato.

Informe o que encontrou, como reproduzir e, se possível, uma sugestão de correção. É um projeto pessoal, então a resposta é feita na medida do possível: vou tentar confirmar o recebimento em até 7 dias.

### O que é esperado e não é vulnerabilidade

São simplificações deliberadas de um exemplo didático (o README lista as principais):

- O broker, o gateway de pagamento e o banco são simulados em memória (sem rede nem autenticação), para manter o exemplo pequeno. Um sistema real usaria um broker, um gateway e um armazenamento durável para o estado da saga.

As dependências vulneráveis são acompanhadas pelo Dependabot; um pull request que atualiza um pacote é bem-vindo.
