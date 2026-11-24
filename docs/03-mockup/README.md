# Mockup di PortPlanner

Qui c'è tutto ciò che descrive l'interfaccia prima dell'implementazione.

- **Mockup in Figma:** _link da inserire_ (file condiviso del gruppo)
- **Prototipo cliccabile:** [prototipo.html](prototipo.html)
- **Bozze delle schermate:** cartella [svg/](svg/), una immagine per schermata o stato
- **Cosa fa ogni schermata e quali regole segue:** [specifiche-dettagliate.md](../02-progettazione/specifiche-dettagliate.md)

Le bozze SVG e il prototipo sono il lavoro preparatorio fatto per decidere schermate, componenti e navigazione. Il mockup finale è il file Figma: se i due non coincidono, vale Figma.

## Come aprire il prototipo

Apri `prototipo.html` con un browser (doppio clic). È un file unico, non serve installare nulla né avviare l'app. Il font Inter viene caricato da Google Fonts: offline il testo usa il font di sistema.

Nel prototipo i click sulle zone attive portano alla schermata collegata; il trascinamento (drag & drop) è simulato da un click. Le schermate presenti sono quelle campione: non tutti i dati sono coerenti tra una pagina e l'altra.

Il prototipo comprende i tre ruoli: Supervisore (desktop), Capogruppo (desktop) e Dipendente (mobile). Si passa da un ruolo all'altro con i pulsanti "Apri Capogruppo" e "Apri Dipendente"; gli stati secondari (conferme, vuoti, errori) si raggiungono dal menu "Altri stati…".

## Come sono organizzate le bozze

I nomi dei file sono `NN-descrizione-LARGHEZZAxALTEZZA.svg`. Il numero è l'ordine di disegno e lo usano le specifiche per rimandare alle schermate ("bozza 07").

| Numeri | Contenuto | Formato |
|---|---|---|
| 01–64, 80–81, 90–92 | Supervisore: home, dettaglio barca e giorno (2.0), gestione team slot (2.1), dettaglio totale barca (3.0, con blocco giorno e cambio del team gestore), richieste, gestione team, gestione utenti, dettaglio utente, pianificazione | 1440×900 |
| 65–79 | Capogruppo: home, 2.1, 3.0, richieste ricevute, avvisi | 1440×900 |
| 82–89, 99 | Dipendente: turni, disponibilità, preferenze, profilo (con contratto e a chiamata) | 390×844 |
| 93–98, 100–102 | Stati del dettaglio utente (creato, disponibilità, tutti i turni, eliminazione), vincoli contrattuali, filtri, 2.1 senza membri disponibili | 1440×900 |
| 103–114 | Ore e straordinari, notifiche del dipendente | 1440×900 e 390×844 |
| 115–134 | Assenze (ferie, permessi, malattia) e cambio contratto | 1440×900 e 390×844 |
| 135–152 | Vincoli per i chiamati, capogruppo assente, ritocchi finali | 1440×900 e 390×844 |

Dove un numero cade in più righe, i dettagli sono nelle specifiche. `03-home-e-pagina-2-0-affiancate.svg` mostra home e pagina 2.0 una accanto all'altra.

## Fondamenta grafiche

- Base: Bootstrap 5 (UI Kit), colore primario `#0D6EFD`, raggio 6 px, griglia a 12 colonne.
- Font: Inter nel mockup, font di sistema Bootstrap nel codice.
- Icone: Font Awesome.
- Stati sempre con colore, icona e testo insieme.

Valori completi in [specifiche-dettagliate.md](../02-progettazione/specifiche-dettagliate.md#parte-1--fondamenta-e-componenti).

## Test di usabilità

I test fatti sul mockup e i miglioramenti che ne sono derivati andranno in `docs/05-test-usabilita/`. All'orale servono per spiegare cosa è emerso e cosa è stato cambiato.
