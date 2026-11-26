# PortPlanner

Pianificazione turni di un porto — progetto d'esame del Laboratorio di Interfaccia Uomo-Macchina (UniBo Cesena, AA 2026-2027).

Gruppo: Diego Andruccioli, Rei Mici, Giovanni Morelli.

## Origine del codice

Partito dal template del docente [onitsmart/Unibo-Template](https://github.com/onitsmart/Unibo-Template) (commit `c626ada`), con namespace rinominati `Template` → `PortPlanner`. Stack: ASP.NET Core MVC (net8.0), Entity Framework Core (InMemory), Vue 2 + TypeScript, SignalR, Bootstrap 5, SG4MVC.

## Struttura

- `src/PortPlanner` — Core: entità, DbContext, servizi (Query/Command), seed
- `src/PortPlanner.Web` — Web: controller, ViewModel, view Razor, SignalR, script TS/Vue, CSS

## Avvio

```bash
cd src/PortPlanner.Web
npm install
dotnet run
```

`npm install` è obbligatorio: `Startup.cs` serve `node_modules/` come file provider e senza la cartella l'app non parte.

App su `http://localhost:5178`. La password di prova è la stessa per tutti gli utenti: `Prova123` (solo demo, dati inventati).

| Profilo | Email | Cosa si vede |
|---|---|---|
| Supervisore | `mario.rossi@portplanner.it` | non è un lavoratore, nessun team |
| Capogruppo | `anna.conti@portplanner.it` | capogruppo del Team 2 |
| Dipendente | `sara.fontana@portplanner.it` | lavoratrice del Team 2 |
| Dipendente senza team | `marta.pini@portplanner.it` | nessun team |
| Dipendente a chiamata | `luigi.marini@portplanner.it` | Team 3, contratto a chiamata |

Gli altri utenti seguono lo stesso schema, `nome.cognome@portplanner.it`.

## Stato

Template rinominato e compilato; modello dei dati, dati di esempio e login con i tre profili (Supervisore / Capogruppo / Dipendente) fatti; le schermate sono ancora da implementare seguendo il prototipo in `docs/03-mockup/`.
