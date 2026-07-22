# Kickoff-plan: Fase 3 — Byggefase
**Projekt:** MyCMSSolution — Nyt CMS-fundament
**Status:** Baseret på arbejdsantagelse om ledelsesgodkendelse (se beslutningsgrundlag afsnit "Status")
**Formål:** Samle de tre parallelle byggespor i én kickoff-klar plan

---

## Spor 1: Teknisk kravspec (Claude Code / dev-miljø & arkitektur)

### Tech stack
- **CMS:** Umbraco **17 LTS** (bevidst valg fremfor nyere version 18 — 17 er LTS med support til 27. november 2028, mod 18's kortere STS-supportperiode til 25. juni 2027. Prioriterer lang stabilitet over de nyeste features)
- **Database:** SQL Server (min. SQL Server 2016), on-premise til at starte
- **Hosting:** On-prem nu, containeriseret via Docker for at holde vejen åben til cloud senere. Bemærk: standard Docker Compose-opsætning fra Umbracos dokumentation er kun til udvikling — produktionsopsætning kræver yderligere hærdning (persistent storage, sikkerhedskonfiguration, ikke default credentials)
- **Frontend-tilgang: Hybrid — besluttet.** Umbraco har siden version 12 haft Content Delivery API'et indbygget i kernen, hvilket gør "Razor vs. headless" til et falsk enten-eller:
  - **Indholdssider** (produktsider, kampagner, nyheder, FAQ osv.): Razor + Block Grid Editor — bedst redaktørsoplevelse, fuld kontrol over markup til WCAG, mest forudsigeligt for Claude Code at arbejde med
  - **Selvbetjening ("min side"):** Stadig en almindelig Umbraco-side, bygget og styret af marketing via Block Grid — men med særlige "data-drevne blokke" (fx "vis mine abonnementer", "vis regninger"), hvor selve rammen/design er CMS-styret, mens indholdet i blokken hentes live via custom controller-kode, der kalder CRM-API'et. Content Delivery API'et bruges **ikke** til dette, da det kun er beregnet til CMS-indhold, ikke transaktionelle CRM-data

### Miljøer
| Miljø | Formål |
|---|---|
| Dev | Native `dotnet run` + lokal SQLite til daglig udvikling (hurtigste iterationsloop med Claude Code) — suppleret med Docker Compose-verificering før hver merge til `develop`, så containerrelaterede problemer fanges tidligt |
| Staging | Samme Docker-opsætning kørende on-prem, adskilt database — bruges til marketing-indholdsgennemgang og WCAG-/sikkerhedstest (jf. tidslinjens uge 16-17) |
| Produktion | Live site — on-prem, hærdet opsætning |

### Arkitekturprincipper (låst fra Fase 1-2, ikke til genforhandling uden ny beslutning)
- API-first — CMS skal kunne forbruge data fra CRM/billing og driftsinfo-systemet, ikke eje dem
- Adskillelse mellem indholdslag (CMS) og transaktionslag (selvbetjening) — koblet via API
- Block-baseret redigering til marketing, inden for udviklersatte rammer — ingen fri side-bygning uden struktur
- Ingen hosting-lock-in (on-prem → cloud skal være muligt uden re-arkitektur)
- WCAG-compliance som designprincip fra første linje kode, ikke en eftertanke

### Claude Code-arbejdsmodel
- Kravspecifikation, arkitekturbeslutninger og løsningsgennemgang sker i denne rådgivningsproces
- Selve implementeringen (document types, templates, integrationer) sker i Claude Code i det faktiske udviklingsmiljø
- Anbefalet arbejdsrytme: små, afgrænsede opgaver (fx "byg produktside-skabelonen", "byg CRM-læse-integration til regningsvisning") frem for store, udefinerede opgaver — mindsker risiko for fejlfortolkning

### Repo-struktur
```
MyCMSSolution-cms/
├── src/
│   ├── MyCMSSolution.Web/              # Umbraco-hovedprojekt (Razor, Block Grid views)
│   ├── MyCMSSolution.SelfService/      # Data-drevne blokke + CRM-controllere
│   ├── MyCMSSolution.Crm.Client/       # Wrapper-lag mod CRM-API'et (isoleret, testbart)
│   └── MyCMSSolution.Core/             # Delte modeller, konfiguration
├── docker/
│   ├── docker-compose.dev.yml      # Lokal udvikling (SQLite/lokal SQL Server)
│   └── docker-compose.prod.yml     # Produktions-hærdet opsætning
├── tests/
│   └── MyCMSSolution.Tests/            # Unit- og integrationstests, særligt CRM-integrationen
└── docs/
```
CRM-integrationen isoleres i sit eget projekt, så den kan testes og ændres uafhængigt af Umbraco-koden — matcher arkitekturprincippet om, at CMS'et forbruger data, men ikke ejer dem.

### Branch-strategi
- `main` — altid deploy-klar, beskyttet
- `develop` — løbende integration af færdigt arbejde
- `feature/*` — én branch pr. opgave
- Pull request mod `develop` med minimum én gennemgang før merge

### Deploy-model (on-prem)
- **Dev:** Lokal Docker Compose
- **Staging:** Samme opsætning on-prem, adskilt database — bruges til indholdsgennemgang og WCAG-/sikkerhedstest
- **Produktion:** Hærdet Docker-opsætning on-prem. Manuel deploy-proces er acceptabel til at starte — fuld CI/CD er en investering, der bedre betaler sig, når løsningen er i drift, end mens fundamentet stadig bygges

### Første opgaver (uge 1-2)
1. Opsæt Umbraco 17 LTS lokalt via Docker (dev-database)
2. Etabler versionsstyring (repo-struktur, branch-strategi)
3. Design den første "data-drevne blok" (fx abonnementsvisning) som mønster for resten af selvbetjeningsblokkene
4. Definer CI/CD-grundtanke (selv en simpel manuel deploy-proces er okay til at starte)

---

## Spor 2: Indholdsmodel for Umbraco (sidetyper & blokke)

### Bekræftede sidetyper (dokumenttyper i Umbraco)
Baseret på bekræftet liste fra marketing:

| Sidetype | Note |
|---|---|
| Produktside | Bredbånd, TV & Streaming, Sommerhus, Foreninger, Erhverv — sandsynligvis én fleksibel dokumenttype med variation via blokke, fremfor separate typer pr. segment |
| Kampagne/tilbud | Bør understøtte start-/slutdato til tidsstyring |
| Nyhed/presse | Standard blog-lignende struktur |
| Driftsinfo | **Ikke** manuelt redigeret — hentes/vises fra eksternt API. Skal modelleres som en visningsskabelon, der forbruger data, ikke en indtastningsskabelon |
| FAQ | Struktureret spørgsmål/svar, gerne med kategori-gruppering |
| Jobopslag | Simpel struktur — titel, beskrivelse, ansøgningslink/-mail |
| Om/kontakt/vilkår | Statiske informationssider |
| Partnerside | Kun redigerbar internt — ingen ekstern partneradgang |

**Krav til alle sidetyper:** Arkitekturen skal tillade, at nye sidetyper kan tilføjes senere uden større ombygning (bekræftet krav fra marketing).

### Foreslået blok-bibliotek (til marketing at bygge sider med)
- **Hero-blok** — billede/video + overskrift + CTA
- **Tjek-adresse-blok** — dækningstjek-integration, sandsynligvis den vigtigste konverteringsblok på forsiden
- **Produktkort-blok** — pris, hastighed, kort beskrivelse, "vælg"-CTA
- **Tekst-blok** — fri tekst med billeder, til generelt indhold
- **CTA-banner** — kampagnefremhævning med tidsstyring
- **FAQ-accordion-blok**
- **"Ring mig op"-widget** — skal videreføres fra nuværende site, inkl. tracking
- **Citat/anbefaling-blok** — hvis relevant til troværdighedsopbygning

**Teknisk anbefaling:** Brug Umbracos Block Grid Editor (fremfor ældre Block List) for at give marketing reel layoutfrihed inden for rammer — det er den nyeste og mest fleksible tilgang til dette i Umbraco.

> **⚠️ VIGTIGT — reference til senere byg:** Dette mønster er skabelonen for **alle** selvbetjeningsblokke (abonnementsoversigt, regningsvisning, produktændring m.fl.). Claude Code og udviklerne bør genbruge præcis denne struktur — CMS-styret ramme + autentificeret controller-kald mod CRM-API + ingen sidecaching af personlige data — fremfor at opfinde nye mønstre pr. blok. Afvigelser herfra bør begrundes eksplicit.

### Mønster: Data-drevne selvbetjeningsblokke (skabelon)
Eksempel: en "Abonnementsoversigt"-blok. Samme mønster genbruges til regningsvisning, produktændring osv. — kun CRM-endpointet skifter.

Flow: Marketing bygger blokken i CMS'et (titel, layout, placering) → siden renderes af Umbraco → en blok-controller autentificerer den indloggede kunde → controlleren kalder CRM-API'et og henter live data → blokken vises med CMS-designet ramme kombineret med de friske data.

- **Redigerbare felter (marketing styrer):** titel, visningstype (kort overblik vs. fuld liste), evt. CTA-tekst
- **Ikke redigerbare (kommer fra CRM live):** selve abonnements-/faktureringsdataene
- **Fejlhåndtering:** ved fejl/timeout fra CRM-API vises en pæn fejlbesked — siden må aldrig crashe
- **Caching:** Umbracos normale side-cache bruges **ikke** til disse blokke, da data er personlige — hver visning henter friske data
- **Sikkerhed:** blokken renderer kun for autentificerede kunder — samme mønster genbruges til MitID senere uden at ændre selve blok-strukturen

### Ikke afklaret endnu — se samlet oversigt over åbne spørgsmål nederst i dette dokument (spørgsmål 2 og 3)

---

## Spor 3: CRM-API-kontrakt (udkast — ejes af CRM-teamet)

Dette er et **udkast til drøftelse**, ikke en færdig spec — det endelige kontraktdesign skal valideres af det team, der kender jeres nuværende CRM-system bedst.

### Læse-endpoints (lavere risiko, byg først)

**GET /customer/{id}/profile**
| Felt | Beskrivelse |
|---|---|
| customerId | Unikt kunde-ID |
| name, email, phone | Kontaktoplysninger |
| customerType | `B2C` eller `B2B` — styrer om skrive-adgang vises i UI |
| address | Kundens adresse |

**GET /customer/{id}/subscriptions**
| Felt | Beskrivelse |
|---|---|
| subscriptionId | Unikt abonnements-ID |
| productName, productType | Fx "Bredbånd 1000/1000", type: bredbånd/tv/streaming |
| speed | Hastighed (kun bredbånd) |
| price | Pris pr. måned |
| startDate | Oprettelsesdato |
| bindingPeriodEndDate | Dato hvor binding udløber — `null` hvis ingen binding |
| status | `aktiv` / `opsagt` / `afventer` |
| addOns | Liste over tilvalg (fx ekstra WiFi-udstyr) |

**GET /customer/{id}/invoices**
| Felt | Beskrivelse |
|---|---|
| invoiceId, invoiceDate, dueDate | Fakturaidentifikation |
| amount | Beløb |
| status | `betalt` / `ikke betalt` / `forfalden` |
| downloadUrl | Link til PDF-faktura |

**GET /coverage?address=** (findes allerede, genbruges)
| Felt | Beskrivelse |
|---|---|
| available | true/false |
| availableProducts | Liste over produkter, der kan leveres på adressen |
| estimatedInstallationTime | Forventet installationstid |

### Skrive-endpoints (høj risiko, kræver ekstra sikkerhedstest)

**PATCH /customer/{id}/subscription/{subId}** (ændre produkt/hastighed)
- Request: `newProductId`, `effectiveDate` (øjeblikkeligt eller næste faktureringsdato)
- Response: opdateret abonnementsobjekt

**POST /customer/{id}/subscription/{subId}/cancel**
- Request: `requestedCancellationDate`, `reason` (valgfrit)
- Response: bekræftet opsigelsesdato (efter opsigelsesvarsel er indregnet)

**POST /customer/{id}/subscription** (nysalg)
- Request: `productId`, `address`, `installationPreference`
- Response: `orderId`, forventet installationsdato

### Fejl-/statuskoder
| Kode | Betyder | Eksempel |
|---|---|---|
| 401 | Ikke autentificeret | Session udløbet |
| 403 | Adgang nægtet | B2B-kunde forsøger skrive-kald; kunde forsøger tilgå andens data |
| 404 | Ikke fundet | Ugyldigt abonnements-ID |
| 409 | Konflikt | Forsøg på ændring under binding — se åbent forretningsspørgsmål nedenfor |
| 422 | Ugyldig anmodning | Ønsket produkt findes ikke på kundens adresse |
| 500 | Systemfejl | CRM-system utilgængeligt/timeout |

> **Åbent forretningsspørgsmål — se samlet oversigt over åbne spørgsmål nederst i dette dokument (spørgsmål 1).**

### Vigtige afgrænsninger
- **B2B/Erhverv:** Kun læse-endpoints skal eksponeres i selvbetjeningen til erhvervskunder — skriveadgang sker fortsat via telefon/mail, som besluttet
- **Autentificering:** Nuværende e-mail/password videreføres til denne fase; MitID kobles på senere som separat spor — API'et bør derfor designes med et autentificeringslag, der kan udskiftes uden at ændre selve forretningslogikken
- **Audit-logging:** Alle skrive-kald bør logges (hvem ændrede hvad, hvornår) — vigtigt både for kundeservice-opklaring og evt. tvister om aftaleændringer

### Anbefalet rækkefølge
1. Læse-endpoints først (lavere risiko, hurtigere at teste, giver værdi tidligt — "min side" kan vise data, mens skrive-siden stadig bygges)
2. Skrive-endpoints dernæst, med dedikeret sikkerhedstest før produktionssætning, da de ændrer kundeaftaler og fakturering

---

## Åbne spørgsmål (samlet oversigt — opdateres løbende)
Når et spørgsmål er afklaret, skrives løsningen ind i kolonnen "Løsning/beslutning" i stedet for at blive fjernet — så vi bevarer historikken.

| # | Spørgsmål | Hvem skal svare | Status | Løsning/beslutning |
|---|---|---|---|---|
| 1 | Hvad skal der ske, hvis en kunde vil ændre/opsige et abonnement i binding? (bloker / gebyr / vent til udløb) | CRM-team/ledelse | Åben | — |
| 2 | Hvilken rækkefølge/prioritet skal blokkene have pr. sidetype? | Marketing | Åben | — |
| 3 | Skal kampagnesider kunne have unik visuel identitet ud over blok-bibliotekets standardudseende? | Marketing | Åben | — |
| 4 | Er CRM-API-kontraktens felter og struktur retvisende ift. det faktiske system? | CRM-team | Afventer validering | — |

---

## Opstartsguide: Første konkrete skridt

### Manuelt opsætning (gøres af projektejer/IT, før Claude Code kan starte)
1. Installer .NET 10 SDK lokalt
2. Installer Docker Desktop (til verificering før merge, ikke daglig brug)
3. Opret et git-repo (fx `MyCMSSolution-cms`) — **oprettet:** https://github.com/NielsenRX/CSM-solution.git
4. Åbn Claude Code i repoet
5. Aftal midlertidig CRM-adgang med CRM-teamet — løses i praksis af opgave 2 nedenfor (mock bag interface), så arbejdet ikke behøver vente på det rigtige CRM-API

### Opgave 1 til Claude Code — scaffold løsningen (kan startes med det samme)
```
Opsæt en ny .NET-løsning kaldet MyCMSSolution med følgende projektstruktur:
- MyCMSSolution.Web (Umbraco 17 LTS-hovedprojekt, .NET 10)
- MyCMSSolution.SelfService (klassebibliotek til selvbetjeningsblokke)
- MyCMSSolution.Crm.Client (klassebibliotek, isoleret CRM-integrationslag bag et interface,
  så en rigtig implementering senere kan erstatte en midlertidig mock uden at ændre
  resten af koden)
- MyCMSSolution.Core (delte modeller og konfiguration)
- MyCMSSolution.Tests (test-projekt)

Installer Umbraco 17 LTS i MyCMSSolution.Web med SQLite som database til lokal udvikling.
Opsæt git med main og develop branches. Tilføj en .gitignore passende til .NET/Umbraco.
```

### Opgave 1 til Claude Code — scaffold løsningen ✅ Gennemført
```
Opsæt en ny .NET-løsning kaldet MyCMSSolution med følgende projektstruktur:
- MyCMSSolution.Web (Umbraco 17 LTS-hovedprojekt, .NET 10)
- MyCMSSolution.SelfService (klassebibliotek til selvbetjeningsblokke)
- MyCMSSolution.Crm.Client (klassebibliotek, isoleret CRM-integrationslag bag et interface,
  så en rigtig implementering senere kan erstatte en midlertidig mock uden at ændre
  resten af koden)
- MyCMSSolution.Core (delte modeller og konfiguration)
- MyCMSSolution.Tests (test-projekt)

Installer Umbraco 17 LTS i MyCMSSolution.Web med SQLite som database til lokal udvikling.
Opsæt git med main og develop branches. Tilføj en .gitignore passende til .NET/Umbraco.
```
**Status:** Alle fem projekter oprettet på `net10.0`, samlet i `MyCMSSolution.slnx`. Umbraco 17 pinnet til `17.*` via `Directory.Packages.props`. `ICrmClient`/`MockCrmClient` allerede stilladset i `Crm.Client` med DI-extension (`AddCrmClient()`), klar til opgave 2. `dotnet build`/`dotnet test` kører grønt. Git initialiseret med `main`/`develop`. **Resterende manuelt skridt:** kør `dotnet run --project MyCMSSolution.Web` for at gennemføre Umbracos installationswizard mod SQLite.

### Opgave 2 til Claude Code — CRM-integrationslag med mock
```
I MyCMSSolution.Crm.Client: byg et interface ICrmClient med metoderne
GetCustomerProfile, GetSubscriptions, GetInvoices baseret på felterne i
kickoff-planens CRM-API-kontrakt. Implementer en MockCrmClient, der returnerer
realistisk testdata, så resten af løsningen kan udvikles og testes, før det
rigtige CRM-API er klart. Den rigtige implementering tilføjes senere som en
separat klasse, der opfylder samme interface.
```

### Opgave 3 til Claude Code — Docker-opsætning
```
Opret docker-compose.dev.yml og docker-compose.prod.yml i /docker, baseret på
Umbracos officielle Docker-anbefalinger for Umbraco 17 på .NET 10. Dev-filen
er kun til verificeringskørsler (ikke daglig brug), prod-filen skal være
hærdet til on-premise drift.
```

### Opgave 4 til Claude Code — Backlog og git-hook
```
Opret en BACKLOG.md i /docs (docs-mappen ligger i repoets rod, side om side med
MyCMSSolution.Web, MyCMSSolution.Core osv. og .slnx-filen — ikke inde i et af
projekterne) med en tabelstruktur til at spore opgaver: kolonnerne skal være
Prioritet, Opgave, Status (Ikke startet/I gang/Færdig), Sprint/uge, Note.
Initialiser den med de opgaver, der allerede er sat i gang fra kickoff-planen
(scaffold af løsningen, CRM-mock, Docker-opsætning).

Opsæt derudover et versioneret git pre-push hook-system:
- Opret en /.githooks-mappe med et pre-push-script
- Scriptet skal advare (ikke blokere) udvikleren, hvis de commits, der pushes,
  indeholder kodeændringer uden en tilsvarende opdatering af docs/BACKLOG.md
- Tilføj en opsætningsinstruktion i README (git config core.hooksPath .githooks),
  så hook'et aktiveres automatisk for alle, der har klonet repoet
```

### Vent med (afhænger af åbne spørgsmål — se tabel ovenfor)
- Byg af selve blok-biblioteket → afventer marketings svar på blok-prioritering (spørgsmål 2, 3)
- Rigtig CRM-integration (i stedet for mock) → afventer CRM-teamets validering og binding-beslutningen (spørgsmål 1, 4)

## Samlet kickoff-tjekliste (uge 1)
- [x] Razor vs. headless-beslutning taget: hybrid-tilgang (Razor + Block Grid til indhold, data-drevne blokke til selvbetjening)
- [x] Mønster for data-drevne blokke designet (skabelon: Abonnementsoversigt)
- [x] CRM-API-kontrakt skærpet med konkrete felter og fejlkoder (afventer stadig CRM-teamets validering + beslutning om binding-håndtering)
- [x] Repo-struktur, branch-strategi og deploy-model fastlagt
- [x] Opstartsguide med manuelle skridt og konkrete Claude Code-opgaver klar (se ovenfor)
- [x] Manuel opsætning gennemført (.NET SDK, Docker, git-repo)
- [x] Opgave 1 (scaffold) gennemført — repo: https://github.com/NielsenRX/CSM-solution.git
- [ ] Umbracos installationswizard gennemført (`dotnet run --project MyCMSSolution.Web`)
- [ ] Opgave 2-4 sat i gang i Claude Code
- [ ] Backlog (docs/BACKLOG.md) og pre-push git-hook oprettet
- [ ] CRM-teamet har set og reageret på API-kontrakt-udkastet
- [ ] Marketing har bekræftet blok-bibliotekets indhold/prioritet

---

*Dette dokument er et arbejdsdokument til opstart af Fase 3 og forudsætter, at den formelle ledelsesgodkendelse (jf. beslutningsgrundlagets afsnit 13) indhentes, før noget sættes i produktion.*
