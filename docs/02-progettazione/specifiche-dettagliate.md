# Specifiche dettagliate — PortPlanner

> Riferimento per chi implementa una schermata: per ogni pagina, componente e stato le regole decise dal gruppo. Per la panoramica (ruoli, flussi, glossario) vedi [specifiche.md](<./specifiche.md>). **Se i due documenti si contraddicono, vale questo.**
>
> Convenzioni di lettura: "**il doc**" = documento di contesto originale del gruppo (Google Doc "PortPlanner | Contesto"); i numeri delle "bozze" (01–152) sono gli SVG in [docs/03-mockup/svg](<../03-mockup/svg/>), il prototipo cliccabile è [prototipo.html](<../03-mockup/prototipo.html>). Le regole contrattuali sono verificate in [regole-contrattuali-verificate.md](<../01-ricerca/regole-contrattuali-verificate.md>). I punti non ancora chiusi sono raccolti in fondo, in "Punti aperti".

# Parte 1 — Fondamenta e componenti

## Fondamenta

| Voce | Scelta |
|---|---|
| Base componenti | Bootstrap 5 UI Kit (community), personalizzato |
| Font | Font di sistema Bootstrap (`system-ui`) in codice; **Inter** nelle bozze e nel prototipo come sostituto |
| Scala tipografica | Bootstrap standard: H1 40, H2 32, H3 28, H4 24, H5 20, H6 16, testo 16 px; testo di tabelle e griglia 14 px (small) |
| Pesi | Regular 400 + Bold 700 |
| Frame | Desktop 1440×900, Tablet 1024×768, Mobile 390×844 |
| Raggio angoli | 6 px (Bootstrap) |
| Icone | Font Awesome, mai emoji nell'interfaccia |
| Griglia | 12 colonne Bootstrap, gutter 24 px, spaziature multiple di 4/8 px |
| Tema | Solo chiaro (scuro eventualmente dopo) |

## Palette

- Primario `#0D6EFD`; grigi Bootstrap come neutri; palette neutra con un solo accento.
- Stati: colori Bootstrap, sempre **colore + icona + testo**: ok `#198754`, avviso `#FFC107`, errore `#DC3545`, info `#0DCAF0`.
- Team: nessun colore proprio per ora (tutti neutri).
- Colori come Variables con nomi semantici (`stato-ok`, `stato-avviso`, ...).

## Componenti generici (dal UI Kit)

Navbar con menu per ruolo · tabella CRUD con ricerca e filtri · modal (le "maschere") · form (input, select, date, switch). Pulsanti: pieno per l'azione primaria, outline per le altre; un solo primario per schermata; dimensioni sm/md/lg. Anomalia: icona triangolo giallo + testo breve.

## Componenti del dominio

### Card barca
- Contenuto: icona nave (`fa-ship`), nome barca, sito occupato.
- Varianti di stato: **Presente**, **Sito libero** (solo onda, come "Sito 3" nel Canva).
- Nella griglia settimanale: **barra orizzontale** su più giorni, colore unico `#0D6EFD`, cliccabile per il dettaglio totale (3.0).

### Card team
- Chiusa: nome team, capogruppo, n. membri, icona anomalia (solo se c'è).
- Aperta: in più la lista membri con ruolo (altre azioni rimandate).
- Apertura: chevron `fa-chevron-down`. Drag & drop: maniglia `fa-grip-vertical` a sinistra.

### Cella griglia pianificazione
- Stati: **vuota** (nessun team in nessuno slot) · **assegnata** (almeno un team in uno slot del dettaglio giorno-barca, cella colorata). Più team nello stesso giorno devono risultare visibili.
- Slot: 0–6, 6–12, 12–18, 18–24.
- Filtro vista: 7 giorni / 31 giorni, a partire da oggi.
- Più team nello stesso giorno: si mostra il numero (es. "3 team"). A 31 giorni le colonne sono strette con scorrimento orizzontale (vedi Pianificazione). La griglia della home (sito × giorni) e quella della Pianificazione (giorni × slot) sono celle diverse.

### Dropdown dipendenti
- Dropdown Bootstrap con ricerca; voce: nome e cognome, ruolo, disponibilità/sito attuale, stato patente.
- Gruppi con intestazioni di sezione (patente valida / non valida / non patentati). Ordine alfabetico, come da doc.
- Dipendente occupato: grigio e disabilitato con motivo ("Occupato Sito 2").

# Parte 2 — Regole di dominio trasversali

## Ruoli multipli e patenti per tipo
Un lavoratore può fare più ruoli e avere più patenti; **ruoli e patenti sono due cose separate**. Sostituisce la regola "un lavoratore = un ruolo e una patente" usata nelle bozze 01–60.

- **Ruoli abilitati:** elenco di ruoli che il lavoratore può svolgere (Gruista, Muletista, Operaio) con **un ruolo principale**. **Operaio non richiede patente e lo può fare chiunque**: è implicito; compare come chip solo se è il ruolo principale. Il ruolo principale serve per raggruppare i membri in Gestione team, per l'ordinamento e come ruolo predefinito nelle assegnazioni.
- **Patenti per tipo: una per tipo, Gru e Muletto**, ognuna con **scadenza** e stato calcolato (Valida / Scaduta; nessuna data = non posseduta). Le pill dicono sempre il tipo: "Gru valida", "Muletto scaduta", "Nessuna patente".
- **Assegnazione a un ruolo senza patente valida:** consentita (poco bloccante). La persona compare nella sezione di quel ruolo con l'avviso "Anomalia: assegnato senza patente valida" (icona + testo), nessuna finestra di conferma, toast "Carlo Fiore aggiunto come gruista, senza patente valida" con Annulla, badge Anomalia sul team. Nel dropdown dei ruoli patentati le voci senza patente valida hanno "Anomalia se assegnato".
- **Ordine dei dropdown (per ruolo):** gruisti e mulettisti: patente **di quel ruolo** valida, poi scaduta, poi senza patente (alfabetico); operai: senza patente prima, con patente dopo.
- **Un lavoratore fa un solo ruolo per slot** (non può stare in due sezioni nello stesso slot).
- **Due tipi di anomalia, un solo badge:** anagrafica (una patente posseduta è scaduta) e di assegnazione (ruolo senza patente valida); il testo specifica quale.
- **Gestione utenti:** colonna **Ruoli** (chip: principale azzurro, altri bianchi) e colonna **Patenti** (una pill per patente); il filtro Ruolo trova chi ha quel ruolo anche se non principale; il filtro Patente distingue tipo e stato. Bozze 48–53, 98.
- **Dettaglio utente:** il campo Ruolo diventa **Ruoli abilitati** (chip multipli, il primo è il principale; obbligatorio almeno un ruolo: errore "Seleziona almeno un ruolo"); la scheda Patente diventa **Patenti** con due righe fisse Gru e Muletto (pill stato + scadenza; in modifica un campo data per tipo). Nei **Turni futuri** e nel modal "Tutti i turni" una colonna **Ruolo** indica come è assegnato (con triangolo se manca la patente valida). Bozze 54–60, 93–97.
- **Altre pagine aggiornate:** 2.1 (supervisore e capogruppo, bozze 06–09, 66, 67, 70: pill per ruolo, dropdown con gruppi per patente del ruolo), toast "Senza patente" nel foglio componenti (10), Gestione team (23–47: pill per ruolo), Profilo dipendente (84, 89).
- **Dati di esempio:** Giorgio Neri (Muletista principale, anche Gruista; Gru valida, Muletto scaduta) ha un'anomalia anagrafica; Anna Conti è Gruista e Muletista con entrambe le patenti valide; Luigi Marini è Muletista e Operaio, Team 3, con patente muletto.

## Ordinamento dei candidati e vincoli contrattuali
Fonte delle regole: le risposte del cliente (il docente) e [regole-contrattuali-verificate.md](<../01-ricerca/regole-contrattuali-verificate.md>). Due contratti: **A contratto** (full-time: 36 h/settimana, ore del singolo lavoratore) e **A chiamata** (nessun monte ore minimo, 1 turno al giorno, preavviso 24 h, disponibilità dichiarata).

- **Ordine dei candidati** (dropdown "+ Aggiungi / Sostituisci" e modal "Richiedi dipendenti"), in cascata: 1) libero prima dei non liberi (chi è su un altro sito nello stesso slot resta grigio e disabilitato: è un blocco vero) · 2) **patente del ruolo**: valida → scaduta → senza (operai: senza patente prima) · 3) **vincoli contrattuali**: prima chi non ha avvisi, poi chi ne ha (avvisi forti in fondo) · 4) **ore mancanti al monte ore settimanale** (solo A contratto, 36 h di default; per chi è a chiamata non si applica) · 5) **preferenza di slot** · 6) alfabetico. **La patente viene prima dei vincoli.**
- **Tutti i criteri sono visibili allo stesso modo:** ogni candidato mostra i criteri con lo stesso linguaggio dei vincoli e della patente (icona + testo, mai solo colore): pill azzurra **"Mancano 14 h"** (ore al monte settimanale, solo A contratto) e pill **"Preferisce slot"** (ha scelto quello slot tra le preferenze), accanto alla pill della patente e alla riga di avviso dei vincoli. La nota sotto la lista elenca l'ordine completo: "libero, patente del ruolo, vincoli contrattuali, ore da completare, preferenza di slot, poi alfabetico". Bozze 07, 08, 67 (la 07 è il dropdown, la 08 il modal richieste).
- **Vincoli = avvisi, mai blocchi.** Per ogni candidato si calcola cosa succede assegnandolo a quello slot (turni prima e dopo) e se ne viola uno compare la riga di avviso (icona + testo) nell'elenco, nessuna conferma, toast "assegnato, con 1 avviso": **riposo inferiore a 11 h** tra due turni (es. 18–24 seguito da 0–6, o 12–18 seguito da 0–6) · **nessun riposo settimanale di 24 h consecutive** · **ore oltre il monte settimanale (36 h)** · **straordinari annui oltre 250 h** · per chi è a chiamata: **secondo turno nello stesso giorno** e **disponibilità non dichiarata**. (In più, già previsti: ferie/permesso/malattia e contratto scaduto = avvisi forti.)
- **Dove si vedono gli avvisi:** riga nel dropdown e nel modal richieste (bozze 07, 08, 67) · **Anomalia** con il testo del vincolo nel dettaglio del team (2.0, bozza 02), nella maschera Live sito (04), in Gestione team (23) · **Dettaglio utente**: link "1 avviso · Vincoli contrattuali" nella card Turni futuri che apre il modal con ore settimanali, riposo 11 h, riposo settimanale 24 h e straordinari annui, ciascuno nei limiti o con avviso (bozza 100). Gli avvisi non bloccano le assegnazioni.
- **Avviso al capogruppo:** quando il supervisore assegna una persona di un team con un vincolo violato, il capogruppo di quel team riceve un avviso informativo nella pagina Avvisi, es. "Giorgio Neri assegnato con un avviso · Ven 20 nov · Slot 12 – 18 · Barca A. 12 h di lavoro consecutive (6 – 12 e 12 – 18)" con link "Vedi la barca". Ora gli avvisi non letti sono 4 (tutti 8); badge della sidebar capogruppo = 4. Bozze 76–79.
- **Monte ore:** nel Dettaglio utente il campo **Ore settimanali** (A contratto, predefinito 36 h); per chi è a chiamata mostra "Nessun monte ore (a chiamata)" (bozze 54–60, 93–97).
- **Preferenze di slot:** **entrambi i contratti** scelgono gli slot preferiti (0–6, 6–12, 12–18, 18–24) **una volta e le cambiano raramente**; **non esistono più i siti preferiti**. La **disponibilità a giorni** resta solo per chi è a chiamata e si imposta **ogni settimana**. Dipendente: Profilo → Preferenze anche per chi ha il contratto (bozze 84, 99, 88, 89); supervisore: scheda "Preferenze di slot" nel Dettaglio utente (54, 56) e dentro la Disponibilità per i chiamati (55, 94).

## "+ Aggiungi" contro "Richiedi"
- **"+ Aggiungi" e "Sostituisci" (2.1, supervisore e capogruppo):** il dropdown elenca **solo i membri del team** dello slot, abilitati a quel ruolo e ordinati con i criteri già decisi. Chi è già nello slot (anche in un altro ruolo: un ruolo per slot) o è occupato in un altro sito è grigio e disabilitato con il motivo ("Già nello slot come muletista", "Occupata Sito 2"). Con nessun membro disponibile: stato "Nessun membro disponibile — Tutti i membri del Team 2 con il ruolo muletista sono già in questo slot" e pulsante "Richiedi dipendenti da altri team" (bozze 07, 102).
- **"Richiedi dipendenti da altri team":** solo persone **non** del team (altri team e senza team), con selettore di ruolo e criteri visibili (bozze 08, 67).
  - **Capogruppo:** "Richiedi" → il capogruppo del team della persona accetta o rifiuta (stato "Inviata"); per chi è **senza team** la richiesta va al **supervisore** (nota "Al supervisore").
  - **Supervisore:** pulsante **"Aggiungi"**, la persona entra **subito** nello slot e il capogruppo del suo team riceve un avviso (nota "Capogruppo avvisato"); le persone senza team le aggiunge direttamente. Toast "Marco Bruno aggiunto · Team 4 avvisato" (foglio toast, bozza 10).
  - **Capogruppo che non risponde:** nessuna scadenza; la richiesta resta in attesa finché il supervisore non decide al suo posto (come già in Richieste).
- **Avviso al capogruppo:** nuovo tipo "Luca Moro è stato aggiunto al Team 5 per uno slot" (aggiunta diretta del supervisore); Avvisi: non letti 4, tutti 9 (bozze 76–79).
- **Dati di esempio:** Paolo Riva non lavora nello slot Mar 17 6–12 del Team 2 (Operai: solo Sara Fontana; Live sito 04 allineata), così il dropdown ha un candidato.
- **Gestione team "+ Aggiungi" (26)** resta invariato: compone il team in modo stabile (spostamenti definitivi), non gli slot.

## Regole contrattuali verificate e ore confermate/in attesa
Fonti e dettaglio in [regole-contrattuali-verificate.md](<../01-ricerca/regole-contrattuali-verificate.md>). Riassunto delle regole applicate (tutte **avvisi**, mai blocchi):

- **Riposo giornaliero:** 11 h consecutive tra due turni, per tutti i turni e entrambi i contratti (non solo dopo il notturno). Il cambio turno è il caso in cui la legge ammette la deroga: resta avviso.
- **Riposo settimanale:** finestra **mobile di 14 giorni** (non settimana di calendario): servono 2 riposi da 24 h consecutive (35 h col riposo giornaliero) e al massimo 10 giorni consecutivi lavorati (CCNL art. 9).
- **Ore settimanali:** 36 h (turnisti h24 e lavoro temporaneo portuale) o **38 h** (altri), valore del lavoratore; **straordinari 300 h/anno con 36 h, 250 h con 38 h** (CCNL art. 8; non 250 per tutti). Il modal Vincoli mostra "52 di 300 h".
- **Lavoro notturno:** media massima **8 h nelle 24** (D.Lgs. 66/2003 art. 13), **solo per chi lavora di notte** (turni 0–6 o almeno 3 ore tra 22 e 06), su finestra di 14 giorni escludendo i riposi. Nel modal Vincoli la riga "Lavoro notturno" è "Non applicabile" per chi non fa turni notturni; nelle liste: avviso "Lavoratore notturno: media oltre 8 h nelle 24 h" (bozza 08).
- **Chiamati:** vale il riposo di 11 h; "1 turno al giorno" e "preavviso 24 h" sono regole del cliente, accettate come regole di progetto.
- **Ore confermate e in attesa:** nelle liste dei candidati "Mancano N h" riguarda le ore **confermate**; se la persona ha una richiesta in attesa si aggiunge il valore con la richiesta accettata ("Mancano 6 h · 0 h con richiesta in attesa"). L'ordine usa solo le confermate; i vincoli si calcolano sulle confermate e, se serve, un avviso separato "se accetti la richiesta in attesa: …". Una richiesta in attesa per lo stesso slot di un altro team è un avviso, non un blocco. Nel modal Vincoli la riga "Ore settimanali" mostra le confermate e il valore con le richieste in attesa.
- **Modal Vincoli (bozza 100):** sei righe — ore settimanali, riposo 11 h, turni consecutivi, riposo settimanale (14 giorni), lavoro notturno, straordinari annui.

## Filtri avvisi in Gestione utenti e Pianificazione
- **Due tipi di avviso, due filtri.** **"Solo con avvisi"** = avvisi sui **dati** della persona (patente scaduta o in scadenza, contratto scaduto o non iniziato), badge **Anomalia** nella riga. **"Con avvisi sui turni"** = avvisi sui vincoli contrattuali dei turni dei **prossimi 7 giorni** (riposo 11 h, 12 h consecutive, riposo settimanale, 36 h, 250 h/anno, per i chiamati 1 turno/giorno e disponibilità), badge arancione **"Turni: 1 avviso"** nella riga. I due badge possono comparire insieme (Giorgio Neri). Motivo: i primi restano veri finché non si aggiorna l'anagrafica, i secondi dipendono dalla pianificazione e cambiano a ogni modifica; mescolarli renderebbe il filtro rumoroso e non risolvibile dalla riga.
- **Layout dei filtri:** ricerca, chip "Solo con avvisi", chip "Con avvisi sui turni", menu Patente, Team, Ruolo, Contratto. Chip attivo = pieno primario; il risultato mostra "N risultati su 42 lavoratori" e "Cancella filtri" (bozze 48, 49, 101). Click sulla riga = Dettaglio utente (il link "1 avviso · Vincoli contrattuali" porta al modal dei vincoli).
- **Pianificazione "Solo con anomalie" (61–64):** include anche le anomalie da vincoli contrattuali; la legenda dice "Team con anomalia (patente o vincolo contrattuale)".

## Regola: gestione della barca (3.0) contro assegnazione puntuale (2.0)

Due livelli, il più preciso vince.

| Livello | Dove | Chi decide | Il capogruppo può contestare? |
|---|---|---|---|
| Gestione della barca (generale) | 3.0 | Il capogruppo del team assegnato sceglie quando e con chi lavorare, negli slot non bloccati dal supervisore | Decide lui |
| Assegnazione puntuale (giorno + slot) | 2.0 | Il supervisore | **No**: è una decisione del supervisore, non si contesta (se serve, si contatta il supervisore) |

- L'assegnazione puntuale **prevale sempre** su quella generale. Resta "un solo team per slot".
- **Poco bloccante, ma con avvisi:** il supervisore può sempre inserire un altro team anche in una barca gestita da un team. Nessun divieto, solo avviso nella finestra di conferma: "Questa barca è gestita dal Team 2. Il capogruppo del Team 2 sarà avvisato di questo inserimento." con "Inserisci comunque". **Nessun campo motivo** (non necessario).
- **Il capogruppo gestore viene solo avvisato**, non può reagire né rispondere (nessun commento, nessuna richiesta da accettare). L'avviso compare nella pagina Avvisi del capogruppo.
- **Cosa si vede:** nella 2.0 la barca mostra l'etichetta "Barca gestita da Team 2" e gli slot imposti dal supervisore hanno il tag "Assegnato dal supervisore"; nella 3.0 la cella diventa "Team 1 inserito · Dal supervisore (sola lettura)".
- Bozze: [21 conferma inserimento](<../03-mockup/svg/21-conferma-inserimento-barca-gestita-1440x900.svg>) · [22 2.0 dopo l'inserimento, con toast](<../03-mockup/svg/22-pagina-2-0-team-inserito-barca-gestita-toast-1440x900.svg>). Toast: "Team 4 inserito in 12 – 18 · Team 2 avvisato" con Annulla.

## Assenze: ferie, permessi, malattia
Pagine SVG 115–130.

- **Flusso:** *ferie e permessi* = il dipendente li **richiede** dalla sua app e il supervisore **approva o rifiuta** (rifiuto con motivo opzionale). *Malattia* = fatto, non richiesta: il dipendente la segnala (telefono, fuori sistema) e il **supervisore la registra subito**, senza approvazione. Fonte: "chi approva ferie e permessi? lo stesso pianificatore"; il cliente non ha detto chi registra la malattia (punto da confermare col prof).
- **Dati di un'assenza:** tipo (Ferie / Permesso / Malattia) + periodo da/a a **giorni interi** + motivo opzionale. Nessun permesso a ore o a slot. Stati: In attesa · Approvata · Rifiutata (la malattia nasce Approvata).
- **Dove si vede (supervisore):** nuova voce **"Assenze"** in sidebar (sotto Ore) con due schede: *Calendario* (persone × giorni del mese, barre colorate per tipo con sigla F/P/M e icona) e *Richieste (n)* (card con tipo, periodo, **turni coinvolti**, Approva/Rifiuta). Pulsante "+ Registra assenza" (soprattutto malattia). Nel **Dettaglio utente** un link "Assenze ›" nella card Turni futuri apre una finestra con prossime assenze, storico e "Registra assenza" (SVG 126; niente card per mancanza di spazio).
- **Turni già assegnati nei giorni di assenza:** **restano assegnati** e diventano **scopertura da assenza** (anomalia con colore + icona + testo "In ferie 23–27 nov"); il supervisore sostituisce a mano dalla 2.0 / 2.1; nessun blocco. Il capogruppo è **avvisato** (Avvisi, solo informativo). Approvando una richiesta con turni coinvolti compare una **conferma** che elenca i turni che diventano scoperti.
- **Liste candidati (dropdown, Aggiungi/Richiedi, sostituzioni):** chi è assente in quel giorno va **in fondo**, con badge rosso "In ferie · 23–27 nov" (avviso forte, riga 12 della tabella scenari), **selezionabile con conferma "Inserisci comunque"**, mai bloccato.
- **Ore:** ferie, permessi e malattia **non contano come ore lavorate**: le ore sono solo dei turni pianificati; nel riepilogo ore una nota "Assente N giorni nella settimana". Nessuna ora figurativa. *Residuo aperto:* le "ore mancanti" di chi è assente restano calcolate sul monte 36 h come oggi (l'ordinamento dei candidati le ignora comunque perché gli assenti vanno in fondo).
- **Dipendente (mobile):** nel Profilo la card **"Le mie assenze"** → elenco (In attesa / Approvata / Rifiutata) + **"Richiedi assenza"** (tipo solo Ferie/Permesso, da/a, motivo opzionale). Se nel periodo ha già turni compare un **avviso nel form** ("Hai 2 turni in questi giorni: diventeranno da coprire"), invio consentito. La malattia non si richiede: compare nell'elenco quando il supervisore la registra.
- **Chiamati:** nella schermata Disponibilità i giorni di assenza hanno interruttore **spento e disabilitato** ("In ferie · approvata"); il supervisore non può renderli disponibili in Modifica.
- **Notifiche:** dipendente ("Ferie approvate", "Permesso rifiutato · motivo", "Malattia registrata"); capogruppo (Avvisi: "Marco Bruno è in malattia 18–20 nov: 2 turni del tuo team da coprire").
- **Esclusi:** assenze nella Pianificazione e nel filtro "Solo anomalie" (non scelto); permessi parziali; conteggio residuo ferie; allegato certificato.

## Cambio contratto: a contratto ↔ a chiamata
Pagine SVG 131–134.

- **Come:** azione dedicata **"Cambia contratto ›"** accanto al campo "Tipo di contratto" nel Dettaglio utente (in Modifica il campo resta di sola lettura). Apre una finestra con il nuovo tipo, il monte ore se si passa a contratto e il riepilogo degli effetti, poi conferma e toast con Annulla (131 → 132 Giorgio Neri in a chiamata; 133 → 134 Luigi Marini in a contratto).
- **Decorrenza:** **subito**, dal giorno del cambio. Nessuna data scelta, nessun cambio programmato.
- **Turni futuri:** **restano assegnati**, con avvisi se violano i vincoli del nuovo contratto (a chiamata: più di un turno nello stesso giorno, disponibilità non dichiarata). La conferma elenca gli avvisi; mai blocchi.
- **Contratto → chiamata:** monte ore e straordinari non si applicano più; le ore di straordinario dell'anno restano nello storico. La disponibilità parte vuota ("non ancora dichiarata") e la imposta il lavoratore.
- **Chiamata → contratto:** **monte ore obbligatorio 36 h o 38 h** (limite annuo 300 h / 250 h). La disponibilità dichiarata viene cancellata; le preferenze di slot restano.
- **Straordinari annui:** il contatore **prosegue** sull'anno, non si azzera; il periodo da chiamato non aggiunge straordinari.
- **Invariati:** team, ruoli, patenti, capogruppo.
- **Tracciamento:** riga sotto il tipo di contratto nel Dettaglio ("Dal 17 nov 2026 · prima: a contratto, 36 h"). Nessuna pagina di storico.
- **Notifiche:** il lavoratore (campanella: "Contratto cambiato"; a chiamata invito a impostare la disponibilità) e il capogruppo (Avvisi: "Marco Bruno ora è a chiamata: 2 turni con avviso", informativo). Il supervisore vede il toast.
- **App del dipendente:** nessuna schermata nuova; la barra inferiore segue il contratto (Disponibilità compare o sparisce, contatore 36 h e straordinari idem).

## Vincoli per i chiamati
Pagine SVG 135–137. Le regole (riposo 11 h per tutti, un turno al giorno e disponibilità per i chiamati) erano già accettate come avvisi; qui si completano le schermate.

- **Modal "Vincoli contrattuali" di un chiamato (135):** tre righe, ciascuna nei limiti o con avviso: *un turno al giorno*, *riposo di 11 h tra due turni*, *disponibilità rispettata* (giorni con turno e disponibilità non dichiarata). Niente monte ore né straordinari. Il link nella card Turni futuri mostra il numero di avvisi ("2 avvisi · Vincoli contrattuali").
- **Secondo turno nello stesso giorno (136–137):** nel dropdown il chiamato compare con l'avviso giallo "Ha già un turno oggi: a chiamata ne vale 1"; sceglierlo apre la conferma **"Inserisci comunque"** (stesso schema delle persone in ferie): il turno resta con l'anomalia e il capogruppo è avvisato. Mai blocchi.
- **Vista del dipendente:** gli avvisi di vincolo sono **solo per il supervisore**; nell'app resta solo il badge "Preavviso breve" già deciso.

## Capogruppo assente
Pagine SVG 138–140 (più badge in 01, 11, 23 e nel calendario assenze 115/122).

- **Nessun sostituto.** Resta la regola "capogruppo = incarico fisso, anche se assente, nessun sostituto". Quando è assente **copre il supervisore**, che già accetta/rifiuta richieste e assegna slot al posto del capogruppo.
- **Richieste:** le richieste destinate al capogruppo assente compaiono subito nella pagina Richieste del supervisore con il badge rosso **"Assente 18 – 20 nov"** nella colonna Capogruppo (bozza 11); non attendono il rientro. Gli **Avvisi** restano al capogruppo, che li legge al rientro.
- **Dove si vede l'assenza:** badge "Assente 18 – 20 nov" accanto al capogruppo nell'elenco di **Gestione team** (23) e nella **pagina 2.1** degli slot del team, con la riga "Anna Conti (capogruppo) è assente: richieste e avvisi del team passano a te" (138). L'assenza è la barra Ferie del capogruppo nel calendario Assenze (115).
- **Barca gestita (3.0):** gli slot già scelti dal capogruppo assente **restano**; il supervisore può modificarli con la 2.0 puntuale, che già prevale.
- **Live page (Home, bozza 01):** sulle celle del team con capogruppo assente (Barca A, Mer 18 – Ven 20) compare un marcatore rosso con triangolo; legenda "Capogruppo assente (team nello slot)". L'assenza di Anna Conti è Mer 18 – Ven 20 nov (barra Ferie nel calendario 115).
- **Conferma di approvazione (139–140):** approvando le ferie di un capogruppo (esempio S. Fabbri, Team 4, 23 – 27 nov) la finestra mostra il riquadro "S. Fabbri è capogruppo del Team 4: richieste e avvisi del team passano a te dal 23 al 27 nov. Non c'è un sostituto"; toast "Ferie di S. Fabbri approvate · richieste e avvisi del Team 4 passano a te".

# Parte 3 — Supervisore (desktop 1440×900)

## Pagina 0 — Live page (home), supervisore

- Layout: **sidebar** a sinistra in primario `#0D6EFD` con logo "PortPlanner", link (Home, Team, Utenti, altri in futuro) e menu utente con ruolo. Tablet: sidebar a sole icone. Mobile: menu hamburger (offcanvas).
- Due sezioni impilate: **Vista Live** (4 siti in riga, card barca presente / sito libero) e **Griglia** (una riga per sito, giorni sull'asse X, barre barca).
- Click: cella griglia → pagina 2.0 · barra barca → maschera 3.0.
- Griglia: filtro **segmented control** "7 giorni / 31 giorni" a destra del titolo; colonne con sigla + numero (es. "Lun 17"), testo 14 px; colonna di oggi con intestazione in primario + etichetta "Oggi"; sito senza barche = solo celle vuote.
- **Click sulla card barca in Vista Live → maschera 1 *Live sito*, in sola visualizzazione:** mostra la situazione attuale di quel sito nello slot orario in cui ci troviamo (team che sta lavorando e relativi operai, come da doc). Maschera (Open overlay).
- Bozza disegnata: [01-home-live-page-1440x900.svg](<../03-mockup/svg/01-home-live-page-1440x900.svg>) (dati di esempio, "oggi" = Mar 17).

## Maschera 1 — Live sito (sola visualizzazione), supervisore

Raggiunta dal click su una card barca in Vista Live. Bozze: [04 con team](<../03-mockup/svg/04-maschera-1-live-sito-con-team-1440x900.svg>) · [05 slot vuoto](<../03-mockup/svg/05-maschera-1-live-sito-slot-vuoto-1440x900.svg>).

- **Forma:** modal Bootstrap centrato (640 px) sopra la home oscurata (overlay). Si chiude con la X (Close overlay).
- **Intestazione:** "Sito X · Barca Y", riga con orologio e slot orario attuale ("Slot 6 – 12 · adesso 09:30"), X in alto a destra. Nessun badge "Sola visualizzazione" (non scelto).
- **Contenuto con team:** nome team, badge "Anomalia" se presente, capogruppo e n. membri, avviso giallo con il testo specifico, operai divisi per ruolo (Gruisti, Mulettisti, Operai) con numero; persona con anomalia segnata con icona + testo.
- **Slot senza team:** icona, "Nessun team assegnato in questo slot" e riga "Prossimo: slot 18 – 24 · Team 3".
- Nessuna azione di modifica nella maschera.

## Pagina 5 — Pianificazione (tutti i siti), supervisore
Nel doc è "pianificazione settimanale di massima di x sito"; la scelta dell'utente è una vista con **tutti i siti e tutti i giorni**, dove ogni giorno ha **4 slot** (0–6, 6–12, 12–18, 18–24). La home resta la vista rapida (Live + griglia sintetica). Bozze 61–64 in `docs/03-mockup/svg/`: [61 7 giorni](<../03-mockup/svg/61-pianificazione-7-giorni-1440x900.svg>) · [62 31 giorni](<../03-mockup/svg/62-pianificazione-31-giorni-1440x900.svg>) · [63 evidenzia un team](<../03-mockup/svg/63-pianificazione-evidenzia-team-1440x900.svg>) · [64 solo con anomalie](<../03-mockup/svg/64-pianificazione-solo-anomalie-1440x900.svg>).

- **Menu:** nuova voce **"Pianificazione"** nella sidebar, **sotto Home** (ora: Home, Pianificazione, Team, Richieste, Utenti). Aggiunta a tutte le pagine disegnate.
- **Struttura:** colonne = giorni; righe = i 4 siti, ognuno un blocco con una riga sottile per la **barra barca** (arrivo → partenza, con nome e icona) e 4 righe di slot. Un solo team per slot.
- **Periodo:** segmented control **7 giorni / 31 giorni** a partire da oggi (colonna di oggi in primario con "Oggi"), come nella home. A 31 giorni le celle diventano **compatte** (solo "T2", scorrimento orizzontale, etichette di sito e slot fisse a sinistra, barra di scorrimento e "scorri →").
- **Cella:** nome del team (es. "Team 2") su sfondo azzurro; icona **scudo** se assegnato dal supervisore; icona **triangolo** gialla se il team ha un'anomalia; **grigio a righe** se non lavorativo; bianca se vuota.
- **Strumenti:** menu **"Team: tutti"** (scegliendo un team si evidenziano i suoi slot e si attenuano gli altri) · chip **"Solo con anomalie"** · **legenda** sotto la griglia (team assegnato, assegnato dal supervisore, anomalia, non lavorativo, vuoto, barca).
- **Click:** su una cella apre la **pagina 2.0** di quel giorno e barca (anche la cella vuota); sulla barra barca apre la **maschera 3.0**.
- **Dati di esempio coerenti con la home:** Sito 1 Barca A (Team 1 e 3 imposti dal supervisore, Team 2 con anomalia); Sito 2 Barca B; Sito 3 libero; Sito 4 Barca C. (In home ho aggiornato Sito 1 · Ven 20 a "1 team", come nella 3.0.)

## Pagina 2.0 — Dettaglio barca giorno X sito Y, supervisore

Corrisponde al punto "2" del doc. Bozza: [02-pagina-2-0-dettaglio-barca-giorno-1440x900.svg](<../03-mockup/svg/02-pagina-2-0-dettaglio-barca-giorno-1440x900.svg>). Raggiunta dal click su una cella della griglia home.

- **Layout:** due colonne. Sinistra: card "Slot orari" con 4 righe (0-6, 6-12, 12-18, 18-24). Destra: card "Team" con la lista dei team (card chiuse con chevron).
- **Intestazione:** link "← Home", titolo "Barca X · Sito Y", data con frecce ← → per cambiare giorno.
- **Regola: un solo team per slot orario.** Più team nella stessa giornata significa team in slot diversi. Se si trascina un team su uno slot già occupato, **l'app chiede conferma** con una finestra (modal 520 px): titolo "Sostituire il team nello slot 6 – 12?", sottotitolo con barca, sito e data, riquadro "Team attuale" (con eventuale badge Anomalia), freccia, riquadro "Nuovo team", avviso "Le persone di Team 2 in questo slot verranno tolte.", pulsanti "Annulla" (outline) e "Sostituisci" (primario); X in alto. Dopo la conferma compare il toast "Team 3 assegnato allo slot 6 – 12" con Annulla. Bozze: [16 conferma](<../03-mockup/svg/16-conferma-sostituzione-team-slot-occupato-1440x900.svg>) · [17 dopo la sostituzione](<../03-mockup/svg/17-pagina-2-0-team-sostituito-toast-1440x900.svg>).
- **Slot:** riga con ora a sinistra e la card del team a destra; vuoto = area tratteggiata "Trascina qui un team"; ogni slot ha un menu ⋯ (tra le voci: "Segna non lavorativo per un team").
- File affiancato: [03-home-e-pagina-2-0-affiancate.svg](<../03-mockup/svg/03-home-e-pagina-2-0-affiancate.svg>). Prototipo cliccabile: [prototipo.html](<../03-mockup/prototipo.html>).
- **Card team nello slot:** maniglia, nome, capogruppo · n. membri, badge "Anomalia" (se c'è), etichetta "Anche su Sito 2" (se lo stesso team è nello stesso slot su un altro sito), chevron, pulsante X per rimuovere.
- **Regola di dominio:** un team **può lavorare nello stesso slot su più siti, con persone diverse**. I team della lista non vengono mai bloccati dopo l'assegnazione; la divisione delle persone si fa nella 2.1.
- **Slot non lavorativo (funzione vera):** il supervisore segna per un team gli slot in cui NON lavora (dal menu ⋯ dello slot). Aspetto: riga grigia a righe + icona `fa-ban` + testo "Team 1 non lavora in questo slot". Se si trascina comunque un team in uno slot così: si inserisce con **avviso giallo non bloccante** ("Slot non lavorativo per questo team").
- **Anomalia:** triangolo giallo + badge sulla card chiusa; il dettaglio specifico (es. "Patente muletto scaduta il 02/11") si legge nella card espansa, insieme alla lista membri con ruolo.
- **Salvataggio:** automatico a ogni drop, con toast di conferma discreto ("Team assegnato"); nessun pulsante Salva.

## Pagina 2.1 — Gestione team slot orario, supervisore

Raggiunta dal click su una card team in uno slot della 2.0. Bozze: [06 pagina](<../03-mockup/svg/06-pagina-2-1-gestione-team-slot-1440x900.svg>) · [07 dropdown aggiungi](<../03-mockup/svg/07-pagina-2-1-dropdown-aggiungi-1440x900.svg>) · [08 richiedi dipendenti](<../03-mockup/svg/08-pagina-2-1-richiedi-dipendenti-1440x900.svg>).

- **Layout:** pagina intera. Intestazione con link "← Barca A · Sito 1" (torna alla 2.0), titolo "Team 2", sottotitolo "Barca · Sito · giorno · slot". Niente capogruppo né badge anomalia nell'intestazione (non scelti). Sotto, tre sezioni impilate: Gruisti, Mulettisti, Operai, ognuna con conteggio.
- **Riga persona:** nome e cognome, stato patente (pill con icona + testo: Patente valida / Patente non valida / Non patentato / Patentato), pulsante "Sostituisci", X per rimuovere.
- **Ordine nella lista:** gruisti e mulettisti: patente valida, non valida, non patentati; operai: non patentati prima, patentati dopo. Alfabetico dentro ogni gruppo.
- **"+ Aggiungi" per sezione:** apre il dropdown con ricerca; dropdown con ricerca dei soli membri del team abilitati a quel ruolo. Ordine, criteri visibili e stati disabilitati: vedi "Ordinamento dei candidati e vincoli contrattuali" e "+ Aggiungi contro Richiedi".
- **"Richiedi dipendenti da altri team":** pulsante outline nell'intestazione, apre un modal con tabella: nome, ruolo, team, stato (Libero / Non libero), patente, pulsante "Richiedi". Ordine: liberi prima, poi non liberi, alfabetico. Dopo "Richiedi" la riga diventa "Inviata" (in attesa); la persona entra nel team solo quando il capogruppo accetta.
- **Ordine della lista "Richiedi dipendenti":** il modal ha un **selettore di ruolo** (Gruista / Muletista / Operaio, predefinito Gruista) e la lista si ordina **prima liberi → non liberi, poi secondo la patente di quel ruolo: valida → scaduta → senza patente** (per gli operai: senza patente prima, con patente dopo), alfabetico dentro ogni gruppo. Colonne: nome, ruoli, team, stato, patente del ruolo scelto ("Patente gru valida", "Senza patente gru"). La richiesta indica il ruolo richiesto. Bozze 08 e 67. "Sostituisci" apre lo stesso dropdown di "+ Aggiungi".
- **Sezione di ruolo vuota:** la sezione resta visibile, con conteggio (0), riga grigia con icona e "Nessun mulettista assegnato" e il pulsante "+ Aggiungi". Bozza: [09](<../03-mockup/svg/09-pagina-2-1-sezione-vuota-e-toast-1440x900.svg>).
- **Toast di conferma** dopo ogni azione (aggiunta, sostituzione, rimozione, richiesta inviata): riquadro verde in basso a destra (480×56), icona di spunta, testo ("Anna Conti aggiunta al team", "Luca Moro sostituito con Marco Bruno", "Giorgio Neri rimosso dal team", "Richiesta inviata al capogruppo del Team 4"), link "Annulla" e X; sparisce da solo dopo ~6 secondi; "Annulla" ripristina lo stato precedente (per la richiesta, la ritira). Componente: [10](<../03-mockup/svg/10-componente-toast-varianti.svg>).
- **Richieste ricevute:** vedi le sezioni "Pagina Richieste" (supervisore) e "Capogruppo — Richieste ricevute".

## Maschera 3.0 — Dettaglio totale barca, supervisore

Raggiunta dal click su una barra barca nella griglia della home. Bozze: [18 base](<../03-mockup/svg/18-maschera-3-0-dettaglio-totale-barca-1440x900.svg>) · [19 senza team](<../03-mockup/svg/19-maschera-3-0-senza-team-1440x900.svg>) · [20 giorno bloccato con toast](<../03-mockup/svg/20-maschera-3-0-giorno-bloccato-toast-1440x900.svg>).

- **Scopo:** il supervisore assegna **un team alla gestione della barca per tutto il periodo** (dal giorno di arrivo a quello di partenza). Sarà poi il **capogruppo** a decidere quando lavorarci e con chi. Il supervisore può segnare gli slot in cui il team **non deve lavorare** su quella barca, o l'intera giornata come non lavorativa.
- **Forma:** modal Bootstrap largo (1040 px) sopra la home oscurata (Open overlay), con X in alto a destra.
- **Intestazione:** "Barca A · Sito 1", periodo ("Mar 17 nov → Ven 20 nov 2026 · 4 giorni"), selettore **"Team che gestisce la barca"** (dropdown con team, capogruppo e n. membri; vuoto = "Seleziona un team"), riepilogo con due pill: "10 slot lavorabili" e "6 non lavorativi".
- **Griglia:** giorni dell'asse X (solo i giorni della barca), slot 0-6, 6-12, 12-18, 18-24 sull'asse Y. Stati della cella: **Lavorabile** (predefinito), **Non lavorativo** (grigio a righe + icona + testo), **Già deciso dal capogruppo** (azzurro, "Team 2 lavora", sola lettura). Ogni intestazione di giorno ha il pulsante "Blocca giorno" (che diventa "Ripristina giorno" quando il giorno è bloccato, con colonna intera non lavorativa).
- **Interazione:** click su una cella alterna lavorabile / non lavorativo; "Blocca giorno" segna l'intera giornata. Dopo ogni azione compare il **toast** con Annulla (es. "Gio 19 nov segnato come non lavorativo").
- **Senza team assegnato:** celle spente e testo guida "Assegna un team alla barca per tutto il periodo: poi potrai segnare gli slot in cui non deve lavorare."
- **Blocco di un giorno con slot già scelti dal capogruppo:** "Blocca giorno" su un giorno dove il team gestore ha già scelto degli slot apre una **finestra di conferma** (520 px): "Segnare Ven 20 nov come non lavorativo?" con tre riquadri — giallo "Slot già scelti dal capogruppo" (es. 6–12 e 12–18, verranno tolti), giallo "Persone e richieste" (persone assegnate e richieste in attesa su quegli slot annullate), azzurro "Capogruppo avvisato" (avviso nella sua pagina Avvisi) — e pulsanti Annulla / **"Blocca comunque"**. Poi toast "Ven 20 nov segnato come non lavorativo · Team 2 avvisato" con Annulla (ripristina slot, persone e richieste). Nessun divieto, coerente con la regola "poco bloccante ma con avvisi". Se il giorno non ha slot scelti dal capogruppo, nessuna finestra (come Gio 19). Bozze: [80 conferma](<../03-mockup/svg/80-maschera-3-0-conferma-blocco-giorno-con-slot-scelti-1440x900.svg>) · [81 dopo, con toast](<../03-mockup/svg/81-maschera-3-0-giorno-bloccato-capogruppo-avvisato-toast-1440x900.svg>). Nella 18/20 Ven 20 ha ora due slot "deciso dal capogruppo" (6–12, 12–18).
- **Cambio del team gestore:** il selettore "Team che gestisce la barca" apre un elenco dei team (nome, capogruppo, membri; il team attuale ha la pill "Attuale"). **Sostituire un team già assegnato chiede sempre conferma**; la prima assegnazione (barca senza team) è immediata, senza conferma. Finestra "Cambiare il team che gestisce Barca A?": riquadro "Da Team 2 → A Team 4" e quattro riquadri — giallo "Slot scelti dal Team 2" (**vengono tolti**, es. 3 slot), giallo "Persone e richieste" (assegnazioni e richieste in attesa annullate), azzurro "Cosa resta" (slot imposti dal supervisore e giorni non lavorativi invariati), azzurro "Capigruppo avvisati" (**entrambi**) — con Annulla / "Cambia team". Poi la 3.0 mostra il nuovo team (celle del vecchio gestore tornano "Lavorabile") e il toast "Barca A ora gestita dal Team 4 · 2 capigruppo avvisati" con Annulla (ripristina team, slot, persone e richieste). Bozze: [90 elenco](<../03-mockup/svg/90-maschera-3-0-scegli-team-gestore-1440x900.svg>) · [91 conferma](<../03-mockup/svg/91-maschera-3-0-conferma-cambio-team-gestore-1440x900.svg>) · [92 dopo, con toast](<../03-mockup/svg/92-maschera-3-0-team-gestore-cambiato-toast-1440x900.svg>). **Avvisi capogruppo:** aggiunti i tipi "Il tuo team gestisce ora Barca A" (nuovo gestore) e "Il tuo team non gestisce più Barca D" (vecchio gestore), nella scheda Tutti.

## Pagina 3 — Gestione team, supervisore
Dal doc: "pagina configurazione team dove ho tutti i team, CRUD sui vari team e sui singoli lavoratori". Bozze 23–29 in `docs/03-mockup/svg/`: [23 base](<../03-mockup/svg/23-gestione-team-1440x900.svg>) · [24 nuovo team](<../03-mockup/svg/24-gestione-team-nuovo-team-1440x900.svg>) · [25 modifica](<../03-mockup/svg/25-gestione-team-modifica-team-1440x900.svg>) · [26 dropdown aggiungi](<../03-mockup/svg/26-gestione-team-dropdown-aggiungi-1440x900.svg>) · [27 conferma sposta](<../03-mockup/svg/27-gestione-team-conferma-sposta-1440x900.svg>) · [28 conferma elimina](<../03-mockup/svg/28-gestione-team-conferma-elimina-1440x900.svg>) · [29 dopo spostamento con toast](<../03-mockup/svg/29-gestione-team-dopo-spostamento-toast-1440x900.svg>).

- **Layout:** due colonne. Sinistra: elenco "Team (5)", una voce per team con nome, badge Anomalia se serve, capogruppo, n. membri e composizione per ruolo ("5 membri · 2 gruisti · 1 muletista · 2 operai"); la voce selezionata ha bordo primario. Destra: dettaglio del team selezionato. Pulsante primario **"Nuovo team"** in alto a destra. Voce "Team" attiva nella sidebar.
- **Dettaglio team:** titolo + badge Anomalia, pulsanti "Modifica" (outline) ed "Elimina" (outline rosso); capogruppo; avviso giallo con il dettaglio dell'anomalia; sezioni Gruisti / Mulettisti / Operai (stesso schema della 2.1) con "+ Aggiungi", righe con nome, stato patente, **"Sposta"** e X (rimuovi dal team; la persona resta in anagrafica).
- **CRUD team:** crea (modal: nome + capogruppo, il team nasce vuoto e si compone dopo), modifica (nome / capogruppo), elimina (conferma che elenca le conseguenze: i membri restano nell'anagrafica **senza team**; la barca gestita dal team resta senza team; gli slot assegnati nella 2.0 vengono liberati; pulsante rosso "Elimina team").
- **Lavoratori:** "+ Aggiungi" apre il dropdown con ricerca; in cima i **senza team**, poi i gruppi per patente; chi è già in un altro team è selezionabile con l'etichetta "ora in Team 4" e viene **spostato definitivamente** (con conferma). **Un lavoratore sta in un solo team.**
- **Spostamento definitivo contro richiesta:** da Gestione team lo spostamento è per sempre; con "Richiedi dipendenti da altri team" (2.1) vale solo per quello slot.
- **Conferma spostamento:** "Spostare definitivamente Marco Bruno?", card "Da Team 4 → A Team 2", avviso sulle assegnazioni future ("ha 2 slot futuri con il Team 4: resteranno assegnati", perché li ha decisi il supervisore), info "I capigruppo di Team 4 e Team 2 saranno avvisati", pulsanti Annulla / "Sposta definitivamente". Dopo: toast "Marco Bruno spostato in Team 2" con Annulla.
- **Capogruppo (deciso):** non è un ruolo. Il capogruppo può essere **qualsiasi lavoratore**; il dropdown "Capogruppo" dei modal Nuovo team / Modifica elenca tutti i lavoratori (con la stessa ricerca e gli stessi gruppi del dropdown "+ Aggiungi"). Ruoli lavorativi restano Gruista, Muletista, Operaio; "capogruppo" è un incarico per team.
- **Il capogruppo è anche un lavoratore del team** (conta nei membri e compare nelle sezioni di ruolo con l'etichetta "Capogruppo", es. Anna Conti, gruista, capogruppo del Team 2). **È fisso per team**: resta il capogruppo anche se in quel turno non lavora e anche quando è assente. **Nessun sostituto per ora** (da riprendere). **Rimuovere o spostare il capogruppo:** prima si deve **scegliere un altro membro del team come nuovo capogruppo** (scelta obbligatoria, nessuna scorciatoia). Finestra "Scegli il nuovo capogruppo del Team 2" con i membri rimasti (radio, nome, ruolo, patente); il pulsante "Rimuovi e nomina capogruppo" (o "Sposta e nomina capogruppo") resta disabilitato finché non si seleziona qualcuno, con il suggerimento "Seleziona una persona per continuare.". Dopo: toast "Anna Conti rimossa. Capogruppo: Luca Moro" con Annulla. Bozze: [30 da scegliere](<../03-mockup/svg/30-gestione-team-nuovo-capogruppo-da-scegliere-1440x900.svg>) · [31 selezionato](<../03-mockup/svg/31-gestione-team-nuovo-capogruppo-selezionato-1440x900.svg>) · [32 dopo con toast](<../03-mockup/svg/32-gestione-team-dopo-cambio-capogruppo-toast-1440x900.svg>) · [33 variante "Sposta"](<../03-mockup/svg/33-gestione-team-sposta-capogruppo-1440x900.svg>). **Ultimo membro:** se si rimuove (X) o si sposta l'**ultimo membro** di un team, compare un avviso che dice che **verrà eliminato l'intero team** ("Rimuovendola, l'intero Team 6 verrà eliminato. Un team senza membri non può esistere."; la persona resta in anagrafica senza team, gli slot assegnati al team vengono liberati). Pulsanti Annulla e "Rimuovi ed elimina team" (rosso); poi toast "Team 6 eliminato" con Annulla. Non si chiede il nuovo capogruppo (non ci sarebbe nessuno). Bozze: [34 team con un solo membro](<../03-mockup/svg/34-gestione-team-team-con-un-solo-membro-1440x900.svg>) · [35 avviso](<../03-mockup/svg/35-gestione-team-avviso-ultimo-membro-1440x900.svg>) · [36 team eliminato con toast](<../03-mockup/svg/36-gestione-team-team-eliminato-toast-1440x900.svg>). Eliminare l'intero team con il pulsante "Elimina" non chiede il nuovo capogruppo.

### Completamento Gestione team
- **Sposta un membro non capogruppo:** "Sposta" apre "Spostare Sara Fontana in un altro team" con scelta del team di destinazione (radio: team, capogruppo, n. membri) e "Continua"; poi la conferma di spostamento definitivo; poi toast "Sara Fontana spostata in Team 4" con Annulla. Bozze [37](<../03-mockup/svg/37-gestione-team-sposta-scegli-team-1440x900.svg>) · [38](<../03-mockup/svg/38-gestione-team-sposta-conferma-1440x900.svg>) · [39](<../03-mockup/svg/39-gestione-team-dopo-spostamento-membro-toast-1440x900.svg>).
- **Slot futuri di chi viene spostato o rimosso: restano assegnati** (le assegnazioni puntuali le ha fatte il supervisore). Le conferme lo dicono: "Ha 2 slot futuri con il Team 2: resteranno assegnati."
- **Rimuovere un membro (X) chiede conferma, come tutte le azioni:** "Rimuovere Sara Fontana dal Team 2?" con tre info (resta nell'anagrafica senza team; slot futuri restano assegnati; il capogruppo sarà avvisato), pulsanti Annulla / "Rimuovi dal team" (rosso); poi toast "Sara Fontana rimossa dal team". Bozze [44](<../03-mockup/svg/44-gestione-team-conferma-rimozione-membro-1440x900.svg>) · [45](<../03-mockup/svg/45-gestione-team-membro-rimosso-toast-1440x900.svg>).
- **Capogruppo nei modal (regola decisa):** in **Nuovo team** si può scegliere come capogruppo **solo una persona senza team** (nessuno spostamento nascosto); in **Modifica** solo un **membro del team stesso**. Dropdown aperti: [41](<../03-mockup/svg/41-gestione-team-nuovo-team-scegli-capogruppo-1440x900.svg>) · [42](<../03-mockup/svg/42-gestione-team-modifica-scegli-capogruppo-1440x900.svg>).
- **Team appena creato:** compare nell'elenco, selezionato, con il capogruppo come unico membro e le sezioni vuote; toast "Team 6 creato". Bozza [40](<../03-mockup/svg/40-gestione-team-team-creato-toast-1440x900.svg>) (riusa lo stato con un solo membro).
- **Aggiunta di un lavoratore:** scegliendo una persona senza team dal dropdown "+ Aggiungi" viene aggiunta subito, **senza finestra di conferma** (non serve: non toglie nessuno a un altro team), con toast "Carlo Fiore aggiunto al Team 2" e Annulla. La conferma serve invece quando la persona è già in un altro team (spostamento definitivo) e per la rimozione. Bozza [46](<../03-mockup/svg/46-gestione-team-lavoratore-aggiunto-toast-1440x900.svg>).
- **Toast di Gestione team:** aggiunta, rimozione, spostamento, modifica ("Team 2 aggiornato"), nuovo team, eliminazione ("Team 2 eliminato"). Foglio componenti [43](<../03-mockup/svg/43-gestione-team-toast-varianti.svg>).
- **Nessun team:** colonna sinistra con icona, "Nessun team", "Crea il primo team per iniziare." e pulsante "Nuovo team"; a destra "Seleziona o crea un team per vedere il dettaglio". Bozza [47](<../03-mockup/svg/47-gestione-team-nessun-team-1440x900.svg>).
- **Gestione team non crea né elimina lavoratori:** "+ Aggiungi" aggiunge soltanto lavoratori **già esistenti**. La creazione/eliminazione delle persone è di Gestione utenti.
- **Chi è senza team si vede in Gestione utenti**, e il **Dettaglio utente** mostra anche il **team di appartenenza** e se la persona è **capogruppo**.
- **Avvisi al capogruppo:** tutti i messaggi "il capogruppo sarà avvisato" (spostamento, rimozione, inserimento in barca gestita) devono comparire nelle pagine del capogruppo **Avvisi** e **Richieste ricevute**. Gli avvisi sono solo informativi; le richieste si accettano o rifiutano.
- **Non fatto per scelta:** ricerca nell'elenco dei team (con pochi team non serve).

## Pagina 4 — Gestione utenti, supervisore
Dal doc: "anagrafica completa di tutti i dipendenti in tabella con CRUD e search e filtri per warning, nome, patente ecc". Bozze 48–53 in `docs/03-mockup/svg/`: [48 base](<../03-mockup/svg/48-gestione-utenti-1440x900.svg>) · [49 solo con avvisi](<../03-mockup/svg/49-gestione-utenti-solo-con-avvisi-1440x900.svg>) · [50 filtro team](<../03-mockup/svg/50-gestione-utenti-filtro-team-1440x900.svg>) · [51 conferma eliminazione](<../03-mockup/svg/51-gestione-utenti-conferma-eliminazione-1440x900.svg>) · [52 eliminato con toast](<../03-mockup/svg/52-gestione-utenti-lavoratore-eliminato-toast-1440x900.svg>) · [53 nessun risultato](<../03-mockup/svg/53-gestione-utenti-nessun-risultato-1440x900.svg>). Voce "Utenti" attiva nella sidebar.

- **Tabella (solo 4 colonne, scelta dell'utente):** nome e cognome (con avatar a iniziali e badge **Anomalia** accanto al nome se ha avvisi) · ruolo (Gruista, Muletista, Operaio) · team (nome del team + etichetta "Capogruppo", oppure pill grigia **"Senza team"** con icona e testo) · patente (pill colore + icona + testo). Contratto, scadenza patente, contatti e altri dati stanno nel **Dettaglio utente**. In fondo a destra due icone: **Modifica** (matita) ed **Elimina** (cestino rosso). Click sulla riga = apre il Dettaglio utente.
- **Ordinamento e scorrimento:** alfabetico per **cognome** (predefinito), colonne ordinabili cliccando l'intestazione (freccia sulla colonna attiva), **paginazione** a 10 righe con "1–10 di 42 lavoratori" e pulsanti pagina.
- **Filtri e ricerca sopra la tabella:** campo "Cerca per nome" · chip **"Solo con avvisi"** (attivo = pieno in primario, mostra "3 risultati su 42 lavoratori" e "Cancella filtri") · menu Patente · menu **Team** (con la voce "Senza team" e i conteggi) · menu Ruolo · menu Contratto.
- **Nessun risultato:** icona, "Nessun lavoratore trovato", testo con la ricerca e pulsante "Cancella filtri".
- **Nuovo lavoratore:** pulsante primario in alto a destra, apre la pagina **Dettaglio utente vuota in modalità creazione**; il lavoratore nasce "senza team" e si assegna da Gestione team.
- **Eliminare un lavoratore (conferma):** "Eliminare Sara Fontana?" con la frase rossa "L'eliminazione è definitiva e non si può annullare." e quattro riquadri: team di appartenenza ("È nel Team 2: verrà tolta dal team"; se è capogruppo prima si sceglie il nuovo capogruppo, come in Gestione team), **slot futuri** ("verranno tolti dagli slot": a differenza dello spostamento, la persona non esiste più), richieste in sospeso (verranno annullate), storico cancellato (turni passati e ore). Non si conserva lo storico (per ora l'utente non lo vuole mantenere) e non c'è la funzione "Disattiva". Pulsanti Annulla e "Elimina definitivamente" (rosso). Poi toast "Sara Fontana eliminata" con Annulla.

## Pagina 3.1 / 4.1 — Dettaglio utente, supervisore
Dal doc: "anagrafica completa di x dipendente con tutti i dettagli con CRUD". Bozze 54–60 in `docs/03-mockup/svg/`: [54 vista con avviso](<../03-mockup/svg/54-dettaglio-utente-1440x900.svg>) · [55 a chiamata](<../03-mockup/svg/55-dettaglio-utente-a-chiamata-1440x900.svg>) · [56 modifica](<../03-mockup/svg/56-dettaglio-utente-modifica-1440x900.svg>) · [57 nuovo lavoratore](<../03-mockup/svg/57-dettaglio-utente-nuovo-lavoratore-1440x900.svg>) · [58 errori di compilazione](<../03-mockup/svg/58-dettaglio-utente-nuovo-lavoratore-errori-1440x900.svg>) · [59 salvato con toast](<../03-mockup/svg/59-dettaglio-utente-salvato-toast-1440x900.svg>) · [60 elimina un capogruppo](<../03-mockup/svg/60-dettaglio-utente-elimina-capogruppo-1440x900.svg>).

- **Layout:** pagina intera. Link "← Gestione utenti", titolo con il nome (+ badge **Anomalia** e/o **Capogruppo**), sottotitolo "Ruolo · Contratto", azioni in alto a destra **Modifica** ed **Elimina**. Avviso giallo a tutta larghezza se ci sono anomalie ("Patente muletto scaduta il 02/11 · Aggiorna la scadenza…"). Sotto, due colonne di schede.
- **Colonna sinistra:** *Anagrafica* (nome, cognome, ruolo, tipo di contratto *a contratto / a chiamata*, data di assunzione) · *Patente* (stato **calcolato** dalla scadenza + data di scadenza; nessuna data = non patentato) · *Contatti* (telefono, email).
- **Colonna destra:** *Team* (nome del team + "Capogruppo: …" oppure etichetta Capogruppo, pulsante "Gestione team"; "Senza team" con rimando a Gestione team) · *Turni futuri* (prossimi 5 slot: data, barca · sito, slot; il click apre la 2.0 di quel giorno; "Vedi tutti i 12 turni"; stato vuoto "Nessun turno assegnato") · *Disponibilità* (solo per chi è **a chiamata**: settimana con i giorni disponibili e le preferenze di slot).
- **Disponibilità:** in vista è in sola lettura (la imposta il lavoratore ogni settimana); in **modalità Modifica** il supervisore può cambiarla al posto del lavoratore.
- **Modifica:** tutta la pagina diventa modificabile; in alto **Salva** (primario) e **Annulla**; campi obbligatori marcati con *; il team non si cambia qui ("Il team si cambia da Gestione team"). Salvando torna la vista con toast "Dati di Giorgio Neri aggiornati" e Annulla.
- **Nuovo lavoratore:** stessa pagina in modalità modifica, titolo "Nuovo lavoratore", pulsante **Crea lavoratore**; obbligatori **nome, cognome, ruolo, tipo di contratto**; nasce **senza team** (si assegna da Gestione team). Errori sotto i campi in rosso ("Il cognome è obbligatorio", "Seleziona un ruolo", "Seleziona il tipo di contratto"). Dopo la creazione si apre la vista con toast "Lavoratore creato".
- **Eliminare un capogruppo:** la finestra chiede prima di scegliere il **nuovo capogruppo** tra i membri (obbligatorio), poi mostra le conseguenze (slot futuri tolti, storico cancellato) e il pulsante "Elimina e nomina capogruppo".
- **Altri stati:** toast "Lavoratore creato: Giulia Bassi" con Annulla (si apre la vista del nuovo lavoratore "senza team"; Annulla annulla la creazione e torna a Gestione utenti) — [93](<../03-mockup/svg/93-dettaglio-utente-lavoratore-creato-toast-1440x900.svg>). **Disponibilità e preferenze in modalità Modifica** (supervisore, solo a chiamata): stessa scheda con selettore settimana, un interruttore per giorno (7 giorni) e chip selezionabili per gli slot preferiti; giorni con un turno bloccati — [94](<../03-mockup/svg/94-dettaglio-utente-modifica-disponibilita-1440x900.svg>). **"Vedi tutti i 12 turni"**: modal con elenco (giorno, barca · sito, slot) e schede **Futuri (12) / Passati (38)**; click su un turno futuro apre la 2.0 di quel giorno; i passati sono solo consultazione — [95](<../03-mockup/svg/95-dettaglio-utente-tutti-i-turni-futuri-1440x900.svg>) · [96](<../03-mockup/svg/96-dettaglio-utente-tutti-i-turni-passati-1440x900.svg>).
- **Eliminazione dal Dettaglio (non capogruppo):** il pulsante Elimina apre la **stessa conferma della lista** ("Eliminare Giorgio Neri?", "L'eliminazione è definitiva e non si può annullare.", quattro riquadri: team di appartenenza, 12 slot futuri tolti, richiesta in sospeso annullata, storico cancellato; Annulla / "Elimina definitivamente") — [97](<../03-mockup/svg/97-dettaglio-utente-conferma-eliminazione-1440x900.svg>). Dopo la conferma si torna a **Gestione utenti** con la riga rimossa, "1–10 di 41 lavoratori" e il toast "Giorgio Neri eliminato" con Annulla — [98](<../03-mockup/svg/98-gestione-utenti-giorgio-eliminato-toast-1440x900.svg>). Per un capogruppo resta la finestra con il nuovo capogruppo (60).
- **Coerenza dati:** il lavoratore a chiamata è **Luigi Marini** (Muletista e Operaio, Team 3, capogruppo A. Gallo) sia nell'app Dipendente sia in Gestione utenti, Dettaglio, Ore, Assenze e Richieste.

## Pagina Richieste — supervisore
Bozze: [11 in attesa](<../03-mockup/svg/11-richieste-supervisore-in-attesa-1440x900.svg>) · [12 accettata con toast](<../03-mockup/svg/12-richieste-supervisore-accettata-toast-1440x900.svg>) · [13 scheda Accettate](<../03-mockup/svg/13-richieste-supervisore-scheda-accettate-1440x900.svg>) · [14 nessuna in attesa](<../03-mockup/svg/14-richieste-supervisore-nessuna-in-attesa-1440x900.svg>).

- **Menu:** voce "Richieste" nella sidebar tra Team e Utenti, con badge numerico (richieste in attesa; sparisce a 0). Aggiunta alla sidebar di tutte le pagine disegnate (Home, 2.0, 2.1, maschera 1); la sidebar mostra ora Home, Team, Richieste, Utenti.
- **Struttura:** titolo "Richieste"; banner informativo azzurro "Come supervisore puoi accettare o rifiutare le richieste al posto di qualsiasi capogruppo"; schede **In attesa / Accettate / Rifiutate** con contatore (si apre su In attesa); filtri: ricerca per persona, team e capogruppo, giorno (prossimi 7 giorni); tabella.
- **Colonne:** persona richiesta (nome, ruolo · team) · richiesta da (team richiedente, "da A. Conti" oppure "da Mario Rossi (supervisore)") · capogruppo a cui è rivolta (es. "S. Fabbri (Team 4)") · dove e quando (barca · sito, giorno · slot) · azioni.
- **Azioni (In attesa):** due pulsanti per riga, **Accetta** (pieno, primario) e **Rifiuta** (outline). Il rifiuto è immediato, senza motivo.
- **Dopo l'azione:** la riga esce da "In attesa", il contatore e il badge scendono, compare il **toast** con Annulla: "Richiesta accettata al posto di S. Fabbri" / "Richiesta rifiutata al posto di S. Fabbri". "Annulla" riporta la richiesta in attesa.
- **Schede Accettate/Rifiutate:** nessun pulsante; colonna "Stato e decisione" con pill (icona + testo) e "Deciso da L. Bianchi" oppure "Mario Rossi, al posto di A. Gallo". Scheda Rifiutate: stessa struttura con pill rossa "Rifiutata" (icona + testo), bozza [15](<../03-mockup/svg/15-richieste-supervisore-scheda-rifiutate-1440x900.svg>).
- **In attesa vuota:** icona, "Nessuna richiesta in attesa", "Le nuove richieste dei capogruppo compariranno qui."

# Parte 4 — Capogruppo (desktop 1440×900)

## Capogruppo — Live page (home)
Utente di esempio: **Anna Conti**, capogruppo del Team 2 (sidebar: avatar "AC", "Anna Conti · Capogruppo"). Bozza: [65](<../03-mockup/svg/65-capogruppo-live-page-1440x900.svg>).

- **Sidebar:** solo **Home, Richieste (badge), Avvisi (badge)**. Niente Pianificazione/Team/Utenti (la programmazione del team è già nella home).
- **Vista Live:** uguale a quella del supervisore (4 siti, click = maschera Live sito) con il **sito dove lavora ora il suo team evidenziato** (bordo primario più spesso + pill "Il tuo team").
- **Riepilogo (due riquadri tra Vista Live e griglia):** "Prossimo turno" (giorno, slot, barca · sito) e "Da pianificare" (avviso giallo: "Barca A · 2 slot da scegliere" con pulsante "Pianifica" che apre la 3.0 capogruppo).
- **Griglia:** stessa struttura della home (4 siti × giorni, 7/31 giorni, oggi in primario), ma **solo la programmazione del suo team**. Cella giornaliera = **4 mini-segmenti** (0–6, 6–12, 12–18, 18–24) + testo "N slot": azzurro = il team lavora (scelto dal capogruppo) · **scudo** = imposto dal supervisore (2.0, non modificabile) · **a righe + icona** = non lavori (bloccato dal supervisore) · tratteggio primario = da scegliere (solo barca gestita) · grigio pieno = occupato da un altro team. Mai solo colore.
- **Barre barca:** barca **gestita dal suo team** = barra primaria con etichetta "Gestisci tu" (click → 3.0 capogruppo); barca dove ha solo slot puntuali = barra grigia (nessun click sulla barra). Click su uno slot già programmato dal supervisore → 2.1 capogruppo. Sito senza barca/senza il suo team = riga grigia, nessun click.
- **Dati di esempio (coerenti con la Pianificazione 61):** il Team 2 lavora solo su Barca A (Sito 1, Mar 17–Ven 20, gestita da lui); Barca B e C sono barre grigie e righe grigie. Slot 0-6/6-12/12-18/18-24: Mar 17 [Team 1, lavora, da scegliere, non lavori] · Mer 18 [non lavori, Team 1, Team 3, non lavori] · Gio 19 [da scegliere, non lavori, imposto dal supervisore, da scegliere] · Ven 20 [non lavori, lavora, lavora, da scegliere] → "4 slot da scegliere".
- **Prossimo turno:** Gio 19 · 12–18 (con riga "Adesso in turno: Barca A · Sito 1 (6–12)").

## Capogruppo — 2.1 Gestione team slot
Bozze: [66 slot imposto](<../03-mockup/svg/66-capogruppo-pagina-2-1-slot-imposto-1440x900.svg>) · [67 richiedi dipendenti](<../03-mockup/svg/67-capogruppo-pagina-2-1-richiedi-dipendenti-1440x900.svg>).

- **Accesso:** click su uno slot già programmato dal supervisore nella home capogruppo (scudo). **Non esiste una 2.0 per il capogruppo**: il link in alto è "← Home" e riporta alla live page.
- **Permessi su slot imposto dal supervisore:** il capogruppo **gestisce solo le persone del suo team** (Aggiungi, Sostituisci, X attivi); team e slot non si contestano. Banner azzurro con scudo: "Slot assegnato dal supervisore: puoi scegliere chi del tuo team lavora, non il team."
- **Richiedi dipendenti da altri team:** disponibile anche al capogruppo, con il suo nome come richiedente ("da A. Conti" nella pagina Richieste); stesso modal, la richiesta va al capogruppo dell'altro team (stato "Inviata").
- Sidebar capogruppo (Home, Richieste, Avvisi), stesse sezioni Gruisti/Mulettisti/Operai, stessi toast con Annulla.

## Capogruppo — 3.0 Pianifica la barca
Bozze: [68 base](<../03-mockup/svg/68-capogruppo-maschera-3-0-pianifica-barca-1440x900.svg>) · [69 slot scelto con toast](<../03-mockup/svg/69-capogruppo-maschera-3-0-slot-scelto-toast-1440x900.svg>) · [70 2.1 dello slot scelto, senza banner](<../03-mockup/svg/70-capogruppo-pagina-2-1-slot-scelto-1440x900.svg>).

- **Accesso:** pulsante "Pianifica" del riquadro "Da pianificare" oppure click sulla barra "Barca A · Gestisci tu" nella home capogruppo. Modal largo 1040 px sopra la home capogruppo.
- **Griglia come quella del supervisore** (giorni × 4 slot), ma **senza** selettore del team (box di sola lettura "Barca assegnata al tuo team · Team 2 · Sei il capogruppo") e **senza** "Blocca giorno" (il capogruppo non può bloccare). Celle: **Il team lavora** (azzurra, spunta, pulsante "Persone") · **Da scegliere** (tratteggio primario + "+") · **Non lavori** (a righe + icona, deciso dal supervisore) · **Imposto dal supervisore** (scudo, bordo primario, sola lettura) · **Altro team** (grigio, "Inserito dal supervisore"). Intestazione di giorno tutto bloccato: "Giornata non lavorativa". Pill: "N slot scelti" (verde) e "N da scegliere" (gialla); stessi numeri del riquadro "Da pianificare" della home.
- **Interazione:** click su una cella "Da scegliere" = il team lavora in quello slot (**salvataggio automatico**, toast "Mar 17 · 12 – 18 aggiunto ai turni del Team 2" con Annulla; click su cella "Il team lavora" la toglie). Nessun pulsante Salva.
- **Persone:** lo slot scelto vale per **tutto il team di default**; il pulsante "Persone" nella cella apre la **2.1 senza banner** (stessa pagina della 2.1 capogruppo, titolo "Team 2", "Barca A · Sito 1 · Ven 20 nov 2026 · Slot 12 – 18", link "← Barca A · Sito 1" che torna alla 3.0).
- **Chiude con la X** e torna alla home capogruppo.
- Risolve il punto aperto "2.1 per slot scelti dal capogruppo": stessa pagina senza banner.

## Capogruppo — Richieste ricevute
Bozze 71–75: [71 in attesa](<../03-mockup/svg/71-capogruppo-richieste-in-attesa-1440x900.svg>) · [72 accettata con toast](<../03-mockup/svg/72-capogruppo-richieste-accettata-toast-1440x900.svg>) · [73 Accettate](<../03-mockup/svg/73-capogruppo-richieste-scheda-accettate-1440x900.svg>) · [74 Rifiutate](<../03-mockup/svg/74-capogruppo-richieste-scheda-rifiutate-1440x900.svg>) · [75 nessuna in attesa](<../03-mockup/svg/75-capogruppo-richieste-nessuna-in-attesa-1440x900.svg>).

- **Struttura come quella del supervisore, ma solo le sue:** titolo "Richieste ricevute", schede In attesa / Accettate / Rifiutate con contatore (il badge in sidebar = in attesa), filtri (cerca persona, team richiedente, giorno). **Nessun banner** (non agisce per conto di nessuno).
- **Colonne (In attesa):** persona del suo team (nome, ruolo · Team 2) · richiesta da (team e persona, anche "Mario Rossi (supervisore)") · dove e quando · **cosa succede** · azioni **Accetta** (primario) / **Rifiuta** (outline). Nelle schede Accettate/Rifiutate la colonna finale è "stato e decisione" (pill + "Accettata da te" / "Mario Rossi, al posto tuo").
- **Impatto (deciso):** riga con freccia "Lavorerà per il Team 4 in questo slot"; se la persona ha già un turno sovrapposto con il suo team, avviso giallo (icona + testo) "Ha già un turno con te (Gio 19 · 12 – 18)", **non bloccante**.
- **Azioni immediate, senza finestra di conferma; rifiuto senza motivo.** Toast con Annulla: "Luca Moro lavorerà per il Team 4 in questo slot" (Annulla riporta la richiesta in attesa).
- **Stato vuoto:** "Nessuna richiesta in attesa" + "Quando un altro team chiede una persona del tuo team, la vedrai qui."

## Capogruppo — Avvisi
Bozze: [76 non letti](<../03-mockup/svg/76-capogruppo-avvisi-non-letti-1440x900.svg>) · [77 tutti](<../03-mockup/svg/77-capogruppo-avvisi-tutti-1440x900.svg>) · [78 segnati come letti con toast](<../03-mockup/svg/78-capogruppo-avvisi-segnati-letti-toast-1440x900.svg>) · [79 nessun avviso](<../03-mockup/svg/79-capogruppo-avvisi-nessun-avviso-1440x900.svg>).

- **Elenco cronologico** (più recenti in alto) con schede **Non letti (n) / Tutti (n)**; il badge della sidebar = non letti. Card per avviso: pallino primario se non letto (titolo in grassetto), icona, titolo, dettaglio, mittente e ora. Pulsante "Segna tutti come letti" (outline) → toast "3 avvisi segnati come letti" con Annulla.
- **Tipi (solo informativi, nessuna risposta/commento):** il supervisore inserisce un altro team in una barca gestita · membro spostato nel/dal team o rimosso · slot o giorno segnato non lavorativo dal supervisore. *(Esito delle richieste fatte dal capogruppo: non incluso per scelta.)*
- **Azione:** link "Vedi la barca" (apre la 3.0 capogruppo) o "Vai alla home"; nessun Accetta/Rifiuta.
- **Stati vuoti:** "Nessun avviso da leggere" (scheda Non letti) e "Nessun avviso" (mai ricevuti).

# Parte 5 — Dipendente (mobile 390×844)

## Dipendente — mobile 390×844
Bozze 82–89: [82 Turni](<../03-mockup/svg/82-dipendente-contratto-turni-390x844.svg>) · [83 nessun turno](<../03-mockup/svg/83-dipendente-contratto-nessun-turno-390x844.svg>) · [84 Profilo](<../03-mockup/svg/84-dipendente-contratto-profilo-390x844.svg>) · [85 Turni a chiamata](<../03-mockup/svg/85-dipendente-a-chiamata-turni-390x844.svg>) · [86 Disponibilità](<../03-mockup/svg/86-dipendente-a-chiamata-disponibilita-390x844.svg>) · [87 salvata con toast](<../03-mockup/svg/87-dipendente-a-chiamata-disponibilita-salvata-toast-390x844.svg>) · [88 Preferenze](<../03-mockup/svg/88-dipendente-a-chiamata-preferenze-390x844.svg>) · [89 Profilo a chiamata](<../03-mockup/svg/89-dipendente-a-chiamata-profilo-390x844.svg>).

- **Formato:** solo **mobile 390×844** (desktop/tablet rimandati). Utenti di esempio: **Sara Fontana** (Operaio, con contratto, Team 2) e **Luigi Marini** (Muletista e Operaio, a chiamata, Team 3, capogruppo A. Gallo).
- **Navigazione:** **barra inferiore** (non hamburger): con contratto = *Turni, Profilo*; a chiamata = *Turni, Disponibilità, Profilo*. Barra superiore con titolo e avatar a iniziali.
- **Turni (con contratto):** card **"Oggi"** in evidenza (bordo primario, pill "In corso adesso", slot grande, barca · sito, team e capogruppo, ruolo) e sotto **"Prossimi turni"** raggruppati per giorno (card: slot | barca · sito, team · capogruppo, ruolo). Mostra: giorno, slot, barca · sito, team e capogruppo, ruolo; **non** i colleghi. Stato vuoto: "Nessun turno in programma".
- **Turni (a chiamata):** stessa lista + riquadro azzurro "Disponibilità non inviata · prossima settimana" con pulsante "Imposta"; "Nessun turno oggi" se non ne ha.
- **Turni solo informativi:** nessuna conferma/rifiuto da parte del dipendente.
- **Disponibilità (solo a chiamata):** selettore settimana con frecce (di default la prossima), **una riga per giorno con interruttore** "Disponibile"; i giorni con un turno sono bloccati ("Hai un turno · 12–18 · Barca C", interruttore acceso e disabilitato); conteggio "4 giorni disponibili"; pulsante primario **"Salva disponibilità"** → toast "Disponibilità salvata per 23 – 29 nov" con Annulla. Disponibilità a giorni, non per slot.
- **Preferenze (una volta, dal Profilo):** chip **slot preferiti** (0–6, 6–12, 12–18, 18–24), selezionabili (pieno + spunta); non esistono preferenze di sito; "Salva preferenze".
- **Profilo:** dati in **sola lettura** (nome, ruolo · contratto), team e capogruppo, patente (stato calcolato + scadenza), contatti; "Per modificare i tuoi dati contatta il supervisore."; a chiamata anche la voce **Preferenze** (›). Pulsante "Esci".
- **Nel prototipo:** pulsante "Apri Dipendente" e voci nel menu "Altri stati…"; "Esci" riporta alla Home (solo collegamento di prototipo).
- **Fuori ambito:** login e scelta del ruolo; versioni tablet/mobile degli altri ruoli. Non fatti: notifiche push, storico turni passati.

## Riepilogo ore e straordinari + notifiche al dipendente
Pagine: supervisore 103–108, dipendente 109–114.

### Ore e straordinari (supervisore)
- **Voce "Ore" nella sidebar** (sotto Utenti) → pagina 103: tabella per lavoratore con contratto, ore della settimana su monte (barra), ore del mese, straordinari nell'anno su limite (300 h con 36 h, 250 h con 38 h; fonte CCNL porti art. 8). Pill "Vicino al limite" oltre l'80 %. Filtro "Vicini al limite annuo", filtro contratto, frecce settimana.
- **Scheda Dettaglio utente:** link "Ore e straordinari" nei Turni futuri → modal 104 (Settimana), 108 (Mese), 105 (Anno, grafico mensile).
- **Ore = pianificate** (ogni turno vale 6 h) **con correzione del supervisore**: dal menu ⋯ di un turno passato/in corso "Segna come non svolto" → conferma 106 (mostra effetto sulle ore, avviso al dipendente, annullabile) → toast 107 con Annulla.
- **Straordinario** = ore oltre il monte settimanale. I chiamati non hanno monte né straordinari: solo ore e giornate.

### Dipendente
- **Contatore settimanale** nella pagina Turni ("24 di 36 h", link "Le mie ore") e card "Ore e straordinari" nel Profilo → pagina 109 (contratto: settimana, straordinari anno 60 di 300 h, turni con stato Non svolto/In corso/Previsto). Chiamato: 110 (ore settimana/mese, nessun monte).
- **Campanella** nella barra superiore con badge non letti → pagina Notifiche 111 (contratto) / 112 (chiamata); "Segna tutte come lette" → 113/114 con toast e Annulla.
- **Eventi notificati:** nuovo turno, turno modificato, turno tolto, correzione ore; aggiunta a un altro team per uno slot; cambio team.
- **Promemoria:** disponibilità non inviata (chiamati), patente in scadenza, correzione ore.
- **Badge "Preavviso breve"** sul turno assegnato con meno di 24 h di preavviso (regola del cliente, accettata).

# Parte 6 — Ritocchi e dati di esempio

## Ritocchi finali
Pagine SVG 141–152.

- **Maschera Live sito del capogruppo (141, 142):** stessa maschera del supervisore, in sola lettura. Se nello slot c'è il suo team compare il tag "Il tuo team" e, se la barca è gestita da lui, un link **"Pianifica la barca"** che apre la 3.0 (141). Con un altro team nessun tag né link (142). Il click sulle card di Vista Live della sua Home apre queste maschere.
- **Vista a 31 giorni delle Home (143 supervisore, 144 capogruppo):** colonne strette (25 giorni visibili, scorrimento orizzontale con barra e "scorri →"), numero di team per giorno (supervisore) oppure 4 mini-segmenti + numero di slot (capogruppo), barre barca e legenda. Il controllo 7/31 giorni passa da una vista all'altra.
- **Voce del dropdown dipendenti (decisa):** nome, pill dei criteri (ore mancanti, preferenza di slot) e una riga di avviso sotto (vincolo, assenza, secondo turno). La patente è data dall'intestazione del gruppo (valida / non valida / non patentati) e non si ripete; nessun sito/slot attuale. Le persone già nello slot restano grigie e disabilitate.
- **Scadenza delle richieste:** una richiesta in attesa **scade quando inizia lo slot richiesto** e passa nella quarta scheda **"Scadute"** (sola consultazione, pill "Scaduta", "Slot iniziato senza decisione"); vale per il supervisore (145) e per il capogruppo (146). Il banner del supervisore lo dice.
- **Esito per chi ha richiesto:** il capogruppo richiedente trova un avviso informativo in **Avvisi** ("Richiesta accettata: Elena Ricci (Team 1)…"); lo stesso per rifiutata e scaduta. Nessuna pagina "Richieste inviate".
- **Filtri della pagina Richieste (147, 148):** Giorno = Oggi · **Prossimi 7 giorni (default)** · Prossimi 31 giorni · Tutti; Team = Tutti i team e capogruppo più i cinque team con il loro capogruppo.
- **Collegamenti chiusi (149–152):** rifiuto del permesso di Sara Fontana con motivo facoltativo (149) e toast (150); richiesta di assenza del chiamato (151, ferie senza turni coinvolti) e toast (152).

## Allineamento dei dati di esempio e del modello
- **Dati canonici per il Team 2 su Barca A** (Pianificazione 61, griglia capogruppo 65, 3.0 supervisore 18–20/80–81/90–92, 3.0 capogruppo 68–69): Mar 17 [Team 1 inserito, il team lavora, da scegliere, non lavori] · Mer 18 [non lavori, Team 1, Team 3, non lavori] · Gio 19 [da scegliere, non lavori, **Team 2 imposto dal supervisore**, da scegliere] · Ven 20 [non lavori, lavora, lavora, da scegliere]. Nella 3.0 del supervisore "da scegliere" = "Lavorabile".
- **Turni di Giorgio Neri** (Dettaglio utente): Mar 17 6–12, Gio 19 12–18, Ven 20 6–12 e 12–18 (Barca A), poi Barca D/E dal 24/11 (12 turni in tutto). Il suo avviso sui vincoli è **"12 h di lavoro consecutive il 20/11 (Ven 20 · 6–12 e 12–18)"**, ammesse solo in emergenza; riposo di 11 h, riposo settimanale e straordinari sono nei limiti (modal Vincoli, bozza 100).
- **Gestione team e Dettaglio** aggiornati al modello "ruoli multipli e patenti per tipo": dropdown "+ Aggiungi" (26) con gruppi per patente gru; scelta del capogruppo (30, 31, 33, 41, 42) e "Elimina capogruppo" (60) con ruoli e una pill per patente ("Gru valida", "Muletto scaduta", "Nessuna patente").

## Coerenza dei dati di esempio
- **Chiamato:** il lavoratore a chiamata è ovunque **Luigi Marini** (Team 3, Muletista e Operaio, Muletto valida fino al 12/12/2026). Il Dettaglio mostra i suoi 3 turni futuri (Mer 18 · Barca A, Ven 20 · Barca B, Lun 23 · Barca C, tutti 12 – 18) e la Disponibilità; la persona senza team nelle liste è ora Marta Pini.
- **Anna Conti (assente Mer 18 – Ven 20 nov):** nelle 2.1 del capogruppo (Gio 19, Ven 20) compare con il badge "Assente 18 – 20 nov"; il suo Dettaglio non ha turni in quei giorni (prossimi: Mar 17, Mar 24, Mer 25); in Ore ha 12 di 36 h nella settimana. Oggi (Mar 17) è presente e pianifica in anticipo.

# Punti aperti

Da chiudere col gruppo o col docente; fino ad allora vale quanto scritto sopra.

- **Chi registra la malattia:** il cliente non l'ha detto; nel progetto la registra il supervisore (sezione "Assenze").
- **Preferenze di slot per entrambi i contratti:** decisione del gruppo, in contrasto con la risposta del cliente "niente preferenze"; da confermare col docente.
- **Disponibilità di un chiamato modificabile dal supervisore** (modalità Modifica del Dettaglio utente): riconciliazione di due risposte del cliente, da confermare.
- **Testi dei messaggi** della conferma di sostituzione team in slot occupato (2.0): proposti dal gruppo.
- **Ore mancanti di chi è assente:** restano calcolate sul monte settimanale (l'ordine dei candidati le ignora perché gli assenti vanno in fondo).
- **Assenze nella Pianificazione** e nel filtro "Solo con anomalie": esclusi per ora.
- **Pianificazione:** assegnazione diretta dalla griglia (oggi solo lettura + pagina 2.0), stampa/esportazione, intervallo di date libero.
- **Parti realtime con SignalR:** da decidere quali schermate aggiornare in diretta (es. Live page, avvisi).
