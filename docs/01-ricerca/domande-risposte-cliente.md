# Domande e risposte al cliente — Caso 3: Pianificazione turni (porto)

> Data: 07/11/2026 · Gruppo: Diego Andruccioli, Rei Mici, Giovanni Morelli
> Referente/cliente per le interviste: **professore del corso**. Il prof ha approvato il gruppo da 3 e il perimetro.
> Contesto: specifiche.md · Traccia: slide 4 della lezione "Proposte elaborati" del corso

Questo file è il materiale di **user research** del progetto (sezione "Osservazione" del Doppio Rombo). Le risposte sono riportate come date dal gruppo; sotto ogni gruppo di domande la riga **→** è una sintesi/decisione di progetto.

Legenda: ✅ risposta chiara · ❓ non specificato o da chiarire · 🔁 risposta con domanda di chiarimento (risolta nella sezione [Chiarimenti](#chiarimenti-alle-domande-del-gruppo)).

---

## 1. Chi pianifica oggi, e come

| Domanda | Risposta |
|---|---|
| Chi gestisce il personale? Quante persone? | ✅ Una sola persona: **ADMIN** |
| Come pianifica adesso (Excel, carta, WhatsApp, gestionale)? | ❓ Non specificato |
| Quanto tempo ci mette per un giorno / una settimana? | ❓ Non specificato |
| Errori/imprevisti più frequenti (doppia assegnazione, patente scaduta, troppe ore, nave in ritardo)? | ✅ Capitano tutti e li vuole evitare. **Nessun vincolo bloccante** (es. patente o ore massime): ci dev'essere un **avviso**, ma l'azione resta permessa |
| Da quale dispositivo lavora? | ✅ Principalmente **PC**, a volte **tablet**. I **dipendenti usano il telefono** |
| Con quanto anticipo pianifica? | ✅ **Settimanale/mensile**; in caso di imprevisti anche **il giorno prima** |
| Se manca qualcuno all'ultimo (malattia)? | ✅ Cerca di inserire un dipendente **che in quel momento non è in turno** |

**→** Un solo utente pianificatore, desktop-first (tablet supportato), pianificazione su orizzonte settimana/mese con correzioni last-minute. Principio guida: **avvisare, mai bloccare**.

## 2. La nave e il lavoro da fare

| Domanda | Risposta |
|---|---|
| Come arrivano le informazioni sulla nave? | ✅ **Fuori perimetro**: nel progetto i dati nave sono già dati |
| Dati noti in anticipo: orario di arrivo, merce (tipo e quantità), tempo di permanenza? | ✅ Esatto |
| Affidabilità: ritardi, partenza anticipata, come si riplanifica? | ❓ Non detto, ma **può succedere** |
| Come si stima personale e ore in base alla merce? | ✅ **A esperienza dell'ADMIN** (nessuna regola formale) |
| Tipi di merce diversi → mezzi e competenze diverse? | ✅ No: **tutte richiedono gli stessi mezzi e competenze** |
| Una nave può restare più giorni? Si lavora di notte e nei festivi? | ✅ Sì a più giorni; il lavoro **continua di notte e nei festivi** |
| Quali figure servono per nave? | ✅ **Gruista, muletista, scaricatori**. Dev'essere **facile aggiungere altre figure** o dare più compiti alla stessa persona (es. un muletista con patente gru può fare anche il gruista o lo scaricatore). In emergenza chi sa guidare **ma non ha la patente** (o l'ha scaduta) può essere assegnato, **con avvertenza** |
| Numero minimo di persone per ruolo? | ✅ Nessun minimo. **Vincolo: una gru per sito** |
| Il sistema deve calcolare il fabbisogno o lo inserisce il pianificatore? | 🔁 "Che cosa intendi?" → vedi [Chiarimenti](#chiarimenti-alle-domande-del-gruppo) |

**→** Il fabbisogno non si calcola con una formula: l'ADMIN decide a occhio quante persone servono. Modello ruoli **estendibile** (tabella ruoli, non enum fisso). Una nave occupa **un solo sito**.

## 3. Siti, gru e mezzi

| Domanda | Risposta |
|---|---|
| Quanti siti? Fissi? | ✅ **Fissi** nel nostro caso, ma il sistema dovrebbe poter essere **flessibile** |
| Ogni sito ha una sola gru? | ✅ Sì |
| Quanti muletti/mezzi? Sono un vincolo? | ❓ Non specificato |
| Gru e mezzi vanno tracciati (manutenzione, guasto)? | ✅ No: **interessa solo il personale** |
| Una nave può occupare più siti? | ✅ No |
| Più navi sullo stesso sito nello stesso giorno? | ✅ Sì, in sequenza: **quella che parte deve farlo sempre prima che arrivi l'altra** |

**→** Entità Sito configurabile (non hardcoded). Gru/mezzi non sono entità: il vincolo "una gru per sito" si traduce in "un gruista per sito per slot". Regola dati: `partenza(nave A) < arrivo(nave B)` sullo stesso sito.

## 4. Personale e disponibilità

| Domanda | Risposta |
|---|---|
| Quanti dipendenti? Quanti a chiamata? | ❓ Non specificato (seed inventato) |
| Tipologie di contratto e regole | ✅ Vedi [sezione dedicata sotto](#regole-contrattuali-ricevute) |
| Una persona può avere più ruoli? | ✅ Sì |
| Patenti/abilitazioni, scadenze, bloccare o avvisare? | ✅ **Avvisare, mai bloccare**. Scadenze di patenti e brevetti (es. sicurezza) secondo **legge italiana** |
| Come comunica le assenze il dipendente? | ✅ Tramite **la sua pagina nel sistema** (il gruppo ne è "quasi" certo) |
| Chi approva ferie e permessi? | ✅ **Lo stesso pianificatore** |
| I dipendenti possono dichiarare preferenze/disponibilità? | ✅ No in questa domanda, **ma vedi sezione 8**: possono dichiarare disponibilità (es. contratto a chiamata) |
| Scambio turni tra dipendenti? | ✅ **No** |
| Rotazione equa (notti)? | ✅ **No** |

> ⚠️ **Incoerenza da risolvere:** alla domanda "preferenze/disponibilità?" la risposta è *No*, ma nella sezione 8 è *Sì* (disponibilità per contratti a chiamata). Interpretazione adottata: **solo i dipendenti a chiamata dichiarano disponibilità**; nessuna "preferenza" generica.

### Regole contrattuali ricevute

> **Verifica del 19/11/2026:** confrontate con D.Lgs. 66/2003 e CCNL porti 2016–2018: l'11 h di riposo vale per tutti i turni; il riposo settimanale è 24 h ogni 7 giorni calcolate come media su 14 giorni (CCNL: 2 riposi ogni 14 giorni, mai più di 10 giorni di fila); orario 36 h (turnisti h24 e lavoro temporaneo portuale) o 38 h; straordinari **300 h/anno con 36 h, 250 h per gli altri** (non 250 per tutti). Dettagli e fonti in [regole-contrattuali-verificate.md](<./regole-contrattuali-verificate.md>).

> ⚠️ Testo ricevuto dal gruppo come risposta. **Non verificato su fonti ufficiali** (CCNL Porti, D.Lgs. 66/2003, L. 84/94, D.Lgs. 81/2015): prima di usarlo per l'orale/le regole di validazione, confermarlo col prof. Per il progetto basta che sia **coerente e dichiarato**.

**1. Full-time** (tempo determinato o indeterminato)

- Orario base: **36 ore/settimana**, di norma **6 turni da 6 ore**
- Straordinario: ammesso fino a **250 ore/anno** (standard), con tetti giornalieri
- Turni da 6 ore: nessuna pausa obbligatoria dei 10 minuti (prevista oltre le 6 ore, D.Lgs. 66/2003); valgono le regole operative di banchina
- **Riposo giornaliero: almeno 11 ore consecutive ogni 24** → vietato concatenare 12-18 e 18-24
- **Riposo settimanale:** 1 giorno ogni 7 (24 h + 11 h = 35 h)
- **Notturno:** turni 00-06 (e parzialmente 18-24); media massima 8 h/24 h per i lavoratori notturni
- Preavviso: programmazione settimanale/mensile, di norma almeno **24-48 ore prima** dell'inizio della settimana
- Sabato/domenica: normale turnazione continua, con maggiorazione domenicale se lavorativa
- Notturno: fascia **22-06** con maggiorazione (turno 00-06 interamente notturno; turno 18-24 con 2 ore notturne)
- Festivi (1° maggio, 25 dicembre, Pasqua…): maggiorazione festiva o festiva notturna

**2. A chiamata** (lavoro intermittente / art. 17 L. 84/94)

- Nessun monte ore minimo: impiegato solo se c'è fabbisogno (arrivo/partenza nave, picchi)
- Limite 400 giornate in 3 anni (solo lavoro intermittente ordinario; non si applica con Agenzie art. 17)
- Massimo giornaliero: di fatto **un solo turno da 6 ore al giorno** (max 2 turni solo rispettando le 11 ore di riposo)
- **Preavviso** della chiamata: minimo 24 ore; in prassi il piano giornaliero esce entro le 16:00-18:00 del giorno prima
- Indennità di disponibilità se c'è obbligo di rispondere; senza obbligo può rifiutare senza penali e viene pagato solo a ore lavorate
- Stesse maggiorazioni (notte/festivi/domenica) del full-time

**→ Regole che il sistema può verificare** (tutte come **avvisi**, vedi [Scenari](#scenari-da-classificare-blocco-o-avviso)): ore settimanali (36 h), straordinari annui (250 h), riposo 11 h tra turni, riposo settimanale, 1 turno/giorno per i chiamati, preavviso piano, tipo di turno (notturno/festivo) per le maggiorazioni.

## 5. Regole e vincoli

| Domanda | Risposta |
|---|---|
| Ore massime, riposo minimo | ✅ Vedi regole contrattuali sopra |
| Pause | ✅ Vedi regole contrattuali (turni da 6 h → nessuna pausa obbligatoria) |
| Straordinari | ✅ Previsti, **pagati, da tracciare** |
| Quali regole bloccano e quali avvisano? | 🔁 "Fammi tutti gli scenari e ti dico" → vedi [Scenari](#scenari-da-classificare-blocco-o-avviso) |
| Un dipendente può stare su due siti nella stessa fascia? | ✅ **No** |
| Lavoro notturno: regole particolari? | ✅ Vedi regole contrattuali |

**→** La domanda aperta dello sketch sulle pause è chiusa: con turni da 6 h non servono pause nel modello. Resta il vincolo riposo 11 h.

## 6. Slot e turni

| Domanda | Risposta |
|---|---|
| Slot fissi o variabili? | ✅ **Fissi**: 0-6, 6-12, 12-18, 18-24 |
| Un turno può coprire più slot di fila? 12 h consecutive? | ✅ **Solo in emergenza** |
| Assegnazione per persona/slot/sito o per squadra? | 🔁 "Spiegati meglio" → vedi [Chiarimenti](#chiarimenti-alle-domande-del-gruppo) |
| Se la nave resta oltre l'orario previsto? | ✅ **Non si predispongono operai** su quella barca se è già svuotata |

**→** Slot fissi chiusi (decisione confermata). Turni consecutivi consentiti ma con avviso (contrasta con riposo 11 h).

## 7. Cosa vuole vedere il pianificatore (cuore UX)

| Domanda | Risposta |
|---|---|
| Prima domanda del mattino | ✅ **"Cosa devo coprire oggi/domani?"** (non "chi è libero?") |
| Vista preferita | ✅ **Tabella: giorni sull'asse X, fasce orarie per sito sull'asse Y.** Deve mostrare **tutti i siti** insieme e quali turni sono assegnati a quale sito in quale giorno |
| Cosa fa più male oggi | ✅ **Tutte e tre**: trovare chi è disponibile, controllare il carico ore, gestire gli imprevisti |
| Suggerimento automatico o assistenza manuale? | ✅ **Dropdown** (credo sia meglio) |
| Come vuole accorgersi dei problemi | ✅ **Avvisi a schermo, colori con icone** (es. patente in scadenza → simbolo rosso) |
| Storico e statistiche ("Turni già fatti") | 🔁 "Spiegati meglio" → vedi [Chiarimenti](#chiarimenti-alle-domande-del-gruppo) |
| Stampa/export/invio ai dipendenti | ✅ Nessun export: i dipendenti vedono **dalla loro pagina** i turni settimanali + eventuali emergenze |

**→** Schermata principale = **griglia giorni × (sito, fascia)**, con celle che mostrano chi è assegnato e segnali di problema. Le tre difficoltà ("disponibilità", "carico ore", "imprevisti") diventano tre funzioni da progettare (dropdown ordinato, indicatore ore, evidenza delle scoperture/emergenze).

## 8. Lato dipendente

| Domanda | Risposta |
|---|---|
| Dispositivo | ✅ **Telefono** → UI **mobile-first** |
| Notifiche quando cambia il turno | ❓ Non si sa |
| Conferma di presa visione | ❓ Non si sa |
| Può proporre o dichiarare disponibilità? | ✅ Può **dichiarare disponibilità** (es. a chiamata, chiamato per 2 turni, dice di essere libero altri giorni) |
| Telefono personale/login? | ✅ Telefono personale; **il login non è un problema** |

## 9. Tempo reale e collaborazione

| Domanda | Risposta |
|---|---|
| Più pianificatori contemporanei? | ✅ **No: uno solo** |
| Cosa significa "live" nello sketch? | 🔁 "Che cosa vuol dire?" → vedi [Chiarimenti](#chiarimenti-alle-domande-del-gruppo) |
| Se un dipendente non si presenta, lo segna qualcuno? | ❓ Non si sa ancora |

## 10. Ambito e vincoli del progetto

| Domanda | Risposta |
|---|---|
| Dentro/fuori: paghe, IBAN, presenze, ore effettive? | ❓ Non si sa ancora |
| Perché c'è l'IBAN nello sketch? | ✅ È un dato del dipendente, **solo informativo**: non si calcolano né erogano paghe |
| Ottimizzazione automatica o UX? | ✅ **Conta più UI e UX** |
| Cliente per le interviste | ✅ **Professore del corso** |
| Dati del seed | ✅ **Inventati** |
| Approvazione gruppo da 3 e perimetro | ✅ **Sì, approvato** |

---

## Chiarimenti alle domande del gruppo

Risposte alle domande di chiarimento. Stato aggiornato il 07/11/2026 dopo il riscontro del gruppo.

**Fabbisogno: calcolato dal sistema o inserito a mano?** ✅ **Confermato: lo inserisce a mano l'ADMIN.**
Intendevo: dato "nave X, 800 t di merce, 2 giorni in porto", il sistema ti dice "servono 1 gruista + 1 muletista + 4 scaricatori per slot", oppure lo decidi tu e il sistema ti fa solo inserire? L'ADMIN indica per ogni slot quante persone di ogni ruolo servono (la gru → 1 gruista è l'unico minimo fisso).

**Assegnazione per persona/slot/sito o per squadra?** ❓ **Da chiedere ai colleghi e al professore.** Nel frattempo si progetta "per persona" (come nello sketch).
- *Per persona*: per ogni cella (sito, giorno, fascia) e per ogni ruolo scegli una persona dal dropdown. È ciò che c'è nello sketch.
- *Per squadra*: si definiscono gruppi fissi (es. "squadra A: Mario + Luca + Anna") e si assegna la squadra intera a un turno.
Dallo sketch e dalla risposta sul dropdown → **per persona**. Le squadre non sono nel perimetro, salvo che il prof non le voglia.

**Storico e statistiche ("Turni già fatti" nello sketch)?** ✅ **Servono entrambe le cose.**
Nello sketch la homepage ha una scheda "Turni già fatti". Due letture: (a) elenco dei turni passati, per consultarli; (b) riepilogo delle **ore lavorate per dipendente** (settimana/mese/anno), che serve per carico ore e straordinari "da tracciare" (limite 250 h/anno). Decisione: la vista contiene sia l'elenco dei turni passati sia il riepilogo ore per dipendente per periodo.

**Cosa significa "live"?** ❓ **Ancora da decidere.**
Nello sketch compare "LIVE" sul dettaglio per sito: una vista di **cosa sta succedendo adesso** (chi è in turno su quel sito ora, quale nave, e le prossime 24 ore), che si aggiorna da sola senza ricaricare la pagina. Poiché pianifica una sola persona, SignalR non serve per la collaborazione tra admin; avrebbe senso solo per (1) aggiornare la vista "adesso" allo scadere di uno slot e (2) avvisare il dipendente sul telefono quando il suo turno cambia o c'è un'emergenza. Da decidere se serve nel progetto (parti collaborative = SignalR, slide 16).

---

## Scenari da classificare: blocco o avviso

Il cliente ha chiesto di elencare tutti gli scenari per decidere quali **bloccano** e quali **avvisano**. Regola già data: patente/brevetti e ore massime = **solo avviso**.

**Stato 07/11/2026:** il gruppo ha deciso di **tenere per ora le proposte della colonna "Proposta"** così come sono (compresi i 3 blocchi: righe 14, 19, 20). Sono provvisorie: da rivedere col professore.

| # | Scenario | Proposta | Decisione |
|---|---|---|---|
| 1 | Assegnato a un ruolo senza la patente richiesta, ma "sa guidare" (emergenza) | Avviso | |
| 2 | Patente / brevetto sicurezza **scaduto** | Avviso | ✅ Avviso (detto dal cliente) |
| 3 | Patente / brevetto **in scadenza** (es. entro 30 giorni) | Avviso lieve | |
| 4 | Ruolo non presente tra le competenze della persona | Avviso | |
| 5 | Ore settimanali oltre il contratto (full-time 36 h) | Avviso | ✅ Avviso (detto dal cliente) |
| 6 | Straordinari annui oltre 250 h | Avviso | |
| 7 | Riposo tra due turni < 11 h (es. 12-18 poi 18-24) | Avviso | |
| 8 | Turno di 12 h consecutive (emergenza) | Avviso | ✅ Permesso solo in emergenza |
| 9 | Nessun riposo settimanale (7 giorni di fila) | Avviso | |
| 10 | Contratto a chiamata con più di 1 turno nello stesso giorno | Avviso | |
| 11 | Dipendente a chiamata che **non ha dichiarato disponibilità** | Avviso | |
| 12 | Persona in ferie/permesso/malattia approvata | Avviso forte | |
| 13 | Contratto scaduto o non ancora iniziato | Avviso forte | |
| 14 | **Stessa persona su due siti nella stessa fascia** | **Blocco** | ✅ Detto dal cliente: "No" |
| 15 | Sito/slot **senza gruista** (vincolo: 1 gru per sito) | Avviso (segnala scopertura) | |
| 16 | Slot senza nessun operatore su nave presente | Avviso (scopertura) | |
| 17 | Personale assegnato quando la nave è già svuotata | Avviso | |
| 18 | Personale assegnato in uno slot senza nave sul sito | Avviso | |
| 19 | Nave che parte **dopo** l'arrivo della successiva sullo stesso sito | **Blocco** (dato incoerente) | |
| 20 | Una nave assegnata a più siti | **Blocco** | ✅ Detto dal cliente: "No" |
| 21 | Piano settimanale pubblicato con meno di 24-48 h di preavviso | Avviso | |
| 22 | Modifica di un turno già passato | Avviso | |
| 23 | Ritardo/anticipo della nave → turni già assegnati non più coerenti | Avviso + evidenza | |
| 24 | Assenza di un dipendente già assegnato (malattia improvvisa) | Evidenza scopertura | |

**Ancora aperto:** soglia di "in scadenza" (riga 3, proposta 30 giorni).

---

## Domande ancora aperte

- Come pianifica oggi (strumento) e quanto tempo ci mette? (nessuno lo sa)
- Quanti muletti, quanti dipendenti, quanti a chiamata? (nel seed li inventiamo)
- Notifiche al dipendente quando cambia il turno / conferma di presa visione
- Cosa succede se un dipendente non si presenta (assenza ingiustificata): serve tracciarlo?
- Cosa è dentro/fuori perimetro: presenze/timbrature, ore effettive, paghe?
- Come si gestisce il ritardo/anticipo della nave nella riplanificazione?
- Assegnazione per persona o per squadra: chiedere a colleghi e professore
- "Live" (vista adesso + prossime 24 h, eventuale SignalR per notificare i dipendenti): decidere se serve
- Validazione delle regole contrattuali ricevute (CCNL/D.Lgs.)
