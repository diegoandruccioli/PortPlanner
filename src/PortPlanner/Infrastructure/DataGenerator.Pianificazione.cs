using System;
using System.Collections.Generic;
using PortPlanner.Domain;

namespace PortPlanner.Infrastructure
{
    // Seed della pianificazione: 4 siti, 5 barche e i turni dei team.
    // I giorni sono scritti come distanza da oggi (0 = oggi, 3 = tra tre giorni).
    // Non c'è storico: tutto parte da oggi in avanti.
    public partial class DataGenerator
    {
        private static List<Sito> CreaPianificazione(Organico organico, DateOnly oggi)
        {
            var siti = new List<Sito>();
            for (var i = 1; i <= 4; i++)
                siti.Add(new Sito { Nome = $"Sito {i}", Ordine = i });

            // I turni li compongo con i nomi, così si leggono come nel prototipo.
            Dipendente P(string nomeCompleto) => organico.Persone[nomeCompleto];
            Team T(string nome) => organico.Team[nome];

            // Barca A: sito 1, oggi + 3 giorni. La gestisce il Team 2, quindi il suo capogruppo sceglie
            // gli slot lavorabili; il supervisore ha inserito altri team e bloccato alcuni slot.
            var barcaA = NuovaBarca("Barca A", oggi, 0, 3, TipoMerce.Container, 1200, T("Team 2"));
            Blocca(barcaA, oggi, (0, Slot.Dalle18Alle24), (1, Slot.Dalle0Alle6), (1, Slot.Dalle18Alle24), (2, Slot.Dalle6Alle12), (3, Slot.Dalle0Alle6));
            // Carlo Fiore fa il gruista senza avere la patente: anomalia di assegnazione.
            Assegna(barcaA, oggi, 0, Slot.Dalle0Alle6, T("Team 1"), OrigineAssegnazione.Supervisore,
                (P("Carlo Fiore"), Ruolo.Gruista), (P("Elena Ricci"), Ruolo.Muletista), (P("Davide Longo"), Ruolo.Operaio), (P("Irene Costa"), Ruolo.Operaio));
            // Giorgio Neri fa il muletista con la patente scaduta.
            Assegna(barcaA, oggi, 0, Slot.Dalle6Alle12, T("Team 2"), OrigineAssegnazione.Capogruppo,
                (P("Anna Conti"), Ruolo.Gruista), (P("Giorgio Neri"), Ruolo.Muletista), (P("Sara Fontana"), Ruolo.Operaio));
            Assegna(barcaA, oggi, 1, Slot.Dalle6Alle12, T("Team 1"), OrigineAssegnazione.Supervisore,
                (P("Luisa Bianchi"), Ruolo.Gruista), (P("Elena Ricci"), Ruolo.Muletista), (P("Davide Longo"), Ruolo.Operaio));
            Assegna(barcaA, oggi, 1, Slot.Dalle12Alle18, T("Team 3"), OrigineAssegnazione.Supervisore,
                (P("Alessandro Gallo"), Ruolo.Gruista), (P("Luigi Marini"), Ruolo.Muletista), (P("Nicola Greco"), Ruolo.Operaio));
            // Anna Conti non c'è da mercoledì a venerdì: il Team 2 lavora senza di lei.
            Assegna(barcaA, oggi, 2, Slot.Dalle12Alle18, T("Team 2"), OrigineAssegnazione.Supervisore,
                (P("Giorgio Neri"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Paolo Riva"), Ruolo.Operaio));
            // Venerdì Giorgio Neri lavora 12 ore di fila (6-12 e 12-18).
            Assegna(barcaA, oggi, 3, Slot.Dalle6Alle12, T("Team 2"), OrigineAssegnazione.Capogruppo,
                (P("Giorgio Neri"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Sara Fontana"), Ruolo.Operaio));
            Assegna(barcaA, oggi, 3, Slot.Dalle12Alle18, T("Team 2"), OrigineAssegnazione.Capogruppo,
                (P("Giorgio Neri"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Paolo Riva"), Ruolo.Operaio));

            // Barca B: sito 2, oggi + 4 giorni. La gestisce il Team 4.
            var barcaB = NuovaBarca("Barca B", oggi, 0, 4, TipoMerce.RinfuseSolide, 5000, T("Team 4"));
            Blocca(barcaB, oggi, (0, Slot.Dalle0Alle6), (3, Slot.Dalle18Alle24));
            Assegna(barcaB, oggi, 0, Slot.Dalle6Alle12, T("Team 4"), OrigineAssegnazione.Capogruppo,
                (P("Stefano Fabbri"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Matteo Sala"), Ruolo.Operaio), (P("Federica Gori"), Ruolo.Operaio));
            Assegna(barcaB, oggi, 0, Slot.Dalle12Alle18, T("Team 4"), OrigineAssegnazione.Capogruppo,
                (P("Valentina Testa"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Matteo Sala"), Ruolo.Operaio), (P("Federica Gori"), Ruolo.Operaio));
            // Marta Pini non ha un team: il supervisore la aggiunge direttamente.
            Assegna(barcaB, oggi, 1, Slot.Dalle0Alle6, T("Team 5"), OrigineAssegnazione.Supervisore,
                (P("Roberto Vitale"), Ruolo.Gruista), (P("Silvia Monti"), Ruolo.Muletista), (P("Marta Pini"), Ruolo.Operaio));
            // Marco Bruno finisce alle 24 e riparte alle 0: riposo di 0 ore, meno delle 11 richieste.
            Assegna(barcaB, oggi, 1, Slot.Dalle18Alle24, T("Team 4"), OrigineAssegnazione.Capogruppo,
                (P("Stefano Fabbri"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Federica Gori"), Ruolo.Operaio));
            Assegna(barcaB, oggi, 2, Slot.Dalle0Alle6, T("Team 4"), OrigineAssegnazione.Capogruppo,
                (P("Valentina Testa"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Matteo Sala"), Ruolo.Operaio));
            // Luigi Marini (a chiamata) lavora venerdì alle 12-18 su questa barca.
            Assegna(barcaB, oggi, 3, Slot.Dalle12Alle18, T("Team 3"), OrigineAssegnazione.Supervisore,
                (P("Alessandro Gallo"), Ruolo.Gruista), (P("Luigi Marini"), Ruolo.Muletista), (P("Chiara Serra"), Ruolo.Operaio));
            Assegna(barcaB, oggi, 4, Slot.Dalle6Alle12, T("Team 4"), OrigineAssegnazione.Capogruppo,
                (P("Stefano Fabbri"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Matteo Sala"), Ruolo.Operaio), (P("Federica Gori"), Ruolo.Operaio));

            // Barca C: sito 4, da oggi + 3 a oggi + 6. Nessun team la gestisce.
            var barcaC = NuovaBarca("Barca C", oggi, 3, 6, TipoMerce.MerciVarie, 800, null);
            Blocca(barcaC, oggi, (3, Slot.Dalle0Alle6));
            Assegna(barcaC, oggi, 3, Slot.Dalle18Alle24, T("Team 5"), OrigineAssegnazione.Supervisore,
                (P("Roberto Vitale"), Ruolo.Gruista), (P("Silvia Monti"), Ruolo.Muletista));
            Assegna(barcaC, oggi, 4, Slot.Dalle6Alle12, T("Team 1"), OrigineAssegnazione.Supervisore,
                (P("Luisa Bianchi"), Ruolo.Gruista), (P("Elena Ricci"), Ruolo.Muletista), (P("Irene Costa"), Ruolo.Operaio));
            // Luca Moro è del Team 2 ma il supervisore lo aggiunge al Team 5 per questo slot.
            // Andrea Pellegrini ha il contratto scaduto: anomalia di anagrafica.
            Assegna(barcaC, oggi, 5, Slot.Dalle12Alle18, T("Team 5"), OrigineAssegnazione.Supervisore,
                (P("Roberto Vitale"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Andrea Pellegrini"), Ruolo.Operaio));
            Assegna(barcaC, oggi, 6, Slot.Dalle12Alle18, T("Team 3"), OrigineAssegnazione.Supervisore,
                (P("Alessandro Gallo"), Ruolo.Gruista), (P("Luigi Marini"), Ruolo.Muletista), (P("Chiara Serra"), Ruolo.Operaio));

            // Barca D: sito 2, dopo una settimana (la Barca B è già partita). Nessun gestore.
            var barcaD = NuovaBarca("Barca D", oggi, 7, 10, TipoMerce.ProdottiForestali, 2500, null);
            Assegna(barcaD, oggi, 7, Slot.Dalle6Alle12, T("Team 2"), OrigineAssegnazione.Supervisore,
                (P("Anna Conti"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Giorgio Neri"), Ruolo.Operaio), (P("Sara Fontana"), Ruolo.Operaio));
            Assegna(barcaD, oggi, 8, Slot.Dalle12Alle18, T("Team 2"), OrigineAssegnazione.Supervisore,
                (P("Anna Conti"), Ruolo.Gruista), (P("Luca Moro"), Ruolo.Muletista), (P("Giorgio Neri"), Ruolo.Operaio), (P("Paolo Riva"), Ruolo.Operaio));
            Assegna(barcaD, oggi, 9, Slot.Dalle0Alle6, T("Team 4"), OrigineAssegnazione.Supervisore,
                (P("Stefano Fabbri"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Federica Gori"), Ruolo.Operaio));

            // Barca E: sito 3, dopo una settimana. La gestisce il Team 1.
            var barcaE = NuovaBarca("Barca E", oggi, 7, 11, TipoMerce.Rotabili, 600, T("Team 1"));
            Blocca(barcaE, oggi, (7, Slot.Dalle0Alle6), (10, Slot.Dalle18Alle24));
            Assegna(barcaE, oggi, 7, Slot.Dalle12Alle18, T("Team 4"), OrigineAssegnazione.Supervisore,
                (P("Valentina Testa"), Ruolo.Gruista), (P("Marco Bruno"), Ruolo.Muletista), (P("Matteo Sala"), Ruolo.Operaio));
            Assegna(barcaE, oggi, 8, Slot.Dalle6Alle12, T("Team 1"), OrigineAssegnazione.Capogruppo,
                (P("Luisa Bianchi"), Ruolo.Gruista), (P("Elena Ricci"), Ruolo.Muletista), (P("Davide Longo"), Ruolo.Operaio));
            Assegna(barcaE, oggi, 9, Slot.Dalle18Alle24, T("Team 1"), OrigineAssegnazione.Capogruppo,
                (P("Carlo Fiore"), Ruolo.Gruista), (P("Elena Ricci"), Ruolo.Muletista), (P("Irene Costa"), Ruolo.Operaio));

            siti[0].Barche.Add(barcaA);
            siti[1].Barche.Add(barcaB);
            siti[3].Barche.Add(barcaC);
            siti[1].Barche.Add(barcaD);
            siti[2].Barche.Add(barcaE);

            return siti;
        }

        // Il sito si collega dopo, aggiungendo la barca alle sue Barche.
        // Con un team gestore la barca ha la sua gestione; senza, la gestione c'è ma è vuota.
        private static Barca NuovaBarca(string nome, DateOnly oggi, int giornoArrivo, int giornoPartenza,
            TipoMerce merce, int tonnellate, Team gestore) => new()
        {
            Nome = nome,
            Arrivo = oggi.AddDays(giornoArrivo),
            Partenza = oggi.AddDays(giornoPartenza),
            TipoMerce = merce,
            QuantitaMerce = tonnellate,
            Gestione = new GestioneBarca { TeamGestore = gestore },
        };

        private static void Blocca(Barca barca, DateOnly oggi, params (int Giorno, Slot Slot)[] slot)
        {
            foreach (var (giorno, s) in slot)
                barca.SlotNonLavorativi.Add(new SlotNonLavorativo { Data = oggi.AddDays(giorno), Slot = s });
        }

        private static void Assegna(Barca barca, DateOnly oggi, int giorno, Slot slot, Team team,
            OrigineAssegnazione origine, params (Dipendente Persona, Ruolo Ruolo)[] persone)
        {
            var assegnazione = new AssegnazioneTeam
            {
                Data = oggi.AddDays(giorno),
                Slot = slot,
                Team = team,
                Origine = origine,
            };

            foreach (var (persona, ruolo) in persone)
                assegnazione.Persone.Add(new AssegnazionePersona { Dipendente = persona, Ruolo = ruolo });

            barca.AssegnazioniTeam.Add(assegnazione);
        }
    }
}
