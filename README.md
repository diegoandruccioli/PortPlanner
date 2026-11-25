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

App su `http://localhost:5178`. Login di esempio dal seed del template: `email1@test.it` / `Prova`.

## Stato

Template rinominato e compilato; dominio, ruoli (Supervisore / Capogruppo / Dipendente) e schermate ancora da implementare seguendo il prototipo in `docs/03-mockup/`.
