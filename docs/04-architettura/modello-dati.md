# Modello dei dati

Analisi del modello di dominio del Core (`src/PortPlanner`). Parte dalle [specifiche](../02-progettazione/specifiche-dettagliate.md) e dal [piano sul template](./template-e-adattamento.md). Le scelte numerate come "D1…" sono decisioni prese dal gruppo.

## Decisioni

| # | Decisione |
|---|---|
| D1 | Il login passa da una tabella `Utente` separata. Il Supervisore ha solo l'`Utente`; ogni lavoratore ha `Utente` e `Dipendente` collegati. |
| D2 | I ruoli Gruista, Muletista, Operaio sono un `enum` nel codice. |
| D3 | Il seed calcola le date a partire da oggi, così la demo è coerente in qualsiasi giorno. |
| D4 | Il seed contiene circa 25 lavoratori e 5 team. |
| D5 | Il contratto ha data di inizio e data di fine (vuota = indeterminato). |
| D6 | La barca ha anche tipo e quantità di merce. |
| D7 | Il Core ha un progetto di test xUnit, con un test sul seed. |
| D8 | `QuantitaMerce` è espressa in tonnellate. |
| D9 | Tutti gli utenti del seed hanno la stessa password di prova (demo). |
| D10 | `Utente` ha `Nome` e `Cognome`; `Dipendente` li legge dall'`Utente` collegato. Così anche il Supervisore ha un nome da mostrare. |
| D11 | Lo stato della patente ha tre valori calcolati: Valida, In scadenza (entro 30 giorni) e Scaduta. |
| D12 | Le date (`Arrivo`, `Partenza`, `Data`, scadenze, contratto) sono `DateOnly`. |
| D13 | Su un sito la partenza di una barca precede l'arrivo della successiva: mai lo stesso giorno. |
| D14 | `TipoMerce` è un `enum`. |
| D15 | Al login il cookie contiene i claim `Profilo`, `DipendenteId` e `TeamId` (se esiste). Se cambia il capogruppo serve un nuovo accesso. |
| D16 | Il seed ha cinque barche (D ed E arrivano dopo 7 giorni), nessuno storico, e la griglia del Team 2 su Barca A identica alle specifiche. |
| D17 | `DataGenerator.Initialize(context, oggi)` riceve la data: l'app passa quella di sistema, i test una fissa. |

## Il database in memoria non fa rispettare i vincoli

`UseInMemoryDatabase` ignora indici univoci, chiavi esterne obbligatorie e vincoli di integrità. Quindi le regole del dominio ("un solo team per slot", "la stessa persona non può stare in due siti nello stesso slot") **vanno controllate nei servizi** (Command e validazione), non delegate a EF. Un test sul seed verifica che i dati di partenza le rispettino.

## Profili e accesso

- Profilo **Supervisore**: `Utente.IsSupervisore = true`, nessun `Dipendente`.
- Profilo **Capogruppo**: non è un campo. Un `Dipendente` è capogruppo se compare in `Team.CapogruppoId`.
- Profilo **Dipendente**: tutti gli altri lavoratori.
- Il profilo si calcola al login e finisce in un claim; il menu cambia di conseguenza.
- Il Supervisore accede dalla stessa pagina di login degli altri, con email e password. Non esiste registrazione: gli account nascono dal seed. Dopo il login il claim di profilo lo manda alla sua home e ne abilita solo le pagine (`[Authorize(Roles = ...)]` o policy); un capogruppo o un dipendente che apre un indirizzo del supervisore viene respinto. Il Supervisore non è un lavoratore: non ha turni, non è in nessun team e non può essere capogruppo.

## Entità

Gli identificatori sono `Guid`, come nel template.

### Accesso e anagrafica

**`Utente`**: `Id`, `Email` (univoca), `PasswordHash`, `Nome`, `Cognome`, `IsSupervisore`.
La password è SHA-256 senza sale, come nel template: va bene solo per la demo.

**`Dipendente`**: `Id`, `UtenteId` (nome, cognome ed email stanno sull'`Utente`), `Telefono`, `TipoContratto` (`AContratto`, `AChiamata`), `OreSettimanali` (36 o 38; vuoto per i chiamati), `DataInizioContratto`, `DataFineContratto` (opzionale), `RuoloPrincipale`, `TeamId` (opzionale).
- Un solo team per lavoratore.
- Le ore settimanali sono obbligatorie per chi è a contratto.

**`DipendenteRuolo`**: `DipendenteId`, `Ruolo`. I ruoli abilitati oltre al principale. L'Operaio è implicito: non richiede patente e lo può fare chiunque.

**`Patente`**: `DipendenteId`, `Tipo` (`Gru`, `Muletto`), `Scadenza`. Nessuna riga = non posseduta. Lo stato si calcola dalla scadenza (`StatoAl(oggi)`): **Scaduta** dal giorno dopo la scadenza, **In scadenza** negli ultimi 30 giorni (scadenza compresa), altrimenti **Valida** (D11).

**`Team`**: `Id`, `Nome`, `CapogruppoId` (un `Dipendente` del team). Un team ha sempre almeno un membro, il capogruppo.

### Siti e barche

**`Sito`**: `Id`, `Nome`, `Ordine`. Quattro siti fissi, una gru ciascuno.

**`Barca`**: `Id`, `Nome`, `SitoId`, `Arrivo` (`DateOnly`), `Partenza` (`DateOnly`), `TipoMerce` (`enum`: Container, RinfuseSolide, RinfuseLiquide, MerciVarie, Rotabili, ProdottiForestali), `QuantitaMerce` (in tonnellate, intero).
- Una barca sta su un solo sito.
- Blocchi veri, da controllare nel servizio: la partenza non precede l'arrivo; su un sito la partenza di una barca **precede** l'arrivo della successiva, mai lo stesso giorno (D13).

### Pianificazione

**`Slot`** (`enum`): `Dalle0Alle6`, `Dalle6Alle12`, `Dalle12Alle18`, `Dalle18Alle24`. Ogni slot vale 6 ore; `OraInizio()` e `OraFine()` danno gli estremi (0–6, 6–12, 12–18, 18–24).

**`GestioneBarca`** (livello 3.0): `BarcaId` (è la chiave: una gestione per barca), `TeamGestoreId` (opzionale). Un team gestisce la barca per tutto il periodo e il capogruppo sceglie quando lavorare.

**`AssegnazioneTeam`** (livello 2.0): `Id`, `BarcaId`, `Data`, `Slot`, `TeamId`, `Origine` (`Supervisore` = imposto, `Capogruppo` = scelto nella gestione).
- Un solo team per barca, data e slot. Nel `DbContext` c'è un indice univoco su questi tre campi, ma con il database in memoria non viene fatto rispettare: il controllo vero è nel servizio.
- L'assegnazione imposta dal supervisore prevale su quella del capogruppo e non si contesta.
- Lo stesso team può lavorare nello stesso slot su siti diversi, con persone diverse.

**`SlotNonLavorativo`**: `BarcaId`, `Data`, `Slot`, `TeamId`. Slot o giorno in cui il team non deve lavorare, deciso dal supervisore (3.0 per il team gestore, 2.0 dal menu dello slot). Non ha un `Id`: la chiave è l'insieme di barca, data, slot e team; un giorno bloccato sono quattro righe.

**`AssegnazionePersona`** (livello 2.1): `Id`, `AssegnazioneTeamId`, `DipendenteId`, `Ruolo`, `NonSvolto` (il supervisore segna un turno passato come non lavorato).
- La stessa persona non può stare in due siti nello stesso slot: è un blocco vero, da controllare nel servizio.
- Un lavoratore fa un solo ruolo per slot.

### Rimandate alle loro schermate

`Richiesta`, `Assenza`, `DisponibilitaGiorno` e `PreferenzaSlot`, `Avviso` del capogruppo, `Notifica` del dipendente, storico del contratto. Non vanno scritte prima di avere le schermate: i loro campi dipendono dai dettagli dell'interfaccia.

## Relazioni in breve

```mermaid
erDiagram
    Utente ||--o| Dipendente : "ha"
    Dipendente }o--o| Team : "appartiene a"
    Team ||--|| Dipendente : "capogruppo"
    Dipendente ||--o{ Patente : "possiede"
    Dipendente ||--o{ DipendenteRuolo : "abilitato a"
    Sito ||--o{ Barca : "ospita"
    Barca ||--o| GestioneBarca : "ha"
    GestioneBarca }o--o| Team : "gestita da"
    Barca ||--o{ AssegnazioneTeam : "ha"
    Team ||--o{ AssegnazioneTeam : "lavora in"
    AssegnazioneTeam ||--o{ AssegnazionePersona : "composta da"
    Dipendente ||--o{ AssegnazionePersona : "lavora come"
    Barca ||--o{ SlotNonLavorativo : "ha"
```

## Seed

Date relative a oggi. Contenuto coerente con il prototipo:

- **Utenti:** il supervisore Mario Rossi e un utente per ogni lavoratore. Password di prova uguale per tutti (scritta nel README, solo demo).
- **Lavoratori:** circa 25, nomi inventati, con i personaggi del prototipo (Anna Conti, Luca Moro, Giorgio Neri, Sara Fontana, Paolo Riva, Luigi Marini a chiamata, Marco Bruno, Marta Pini senza team, Davide Longo). Mix di contratti, ruoli multipli e patenti (valide, scadute, mancanti).
- **Team:** cinque, ognuno con capogruppo (Anna Conti nel Team 2).
- **Siti e barche:** Sito 1–4. Barca A (Sito 1, oggi + 3 giorni), Barca B (Sito 2, oggi + 4), Barca C (Sito 4, da oggi + 3 a oggi + 6), Barca D ed E dopo 7 giorni, Sito 3 libero oggi. Il Team 2 gestisce la Barca A. Nessuno storico: il seed parte da oggi (D16).
- **Griglia di Barca A:** i 16 slot del Team 2 riproducono la tabella "Dati canonici" delle specifiche dettagliate.
- **Anomalie di anagrafica:** una patente scaduta (Giorgio Neri), una in scadenza, un contratto scaduto, uno non ancora iniziato, Luigi Marini a chiamata, Marta Pini senza team.
- **Assegnazioni:** alcune imposte dal supervisore, alcune scelte dal capogruppo, qualche slot non lavorativo, e almeno un caso utile alle anomalie (patente scaduta, riposo breve).

Il test sul seed verifica: un solo team per slot, nessuna persona in due siti nello stesso slot, ogni capogruppo è membro del suo team, barche senza sovrapposizioni sullo stesso sito.

## Come si divide il lavoro in Pull Request

Cinque PR in sequenza: la successiva parte da `main` dopo il merge della precedente.

1. `feat/dominio-anagrafica`: `Utente`, `Dipendente`, `DipendenteRuolo`, `Patente`, `Team` e i loro `enum`, `PortPlannerDbContext`, progetto `PortPlanner.Tests`. Il vecchio `User` del template resta finché il login non passa a `Utente` (PR 5).
2. `feat/dominio-pianificazione`: `Sito`, `Barca`, `GestioneBarca`, `AssegnazioneTeam`, `SlotNonLavorativo`, `AssegnazionePersona`, `Slot`, `TipoMerce`, `OrigineAssegnazione`.
3. `feat/seed`: `DataGenerator` con circa 25 lavoratori, 5 team, 4 siti e 5 barche; password di prova nel README.
4. `test/seed`: i test sul seed (un solo team per slot, nessuna persona in due siti nello stesso slot, ogni capogruppo è membro del suo team, barche senza sovrapposizioni sullo stesso sito).
5. `feat/login-profilo`: login agganciato a `Utente`, profilo nel claim (D15), test di login.

Fuori da queste PR: Richiesta, Assenza, Avviso, Notifica, disponibilità e preferenze, servizio di validazione degli avvisi, schermate.
