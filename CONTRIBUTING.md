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

`npm install` è obbligatorio: senza `node_modules/` l'app non parte. Login di prova del template: `email1@test.it` / `Prova`.

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

## Pull Request

1. Apri la PR verso `main` quando il lavoro è pronto (o come bozza, se vuoi un parere prima).
2. Descrivi in due righe cosa cambia e quale schermata o regola delle specifiche implementa.
3. L'approvazione non è obbligatoria, ma se la modifica è grande o tocca regole di dominio conviene far guardare la PR a un collega.
4. Merge con **squash**, poi si cancella il branch.

Prima di aprire la PR controlla:

- [ ] `dotnet build src/PortPlanner.sln` senza errori
- [ ] l'app parte e la schermata funziona nel browser
- [ ] la schermata corrisponde al mockup (vedi sotto)
- [ ] se hai cambiato una regola di dominio, hai aggiornato `docs/` nella stessa PR

## Mockup e specifiche vengono prima del codice

Il corso richiede che le parti principali siano progettate in Figma e poi implementate in modo coerente. Quindi:

- Per ogni schermata si parte da [docs/02-progettazione/specifiche-dettagliate.md](docs/02-progettazione/specifiche-dettagliate.md) e dal mockup in [docs/03-mockup](docs/03-mockup/README.md).
- Se il codice deve discostarsi dal mockup, si cambia **prima** il mockup (o si annota il motivo nella PR) e poi il codice.
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
