# Template di partenza e piano di adattamento

Il codice parte dal template fornito dal docente, [onitsmart/Unibo-Template](https://github.com/onitsmart/Unibo-Template) (commit `c626ada`), con i namespace rinominati da `Template` a `PortPlanner`. Questo documento descrive com'è fatto, cosa è stato verificato e come lo adattiamo al dominio.

## Verificato

- `dotnet build src/PortPlanner.sln` compila senza errori (warning `NU1903` su `System.Linq.Dynamic.Core` 1.5.0, vulnerabilità nota di gravità alta).
- **`npm install` è obbligatorio** in `src/PortPlanner.Web`: senza `node_modules/` l'app va in errore all'avvio (`DirectoryNotFoundException`), perché `Startup.cs` serve quella cartella come file provider.
- Dopo `npm install` l'app parte su `http://localhost:5178`; il login del seed (`email1@test.it` / `Prova`) porta a `/Dashboard/Index`.

Non verificato: avvio su un clone pulito di un altro computer, rigenerazione di TypeScript/SCSS/bundle fuori da Visual Studio.

## Struttura

Solution `src/PortPlanner.sln`, due progetti `net8.0`:

| Progetto | Tipo | Ruolo |
|---|---|---|
| `PortPlanner` | Class Library | **Core**: entità EF, DbContext, servizi (Query/Command), seed |
| `PortPlanner.Web` | ASP.NET Core MVC | **Web**: controller, ViewModel, view Razor, SignalR, TypeScript/Vue, CSS |

Tecnologie: Entity Framework Core 8 con database **in memoria**, Bootstrap 5.3, Vue 2 (non Vue 3) con TypeScript, SignalR, login a cookie, SG4MVC (genera `MVC.Area.Controller.Action()` tipizzati, in versione beta), localizzazione `it-it`.

Le view stanno in `Areas/{area}/{controller}/` e `Features/{controller}/`, non nella classica cartella `Views/`.

### Pattern da seguire

- Un'entità = un file; un servizio `partial` diviso in `X.Queries.cs` e `X.Commands.cs`.
- Ogni operazione è una coppia oggetto Query/Command e DTO; il servizio espone `Query(qry)` e `Handle(cmd)`. Il controller non tocca mai il `DbContext`.
- Un'area funzionale in più = un nuovo file partial (o servizio) registrato in `Container.cs`.
- Eventi in tempo reale: il controller chiama `IPublishDomainEvents.Publish(...)`, il publisher inoltra tramite l'hub SignalR (`PortPlannerHub`); il client si registra in TypeScript con `hub.on(...)`.
- Seed: `DataGenerator`, invocato dal costruttore del DbContext; ogni entità nuova va aggiunta, altrimenti la demo parte vuota.

## Cose da sapere

1. **Visual Studio:** TypeScript, SCSS, bundle e i `.d.ts` dei ViewModel si rigenerano con le estensioni di Visual Studio. Con altri editor vanno rigenerati a mano e i file generati restano committati.
2. **Vue 2:** attenzione agli esempi trovati online, spesso per Vue 3.
3. **SG4MVC beta:** se il build fallisce con errori su `MVC.*`, guardare prima lì.
4. **Password:** SHA-256 senza sale, adatto solo alla demo.
5. **.NET 8** è fuori supporto dal 10/11/2026: non si migra a .NET 10 senza un motivo.
6. Pulizia già fatta: tolti logo Onit e titolo "OnModuli" (ora barra con nome PortPlanner), eliminata l'area `Example/Users` (resta consultabile nel repository del docente), aggiunta una pagina `Dashboard` provvisoria come destinazione del login, aggiornato `System.Linq.Dynamic.Core` alla 1.7.4 (nessun avviso di vulnerabilità). Rimossi dal `.csproj` anche i riferimenti `DtsGenerator` dell'esempio: per un nuovo ViewModel che serve a TypeScript vanno ricreati copiandoli dal template.

## Piano di adattamento

### Entità del Core

Proposta di partenza, da affinare con le [specifiche](../02-progettazione/specifiche-dettagliate.md):

| Entità | Contenuto principale |
|---|---|
| `Dipendente` (al posto di `User`) | anagrafica, contratto (a contratto / a chiamata), monte ore 36/38, scadenza contratto |
| `Ruolo` / ruoli abilitati | Gruista, Muletista, Operaio, con ruolo principale; tabella estendibile |
| `Patente` | tipo (Gru, Muletto) e scadenza; lo stato valida/scaduta è calcolato |
| `Team` | nome, capogruppo (un dipendente), membri |
| `Sito`, `Nave` (barca) | siti fissi, nave con arrivo e partenza su un solo sito |
| `GestioneBarca` | team assegnato alla barca per tutto il periodo (3.0) e slot non lavorativi |
| `Turno` / `Assegnazione` | sito, giorno, slot (0–6, 6–12, 12–18, 18–24), team, dipendente, ruolo |
| `Richiesta` | richiesta di una persona di un altro team per uno slot; stato in attesa, accettata, rifiutata, scaduta |
| `Assenza` | tipo (ferie, permesso, malattia), periodo a giorni interi, stato |
| `Disponibilità`, `PreferenzaSlot` | disponibilità a giorni (solo chiamati), slot preferiti |
| `Avviso` / `Notifica` | avvisi del capogruppo e notifiche del dipendente |

### Servizi

Servizi partial per area (turni, dipendenti, team, navi, richieste, assenze, avvisi) più un **servizio di validazione** che, dato un candidato e uno slot, restituisce l'elenco degli avvisi (riposo 11 h, riposo settimanale, ore, straordinari, vincoli dei chiamati, assenze). È codice C# puro, provabile senza interfaccia. Le regole sono nelle specifiche e in [regole-contrattuali-verificate.md](../01-ricerca/regole-contrattuali-verificate.md): sono **avvisi, mai blocchi**.

### Ruoli e accesso

Il template ha un solo livello (`[Authorize]`). Servono i profili **Supervisore**, **Capogruppo** e **Dipendente**: un claim di ruolo al login e `[Authorize(Roles = ...)]` o policy. Il capogruppo è un incarico (un dipendente può esserlo), quindi il suo menu dipende dal team di cui è capogruppo, non da un ruolo di sistema.

### Mappa schermate

| Schermata | Dove nel template | Tecnologia |
|---|---|---|
| Live page e griglia della home (supervisore, capogruppo) | nuova Area `Supervisore` / `Capogruppo` | MVC + Vue |
| Pianificazione, dettaglio barca e giorno (2.0), gestione team slot (2.1) | Area `Supervisore` | MVC + Vue (drag & drop) |
| Dettaglio totale barca (3.0, modal) | Area `Supervisore` e `Capogruppo` | MVC + Vue |
| Gestione team, gestione utenti, dettaglio utente | Area `Supervisore`, parte dall'esempio Users del template (Index + Edit) | MVC |
| Richieste, avvisi | Aree `Supervisore` e `Capogruppo` | MVC |
| Turni, disponibilità, profilo, notifiche del dipendente | Area `Dipendente`, layout mobile | MVC responsive |
| Live page che si aggiorna da sola, avvisi in tempo reale | `PortPlannerHub` + `IPublishDomainEvents` | SignalR, se deciso |

## Ordine di lavoro suggerito

1. ~~Pulizia del template~~ (fatta).
2. Entità del Core e seed con dati inventati coerenti con il prototipo.
3. Login con i tre profili.
4. Servizio di validazione con test.
5. Le tre aree in parallelo, una schermata alla volta, seguendo specifiche e mockup.
