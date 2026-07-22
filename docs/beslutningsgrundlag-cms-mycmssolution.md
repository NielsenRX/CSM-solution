# Beslutningsgrundlag: Nyt CMS-fundament til hjemmeside
**Projekt:** MyCMSSolution — Fase 1: Analyse & afklaring
**Status:** Fase 2 afsluttet — indstilling klar (se afsnit 13). **Bemærk:** Fase 3 (byggefase) er igangsat parallelt under en **arbejdsantagelse om ledelsesgodkendelse**, for ikke at tabe tid — den reelle godkendelse fra ledelsen/IT er endnu ikke indhentet og skal eftervises, før noget bygges i produktion
**Sidst opdateret:** 22. juli 2026

## 1. Projektbaggrund & formål
- Nuværende løsning er teknisk forældet (WordPress/WooCommerce + fuldt hardcoded selvbetjening på mit.mycmssolution.dk)
- Ønske om at styrke hjemmesidens image
- Mål: et selvbetjeningsunivers, der fungerer som selvdreven salgskanal
- Ikke et KPI-drevet projekt — primært en nødvendighed (teknisk gæld + redaktørfrustration + begrænset selvbetjening)
- Intet hårdt deadline, men ønske om at komme i gang hurtigst muligt

## 2. Nuværende arkitektur (as-is)
- Hovedsite: WordPress + WooCommerce
- Selvbetjening (mit.mycmssolution.dk): fuldstændig hardcoded, separat fra hovedsite
- CRM/billing: internt ejet system med API (kunde-, produkt- og faktureringsdata)
- Login: e-mail/password
- Betaling: kort via tredjepart + PBS
- Dækningstjek: findes allerede
- Analytics: Google Analytics (formentlig GA4)
- Driftsinfo: opdateres automatisk via API (ikke manuel redigering)

## 3. Scope for nyt system
- **Alt skiftes ud** — ét samlet fundament til både indhold/marketing og fuld selvbetjening
- Selvbetjening skal udvides til: køb af nyt abonnement, ændring af eksisterende abonnement, se regninger (i dag kun regninger + købshistorik)
- "Ring mig op"-funktion videreføres, inkl. tracking

## 4. Bekræftede krav

### Indhold & redaktion
- Indholdstyper: produktsider, kampagner/tilbud, nyheder, driftsinfo, FAQ, jobopslag, om/kontakt/vilkår, partnersider — skal kunne udvides senere
- Redaktører (marketing) skal selv kunne bygge/redigere sider med prædefinerede blokke — uden udvikler
- Ønske om enkelthed i brugerfladen (ikke enterprise-tungt)
- A/B-testning: nødvendighed, ikke højt prioriteret
- Ingen regional differentiering nu (Bornholm vs. Sjælland), men skal være muligt at bygge senere

### Integrationer
- CRM/billing-integration er centralt (internt ejet, kan videreudvikles)
- Betaling: kort (tredjepart) + PBS
- MitID: bekræftet fremtidigt krav (kræver certificeret broker — ikke valgt endnu), ikke blokerende for opstart

### Teknik & drift
- Hosting: start on-premise, med krav om mulig senere overgang til cloud (ingen lock-in)
- GDPR: overholdes fuldt ud i dag — ingen kendt risiko
- WCAG: **kendt gap** — skal løses i ny løsning
- SEO og adfærdstracking gennem hele bestillingsflowet (funnel-analyse)

## 5. Smertepunkter (den reelle anledning)
- Marketing kan ikke selv redigere/oprette sider — kræver altid udvikler, stor kilde til frustration
- Nuværende selvbetjening viser kun regninger/købshistorik — ingen mulighed for at ændre produkter
- Kunder kan ikke selv lave ændringer på deres abonnement (bekræftet af kundeservice)
- Ingen mulighed for landingssider/A/B-test i dag (bekræfter udviklerafhængighed)

## 6. Åbne spørgsmål (samlet oversigt — opdateres løbende)
Når et spørgsmål er afklaret, skrives løsningen ind i kolonnen "Løsning/beslutning" i stedet for at blive fjernet — så vi bevarer historikken.

| # | Spørgsmål | Hvem skal svare | Status | Løsning/beslutning |
|---|---|---|---|---|
| 1 | Hvilken MitID-broker skal vælges? | IT/ledelse | Nedprioriteret — afventer opstart af MitID-sporet | — |
| 2 | GDPR-dokumentation/retlig hjemmel til håndtering af personnumre via MitID | IT/Juridisk | Nedprioriteret — afventer opstart af MitID-sporet | — |
| 3 | Videre involvering af IT/drift, marketing og kundeservice i processen | Projektejer | Afklares løbende | — |
| 4 | Brugsdata (Analytics/support-henvendelser) er ikke tilstrækkelig kvalitet til konkrete designbeslutninger | Marketing/IT | Løst — antagelser bruges i stedet, valideres løbende med projektejer | Se afsnit om brugsdata i status runde 2 |

## 7. Segmenter: B2B vs. B2C
- **Privatkunder:** fuld selvbetjening — se og ændre produkter/abonnement, se regninger
- **Erhvervskunder:** kan se regninger og produkter i selvbetjeningen, men **kan ikke selv ændre** — ændringer sker fortsat via telefon/mail som i dag
- Ingen konkret data på fordelingen mellem nye og eksisterende kunders sitebesøg — ikke fokus for denne analyse

## 8. Bestillingsflow (as-is, kortlagt via offentligt indhold)
1. Adresseopslag (søgefelt på forsiden)
2. Produkt-/hastighedsvalg (WooCommerce)
3. Bestilling gennemføres online eller telefonisk
4. Ordrebekræftelse pr. mail
5. Kundekonsulent kontakter ift. installation/gravearbejde
6. Aftale underskrives
7. Fysisk installation
*Bemærk: flowet er kortlagt via offentligt tilgængeligt indhold, ikke ved gennemført testordre.*

## 9. Kundeservice-system & partnerindhold
- Separat kundeservice-system findes, men skal **ikke** integreres med ny løsning nu (hverken chat eller ticketing)
- Mulig fremtidig udvidelse: oprettelse af support-ticket direkte fra "min side"
- Partnerindhold (Waoo, Allente): redigeres udelukkende internt — partnere har ingen direkte adgang, men kan henvende sig med ønsker

## 10. Projektrammer
- **Udviklingsmodel:** Internt — kodning forventes udført med **Claude Code**, i samarbejde med denne rådgivningsproces
- **Tidsramme:** 6 måneder — deadline ultimo 2026 (justeret fra oprindeligt 2-3 mdr. efter vurdering af fuldt scope)
- **Budget:** Ingen fast ramme, men omkostningsbevidsthed er et krav — ikke den dyreste løsning på markedet
- **Vigtigt arkitekturkriterie afledt heraf:** CMS-kandidater bør have godt dokumenteret API/headless-tilgang og stærk (gerne open source) dokumentation, så Claude Code kan arbejde struktureret med platformen — proprietære "low-code magic"-lag uden god ekstern dokumentation er en risiko

## 11. Bekræftet: CRM-API skal bygges om, uanset CMS-valg
- Det nuværende CRM/billing-system understøtter **ikke** abonnementsstyring (ændring af løbende produkter/aftaler) i dag
- Dette skal bygges **uanset hvilket CMS/arkitektur der vælges** — det er ikke en konsekvens af CMS-valget, men en forudsætning, der allerede er besluttet
- Skal indregnes som en selvstændig budget- og tidsomkostning i projektet, adskilt fra selve CMS-implementeringen

## 7. Foreløbige arkitekturprincipper (input til Fase 2)
- API-first / composable tilgang — CMS skal kunne forbruge data fra CRM/billing/driftssystem, ikke nødvendigvis eje dem
- Adskillelse mellem indholdslag (CMS) og transaktionslag (selvbetjening/CRM) — evt. i samme platform (fx Umbraco Commerce) eller separat, afklares endeligt i Fase 2
- Block-based/komponent-redigering til marketing, inden for udviklersatte rammer
- Ingen hosting-lock-in (on-prem → cloud skal være muligt)
- WCAG-compliance som designprincip fra start

---
*Dette dokument opdateres løbende gennem Fase 1. Fase 2 (vurdering af CMS-kandidater) igangsættes først, når de åbne punkter i afsnit 6 er afklaret.*

## 12. Fase 2 — Vurdering af CMS-kandidater

### 12.1 Arkitekturretninger
- **Retning A — Alt-i-én:** CMS og selvbetjening/e-commerce i samme platform (fx Umbraco + Umbraco Commerce, WordPress + WooCommerce)
- **Retning B — Composable/headless:** CMS til indhold, separat kundeportal til selvbetjening, koblet via API'er mod CRM

### 12.2 Dybdegennemgang: Umbraco Commerce
- Officielt e-commerce-tilføjelse til Umbraco — dækker produkter, ordrer, butikker, rabatter, betalinger, kundeoverblik, multi-market (valuta/sprog/afgifter)
- **Vigtig begrænsning:** Bygget til *ordre-/produktbaseret* handel (kurv, checkout, engangskøb) — **ikke** til abonnements-/kontraktstyring (ændring af løbende aftaler). Dette gælder formentlig alle tilsvarende CMS-commerce-moduler, ikke kun Umbracos
- Open source kerne, fuldt on-prem-egnet, Umbraco Cloud er valgfrit tilkøb
- Pris: CMS-kerne gratis; tilkøb (support/cloud/Commerce) starter lavt og skalerer med behov — økonomisk fornuftig base, men Commerce + evt. Forms/Deploy lægger sig oveni

### 12.3 Scorecard: A (Umbraco) vs. B (WordPress moderniseret) vs. C (Headless + custom portal)

| Kriterie | A: Umbraco | B: WordPress/WooCommerce | C: Headless + custom |
|---|---|---|---|
| Block-baseret redigering til marketing | Stærkt | Stærkt (kendt af marketing) | Varierer — kræver god visuel editor |
| Abonnements-ændring ud af boksen | Nej — custom kræves under alle omstændigheder | Nej — samme | N/A — bygges custom fra start |
| API-first mod CRM | Godt | Middel — mere monolitisk | Bedst egnet |
| Claude Code-venlighed | Stærk | Meget stærk | Afhænger af valgt platform |
| On-prem → cloud uden lock-in | Ja | Ja | Ja |
| WCAG-egnethed | God | Varierer, historisk rodet | Fuld kontrol |
| Opfattet image/kvalitet | Professionelt | Kan opleves "budget" | Fuld designfrihed |
| Realistisk inden for 6 mdr. | Sandsynligt | Hurtigst at starte | Mest tidskrævende |
| Dansk/nordisk modenhed | Stærk | Bred, mindre "enterprise-DK" | Varierer |

### 12.5 Alternative kandidater undersøgt
- **Craft CMS:** Fleksibel, udviklertung (Twig-templates skal bygges fra bunden af udviklere) — matcher dårligt kravet om, at marketing selv skal kunne bygge/redigere sider uden udvikler. Svagere dansk/nordisk tilstedeværelse end Umbraco. Vurderes fra.
- **Sanity/Contentful:** Udviklerfokuserede headless-CMS'er uden indbygget visuel side-builder — matcher dårligt "blok-baseret redigering uden udvikler". Vurderes fra til dette formål.
- **Storyblok:** Eneste headless-kandidat med en ægte visuel blok-editor til marketing uden udvikler-involvering. Mest seriøse udfordrer til Umbraco på Retning B. Ulemper: betalt SaaS (ikke open source/selv-hostet), løbende abonnementsomkostning oveni, mindre gennemsigtig for Claude Code at arbejde direkte i kildekoden, svagere dansk/nordisk modenhed end Umbraco.

### 12.6 Uddybet Umbraco-gennemgang (on-prem & Claude Code)
- **Teknisk fundament:** .NET-baseret (aktuelt .NET 8/9), database enten SQLite eller SQL Server (min. SQL Server 2016). Beskedne hardwarekrav — en server der kan køre .NET + SQL Server er tilstrækkeligt; eksisterende IT-drift kan sandsynligvis allerede understøtte dette
- **Docker:** Umbraco kan containeriseres til en portabel on-prem-opsætning, der lettere kan flyttes til cloud senere. Standard Docker Compose-opsætning er dog kun til udvikling — produktionsopsætning kræver yderligere hærdning
- **Claude Code-egnethed:** Åben, veldokumenteret .NET/C#-platform med stort community og klare, gentagne mønstre (document types, templates, controllers) — godt match til struktureret AI-assisteret udvikling. Praktisk arbejdsmodel: rådgivning/arkitektur/kravspecifikation her, faktisk implementering (document types, templates, CRM-integration) via Claude Code i udviklingsmiljøet

### 12.7 Opdateret samlet vurdering
- Umbraco fastholdes som stærkeste kandidat til CMS-delen — bekræftet med lave on-prem-krav og klar Claude Code-arbejdsmodel
- Storyblok er den mest seriøse alternative retning (Retning B), men taber på pris, dansk modenhed og Claude Code-transparens
- Craft CMS og traditionelle headless-CMS'er (Sanity/Contentful) vurderes fra grundet manglende non-udvikler blok-redigering

## 13. Indstilling til ledelsen/IT

**Anbefaling:** Gå videre med **Umbraco** som CMS-fundament for den nye hjemmeside.

**Begrundelse i korte træk:**
- Dækker kravet om, at marketing selv skal kunne bygge og redigere sider med blokke, uden udvikler involveret pr. ændring
- Open source, .NET-baseret — kan køres on-premise med beskedne hardwarekrav, med reel mulighed for at skifte til cloud senere uden lock-in
- Modent og velafprøvet i dansk/nordisk sammenhæng — lettere at finde support, bureauer og viden lokalt, hvis det bliver nødvendigt
- God dokumentation og struktur, som gør platformen velegnet til udvikling med Claude Code
- Økonomisk fornuftig base (gratis kerne, betalte tilkøb skalerer med behov) — matcher ønsket om ikke at vælge den dyreste løsning på markedet

**Vigtigt forbehold, som skal kommunikeres tydeligt:**
- Umbracos e-commerce-tilføjelse (Umbraco Commerce) dækker **ikke** jeres kernebehov om at ændre et løbende abonnement — det er en ordre-/engangskøbsløsning
- Abonnementsstyring (se, ændre, opsige løbende produkter) skal derfor bygges som en **custom løsning oven på jeres eksisterende CRM/billing-API** — dette gælder uanset hvilket CMS der var blevet valgt, og er allerede besluttet som en nødvendig omkostning i tid og budget
- De to spor (CMS-implementering og CRM-API-ombygning) kan og bør køre parallelt for at holde den samlede tidsramme på 6 måneder

**Ikke medregnet i denne indstilling (bevidst fravalgt fra Fase 1):**
- MitID-integration (bekræftet fremtidigt krav, men ikke en del af denne bygge-fase)
- Regional differentiering af indhold (arkitektonisk mulighed holdes åben, men bygges ikke nu)

## 14. Tidslinje & milepæle (til ledelsen)

![Tidslinje for CMS-projektet](tidslinje-cms-projekt.svg)

**Projektstart:** 1. august 2026 | **Slutdato:** ultimo januar 2027 (6 måneder)
**Lanceringsmodel:** Faseopdelt — anbefalet fremfor samlet lancering, fordi CRM-API-sporet (abonnementsstyring) er det mest usikre element i projektet. Faseopdeling reducerer risikoen for, at hele projektet forsinkes af det spor.
**Ressourcer:** To separate teams — ét på CMS-sporet (Umbraco), ét på CRM-API-sporet — kører parallelt.

| Periode | Milepæl | CMS-spor (Umbraco) | CRM-API-spor (abonnementsstyring) |
|---|---|---|---|
| Uge 1 (1.-7. aug) | Projektstart | Kickoff, kravgennemgang, miljøopsætning | Kickoff, teknisk deep-dive i eksisterende API/database |
| Uge 2-4 (aug) | Design & arkitektur låst | Informationsarkitektur, design/UI-retning, opsætning af Umbraco-instans (dev-miljø) | Detaljeret scoping af abonnements-ændringslogik (valideringsregler, kobling til billing) |
| Uge 5-8 (sep) | Første byggeblok klar | Blok-bibliotek til marketing, skabeloner for produktsider/kampagner/nyheder | Første API-endpoints til at *se* abonnement/produkter (læsning) |
| Uge 9-12 (okt) | Indhold & kernefunktion | Indholdsmigrering fra WordPress, "ring mig op"-funktion genopbygges, dækningstjek integreres | API-endpoints til at *ændre* abonnement (skrivning) — det tunge, usikre arbejde |
| Uge 13-15 (nov) | **Milepæl: Fase 1-kandidat klar** | Marketing-site + læsevisning af "min side" klar til test | CRM-skriveendpoints i test/QA |
| Uge 16-17 (nov/dec) | WCAG- & sikkerhedstest | Tilgængelighedstest (automatiseret + manuel gennemgang) og rettelser, performance-test | Sikkerhedstest af skriveendpoints (kritisk, da det ændrer aftaler/fakturering) |
| Uge 18 (start dec) | **🚀 Lancering Fase 1** | Nyt CMS + marketing-site + "min side" (læsning) går i luften | — |
| Uge 19-22 (dec) | Integration | — | Selvbetjenings-UI til at ændre abonnement bygges ind i "min side" |
| Uge 23-24 (jan) | UAT & test | Fælles ende-til-ende-test af fuld selvbetjening | Fælles ende-til-ende-test af fuld selvbetjening |
| Uge 25-26 (slut jan) | **🚀 Lancering Fase 2** | **Fuld selvbetjening (køb, ændre, se regninger) i luften** | |

**Kritiske forudsætninger, der kan påvirke tidslinjen:**
- CRM-API-sporets tidslinje er baseret på en antagelse om kompleksitet — bør valideres i uge 2-4 og justeres, hvis arbejdet viser sig større end forventet
- WCAG-test kræver en manuel/menneskelig gennemgang ud over automatiseret scanning — automatiserede værktøjer fanger kun en del af de relevante succeskriterier (bl.a. ikke tastaturnavigation, skærmlæser-oplevelse eller sprogklarhed)
- Indholdsmigrering forudsætter, at marketing kan afsætte tid til gennemgang/godkendelse parallelt med udviklingsarbejdet
