using System;
using PortPlanner.Domain;

namespace PortPlanner.Infrastructure
{
    // Seed dell'anagrafica: supervisore, 25 lavoratori in 5 team più una persona senza team.
    public partial class DataGenerator
    {
        private static Organico CreaOrganico(DateOnly oggi)
        {
            var organico = new Organico
            {
                Supervisore = NuovoUtente("Mario", "Rossi", supervisore: true),
            };

            // Team 1: capogruppo Luisa Bianchi.
            var luisa = Persona(organico, "Luisa", "Bianchi", Ruolo.Gruista, 36, oggi.AddYears(-8));
            Abilita(luisa, Ruolo.Muletista);
            AggiungiPatente(luisa, TipoPatente.Gru, oggi.AddDays(400));
            AggiungiPatente(luisa, TipoPatente.Muletto, oggi.AddDays(300));

            var elena = Persona(organico, "Elena", "Ricci", Ruolo.Muletista, 36, oggi.AddYears(-4));
            AggiungiPatente(elena, TipoPatente.Muletto, oggi.AddDays(250));

            // Carlo è abilitato come gruista ma non ha nessuna patente: serve per l'anomalia "senza patente valida".
            Persona(organico, "Carlo", "Fiore", Ruolo.Gruista, 36, oggi.AddYears(-1));
            Persona(organico, "Davide", "Longo", Ruolo.Operaio, 38, oggi.AddYears(-6));
            Persona(organico, "Irene", "Costa", Ruolo.Operaio, 36, oggi.AddYears(-2));
            NuovoTeam(organico, "Team 1", "Luisa Bianchi", "Elena Ricci", "Carlo Fiore", "Davide Longo", "Irene Costa");

            // Team 2: gestisce la Barca A. Capogruppo Anna Conti.
            var anna = Persona(organico, "Anna", "Conti", Ruolo.Gruista, 36, oggi.AddYears(-7));
            Abilita(anna, Ruolo.Muletista);
            AggiungiPatente(anna, TipoPatente.Gru, oggi.AddDays(500));
            AggiungiPatente(anna, TipoPatente.Muletto, oggi.AddDays(500));

            var luca = Persona(organico, "Luca", "Moro", Ruolo.Muletista, 36, oggi.AddYears(-5));
            AggiungiPatente(luca, TipoPatente.Muletto, oggi.AddDays(200));

            // Giorgio ha la patente del muletto scaduta: anomalia di anagrafica.
            var giorgio = Persona(organico, "Giorgio", "Neri", Ruolo.Muletista, 36, oggi.AddYears(-9));
            Abilita(giorgio, Ruolo.Gruista);
            AggiungiPatente(giorgio, TipoPatente.Gru, oggi.AddDays(180));
            AggiungiPatente(giorgio, TipoPatente.Muletto, oggi.AddDays(-15));

            Persona(organico, "Sara", "Fontana", Ruolo.Operaio, 36, oggi.AddYears(-3));
            Persona(organico, "Paolo", "Riva", Ruolo.Operaio, 38, oggi.AddYears(-3));
            NuovoTeam(organico, "Team 2", "Anna Conti", "Luca Moro", "Giorgio Neri", "Sara Fontana", "Paolo Riva");

            // Team 3: capogruppo Alessandro Gallo. Luigi Marini è a chiamata (senza monte ore).
            var alessandro = Persona(organico, "Alessandro", "Gallo", Ruolo.Gruista, 36, oggi.AddYears(-10));
            AggiungiPatente(alessandro, TipoPatente.Gru, oggi.AddDays(350));

            var luigi = Persona(organico, "Luigi", "Marini", Ruolo.Muletista, null, oggi.AddYears(-2), TipoContratto.AChiamata);
            AggiungiPatente(luigi, TipoPatente.Muletto, oggi.AddDays(45));

            var beatrice = Persona(organico, "Beatrice", "Villa", Ruolo.Muletista, 36, oggi.AddYears(-4));
            AggiungiPatente(beatrice, TipoPatente.Muletto, oggi.AddDays(420));

            Persona(organico, "Nicola", "Greco", Ruolo.Operaio, 38, oggi.AddYears(-5));
            Persona(organico, "Chiara", "Serra", Ruolo.Operaio, 36, oggi.AddYears(-1));
            NuovoTeam(organico, "Team 3", "Alessandro Gallo", "Luigi Marini", "Beatrice Villa", "Nicola Greco", "Chiara Serra");

            // Team 4: capogruppo Stefano Fabbri. Valentina Testa ha la patente della gru in scadenza.
            var stefano = Persona(organico, "Stefano", "Fabbri", Ruolo.Gruista, 36, oggi.AddYears(-11));
            Abilita(stefano, Ruolo.Muletista);
            AggiungiPatente(stefano, TipoPatente.Gru, oggi.AddDays(600));
            AggiungiPatente(stefano, TipoPatente.Muletto, oggi.AddDays(600));

            var marco = Persona(organico, "Marco", "Bruno", Ruolo.Muletista, 36, oggi.AddYears(-4));
            AggiungiPatente(marco, TipoPatente.Muletto, oggi.AddDays(280));

            var valentina = Persona(organico, "Valentina", "Testa", Ruolo.Gruista, 36, oggi.AddYears(-6));
            AggiungiPatente(valentina, TipoPatente.Gru, oggi.AddDays(12));

            Persona(organico, "Matteo", "Sala", Ruolo.Operaio, 36, oggi.AddYears(-2));
            Persona(organico, "Federica", "Gori", Ruolo.Operaio, 38, oggi.AddYears(-3));
            NuovoTeam(organico, "Team 4", "Stefano Fabbri", "Marco Bruno", "Valentina Testa", "Matteo Sala", "Federica Gori");

            // Team 5: capogruppo Roberto Vitale. Andrea ha il contratto scaduto, Laura inizia tra dieci giorni.
            var roberto = Persona(organico, "Roberto", "Vitale", Ruolo.Gruista, 36, oggi.AddYears(-12));
            AggiungiPatente(roberto, TipoPatente.Gru, oggi.AddDays(450));

            var silvia = Persona(organico, "Silvia", "Monti", Ruolo.Muletista, 36, oggi.AddYears(-3));
            AggiungiPatente(silvia, TipoPatente.Muletto, oggi.AddDays(330));

            Persona(organico, "Andrea", "Pellegrini", Ruolo.Operaio, 36, oggi.AddYears(-3), fine: oggi.AddDays(-5));
            Persona(organico, "Laura", "Basile", Ruolo.Operaio, 36, oggi.AddDays(10));
            NuovoTeam(organico, "Team 5", "Roberto Vitale", "Silvia Monti", "Andrea Pellegrini", "Laura Basile");

            // Marta non è in nessun team.
            Persona(organico, "Marta", "Pini", Ruolo.Operaio, 36, oggi.AddDays(-90));

            return organico;
        }

        private static Utente NuovoUtente(string nome, string cognome, bool supervisore = false) => new()
        {
            Email = $"{nome}.{cognome}@portplanner.it".ToLowerInvariant(),
            PasswordHash = Utente.CalcolaHash(PasswordDiProva),
            Nome = nome,
            Cognome = cognome,
            IsSupervisore = supervisore,
        };

        // Crea un lavoratore con il suo utente e lo aggiunge all'organico, con il nome completo come chiave.
        // Le ore settimanali sono vuote per chi è a chiamata.
        private static Dipendente Persona(Organico organico, string nome, string cognome, Ruolo principale, int? ore,
            DateOnly inizioContratto, TipoContratto contratto = TipoContratto.AContratto, DateOnly? fine = null)
        {
            var dipendente = new Dipendente
            {
                Utente = NuovoUtente(nome, cognome),
                // Numeri chiaramente finti, il repository è pubblico.
                Telefono = $"+39 000 000 {organico.Persone.Count + 101:0000}",
                TipoContratto = contratto,
                OreSettimanali = ore,
                DataInizioContratto = inizioContratto,
                DataFineContratto = fine,
                RuoloPrincipale = principale,
            };

            organico.Persone.Add($"{nome} {cognome}", dipendente);
            return dipendente;
        }

        // Un ruolo in più oltre a quello principale. L'Operaio non si aggiunge mai: lo può fare chiunque.
        private static void Abilita(Dipendente dipendente, Ruolo ruolo)
        {
            dipendente.Ruoli.Add(new DipendenteRuolo { Ruolo = ruolo });
        }

        private static void AggiungiPatente(Dipendente dipendente, TipoPatente tipo, DateOnly scadenza)
        {
            dipendente.Patenti.Add(new Patente { Tipo = tipo, Scadenza = scadenza });
        }

        // Il primo nome è il capogruppo; l'elenco comprende anche lui, perché è un membro del team.
        private static void NuovoTeam(Organico organico, string nome, params string[] membri)
        {
            var team = new Team { Nome = nome, Capogruppo = organico.Persone[membri[0]] };

            foreach (var membro in membri)
                team.Membri.Add(organico.Persone[membro]);

            organico.Team.Add(nome, team);
        }
    }
}
