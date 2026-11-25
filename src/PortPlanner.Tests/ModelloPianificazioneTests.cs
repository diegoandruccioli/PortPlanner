using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Services;

namespace PortPlanner.Tests
{
    // Controlla che il DbContext salvi e rilegga siti, barche e assegnazioni con le loro relazioni.
    // Il database in memoria non fa rispettare vincoli né indici univoci: le regole (un solo team per slot,
    // una persona su un solo sito per slot, barche che non si sovrappongono) le dovrà controllare il servizio.
    public class ModelloPianificazioneTests
    {
        private static readonly DateOnly Oggi = new(2026, 11, 17);

        private static PortPlannerDbContext NuovoContesto()
        {
            var options = new DbContextOptionsBuilder<PortPlannerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new PortPlannerDbContext(options);
        }

        private static Dipendente NuovoDipendente(string nome, string cognome, Ruolo ruolo) => new()
        {
            Utente = new Utente { Email = $"{nome}.{cognome}@portplanner.it".ToLower(), PasswordHash = Utente.CalcolaHash("x"), Nome = nome, Cognome = cognome },
            TipoContratto = TipoContratto.AContratto,
            OreSettimanali = 36,
            DataInizioContratto = new DateOnly(2020, 1, 1),
            RuoloPrincipale = ruolo,
        };

        private static Team NuovoTeam(string nome, Dipendente capogruppo)
        {
            var team = new Team { Nome = nome, Capogruppo = capogruppo };
            team.Membri.Add(capogruppo);
            return team;
        }

        [Fact]
        public void Barca_si_rilegge_con_sito_gestore_e_assegnazioni()
        {
            var anna = NuovoDipendente("Anna", "Conti", Ruolo.Gruista);
            var sara = NuovoDipendente("Sara", "Fontana", Ruolo.Operaio);
            var team2 = NuovoTeam("Team 2", anna);
            team2.Membri.Add(sara);

            var sito = new Sito { Nome = "Sito 1", Ordine = 1 };
            var barca = new Barca
            {
                Nome = "Barca A",
                Sito = sito,
                Arrivo = Oggi,
                Partenza = Oggi.AddDays(3),
                TipoMerce = TipoMerce.Container,
                QuantitaMerce = 1200,
                Gestione = new GestioneBarca { TeamGestore = team2 },
            };

            var assegnazione = new AssegnazioneTeam
            {
                Barca = barca,
                Data = Oggi.AddDays(1),
                Slot = Slot.Dalle6Alle12,
                Team = team2,
                Origine = OrigineAssegnazione.Capogruppo,
            };
            assegnazione.Persone.Add(new AssegnazionePersona { Dipendente = anna, Ruolo = Ruolo.Gruista });
            assegnazione.Persone.Add(new AssegnazionePersona { Dipendente = sara, Ruolo = Ruolo.Operaio });
            barca.AssegnazioniTeam.Add(assegnazione);

            using var contesto = NuovoContesto();
            contesto.Barche.Add(barca);
            contesto.SaveChanges();

            var letta = contesto.Barche
                .Include(b => b.Sito)
                .Include(b => b.Gestione).ThenInclude(g => g.TeamGestore)
                .Include(b => b.AssegnazioniTeam).ThenInclude(a => a.Persone).ThenInclude(p => p.Dipendente)
                .Single();

            Assert.Equal("Sito 1", letta.Sito.Nome);
            Assert.Equal("Team 2", letta.Gestione.TeamGestore.Nome);
            Assert.Equal(letta.Id, letta.Gestione.BarcaId);

            var assegnata = letta.AssegnazioniTeam.Single();
            Assert.Equal(Slot.Dalle6Alle12, assegnata.Slot);
            Assert.Equal(OrigineAssegnazione.Capogruppo, assegnata.Origine);
            Assert.Equal(2, assegnata.Persone.Count);
            Assert.Contains(assegnata.Persone, p => p.Ruolo == Ruolo.Gruista && p.DipendenteId == anna.Id);
        }

        [Fact]
        public void Barca_senza_gestore_ha_la_gestione_ma_nessun_team()
        {
            var barca = new Barca
            {
                Nome = "Barca B",
                Sito = new Sito { Nome = "Sito 2", Ordine = 2 },
                Arrivo = Oggi,
                Partenza = Oggi.AddDays(4),
                TipoMerce = TipoMerce.RinfuseSolide,
                QuantitaMerce = 5000,
                Gestione = new GestioneBarca(),
            };

            using var contesto = NuovoContesto();
            contesto.Barche.Add(barca);
            contesto.SaveChanges();

            var gestione = contesto.GestioniBarche.Single();

            Assert.Equal(barca.Id, gestione.BarcaId);
            Assert.Null(gestione.TeamGestoreId);
        }

        [Fact]
        public void Persona_di_un_altro_team_puo_essere_assegnata_allo_slot()
        {
            var anna = NuovoDipendente("Anna", "Conti", Ruolo.Gruista);
            var marco = NuovoDipendente("Marco", "Bruno", Ruolo.Muletista);
            var team2 = NuovoTeam("Team 2", anna);
            var team4 = NuovoTeam("Team 4", marco);

            var barca = new Barca
            {
                Nome = "Barca A",
                Sito = new Sito { Nome = "Sito 1", Ordine = 1 },
                Arrivo = Oggi,
                Partenza = Oggi.AddDays(3),
                TipoMerce = TipoMerce.MerciVarie,
                QuantitaMerce = 800,
            };
            var assegnazione = new AssegnazioneTeam { Barca = barca, Data = Oggi, Slot = Slot.Dalle12Alle18, Team = team2, Origine = OrigineAssegnazione.Supervisore };
            assegnazione.Persone.Add(new AssegnazionePersona { Dipendente = marco, Ruolo = Ruolo.Muletista });

            using var contesto = NuovoContesto();
            contesto.Team.AddRange(team2, team4);
            contesto.AssegnazioniTeam.Add(assegnazione);
            contesto.SaveChanges();

            var persona = contesto.AssegnazioniPersone.Include(p => p.Dipendente).Single();

            Assert.Equal(team4.Id, persona.Dipendente.TeamId);
            Assert.NotEqual(team2.Id, persona.Dipendente.TeamId);
        }

        private static Barca NuovaBarca() => new()
        {
            Nome = "Barca A",
            Sito = new Sito { Nome = "Sito 1", Ordine = 1 },
            Arrivo = Oggi,
            Partenza = Oggi.AddDays(3),
            TipoMerce = TipoMerce.Container,
            QuantitaMerce = 100,
        };

        [Fact]
        public void Slot_non_lavorativo_si_distingue_per_barca_giorno_e_slot()
        {
            var barca = NuovaBarca();

            using var contesto = NuovoContesto();
            contesto.Barche.Add(barca);
            contesto.SlotNonLavorativi.AddRange(
                new SlotNonLavorativo { Barca = barca, Data = Oggi, Slot = Slot.Dalle0Alle6 },
                new SlotNonLavorativo { Barca = barca, Data = Oggi, Slot = Slot.Dalle18Alle24 });
            contesto.SaveChanges();

            var bloccati = contesto.SlotNonLavorativi.Select(s => s.Slot).OrderBy(s => s).ToList();

            Assert.Equal(new[] { Slot.Dalle0Alle6, Slot.Dalle18Alle24 }, bloccati);
            Assert.Equal(2, contesto.Barche.Include(b => b.SlotNonLavorativi).Single().SlotNonLavorativi.Count);
        }

        [Fact]
        public void Giorno_bloccato_sono_quattro_righe_una_per_slot()
        {
            var barca = NuovaBarca();

            using var contesto = NuovoContesto();
            contesto.Barche.Add(barca);
            foreach (var slot in Enum.GetValues<Slot>())
                contesto.SlotNonLavorativi.Add(new SlotNonLavorativo { Barca = barca, Data = Oggi.AddDays(2), Slot = slot });
            contesto.SaveChanges();

            Assert.Equal(4, contesto.SlotNonLavorativi.Count(s => s.Data == Oggi.AddDays(2)));
        }

        [Fact]
        public void Blocchi_e_assegnazione_del_supervisore_convivono_sullo_stesso_slot()
        {
            var anna = NuovoDipendente("Anna", "Conti", Ruolo.Gruista);
            var luca = NuovoDipendente("Luca", "Moro", Ruolo.Gruista);
            var team1 = NuovoTeam("Team 1", luca);
            var team2 = NuovoTeam("Team 2", anna);
            var barca = NuovaBarca();
            barca.Gestione = new GestioneBarca { TeamGestore = team2 };

            using var contesto = NuovoContesto();
            contesto.Team.AddRange(team1, team2);
            contesto.Barche.Add(barca);
            contesto.SlotNonLavorativi.Add(new SlotNonLavorativo { Barca = barca, Data = Oggi, Slot = Slot.Dalle6Alle12 });
            contesto.AssegnazioniTeam.Add(new AssegnazioneTeam
            {
                Barca = barca, Data = Oggi, Slot = Slot.Dalle6Alle12, Team = team1, Origine = OrigineAssegnazione.Supervisore,
            });
            contesto.SaveChanges();

            // Lo slot è bloccato per il gestore (Team 2) ma il supervisore ci ha messo il Team 1.
            Assert.Single(contesto.SlotNonLavorativi);
            Assert.Equal(team1.Id, contesto.AssegnazioniTeam.Single().TeamId);
            Assert.Equal(team2.Id, contesto.GestioniBarche.Single().TeamGestoreId);
        }

        [Fact]
        public void Cambiando_il_team_gestore_i_blocchi_restano()
        {
            var team2 = NuovoTeam("Team 2", NuovoDipendente("Anna", "Conti", Ruolo.Gruista));
            var team4 = NuovoTeam("Team 4", NuovoDipendente("Marco", "Bruno", Ruolo.Muletista));
            var barca = NuovaBarca();
            barca.Gestione = new GestioneBarca { TeamGestore = team2 };

            using var contesto = NuovoContesto();
            contesto.Team.AddRange(team2, team4);
            contesto.Barche.Add(barca);
            contesto.SlotNonLavorativi.Add(new SlotNonLavorativo { Barca = barca, Data = Oggi, Slot = Slot.Dalle0Alle6 });
            contesto.SaveChanges();

            contesto.GestioniBarche.Single().TeamGestoreId = team4.Id;
            contesto.SaveChanges();

            Assert.Equal(team4.Id, contesto.GestioniBarche.Single().TeamGestoreId);
            Assert.Single(contesto.SlotNonLavorativi);
        }
    }
}
