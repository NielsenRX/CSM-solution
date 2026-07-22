# Backlog

Spor opgaver for MyCMSSolution her. Opdater status og note, når en opgave flytter sig — se `.githooks/pre-push`, som advarer, hvis kodeændringer pushes uden en tilhørende opdatering af denne fil.

| Prioritet | Opgave | Status | Sprint/uge | Note |
|---|---|---|---|---|
| Høj | Scaffold af .NET-løsningen (Web, Core, Crm.Client, SelfService, Tests) | Færdig | Uge 1 | Umbraco 17 LTS på .NET 10, SQLite til lokal udvikling. Solution + projektreferencer oprettet, build og test verificeret. |
| Høj | Isoler CRM-integration bag interface med midlertidig mock | Færdig | Uge 1 | `ICrmClient` + `MockCrmClient` + `AddCrmClient()`-DI-registrering i `MyCMSSolution.Crm.Client`. Rigtig implementering kan senere erstatte mock'en ved kun at ændre DI-registreringen. |
| Mellem | Docker-opsætning til lokal udvikling/deployment | Ikke startet | Uge 1-2 | Umbraco-skabelonen understøtter `--add-docker`; afklar om Web-projektet skal have egen Dockerfile/docker-compose, og om SQLite er tilstrækkeligt i container eller om der skal en ekstern DB-service til. |
