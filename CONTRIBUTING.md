# Come lavoriamo sul repository

Regole minime per non pestarci i piedi in tre. Se una regola non funziona, la si cambia con una PR su questo file.

## Prima di iniziare

```bash
git clone https://github.com/diegoandruccioli/PortPlanner.git
cd PortPlanner
git config user.name "Nome Cognome"
git config user.email "la-tua-email-github"
cd src/PortPlanner.Web
npm install
dotnet run
```

`npm install` è obbligatorio: senza `node_modules/` l'app non parte. Login di prova: `mario.rossi@portplanner.it` / `Prova123` (gli altri utenti sono nel README).

## Branch

- `main` è sempre eseguibile: non si committa direttamente.
- Un branch per ogni lavoro, con prefisso nel nome:
  - `feat/<area>-<cosa>` — nuova funzione (es. `feat/supervisore-griglia-turni`)
  - `fix/<cosa>` — correzione
  - `docs/<cosa>` — solo documentazione
  - `chore/<cosa>` — configurazione, pulizia, dipendenze
- Branch brevi: se dura più di qualche giorno, spezzalo o fai `git pull --rebase origin main` spesso.

## Commit

[Conventional Commits](https://www.conventionalcommits.org/), una riga di riassunto, in modo imperativo:

```
feat: aggiungi griglia pianificazione 7 giorni
fix: correggi ordine candidati con patente scaduta
docs: aggiorna specifiche del capogruppo
```

Il corpo del messaggio serve solo se il "perché" non si capisce dalla modifica. Commit piccoli, uno per cambiamento logico.

## Cosa fare e in che ordine: le issue

Il lavoro è diviso in issue su GitHub, una per PR piccola.

- Ogni issue ha un **assegnatario**, una **milestone** (la fase, in ordine: 1, 2, 3, 4), una stima in punti e l'elenco delle **dipendenze**. Si lavora sulle proprie, dalla milestone più bassa.
- Per vedere le proprie: `is:open assignee:<tuo-utente>` nella pagina Issues. L'avanzamento di ogni fase è in Milestones; la bacheca del progetto mostra lo stato di tutte.
- Un'issue **bloccata da** un'altra non si inizia dalla parte che dipende dall'altra: si fa il resto (spesso basta un finto del servizio) e si scrive un commento sull'issue. Se un'attesa dura più del previsto, lo si dice al gruppo.
- Se un'issue è troppo grande o la stima non regge, si spezza o si riassegna: si cambia l'issue, non si lavora fuori dall'elenco.
- Nella PR si scrive `Chiude #numero`: l'issue si chiude da sola al merge.
- Quando ci si accorge di un lavoro mancante, si apre una nuova issue invece di aggiungerlo di nascosto a un'altra PR.

## Pull Request

1. Apri la PR verso `main` quando il lavoro è pronto (o come bozza, se vuoi un parere prima). Il modulo si compila da solo: segui la lista.
2. Descrivi in due righe cosa cambia e quale schermata o regola delle specifiche implementa, e scrivi `Chiude #numero`.
3. **Ogni PR la guarda un altro del gruppo** prima del merge, anche se GitHub non lo impone: così tutti conosciamo il codice degli altri, e all'orale ognuno sa spiegare tutto il progetto. Chi rivede prova l'app, non si limita a leggere.
4. Merge con **squash**. Il titolo del commit di squash si scrive a mano: `<tipo>: <riassunto> (#numero)`, corpo vuoto. Poi si cancella il branch.
5. PR piccole: se supera qualche centinaio di righe di codice, valuta di spezzarla.

Prima di aprire la PR controlla:

- [ ] `dotnet build src/PortPlanner.sln` senza errori né avvisi
- [ ] `dotnet test` verde, con test dove c'è logica
- [ ] l'app parte e la schermata funziona nel browser, col profilo giusto
- [ ] la schermata corrisponde al mockup (vedi sotto)
- [ ] se hai cambiato una regola di dominio, hai aggiornato `docs/` nella stessa PR

## Convenzioni di codice

- **Commenti:** in italiano, con `//`, a frase compiuta e con il punto finale. Niente tag XML (`///`, `<summary>`, `<param>`): spiegano il perché, non ripetono il nome. I file del template che hanno commenti in inglese si sistemano quando li si tocca per altro, senza una PR apposta.
- **Nomi:** entità, proprietà ed enum in italiano come nei documenti (`Dipendente`, `Barca`, `Slot.Dalle6Alle12`); terminologia tecnica in inglese come nelle slide (`Controller`, `ViewModel`, `Dependency Injection`).
- **Un'entità per file**, in `src/PortPlanner/Domain/`; i servizi in `Services/`, con le operazioni come coppie Query o Command e DTO (`Query(qry)`, `Handle(cmd)`).
- **Il seed** parte da `Startup` (`DataGenerator.Initialize`) con date relative a oggi; i test creano un `DbContext` vuoto e chiamano il seed con una data fissa.
- **Test:** ogni regola di dominio ha il suo test nel progetto `PortPlanner.Tests`. Il database in memoria non fa rispettare indici né vincoli, quindi le regole (un solo team per slot, una persona su un solo sito per slot, barche che non si sovrappongono) vanno controllate nei servizi e provate. Un test nuovo conviene farlo fallire una volta apposta per essere sicuri che sappia accorgersi del problema.
- **File toccati da tutti** (`_PortPlannerDbContext.cs`, `Container.cs`, `_Layout.cshtml`, `site.scss`): modifiche brevi e PR rapide, `git pull --rebase origin main` spesso. Se due PR conflittano, chi arriva dopo risolve.

## Mockup e specifiche vengono prima del codice

Il corso richiede che le parti principali siano progettate con un mockup e poi implementate in modo coerente. Per noi il mockup è il prototipo cliccabile in `docs/03-mockup/`: il docente ha accettato di usarlo al posto di Figma. Quindi:

- Per ogni schermata si parte da [docs/02-progettazione/specifiche-dettagliate.md](docs/02-progettazione/specifiche-dettagliate.md) e dal mockup in [docs/03-mockup](docs/03-mockup/README.md).
- Se il codice deve discostarsi dal mockup, si annota il motivo nella PR e si aggiornano le specifiche in `docs/02-progettazione/` nella stessa PR.
- Se le specifiche si contraddicono o mancano, non si inventa: si aggiunge una riga in "Punti aperti" e si chiede al gruppo.

## Regole del progetto in breve

- Le regole contrattuali (riposi, ore, straordinari) producono **avvisi, mai blocchi**; i soli blocchi sono quelli elencati nelle specifiche.
- La logica di validazione sta nel progetto Core (`src/PortPlanner`), non nei controller o nelle view, così si può provare senza interfaccia.
- Il controller non usa mai il `DbContext` direttamente: passa dai servizi con oggetti Query e Command, come nell'esempio Users del [template del docente](https://github.com/onitsmart/Unibo-Template) (cartella `src/Template.Web/Areas/Example/Users`, rimossa da questo repository).
- Ogni nuova entità va aggiunta anche al seed (`DataGenerator`), altrimenti la demo parte vuota. I dati di esempio sono inventati: niente persone o codici reali.

## File generati

Il template compila TypeScript, SCSS e bundle con gli strumenti di Visual Studio e committa il risultato (`.js`, `.css`, `*.d.ts`).

- Se cambi un `.ts`, un `.scss` o un ViewModel, ricompila e committa anche i file generati.
- Chi non usa Visual Studio lo dica in PR: i file generati vanno rifatti da chi ha gli strumenti.
- Non committare `bin/`, `obj/` e `node_modules/` (sono già in `.gitignore`).

## Cosa non va nel repository

Password o chiavi reali, dati personali veri, le slide del corso e i riassunti delle lezioni.
