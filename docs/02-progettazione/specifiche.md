# PortPlanner — Documento di progetto (UX e flussi)

> Panoramica per chi inizia: ruoli, regole di dominio, convenzioni, schermate e flussi. Il dettaglio di ogni pagina è in [specifiche-dettagliate.md](<./specifiche-dettagliate.md>); **se i due documenti si contraddicono, vale quello dettagliato**. "Documento originale" = il documento di contesto iniziale del gruppo (Google Doc "PortPlanner | Contesto"). Mockup in [docs/03-mockup](<../03-mockup/>).

## 1. Contesto, ruoli e regole di dominio

PortPlanner è l'app con cui tre ruoli pianificano e consultano i turni di lavoro di un porto: il supervisore assegna i team alle barche, il capogruppo organizza il proprio team, il dipendente vede quando lavora. Il documento descrive ogni pagina, ogni stato e ogni collegamento del prototipo (bozze SVG 01–152), così da poterlo sviluppare in ASP.NET Core MVC con Bootstrap 5.

**Progetto:** caso 3 "Pianificazione turni porto", Laboratorio di Interfaccia Uomo-Macchina (UniBo Cesena, prof. Andrea Bianchi). Gruppo: Diego Andruccioli, Rei Mici, Giovanni Morelli.

### Ruoli

| Ruolo | Dispositivo | Cosa fa | Menu |
| --- | --- | --- | --- |
| Supervisore (es. Mario Rossi) | Desktop 1440×900 | Assegna team a barche, giorni e slot; gestisce team, utenti e richieste | Home, Pianificazione, Team, Richieste, Utenti, Ore, Assenze |
| Capogruppo (es. Anna Conti, Team 2) | Desktop 1440×900 | Decide quando lavora il proprio team sulle barche che gestisce; risponde alle richieste; legge gli avvisi | Home, Richieste, Avvisi |
| Dipendente con contratto (es. Sara Fontana) | Mobile 390×844 | Vede quando deve lavorare (giorno e slot) | Turni, Profilo |
| Dipendente a chiamata (es. Luigi Marini) | Mobile 390×844 | Vede i turni, indica la disponibilità settimanale e le preferenze | Turni, Disponibilità, Profilo |

Il capogruppo non è un ruolo ma un incarico: può essere qualsiasi lavoratore e usa l'app con il menu del capogruppo.

### Regole di dominio

- **Slot:** ogni giorno ha 4 slot, 0–6, 6–12, 12–18 e 18–24. **Un solo team per slot** su una barca; lo stesso team può lavorare nello stesso slot su siti diversi con persone diverse.
- **Due livelli di assegnazione, il più preciso vince.** La 2.0 è l'assegnazione puntuale (giorno + slot) del supervisore e il capogruppo non la contesta. La 3.0 assegna un team alla gestione della barca per tutto il periodo (da arrivo a partenza): il capogruppo sceglie quando lavorare e con chi negli slot non bloccati dal supervisore.
- **Poco bloccante, ma con avvisi.** Il supervisore può sempre inserire un altro team anche in una barca gestita: compare un avviso e il capogruppo gestore viene avvisato, senza campo motivo e senza possibilità di contestare.
- **Capogruppo fisso:** resta tale anche se non lavora nel turno o è assente; nessun sostituto. Per rimuoverlo, spostarlo o eliminarlo si sceglie prima un nuovo capogruppo tra i membri.
- **Un lavoratore sta in un solo team.** Lo spostamento da Gestione team è definitivo; con "Richiedi dipendenti" vale solo per lo slot e deve accettarlo il capogruppo del team della persona (o il supervisore al suo posto).
- **Slot futuri:** se un lavoratore viene spostato o rimosso dal team i suoi slot restano assegnati; se viene eliminato vengono tolti. Non si conserva storico dopo l'eliminazione e non esiste "Disattiva".
- **Patente:** un lavoratore ha un ruolo principale e può avere altri ruoli abilitati (Gruista, Muletista, Operaio); l'Operaio non richiede patente e lo può fare chiunque. Le patenti sono una per tipo (Gru, Muletto), ognuna con scadenza e stato calcolato (valida, scaduta, non posseduta). Si può assegnare una persona a un ruolo senza patente valida: compare l'anomalia "assegnato senza patente valida" e un toast, senza finestra di conferma. Un lavoratore fa un solo ruolo per slot.
- **Disponibilità** (solo dipendenti a chiamata): la imposta il lavoratore a giorni ogni settimana; il supervisore può modificarla al suo posto.

* **Contratti e vincoli:** esistono due contratti. **A contratto** (full-time): 36 o 38 ore a settimana (valore del singolo lavoratore), riposo di 11 ore consecutive tra i turni (per tutti i turni), riposo settimanale di 24 ore su una finestra mobile di 14 giorni (due riposi ogni 14 giorni, mai più di 10 giorni di fila), lavoro notturno con media massima di 8 ore nelle 24 solo per chi lavora di notte, straordinari fino a 300 ore all'anno con orario a 36 ore e 250 con orario a 38 ore (fonti: D.Lgs. 66/2003 e CCNL lavoratori dei porti, vedi [regole-contrattuali-verificate.md](<../01-ricerca/regole-contrattuali-verificate.md>)). **A chiamata:** nessun monte ore minimo, un turno al giorno, preavviso di 24 ore, disponibilità dichiarata. **Tutti i vincoli sono avvisi e non bloccano**; i soli blocchi veri sono la stessa persona su due siti nello stesso slot, una nave su più siti e la partenza di una nave dopo l'arrivo della successiva.
* **Preferenze di slot:** entrambi i contratti scelgono gli slot preferiti (0–6, 6–12, 12–18, 18–24) una volta sola e li cambiano raramente; non esistono preferenze di sito. La disponibilità a giorni riguarda solo chi è a chiamata e si imposta ogni settimana.

### Dati di esempio del prototipo

Oggi = martedì 17 novembre 2026. Siti 1–4; Barca A (Sito 1, Mar 17–Ven 20), Barca B (Sito 2, Mar 17–Sab 21), Barca C (Sito 4, Ven 20–Lun 23), Sito 3 libero. Team 2: capogruppo Anna Conti (gruista), membri Luca Moro, Giorgio Neri, Sara Fontana, Paolo Riva; gestisce Barca A.

## 2. Fondamenta del design e convenzioni

Tutte le pagine usano lo stesso linguaggio visivo (Bootstrap 5 UI Kit personalizzato) e le stesse regole di interazione, descritte una volta qui e richiamate nelle pagine.

### Fondamenta

| Voce | Scelta |
| --- | --- |
| Frame | Desktop 1440×900 (supervisore, capogruppo), Mobile 390×844 (dipendente) |
| Font | Inter nelle bozze e nel prototipo (font di sistema Bootstrap in codice); scala Bootstrap, testo 16 px, tabelle 14 px, pesi 400 e 700 |
| Colore primario | #0D6EFD; neutri = grigi Bootstrap; team senza colore proprio |
| Colori di stato | ok #198754, avviso #FFC107, errore #DC3545, info #0DCAF0 |
| Forme e griglia | Raggio 6 px, griglia 12 colonne con gutter 24 px, spaziature multiple di 4 e 8 px |
| Icone | Font Awesome, mai emoji; tema solo chiaro |

### Convenzioni di interazione

- **Mai solo il colore:** ogni stato è colore + icona + testo (es. "Patente non valida" con croce rossa).
- **Un solo pulsante primario per schermata**; le altre azioni sono outline. Le azioni distruttive sono rosse.
- **Poco bloccante, sempre con avvisi:** le anomalie e i conflitti si segnalano con riquadri gialli o azzurri e un pulsante "... comunque"; non si impedisce quasi mai un'azione del supervisore.
- **Conferma** per spostamento, rimozione, eliminazione, cambio di capogruppo, sostituzione di un team, slot occupato; **non** per aggiungere una persona senza team o per scelte reversibili.
- **Toast di conferma** in basso a destra (480 px, verde, spunta) dopo ogni azione, con link **Annulla** che ripristina lo stato precedente e chiusura automatica dopo circa 6 secondi. Su mobile occupa la larghezza della schermata.
- **Finestre (modal)** centrate sopra la pagina oscurata, con X in alto a destra; le "maschere" del documento originale sono modal, le "pagine" sono schermate intere.
- **Salvataggio automatico** dove la scelta è singola e reversibile (assegnazione di un team, scelta di uno slot); pulsante Salva dove si compila un modulo (Dettaglio utente, disponibilità, preferenze).
- **Stati vuoti** sempre disegnati: icona, frase che dice cosa manca e cosa succede quando arriverà.

### Corrispondenza con la numerazione del documento originale

| Documento originale | Nome in questo documento | Tipo |
| --- | --- | --- |
| 0 Live page | 3.1 Home (Live page) del supervisore | Pagina |
| 1 Live sito (sola visualizzazione) | 3.2 Maschera Live sito | Modal |
| 2 Dettaglio barca giorno X, sito Y | 3.4 Pagina 2.0 Dettaglio barca e giorno | Pagina |
| 2.1 Gestione team slot orario | 3.5 Pagina 2.1 Gestione team slot | Pagina |
| 3.0 Dettaglio totale barca | 3.6 Maschera 3.0 Dettaglio totale barca | Modal |
| 3 Gestione team | 3.8 Gestione team | Pagina |
| 4 Gestione utenti | 3.9 Gestione utenti | Pagina |
| 3.1 / 4.1 Dettaglio utente | 3.10 Dettaglio utente | Pagina |
| 5 Pianificazione settimanale di un sito | 3.3 Pianificazione (tutti i siti) | Pagina |

Nota: nel documento originale la voce "1.0 dettaglio giorno e barca" indicava l'assegnazione di un team a più slot; nel prototipo corrisponde alla pagina 2.0. Il click su una card barca della Vista Live apre la maschera Live sito (1), non la 1.0.

## 3. Ruolo Supervisore (desktop 1440×900)

Il supervisore ha la visione completa: vede tutti i siti, assegna i team alle barche, compone i team, gestisce le persone e può decidere al posto dei capigruppo. Sidebar blu a sinistra su ogni pagina: Home, Pianificazione, Team, Richieste (con badge delle richieste in attesa), Utenti, Ore, Assenze, più il menu utente in basso ("Mario Rossi · Supervisore"). Sotto i 992 px di larghezza la sidebar, anche del capogruppo, diventa un menu a scomparsa aperto da un pulsante. Ogni pagina qui sotto indica le bozze SVG di riferimento.

### 3.1 Home (Live page)

Bozze: 01 (e 03, home e 2.0 affiancate). Prima pagina dopo l'accesso: dice cosa succede ora e cosa è pianificato nei prossimi giorni.

- **Vista Live** (in alto): 4 card, una per sito. Card con barca presente (icona nave, "Sito 1 · Barca A") oppure sito libero (bordo tratteggiato, icona onda, "Libero").
- **Griglia** (sotto): una riga per sito, giorni sull'asse X con sigla e numero ("Mar 17"); la colonna di oggi ha intestazione primaria e etichetta "Oggi". Le barche sono barre azzurre dal giorno di arrivo a quello di partenza. Le celle mostrano il numero di team assegnati ("2 team"), vuote se nessuno. Segmented control **7 giorni / 31 giorni** a destra del titolo.
- **Collegamenti:** card barca → maschera Live sito (3.2); cella assegnata → pagina 2.0 (3.4); barra barca → maschera 3.0 (3.6); sidebar → le altre sezioni.
- **Vista a 31 giorni (143):** celle compatte con il numero di team per giorno, 25 giorni visibili con scorrimento orizzontale, barre barca e legenda; il controllo 7/31 giorni cambia vista.

### 3.2 Maschera Live sito

Bozze: 04 (con team), 05 (slot vuoto). Sola visualizzazione dello stato attuale di un sito; non modifica nulla.

- **Forma:** modal da 640 px sopra la home oscurata; si chiude con la X e si torna alla home.
- **Intestazione:** "Sito X · Barca Y" e la riga con l'orologio: slot corrente e ora ("Slot 6 – 12 · adesso 09:30").
- **Con team:** nome del team, badge Anomalia se presente, capogruppo e numero di membri, riquadro giallo con il testo specifico dell'anomalia, e gli operai divisi per ruolo (Gruisti, Mulettisti, Operai) con la persona in anomalia segnata con icona e testo.
- **Slot senza team:** icona, "Nessun team assegnato in questo slot" e la riga "Prossimo: slot 18 – 24 · Team 3".

### 3.3 Pianificazione (tutti i siti)

Bozze: 61 (7 giorni), 62 (31 giorni), 63 (evidenzia un team), 64 (solo anomalie). Nel documento originale "pianificazione settimanale di massima di un sito"; la scelta presa è una sola vista con tutti i siti e tutti i giorni.

- **Struttura:** colonne = giorni, righe = i 4 siti; ogni sito ha una riga sottile con la barra barca e 4 righe di slot. Un solo team per slot.
- **Periodo:** 7 giorni o 31 giorni da oggi; a 31 giorni le celle diventano compatte ("T2") con scorrimento orizzontale e etichette di sito e slot fisse a sinistra.
- **Cella:** nome del team su sfondo azzurro; scudo se assegnato dal supervisore; triangolo giallo se il team ha un'anomalia; grigio a righe se non lavorativo; bianca se vuota.
- **Strumenti:** menu "Team: tutti" (scegliendo un team si evidenziano i suoi slot e si attenuano gli altri), chip "Solo con anomalie" e legenda sotto la griglia.
- **Collegamenti:** cella (anche vuota) → 2.0 di quel giorno e barca; barra barca → 3.0. Non c'è assegnazione diretta dalla griglia: per assegnare si passa dalla 2.0.

### 3.4 Pagina 2.0 — Dettaglio barca, giorno e sito

Bozze: 02, 16 (conferma sostituzione), 17 (dopo la sostituzione), 21 (conferma inserimento in barca gestita), 22 (dopo l'inserimento). È la pagina dove il supervisore assegna un team a un singolo slot (assegnazione puntuale, che prevale sempre).

- **Layout:** due colonne. A sinistra la card "Slot orari" con 4 righe (0–6, 6–12, 12–18, 18–24); a destra la card "Team" con la lista dei team, in card chiuse (nome, capogruppo, numero membri, badge Anomalia) o aperte (con chevron, mostrano la lista membri con ruolo e il dettaglio dell'anomalia, ad esempio "Patente muletto scaduta il 02/11").
- **Intestazione:** link "← Home", titolo "Barca X · Sito Y", data con frecce ← → per cambiare giorno. Se la barca è gestita da un team compare "Barca gestita da Team 2".
- **Assegnare un team:** si trascina la card team (maniglia a sinistra) in uno slot; lo slot vuoto è un'area tratteggiata "Trascina qui un team". Salvataggio automatico a ogni drop con toast. Nel prototipo il click simula il drag & drop.
- **Slot occupato:** se si trascina un team su uno slot già occupato, modal "Sostituire il team nello slot 6 – 12?" con team attuale, freccia, nuovo team e avviso "Le persone di Team 2 in questo slot verranno tolte"; pulsanti Annulla e Sostituisci; poi toast "Team 3 assegnato allo slot 6 – 12".
- **Slot non lavorativo:** dal menu ⋯ di uno slot il supervisore segna che un team non lavora in quello slot (riga grigia a righe, "Team 1 non lavora in questo slot"). Se poi si trascina comunque un team lì, si inserisce con avviso giallo non bloccante.
- **Barca gestita da un altro team:** inserire un team diverso nello slot di una barca gestita non è vietato; il modal "Questa barca è gestita dal Team 2. Il capogruppo del Team 2 sarà avvisato" ha il pulsante "Inserisci comunque" e nessun campo motivo. Toast: "Team 4 inserito in 12 – 18 · Team 2 avvisato". Lo slot ha il tag "Assegnato dal supervisore".
- **Regola:** un team può stare nello stesso slot su più siti; i team della lista non si bloccano dopo l'assegnazione, la divisione delle persone si fa nella 2.1.
- **Collegamenti:** click su una card team in uno slot → 2.1 (3.5); "← Home" e voce Home → home.

### 3.5 Pagina 2.1 — Gestione team slot orario

Bozze: 06 (base), 07 (dropdown Aggiungi), 08 (richiedi dipendenti), 09 (sezione vuota e toast), 10 (componente toast). Mostra e modifica chi lavora in quello slot.

- **Layout:** link "← Barca A · Sito 1" (torna alla 2.0), titolo "Team 2", sottotitolo con barca, sito, giorno e slot. Tre sezioni impilate (Gruisti, Mulettisti, Operai), ognuna con conteggio e pulsante "+ Aggiungi". Il capogruppo compare tra i membri con l'etichetta "Capogruppo".
- **Riga persona:** nome, stato patente (pill con icona e testo), pulsante "Sostituisci" e X per rimuoverla dallo slot.
- **Ordine delle liste:** gruisti e mulettisti con la patente di quel ruolo valida, poi scaduta, poi senza patente; operai senza patente prima, con patente dopo; alfabetico dentro ogni gruppo. Chi non ha la patente valida si può assegnare lo stesso: la riga mostra "Anomalia: assegnato senza patente valida" e il toast dice "Carlo Fiore aggiunto come gruista, senza patente valida".
- **"+ Aggiungi" e "Sostituisci":** aprono lo stesso dropdown, che elenca solo i membri del team dello slot abilitati a quel ruolo, con ricerca e intestazioni per patente. Chi è già nello slot (anche con un altro ruolo) o è occupato in un altro sito è grigio e disabilitato con il motivo ("Già nello slot come muletista", "Occupata Sito 2"). Se non resta nessun membro compare "Nessun membro disponibile" con il pulsante "Richiedi dipendenti da altri team".
- **"Richiedi dipendenti da altri team":** pulsante outline in alto a destra; modal con un selettore di ruolo (Gruista, Muletista, Operaio) e una tabella (nome, ruoli, team, stato Libero/Non libero, patente del ruolo scelto, pulsante Richiedi). Ordine: prima i liberi e poi i non liberi; dentro ogni gruppo prima chi ha la patente di quel ruolo valida, poi scaduta, poi chi non ce l'ha (per gli operai: senza patente prima, con patente dopo), alfabetico. La richiesta indica il ruolo richiesto. Il modal elenca solo persone di altri team o senza team, mai i membri del proprio team. Per il supervisore il pulsante è "Aggiungi": la persona entra subito nello slot e il capogruppo del suo team riceve un avviso; per il capogruppo resta "Richiedi". Dopo "Richiedi" la riga diventa "Inviata"; la persona entra nel team solo quando il capogruppo del suo team accetta (flusso in 6.1).
- **Sezione di ruolo vuota:** resta visibile, con conteggio (0), "Nessun mulettista assegnato" e il pulsante Aggiungi.
- **Toast con Annulla** dopo ogni azione: "Anna Conti aggiunta al team", "Luca Moro sostituito con Marco Bruno", "Giorgio Neri rimosso dal team", "Richiesta inviata al capogruppo del Team 4".

* **Ordine dei candidati e vincoli contrattuali:** i dropdown "+ Aggiungi / Sostituisci" e il modal "Richiedi dipendenti" ordinano in cascata: 1) liberi prima dei non liberi; 2) patente del ruolo valida, scaduta, senza; 3) vincoli contrattuali, prima chi non ha avvisi e poi chi ne ha; 4) ore mancanti al monte ore settimanale (solo A contratto); 5) preferenza di slot; 6) alfabetico. La patente viene prima dei vincoli. **Tutti i criteri sono visibili allo stesso modo: ogni candidato ha la pill della patente, la riga di avviso dei vincoli, la pill "Mancano 14 h" (ore al monte settimanale) e la pill "Preferisce slot"; una nota sotto la lista riporta l'ordin**e. Le ore mancanti sono quelle confermate; se la persona ha una richiesta in attesa compare anche il valore con la richiesta accettata ("Mancano 6 h · 0 h con richiesta in attesa"), ma l'ordine usa solo le confermate. Un candidato che violerebbe un vincolo resta selezionabile e ha una riga di avviso con icona e testo ("Riposo inferiore a 11 h dopo Lun 16 · 18 – 24", "Supera le 36 h settimanali", "Disponibilità non dichiarata"); nessuna conferma e toast "assegnato, con 1 avviso".
- **Voce del dropdown:** nome, pill ore mancanti e preferenza, riga di avviso (vincolo, assenza, secondo turno); la patente è data dal gruppo e non si ripete.

### 3.6 Maschera 3.0 — Dettaglio totale barca

Bozze: 18 (base), 19 (senza team), 20 (giorno bloccato), 90 (scelta del team), 91 (conferma cambio team), 92 (team cambiato), 80 (conferma blocco giorno), 81 (giorno bloccato con capogruppo avvisato). Il supervisore assegna **un team alla gestione della barca per tutto il periodo**; sarà poi il capogruppo a decidere quando lavorarci e con chi. Il supervisore può segnare gli slot o i giorni in cui il team non deve lavorare.

- **Forma:** modal largo (1040 px) sopra la home oscurata, con X in alto a destra.
- **Intestazione:** "Barca A · Sito 1", periodo ("Mar 17 nov → Ven 20 nov 2026 · 4 giorni"), selettore "Team che gestisce la barca" (team, capogruppo e membri; vuoto = "Seleziona un team") e due pill di riepilogo ("10 slot lavorabili", "6 non lavorativi").
- **Griglia:** giorni sull'asse X (solo quelli della barca), 4 slot sull'asse Y. Celle: **Lavorabile** (predefinita), **Non lavorativo** (grigio a righe), **Già deciso dal capogruppo** (azzurro, sola lettura, "Team 2 lavora"), **Inserito dal supervisore** (bordo primario, scudo, sola lettura). Ogni giorno ha il pulsante "Blocca giorno" (diventa "Ripristina giorno").
- **Interazione:** il click su una cella lavorabile la segna non lavorativa e viceversa; "Blocca giorno" segna l'intera giornata. Dopo ogni azione compare il toast con Annulla ("Gio 19 nov segnato come non lavorativo").
- **Senza team assegnato:** celle spente e testo guida "Assegna un team alla barca per tutto il periodo". La **prima assegnazione è immediata**, senza conferma.
- **Blocco di un giorno con slot già scelti dal capogruppo:** "Blocca giorno" apre la conferma "Segnare Ven 20 nov come non lavorativo?" con tre riquadri: gli slot scelti verranno tolti, persone e richieste in attesa verranno annullate, il capogruppo sarà avvisato. Pulsanti Annulla e "Blocca comunque"; toast "Ven 20 nov segnato come non lavorativo · Team 2 avvisato". Un giorno senza slot scelti si blocca senza finestra (flusso in 6.2).
- **Cambio del team gestore:** il selettore apre l'elenco dei team (il team attuale ha la pill "Attuale"). **Sostituire un team già assegnato chiede sempre conferma:** "Cambiare il team che gestisce Barca A?" con "Da Team 2 → A Team 4", slot scelti dal vecchio team tolti, persone e richieste annullate, slot imposti dal supervisore e giorni non lavorativi invariati, entrambi i capigruppo avvisati. Pulsanti Annulla e "Cambia team"; toast "Barca A ora gestita dal Team 4 · 2 capigruppo avvisati" (flusso in 6.3).

### 3.7 Richieste

Bozze: 11 (in attesa), 12 (accettata con toast), 13 (Accettate), 14 (nessuna in attesa), 15 (Rifiutate). Il supervisore vede **tutte** le richieste di tutti i team e può accettare o rifiutare al posto di un qualsiasi capogruppo.

- **Struttura:** titolo "Richieste", banner azzurro "Come supervisore puoi accettare o rifiutare le richieste al posto di qualsiasi capogruppo", schede **In attesa / Accettate / Rifiutate** con contatore, filtri (ricerca persona, team e capogruppo, giorno nei prossimi 7), tabella.
- **Colonne:** persona richiesta (nome, ruolo · team), richiesta da (team richiedente, "da A. Conti" oppure "da Mario Rossi (supervisore)"), capogruppo a cui è rivolta, dove e quando, azioni.
- **Azioni:** **Accetta** (primario) e **Rifiuta** (outline), immediate, senza motivo. La riga esce da "In attesa", il contatore e il badge in sidebar scendono e compare il toast "Richiesta accettata al posto di S. Fabbri" con Annulla (riporta la richiesta in attesa).
- **Schede Accettate/Rifiutate:** nessun pulsante; colonna "Stato e decisione" con pill (icona + testo) e chi ha deciso ("Deciso da L. Bianchi" oppure "Mario Rossi, al posto di A. Gallo").
- **Stato vuoto:** "Nessuna richiesta in attesa. Le nuove richieste dei capigruppo compariranno qui."
- **Stati di una richiesta:** In attesa, Accettata, Rifiutata (e ritirata con Annulla dal richiedente).
- **Scadenza (145):** una richiesta in attesa scade quando inizia lo slot richiesto e passa nella scheda **Scadute** (sola consultazione).
- **Filtri (147–148):** Giorno (Oggi, Prossimi 7 giorni predefinito, Prossimi 31 giorni, Tutti) e Team con capogruppo.

### 3.8 Gestione team

Bozze 23–47. Configurazione dei team: tutti i team, CRUD sui team e sui singoli lavoratori. Voce "Team" attiva in sidebar.

- **Layout:** due colonne. A sinistra l'elenco "Team (5)": nome, badge Anomalia se serve, capogruppo, numero membri e composizione ("6 membri · 2 gruisti · 1 muletista · 3 operai"); la voce selezionata ha bordo primario. A destra il dettaglio del team selezionato. Pulsante primario **"Nuovo team"** in alto a destra.
- **Dettaglio:** titolo con badge Anomalia, pulsanti Modifica (outline) ed Elimina (outline rosso), capogruppo, avviso giallo con il dettaglio dell'anomalia, sezioni Gruisti / Mulettisti / Operai con "+ Aggiungi"; ogni riga ha nome, patente, **Sposta** e X (rimuovi dal team, la persona resta in anagrafica).
- **Nuovo team** (modal): nome e capogruppo; il capogruppo può essere solo una **persona senza team**. Il team nasce con il solo capogruppo e il toast "Team 6 creato". **Modifica** (modal): nome e capogruppo, scelto tra i membri del team.
- **Elimina team:** la conferma elenca le conseguenze: i membri restano in anagrafica senza team, la barca gestita dal team resta senza team, gli slot assegnati nella 2.0 vengono liberati; pulsante rosso "Elimina team".
- **"+ Aggiungi":** dropdown con ricerca, in cima i senza team, poi i gruppi per patente; una persona senza team viene aggiunta subito, senza conferma, con toast. Chi è già in un altro team ("ora in Team 4") viene **spostato definitivamente** dopo conferma. Gestione team non crea né elimina lavoratori: quello è di Gestione utenti.
- **Sposta:** per un membro non capogruppo si apre "Spostare Sara Fontana in un altro team" con la scelta del team (radio con capogruppo e membri), poi la conferma di spostamento definitivo (card "Da Team 4 → A Team 2", avviso che gli slot futuri restano assegnati, i due capigruppo avvisati), poi il toast.
- **Rimuovere (X):** conferma "Rimuovere Sara Fontana dal Team 2?" con tre informazioni (resta in anagrafica senza team; slot futuri restano assegnati; il capogruppo sarà avvisato) e toast.
- **Capogruppo:** rimuovere o spostare il capogruppo obbliga a scegliere un nuovo capogruppo (finestra con radio, pulsante disabilitato finché non si seleziona qualcuno). Rimuovere o spostare l'**ultimo membro** avvisa che verrà eliminato l'intero team ("Rimuovi ed elimina team").
- **Nessun team:** icona, "Nessun team", "Crea il primo team per iniziare." e pulsante "Nuovo team".

### 3.9 Gestione utenti

Bozze 48–53, 98. Anagrafica completa dei lavoratori in tabella con CRUD, ricerca e filtri. Voce "Utenti" attiva.

- **Tabella (4 colonne):** nome e cognome (avatar a iniziali, badge Anomalia), ruoli (chip: il principale in azzurro, gli altri ruoli abilitati in bianco), team (nome con etichetta "Capogruppo" oppure pill "Senza team"), patenti (una pill per patente: "Gru valida", "Muletto scaduta", "Nessuna patente"). A destra le icone Modifica ed Elimina. Il click sulla riga apre il Dettaglio utente.
- **Ordinamento e paginazione:** alfabetico per cognome, colonne ordinabili dall'intestazione, 10 righe per pagina ("1–10 di 42 lavoratori").
- **Filtri:** campo "Cerca per nome", chip **"Solo con avvisi"** (avvisi sui dati: patente, contratto; attivo: "3 risultati su 42 lavoratori" e "Cancella filtri") e chip "Con avvisi sui turni" (vincoli contrattuali dei prossimi 7 giorni, badge arancione "Turni: 1 avviso" nella riga), menu Patente, Team (con "Senza team" e conteggi), Ruolo (trova chi ha quel ruolo anche se non è il principale), Contratto. Con zero risultati: "Nessun lavoratore trovato" e "Cancella filtri".
- **Nuovo lavoratore** (pulsante primario): apre il Dettaglio utente vuoto in modalità creazione.
- **Eliminare un lavoratore:** modal "Eliminare Sara Fontana?" con "L'eliminazione è definitiva e non si può annullare." e quattro riquadri: team di appartenenza, slot futuri tolti, richieste in sospeso annullate, storico cancellato. Pulsanti Annulla e "Elimina definitivamente"; toast "Sara Fontana eliminata".

### 3.10 Dettaglio utente

Bozze 54–60, 93–97. Anagrafica completa di un lavoratore, in vista, modifica e creazione.

- **Vista:** link "← Gestione utenti", titolo con badge Anomalia e/o Capogruppo, sottotitolo "Ruolo · Contratto", azioni **Modifica** ed **Elimina**; avviso giallo se ci sono anomalie. Colonna sinistra: Anagrafica (nome, cognome, ruoli abilitati con il principale in azzurro, tipo di contratto a contratto/a chiamata, data di assunzione), Patenti (due righe fisse, Gru e Muletto, ciascuna con stato calcolato e scadenza) e Contatti. Colonna destra: Team (con "Gestione team" o "Senza team"), Turni futuri e, solo per chi è a chiamata, Disponibilità.
- **Turni futuri:** i prossimi 5 slot (data, barca · sito, slot, ruolo con cui è assegnato, con triangolo se manca la patente valida), il click apre la 2.0 di quel giorno; "Vedi tutti i 12 turni" apre un modal con le schede **Futuri (12) / Passati (38)** (i passati sono solo consultazione). Stato vuoto: "Nessun turno assegnato".
- **Modifica:** tutta la pagina diventa modificabile con **Salva** (primario) e **Annulla**; i campi obbligatori hanno \*; il team non si cambia qui. Dopo il salvataggio torna la vista con toast "Dati di Giorgio Neri aggiornati". Per i lavoratori a chiamata la scheda Disponibilità diventa modificabile: selettore settimana, un interruttore per giorno e chip per gli slot preferiti; i giorni con un turno restano bloccati.
- **Nuovo lavoratore:** obbligatori nome, cognome, almeno un ruolo e tipo di contratto; gli errori compaiono sotto i campi ("Il cognome è obbligatorio"). Dopo "Crea lavoratore" si apre la vista del nuovo lavoratore, senza team, con toast "Lavoratore creato: Giulia Bassi"; Annulla annulla la creazione.
- **Eliminare:** stessa conferma della lista; dopo la conferma si torna a Gestione utenti con la riga rimossa e il toast "Giorgio Neri eliminato". Per un capogruppo la finestra chiede prima il nuovo capogruppo tra i membri e il pulsante diventa "Elimina e nomina capogruppo".

* **Ore settimanali, preferenze e vincoli:** per chi è a contratto il campo "Ore settimanali" (36 h di default) sta accanto alla data di assunzione; per chi è a chiamata compare "Nessun monte ore". La scheda "Preferenze di slot" mostra gli slot scelti dal lavoratore. Il link "1 avviso · Vincoli contrattuali" nella card Turni futuri apre un modal con sei controlli, ciascuno nei limiti, con avviso o non applicabile: ore settimanali (confermate e con le richieste in attesa), riposo di 11 h tra due turni, turni consecutivi, riposo settimanale (due riposi da 24 h in 14 giorni, al massimo 10 giorni di fila), lavoro notturno (media di 8 h nelle 24, solo per chi lavora di notte) e straordinari annui.

### 3.11 Ore e straordinari

Bozze 103–108. Voce **Ore** nella sidebar, sotto Utenti. Le ore sono **pianificate** (ogni turno vale 6 h) e il supervisore può correggerle.

- **Pagina Ore (103):** tabella con un lavoratore per riga: contratto, ore della settimana su monte (barra), ore del mese, straordinari nell'anno su limite (300 h con orario a 36 h, 250 h con 38 h, CCNL porti art. 8). Sopra l'80% del limite compare la pill "Vicino al limite". Filtri: ricerca per nome, "Vicini al limite annuo", contratto; frecce per cambiare settimana. Il click su una riga apre il riepilogo del lavoratore.
- **Riepilogo ore (104 Settimana, 108 Mese, 105 Anno):** modal con tre riquadri (ore pianificate, monte ore, straordinari), barra di avanzamento e tabella dei turni con stato Svolto, In corso, Previsto o Non svolto. La scheda Anno mostra 52 di 300 h con il grafico per mese. Si apre anche dal link "Ore e straordinari" nei Turni futuri del Dettaglio utente (3.10).
- **Segnare un turno come non svolto (106, 107):** dal menu ⋯ di un turno passato o in corso. La conferma mostra l'effetto (settimana da 30 h a 24 h, mese da 106 h a 100 h), avvisa che il dipendente riceve la notifica "Correzione delle tue ore" e ricorda che si può annullare. Dopo la conferma il toast "Turno di Lun 16 segnato come non svolto · Giorgio Neri avvisato" ha Annulla.
- **Straordinario** = ore oltre il monte settimanale. I lavoratori a chiamata non hanno monte né straordinari: si contano solo ore e giornate.

### 3.12 Assenze

Bozze 115–126 (più 123–125 nella pagina 2.1). Voce **Assenze** nella sidebar, sotto Ore. Ferie e permessi si richiedono e si approvano; la malattia è un fatto che il supervisore registra subito.

- **Calendario (115):** persone × giorni del mese, barre per tipo con icona e sigla (Ferie, Permesso, Malattia), tratteggiate se in attesa. Pulsante "Registra assenza".
- **Richieste (116–120):** card con tipo, periodo, motivo e **turni già assegnati coinvolti**. Approva (conferma 117 con l'elenco dei turni che diventano da coprire, capogruppo e dipendente avvisati) o Rifiuta (motivo facoltativo, 119). Toast con Annulla (118, 120).
- **Registra assenza (121–122):** lavoratore, tipo, da/a a giorni interi, motivo facoltativo; la malattia vale subito e mostra i turni che diventano da coprire.
- **Turni colpiti:** restano assegnati e diventano scopertura da assenza (125: "Assente: malattia 17 – 19 nov · turno scoperto"); si sostituiscono a mano.
- **Candidati (123–124):** chi è assente va in fondo con badge rosso "In ferie · 16 – 20 nov", selezionabile con conferma "Inserisci comunque"; mai un blocco.
- **Dettaglio utente (126):** link "Assenze" nella card Turni futuri con prossime assenze e storico.
- **Ore:** le assenze non contano come ore lavorate (nessuna ora figurativa).
- **Rifiuto di un permesso (149–150):** finestra con motivo facoltativo e toast con Annulla.

### 3.13 Cambio contratto

Bozze 131–134. Link **Cambia contratto ›** nel Dettaglio utente, accanto al tipo di contratto.

- **Finestra (131, 133):** nuovo contratto, monte ore 36 h o 38 h se si passa a contratto, riepilogo degli effetti, conferma. Vale subito.
- **Turni futuri:** restano assegnati; gli avvisi (più turni nello stesso giorno, disponibilità non dichiarata) sono elencati nella conferma.
- **Dati:** da contratto a chiamata monte e straordinari non si applicano (lo storico resta); da chiamata a contratto la disponibilità dichiarata si cancella e le preferenze restano. Il contatore straordinari annuo prosegue.
- **Esito (132, 134):** toast con Annulla; riga "Dal 17 nov 2026 · prima: a contratto, 36 h" nel Dettaglio. Avvisati lavoratore (campanella) e capogruppo (Avvisi); nell'app la barra segue il nuovo contratto.

### 3.14 Vincoli per i chiamati

Bozze 135–137. Riposo di 11 h per tutti i turni, un turno al giorno e disponibilità per i chiamati sono avvisi, mai blocchi.

- **Modal Vincoli di un chiamato (135):** tre righe (un turno al giorno, riposo 11 h, disponibilità rispettata), ciascuna nei limiti o con avviso; nessun monte ore né straordinari.
- **Secondo turno nello stesso giorno (136–137):** avviso giallo nel dropdown e conferma "Inserisci comunque"; il turno resta con l'anomalia e il capogruppo è avvisato.
- **Dipendente:** non vede gli avvisi di vincolo, solo il badge "Preavviso breve".

### 3.15 Capogruppo assente

Bozze 138–140 (badge anche in 01, 11, 23, 115). Il capogruppo resta un incarico fisso e **non ha sostituto**: se è assente, copre il supervisore.

- **Richieste:** quelle destinate al capogruppo assente compaiono subito nella pagina Richieste del supervisore con il badge rosso "Assente 18 – 20 nov"; gli Avvisi restano al capogruppo fino al rientro.
- **Evidenza:** badge accanto al capogruppo in Gestione team e nella 2.1, con la riga "Anna Conti (capogruppo) è assente: richieste e avvisi del team passano a te".
- **Barca gestita:** gli slot già scelti restano; il supervisore può modificarli dalla 2.0.
- **Live page (01):** marcatore rosso sulle celle del team con capogruppo assente (Mer 18 – Ven 20) e voce in legenda.
- **Conferma di approvazione (139–140):** approvando le ferie di un capogruppo la finestra avvisa che richieste e avvisi del team passano al supervisore, senza sostituto.

## 4. Ruolo Capogruppo (desktop 1440×900)

Il capogruppo (esempio: Anna Conti, Team 2) vede la stessa Live del supervisore, ma sotto vede solo la programmazione del proprio team e può organizzarla entro i limiti decisi dal supervisore. Sidebar con tre voci: **Home, Richieste** (badge delle richieste in attesa) e **Avvisi** (badge dei non letti). Non ha Pianificazione, Team né Utenti: la programmazione del team è già nella home.

### 4.1 Home (Live page)

Bozza: 65. Corrisponde al "live sito modificato per la visualizzazione capogruppo" del documento originale.

- **Vista Live:** uguale a quella del supervisore (4 siti; il click su una card apre la maschera Live sito), con il sito dove lavora ora il suo team evidenziato da bordo primario più spesso e pill "Il tuo team".
- **Riepilogo** in due riquadri: "Prossimo turno del tuo team" (giorno, slot, barca · sito, più la riga "Adesso in turno: ...") e "Da pianificare" (riquadro giallo: "Barca A · 4 slot da scegliere") con il pulsante primario **Pianifica**, che apre la 3.0 del capogruppo.
- **Griglia "Programmazione del tuo team":** stessa struttura della home del supervisore (4 siti × giorni, 7 o 31 giorni, oggi in primario) ma con la sola programmazione del suo team. Ogni giorno mostra **4 mini-segmenti** (uno per slot) e il testo "N slot": azzurro pieno = il team lavora (scelto dal capogruppo); scudo = imposto dal supervisore, non modificabile; a righe = non lavori (bloccato dal supervisore); tratteggio primario = da scegliere (solo barca gestita); grigio con "T1" = occupato da un altro team.
- **Barre barca:** la barca gestita dal suo team è una barra primaria con "Barca A · Gestisci tu"; le altre barche sono barre grigie senza click. I siti dove il team non lavora hanno la riga grigia.
- **Collegamenti:** "Pianifica" o barra "Gestisci tu" → 3.0 capogruppo (4.3); slot imposto dal supervisore (scudo) → 2.1 capogruppo (4.2); Richieste → 4.4; Avvisi → 4.5.
- **Maschera Live sito (141–142):** sola lettura come quella del supervisore; con il suo team nello slot compaiono il tag "Il tuo team" e, se la barca è sua, il link "Pianifica la barca" (3.0).
- **Vista a 31 giorni (144):** 4 mini-segmenti e numero di slot per giorno, con scorrimento orizzontale.

### 4.2 Pagina 2.1 — Gestione team slot (versione capogruppo)

Bozze: 66 (slot imposto), 67 (richiedi dipendenti), 70 (slot scelto dal capogruppo). Non esiste una 2.0 per il capogruppo: la pagina si apre direttamente dallo slot della griglia e il link in alto è "← Home" (oppure "← Barca A · Sito 1" se arriva dalla 3.0).

- **Slot imposto dal supervisore:** banner azzurro con scudo "Slot assegnato dal supervisore: puoi scegliere chi del tuo team lavora, non il team". Il capogruppo gestisce **solo le persone del suo team**: Aggiungi, Sostituisci e X restano attivi, ma team e slot non si contestano.
- **Slot scelto dal capogruppo (barca gestita):** stessa pagina **senza banner**; il pulsante "Persone" della 3.0 porta qui (bozza 70).
- **Richiedi dipendenti da altri team:** disponibile anche al capogruppo, con il suo nome come richiedente ("da A. Conti" nella pagina Richieste). Stesso modal del supervisore; la richiesta va al capogruppo dell'altro team (stato "Inviata").
- Sezioni Gruisti, Mulettisti, Operai, ordinamenti e toast con Annulla identici a 3.5.

### 4.3 Maschera 3.0 — Pianifica la barca (versione capogruppo)

Bozze: 68 (base), 69 (slot scelto con toast). Il capogruppo vede che la barca è stata assegnata al suo team e sceglie **quando il suo team lavora** nel periodo disponibile.

- **Accesso:** pulsante "Pianifica" del riquadro "Da pianificare" o click sulla barra "Gestisci tu". Modal largo 1040 px sopra la home capogruppo; X per chiudere.
- **Intestazione:** "Barca A · Sito 1", periodo, box di sola lettura "Barca assegnata al tuo team · Team 2 · Sei il capogruppo" (nessun selettore del team), pill "N slot scelti" e "N da scegliere" (stessi numeri della home).
- **Griglia** giorni × 4 slot, senza "Blocca giorno" (il capogruppo non può bloccare). Celle: **Il team lavora** (azzurra, spunta, pulsante "Persone"), **Da scegliere** (tratteggio primario con "+"), **Non lavori** (a righe, deciso dal supervisore), **Imposto dal supervisore** (scudo, sola lettura), **Altro team** (grigio, "Inserito dal supervisore"). Un giorno tutto bloccato ha l'intestazione "Giornata non lavorativa".
- **Interazione:** un click su una cella "Da scegliere" fa lavorare il team in quello slot; salvataggio automatico con toast "Mar 17 · 12 – 18 aggiunto ai turni del Team 2" e Annulla. Un click su una cella "Il team lavora" la toglie.
- **Persone:** lo slot scelto vale per **tutto il team** di default; "Persone" apre la 2.1 senza banner per cambiare chi lavora.

### 4.4 Richieste ricevute

Bozze 71–75. Le richieste di altri team per le persone del proprio team: il capogruppo decide se accettare o rifiutare.

- **Struttura:** come la pagina del supervisore ma **solo le sue richieste**, senza banner (non agisce per conto di nessuno): titolo "Richieste ricevute", schede In attesa / Accettate / Rifiutate con contatore (il badge in sidebar è il numero in attesa), filtri (cerca persona, team richiedente, giorno).
- **Colonne (In attesa):** persona del suo team (nome, ruolo · Team 2), richiesta da (team e persona, anche "Mario Rossi (supervisore)"), dove e quando, **cosa succede**, azioni.
- **Cosa succede:** riga con freccia "Lavorerà per il Team 4 in questo slot"; se la persona ha già un turno sovrapposto con il suo team compare un avviso giallo non bloccante, ad esempio "Ha già un turno con te (Gio 19 · 12 – 18)".
- **Azioni:** **Accetta** (primario) e **Rifiuta** (outline), immediate, senza finestra di conferma e senza motivo. Toast "Luca Moro lavorerà per il Team 4 in questo slot" con Annulla, che riporta la richiesta in attesa.
- **Schede Accettate/Rifiutate:** pill con icona e testo e decisione ("Accettata da te", "Mario Rossi, al posto tuo").
- **Stato vuoto:** "Nessuna richiesta in attesa. Quando un altro team chiede una persona del tuo team, la vedrai qui."
- **Scadute (146):** quarta scheda in sola consultazione, come per il supervisore.

### 4.5 Avvisi

Bozze 76–79. Raccoglie tutti i messaggi "il capogruppo sarà avvisato" generati dalle azioni del supervisore. Sono **solo informativi**: nessuna risposta, nessun commento, nessun Accetta/Rifiuta.

- **Struttura:** titolo "Avvisi", elenco cronologico (più recenti in alto), schede **Non letti (4) / Tutti (9)**; pulsante "Segna tutti come letti" (toast "4 avvisi segnati come letti" con Annulla). Il badge in sidebar è il numero dei non letti.
- **Card per avviso:** pallino primario e titolo in grassetto se non letto, icona del tipo, titolo, dettaglio, mittente e ora, e un link ("Vedi la barca" apre la 3.0 del capogruppo; "Vai alla home").
- **Tipi di avviso:** il supervisore inserisce un altro team in una barca gestita; uno slot o un giorno viene segnato non lavorativo; un membro viene spostato nel team o rimosso; il team inizia o smette di gestire una barca (cambio del team gestore); un membro del team viene assegnato con un vincolo contrattuale violato (ad esempio riposo inferiore a 11 h); il supervisore aggiunge un membro del team a un altro team per uno slot.
- **Stati vuoti:** "Nessun avviso da leggere" (scheda Non letti) e "Nessun avviso" (mai ricevuti).

Avviso da assenza (bozza 76–77): quando un membro del team ha turni colpiti da una malattia registrata o da ferie e permessi approvati, il capogruppo trova un avviso informativo (es. "Luca Moro è in malattia 17 – 19 nov: 2 turni del tuo team da coprire").
Avviso di esito (bozza 76–77): il capogruppo che ha richiesto una persona trova un avviso informativo quando la richiesta è accettata, rifiutata o scaduta (es. "Richiesta accettata: Elena Ricci (Team 1)").

## 5. Ruolo Dipendente (mobile 390×844)

Il dipendente usa il telefono. Ci sono due varianti: **con contratto**, che vede solo quando deve lavorare (giorno e slot), e **a chiamata**, che in più indica la disponibilità ogni settimana; le preferenze di slot, scelte una volta sola, valgono per entrambi. Esempi: Sara Fontana (Operaio, con contratto, Team 2) e Luigi Marini (Muletista e Operaio, a chiamata, Team 3, capogruppo A. Gallo). I turni sono **solo informativi**: il dipendente non conferma né rifiuta; se ha un problema contatta il capogruppo.

**Navigazione:** barra inferiore. Con contratto: *Turni* e *Profilo*; a chiamata: *Turni*, *Disponibilità* e *Profilo*. Barra superiore con il titolo della pagina, la campanella delle notifiche e l'avatar a iniziali.

### 5.1 Turni (con contratto)

Bozze: 82 (con turni), 83 (nessun turno).

- **Card "Oggi":** in evidenza con bordo primario: data, pill "In corso adesso", slot in grande ("6 – 12"), barca · sito, team e capogruppo, ruolo.
- **"Prossimi turni":** elenco raggruppato per giorno ("GIO 19 NOV"); ogni card mostra lo slot a sinistra e a destra barca · sito, team · capogruppo e ruolo. Non si mostrano i colleghi.
- **Stato vuoto:** icona, "Nessun turno in programma" e "Quando il supervisore ti assegna un turno lo vedrai qui".
- **Collegamenti:** barra inferiore → Profilo.

* **Contatore ore e notifiche:** sotto la card Oggi c'è "Questa settimana · 24 di 36 h" con barra e link **Le mie ore** (5.6); per i chiamati "Questo mese · 48 h · 8 giornate pianificate". Se il turno è assegnato con meno di 24 h di preavviso la card Oggi mostra la pill **Preavviso breve** (regola del cliente). Nella barra superiore la **campanella** con il numero di notifiche non lette apre 5.7.

### 5.2 Profilo

Bozze: 84 (con contratto), 89 (a chiamata). Dati in **sola lettura**; le modifiche le fa il supervisore ("Per modificare i tuoi dati contatta il supervisore").

- **Contenuto:** avatar, nome, "Ruolo · Tipo di contratto"; card Team (nome e capogruppo); card Patenti (una pill per patente posseduta, con tipo, stato e scadenza; "Nessuna patente" se non ne ha); card Contatti (telefono, email).
- **A chiamata:** in più la card **Preferenze** ("Slot 6 – 12, 12 – 18") con un chevron che apre 5.5; vale anche per chi ha il contratto.
- **Esci:** pulsante outline in fondo.

### 5.3 Turni (a chiamata)

Bozza: 85. Come 5.1, con in più un riquadro azzurro in cima, "Disponibilità non inviata · Prossima settimana (23 – 29 nov)", con il pulsante **Imposta** che apre 5.4. Se oggi non ha turni, la card mostra "Nessun turno oggi".

### 5.4 Disponibilità (solo a chiamata)

Bozze: 86 (da compilare), 87 (salvata con toast). La disponibilità si indica **a giorni**, non per slot.

- **Selettore di settimana** con frecce ← →; di default la settimana successiva ("23 – 29 nov · Prossima settimana").
- **Elenco dei 7 giorni**, una riga ciascuno con un interruttore "Disponibile". I giorni in cui ha un turno sono bloccati: interruttore acceso e disabilitato con la scritta "Hai un turno · 12 – 18 · Barca C".
- **Conteggio** "4 giorni disponibili" e pulsante primario **Salva disponibilità**. Dopo il salvataggio il pulsante diventa disabilitato e compare il toast "Disponibilità salvata per 23 – 29 nov" con Annulla.
- Il supervisore può vedere e modificare la stessa disponibilità dal Dettaglio utente (3.10).

### 5.5 Preferenze di slot (entrambi i contratti)

Bozza: 88. Si impostano una volta sola dal Profilo; il supervisore le usa come indicazione quando assegna i turni.

- **Slot preferiti:** quattro chip selezionabili (0–6, 6–12, 12–18, 18–24).
- **Siti:** non esistono preferenze di sito. Un chip selezionato è pieno primario con spunta; non selezionato è outline.
- Pulsante primario **Salva preferenze**; il link ← in alto riporta al Profilo.

### 5.6 Le mie ore

Bozze: 109 (con contratto), 110 (a chiamata). Si apre dal link nei Turni o dalla card "Ore e straordinari" del Profilo.

- **Con contratto:** "24 di 36 h" con barra, straordinari della settimana, straordinari nell'anno (60 di 300 h) e l'elenco dei turni con stato Non svolto, In corso o Previsto; una nota spiega quando il supervisore ha corretto un turno.
- **A chiamata:** ore della settimana e del mese, giornate pianificate, nessun monte ore e nessuno straordinario.
- Sola lettura: le correzioni le fa il supervisore (3.11).

### 5.7 Notifiche

Bozze: 111 (con contratto), 112 (a chiamata), 113 e 114 (tutte lette, con toast e Annulla). Elenco cronologico con pallino azzurro sui non letti, schede Non letti e Tutte, azione "Segna tutte come lette".

- **Eventi sui turni:** nuovo turno assegnato (con badge "Preavviso breve" se meno di 24 h), turno modificato, turno tolto, correzione delle ore.
- **Eventi sul team:** aggiunta a un altro team per uno slot, cambio di team.
- **Promemoria:** disponibilità non inviata (chiamati), patente in scadenza (esempio: patente muletto scade il 12/12/2026), correzione delle ore.
- Il click su una notifica apre la pagina collegata (turni, Le mie ore).

### 5.8 Le mie assenze

Bozze 127–130. Card "Le mie assenze" nel Profilo (con contratto e a chiamata).

- **Elenco (127, 130):** richieste con stato In attesa, Approvata, Rifiutata (con motivo); la malattia compare come Registrata dal supervisore.
- **Richiedi assenza (128–129):** solo Ferie o Permesso, da/a, motivo facoltativo. Se ci sono turni nel periodo un avviso li elenca ("Hai 3 turni in questi giorni") e l'invio resta consentito. Toast "Richiesta inviata al supervisore" con Annulla.
- **Malattia:** non si richiede; il dipendente avvisa il supervisore, che la registra.
- **A chiamata (86):** nei giorni di assenza l'interruttore della Disponibilità è spento e disabilitato ("In ferie · approvata").
- **Notifiche (111–112):** "Ferie approvate", "Permesso rifiutato", "Malattia registrata".
- **Richiesta del chiamato (151–152):** stesso form delle ferie, con avviso verde se non ci sono turni; toast e voce "In attesa" nell'elenco.

## 6. Flussi trasversali tra ruoli

Questi flussi attraversano più pagine e più ruoli; ogni passo rimanda alla pagina descritta sopra.

### 6.1 Richiesta di un dipendente di un altro team

Un capogruppo chiede una persona di un altro team per un singolo slot; il supervisore invece la aggiunge subito e il capogruppo della persona viene avvisato. Le persone del proprio team non si richiedono: si aggiungono con + Aggiungi. Le persone senza team non hanno un capogruppo: le aggiunge il supervisore, mentre il capogruppo le richiede al supervisore.

&#91;embedded content: flusso della richiesta di un dipendente · 1 decisione\]

La richiesta resta "Inviata" finché il capogruppo del team della persona accetta o rifiuta; il supervisore può decidere al posto suo.

1. Il richiedente apre la 2.1 dello slot (3.5 o 4.2) e preme "Richiedi dipendenti da altri team".
2. Nel modal sceglie la persona (liberi prima, poi non liberi) e preme "Richiedi": la riga diventa "Inviata" e compare il toast "Richiesta inviata al capogruppo del Team 4" (Annulla la ritira).
3. La richiesta compare in "Richieste ricevute" del capogruppo del team della persona (4.4), con il badge in sidebar e la riga "Cosa succede" che segnala eventuali turni sovrapposti.
4. Il capogruppo **accetta**: la persona lavora per il team richiedente solo in quello slot; entra nella lista della 2.1 del richiedente. Oppure **rifiuta**, senza motivo.
5. In alternativa decide il supervisore, dalla pagina Richieste (3.7), al posto di quel capogruppo; lo stato finale riporta "Mario Rossi, al posto di A. Gallo".
6. Una richiesta annullata (Annulla nel toast) torna "In attesa". Lo spostamento definitivo di una persona non passa da qui: si fa da Gestione team (3.8).

### 6.2 Il supervisore blocca uno slot o un giorno di una barca gestita

1. Nella 3.0 (3.6) il supervisore preme "Blocca giorno" o clicca uno slot lavorabile.
2. Se il giorno non contiene slot già scelti dal capogruppo, lo slot o il giorno diventa non lavorativo e compare il toast con Annulla.
3. Se contiene slot scelti (esempio: Ven 20 con 6–12 e 12–18), si apre la conferma con tre riquadri: slot tolti, persone e richieste in attesa annullate, capogruppo avvisato.
4. "Blocca comunque": i turni scelti dal Team 2 spariscono, il toast dice "Team 2 avvisato" e in Avvisi del capogruppo (4.5) compare "Slot segnato non lavorativo dal supervisore" con il link "Vedi la barca".
5. Il capogruppo non può contestare; nella sua 3.0 (4.3) quegli slot sono "Non lavori".

### 6.3 Il supervisore cambia il team gestore di una barca

1. Nella 3.0 il supervisore apre il selettore "Team che gestisce la barca" e sceglie un altro team. Se la barca non aveva un team, l'assegnazione è immediata.
2. Se c'era già un team, si apre la conferma "Cambiare il team che gestisce Barca A?": gli slot scelti dal vecchio team vengono tolti con persone e richieste; slot imposti dal supervisore e giorni non lavorativi restano; i due capigruppo vengono avvisati.
3. "Cambia team": il selettore mostra il nuovo team e il toast "Barca A ora gestita dal Team 4 · 2 capigruppo avvisati" ha Annulla, che ripristina team, slot, persone e richieste.
4. Il vecchio capogruppo riceve "Il tuo team non gestisce più Barca D"; il nuovo "Il tuo team gestisce ora Barca A" (4.5) e vede la barca "da pianificare" nella home (4.1).

### 6.4 Inserimento di un altro team in una barca gestita

1. Nella 2.0 (3.4) il supervisore trascina un team in uno slot di una barca gestita da un altro team.
2. Compare il modal "Questa barca è gestita dal Team 2. Il capogruppo del Team 2 sarà avvisato"; non esiste divieto e non c'è campo motivo.
3. "Inserisci comunque": lo slot è imposto dal supervisore (scudo), il toast dice "Team 2 avvisato".
4. Il capogruppo gestore vede lo slot come "Altro team" nella sua 3.0 e riceve l'avviso "Il supervisore ha inserito il Team 1 in Barca A".

### 6.5 Spostare, rimuovere o eliminare un lavoratore

| Azione | Dove | Conferma | Cosa succede agli slot futuri | Chi viene avvisato |
| --- | --- | --- | --- | --- |
| Aggiungere una persona senza team | Gestione team | No | Nessun effetto | Nessuno |
| Spostare da un altro team (definitivo) | Gestione team | Sì | Restano assegnati | Capigruppo dei due team |
| Rimuovere dal team | Gestione team | Sì | Restano assegnati | Capogruppo del team |
| Spostare o rimuovere il capogruppo | Gestione team | Sì, prima si sceglie il nuovo capogruppo | Restano assegnati | Capogruppo |
| Rimuovere l'ultimo membro | Gestione team | Sì: l'intero team viene eliminato | Slot del team liberati | Nessuno (non ci sono più membri) |
| Eliminare il lavoratore | Gestione utenti o Dettaglio utente | Sì, definitiva | Tolti dagli slot, richieste annullate, storico cancellato | Capogruppo del team |
| Eliminare un capogruppo | Gestione utenti o Dettaglio utente | Sì, con scelta del nuovo capogruppo | Come sopra | Capogruppo nuovo |

### 6.6 Disponibilità e turni del dipendente a chiamata

1. Il dipendente a chiamata vede nel riquadro di Turni (5.3) che la disponibilità della settimana non è stata inviata e preme "Imposta".
2. In 5.4 attiva i giorni in cui può essere chiamato e preme "Salva disponibilità"; le preferenze (slot e siti, 5.5) si impostano una volta sola dal Profilo.
3. Il supervisore vede la stessa disponibilità nel Dettaglio utente (3.10) e, in modalità Modifica, può cambiarla al posto del lavoratore.
4. Quando il supervisore o il capogruppo assegnano il lavoratore a uno slot, il turno compare in Turni (5.3); i giorni con un turno risultano bloccati in Disponibilità.

### 6.7 Correzione delle ore e notifica al dipendente

1. Il supervisore apre Ore (3.11) o il Dettaglio utente (3.10) e sceglie il riepilogo del lavoratore.
2. Dal menu ⋯ del turno Lun 16 sceglie "Segna come non svolto" e legge nella conferma l'effetto sulle ore.
3. Dopo la conferma le ore del lavoratore scendono e compare il toast con Annulla.
4. Il dipendente riceve la notifica "Correzione delle tue ore" (5.7) con la campanella e vede il turno come "Non svolto" in Le mie ore (5.6).

### 6.8 Assenza di un lavoratore

1. Il dipendente richiede ferie o permesso (5.8), oppure il supervisore registra una malattia (3.12).
2. Per ferie e permessi il supervisore approva o rifiuta; la conferma elenca i turni coinvolti.
3. I turni già assegnati restano e diventano scopertura da assenza; il capogruppo è avvisato (4.5), il dipendente notificato (5.7).
4. Il supervisore sostituisce a mano dalla 2.0 o dalla 2.1: la persona assente compare in fondo alla lista, selezionabile con "Inserisci comunque".
5. Il chiamato non può dichiarare disponibilità nei giorni di assenza (5.4).

## 7. Collegamenti del prototipo, punti aperti e glossario

Il prototipo cliccabile ([prototipo.html](<../03-mockup/prototipo.html>)) ha 151 pagine e 1166 zone cliccabili, tutte verificate con un test automatico. La tabella riassume i collegamenti per ruolo; "Tipo" indica il comportamento del collegamento (apre una finestra, naviga, cambia variante).

### 7.1 Tabella dei collegamenti

| Ruolo | Da | Elemento | A | Tipo di collegamento |
| --- | --- | --- | --- | --- |
| Supervisore | Home | Card barca in Vista Live | Maschera Live sito | Open overlay |
| Supervisore | Home, Pianificazione | Cella della griglia | Pagina 2.0 | Navigate to |
| Supervisore | Home, Pianificazione | Barra barca | Maschera 3.0 | Open overlay |
| Supervisore | Maschera Live sito, 3.0 | X | Home | Close overlay |
| Supervisore | Pianificazione | 7/31 giorni, Team, Solo con anomalie | Varianti della griglia | Variante / Smart animate |
| Supervisore | Pagina 2.0 | Card team in uno slot | Pagina 2.1 | Navigate to |
| Supervisore | Pagina 2.0 | Trascina un team su slot occupato | Conferma sostituzione | Open overlay |
| Supervisore | Pagina 2.0 | Trascina un team in barca gestita | Conferma inserimento → 2.0 con toast | Open overlay |
| Supervisore | Pagina 2.1 | ← Barca A · Sito 1 | Pagina 2.0 | Navigate to |
| Supervisore | Pagina 2.1 | + Aggiungi / Sostituisci | Dropdown persone | Open overlay |
| Supervisore | Pagina 2.1 | Richiedi dipendenti da altri team | Modal richieste | Open overlay |
| Supervisore | Pagina 2.1 | X su una persona | 2.1 con sezione vuota e toast | Variante |
| Supervisore | Maschera 3.0 | Blocca giorno | Toast (o conferma se slot scelti dal capogruppo) | Variante / Open overlay |
| Supervisore | Maschera 3.0 | Selettore team gestore | Elenco → conferma → toast | Open overlay |
| Supervisore | Sidebar | Richieste (con badge) | Richieste | Navigate to |
| Supervisore | Richieste | Accetta / Rifiuta | Riga spostata di scheda + toast | Variante / Smart animate |
| Supervisore | Richieste | Schede In attesa / Accettate / Rifiutate | Scheda corrispondente | Navigate to |
| Supervisore | Sidebar | Team | Gestione team | Navigate to |
| Supervisore | Gestione team | Nuovo team / Modifica / Elimina | Modal o conferma | Open overlay |
| Supervisore | Gestione team | Sposta / X su un membro | Scelta team → conferma → toast, oppure conferma di rimozione | Open overlay |
| Supervisore | Gestione team | Sposta / X sul capogruppo | Scelta del nuovo capogruppo (obbligatoria) | Open overlay |
| Supervisore | Gestione team | X sull'ultimo membro | Avviso "verrà eliminato l'intero team" | Open overlay |
| Supervisore | Sidebar | Utenti | Gestione utenti | Navigate to |
| Supervisore | Gestione utenti | Click su una riga / Modifica | Dettaglio utente (vista o modifica) | Navigate to |
| Supervisore | Gestione utenti | Nuovo lavoratore | Dettaglio utente vuoto → errori → lavoratore creato con toast | Navigate to |
| Supervisore | Gestione utenti | Cestino | Conferma → elenco con toast | Open overlay |
| Supervisore | Dettaglio utente | Elimina | Conferma → Gestione utenti con toast | Open overlay |
| Supervisore | Dettaglio utente | Vedi tutti i 12 turni | Modal Futuri / Passati; un turno futuro apre la 2.0 | Open overlay |
| Supervisore | Dettaglio utente | Gestione team | Gestione team | Navigate to |
| Capogruppo | Home | Card barca in Vista Live | Maschera Live sito | Open overlay |
| Capogruppo | Home | Slot con scudo | 2.1 capogruppo | Navigate to |
| Capogruppo | Home | Pianifica o barra "Gestisci tu" | 3.0 capogruppo | Open overlay |
| Capogruppo | 3.0 capogruppo | Cella "Da scegliere" | Cella "Il team lavora" + toast | Variante |
| Capogruppo | 3.0 capogruppo | Pulsante Persone | 2.1 senza banner | Navigate to |
| Capogruppo | 2.1 capogruppo | Richiedi dipendenti | Modal richieste | Open overlay |
| Capogruppo | Sidebar | Richieste | Richieste ricevute | Navigate to |
| Capogruppo | Richieste ricevute | Accetta / Rifiuta | Riga spostata + toast | Variante |
| Capogruppo | Sidebar | Avvisi | Avvisi | Navigate to |
| Capogruppo | Avvisi | Vedi la barca | 3.0 capogruppo | Open overlay |
| Capogruppo | Avvisi | Segna tutti come letti | Stato letti + toast | Variante |
| Dipendente | Turni | Voce Profilo | Profilo | Navigate to |
| Dipendente a chiamata | Turni | Imposta | Disponibilità | Navigate to |
| Dipendente a chiamata | Disponibilità | Salva disponibilità | Stato salvato + toast | Variante |
| Dipendente a chiamata | Profilo | Card Preferenze | Preferenze | Navigate to |
| Dipendente | Profilo | Esci | Home (solo nel prototipo) | Navigate to |
| Supervisore | Sidebar | Voce Ore | Ore e straordinari | Navigate to |
| Supervisore | Ore e straordinari | Riga lavoratore | Riepilogo ore (Settimana) | Open overlay |
| Supervisore | Dettaglio utente | Link Ore e straordinari | Riepilogo ore (Settimana) | Open overlay |
| Supervisore | Riepilogo ore | Schede Settimana, Mese, Anno | Scheda corrispondente | Variante |
| Supervisore | Riepilogo ore | Menu ⋯ Segna come non svolto | Conferma, poi toast con Annulla | Open overlay |
| Dipendente | Turni | Campanella | Notifiche | Navigate to |
| Dipendente | Notifiche | Segna tutte come lette | Stato letto + toast | Variante |
| Dipendente | Turni o Profilo | Le mie ore | Le mie ore | Navigate to |

### 7.2 Punti aperti

- **Ordine "ruolo specifico" nella lista delle richieste:** risolto: l'ordine per ruolo del documento originale è sostituito dall'ordine per patente del ruolo scelto (vedi 3.5).
- **Contenuto della voce del dropdown dipendenti:** deciso (vedi 3.5).
- **Vista a 31 giorni:** disegnata per la Pianificazione e per le due Home (143, 144).
- **Notifiche, filtri e scadenza delle richieste** risolti (3.7, 4.4, 4.5), insieme alle notifiche al dipendente (5.7) e al riepilogo ore (3.11). Assenze (ferie, permessi, malattia) fatte: vedi 3.12, 5.8 e 6.8. Cambio contratto fatto: vedi 3.13. Vincoli dei chiamati fatti: vedi 3.14.
- **Capogruppo assente:** deciso, nessun sostituto; richieste e avvisi passano al supervisore (vedi 3.15).
- **Non necessari (deciso):** login e scelta del ruolo, versioni tablet e mobile disegnate per gli altri ruoli (la sidebar diventa comunque un menu a scomparsa sotto i 992 px).
- **Fuori ambito:** mockup in Figma. Il docente ha stabilito che il prototipo cliccabile lo sostituisce.

### 7.3 Glossario

| Termine | Significato |
| --- | --- |
| Slot | Fascia di 6 ore: 0–6, 6–12, 12–18, 18–24 |
| Barca gestita | Barca assegnata in 3.0 a un team per tutto il periodo; il capogruppo sceglie gli slot |
| Assegnazione puntuale | Team assegnato dal supervisore a un giorno e slot (2.0); prevale e non si contesta |
| Capogruppo | Incarico, non ruolo: lavoratore fisso di un team che decide per il team |
| Imposto dal supervisore | Slot con scudo, in sola lettura per il capogruppo |
| Non lavorativo | Slot o giorno in cui il team non deve lavorare, deciso dal supervisore |
| Anomalia | Avviso non bloccante, ad esempio patente scaduta di un membro |
| Maschera | Finestra modal sopra la pagina |
| Toast | Messaggio di conferma con Annulla in basso a destra |
| A chiamata | Dipendente che indica la disponibilità ogni settimana |
| Straordinario | Ore oltre il monte settimanale; limite annuo 300 h (orario a 36 h) o 250 h (38 h). Non esiste per i chiamati |
| Non svolto | Turno pianificato che il supervisore segna come non lavorato: le ore scendono e il dipendente è avvisato |
| Preavviso breve | Badge su un turno assegnato con meno di 24 h di anticipo (regola del cliente) |
