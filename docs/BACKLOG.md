# Backlog

Spor opgaver for MyCMSSolution her. Opdater status og note, når en opgave flytter sig — se `.githooks/pre-push`, som advarer, hvis kodeændringer pushes uden en tilhørende opdatering af denne fil.

| Prioritet | Opgave | Status | Sprint/uge | Note |
|---|---|---|---|---|
| Høj | Scaffold af .NET-løsningen (Web, Core, Crm.Client, SelfService, Tests) | Færdig | Uge 1-2 | Umbraco 17 LTS på .NET 10, SQLite til lokal dev. Solution, projektreferencer, build og test verificeret. Flad mappestruktur (alle projekter i repo-roden, ingen `src/`+`tests/`-opdeling) er bekræftet beslutning — kickoff-planen er opdateret til at afspejle dette. |
| Høj | CRM-integrationslag isoleret bag interface, med midlertidig mock | Færdig | Uge 1-2 | `ICrmClient` (`GetCustomerProfile`, `GetSubscriptions`, `GetInvoices`) implementeret i `MockCrmClient` med 3 eksempelkunder (B2C x2, B2B x1), heriblandt et abonnement i binding (kunde 1002) til senere test af binding-scenariet. 10 unit-tests i `MyCMSSolution.Tests` dækker alle tre metoder. Rigtig implementering tilføjes senere som ny klasse bag samme interface. |
| Mellem | Docker-opsætning (dev + prod compose) | Ikke startet | Uge 1-2 | `docker-compose.dev.yml` og `docker-compose.prod.yml` i `/docker`, jf. kickoff-planens Opgave 3. Dev-filen er kun til verificeringskørsler før merge til `develop` (ikke daglig brug), prod skal hærdes (persistent storage, sikkerhedskonfig, ingen default credentials).
