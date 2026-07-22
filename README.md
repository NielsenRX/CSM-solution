# MyCMSSolution

Umbraco 17 LTS-løsning (.NET 10) med isoleret CRM-integrationslag og selvbetjeningsblokke.

## Projektstruktur

- `MyCMSSolution.Web` — Umbraco 17 LTS-hovedprojekt (SQLite til lokal udvikling)
- `MyCMSSolution.SelfService` — klassebibliotek til selvbetjeningsblokke
- `MyCMSSolution.Crm.Client` — CRM-integrationslag bag `ICrmClient`, med midlertidig `MockCrmClient`
- `MyCMSSolution.Core` — delte modeller og konfiguration
- `MyCMSSolution.Tests` — testprojekt (xUnit)

## Kom i gang

```
dotnet build MyCMSSolution.slnx
dotnet run --project MyCMSSolution.Web
```

Ved første kørsel gennemføres Umbracos installationswizard mod den lokale SQLite-database.

## Git hooks

Repoet indeholder et versioneret pre-push hook-system i `.githooks/`. Aktivér det efter kloning med:

```
git config core.hooksPath .githooks
```

`pre-push`-hooket advarer (blokerer ikke) hvis en push indeholder kodeændringer uden en tilsvarende opdatering af [`docs/BACKLOG.md`](docs/BACKLOG.md).

## Backlog

Opgaver spores i [`docs/BACKLOG.md`](docs/BACKLOG.md).
