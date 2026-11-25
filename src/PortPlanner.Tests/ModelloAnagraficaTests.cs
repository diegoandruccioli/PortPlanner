using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Services;

namespace PortPlanner.Tests
{
    // Controlla che il DbContext salvi e rilegga le relazioni dell'anagrafica.
    // Il database in memoria non fa rispettare i vincoli, quindi qui si provano solo le relazioni, non l'integrità.
    public class ModelloAnagraficaTests
    {
        private static PortPlannerDbContext NuovoContesto()
        {
            var options = new DbContextOptionsBuilder<PortPlannerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new PortPlannerDbContext(options);
        }

        [Fact]
        public void Lavoratore_con_team_ruoli_e_patenti_si_rilegge_per_intero()
        {
            var utente = new Utente { Email = "anna.conti@portplanner.it", PasswordHash = Utente.CalcolaHash("x"), Nome = "Anna", Cognome = "Conti" };
            var anna = new Dipendente
            {
                Utente = utente,
                TipoContratto = TipoContratto.AContratto,
                OreSettimanali = 36,
                DataInizioContratto = new DateOnly(2020, 1, 1),
                RuoloPrincipale = Ruolo.Gruista,
            };
            anna.Ruoli.Add(new DipendenteRuolo { Ruolo = Ruolo.Muletista });
            anna.Patenti.Add(new Patente { Tipo = TipoPatente.Gru, Scadenza = new DateOnly(2027, 6, 30) });
            anna.Patenti.Add(new Patente { Tipo = TipoPatente.Muletto, Scadenza = new DateOnly(2027, 6, 30) });

            var team = new Team { Nome = "Team 2", Capogruppo = anna };
            team.Membri.Add(anna);

            Guid teamId;
            using (var scrittura = NuovoContesto())
            {
                scrittura.Team.Add(team);
                scrittura.SaveChanges();
                teamId = team.Id;

                var letto = scrittura.Team
                    .Include(t => t.Capogruppo).ThenInclude(d => d.Utente)
                    .Include(t => t.Capogruppo).ThenInclude(d => d.Ruoli)
                    .Include(t => t.Capogruppo).ThenInclude(d => d.Patenti)
                    .Include(t => t.Membri)
                    .Single(t => t.Id == teamId);

                Assert.Equal("Anna", letto.Capogruppo.Utente.Nome);
                Assert.Equal(letto.CapogruppoId, letto.Membri.Single().Id);
                Assert.Equal(letto.Id, letto.Membri.Single().TeamId);
                Assert.Equal(Ruolo.Muletista, letto.Capogruppo.Ruoli.Single().Ruolo);
                Assert.Equal(2, letto.Capogruppo.Patenti.Count);
            }
        }

        [Fact]
        public void Supervisore_ha_solo_l_utente_e_nessun_dipendente()
        {
            using var contesto = NuovoContesto();
            contesto.Utenti.Add(new Utente { Email = "mario.rossi@portplanner.it", PasswordHash = Utente.CalcolaHash("x"), Nome = "Mario", Cognome = "Rossi", IsSupervisore = true });
            contesto.SaveChanges();

            var supervisore = contesto.Utenti.Include(u => u.Dipendente).Single();

            Assert.True(supervisore.IsSupervisore);
            Assert.Null(supervisore.Dipendente);
        }

        [Fact]
        public void Lavoratore_senza_team_ha_TeamId_vuoto()
        {
            using var contesto = NuovoContesto();
            contesto.Dipendenti.Add(new Dipendente
            {
                Utente = new Utente { Email = "marta.pini@portplanner.it", PasswordHash = Utente.CalcolaHash("x"), Nome = "Marta", Cognome = "Pini" },
                TipoContratto = TipoContratto.AChiamata,
                DataInizioContratto = new DateOnly(2025, 3, 1),
                RuoloPrincipale = Ruolo.Operaio,
            });
            contesto.SaveChanges();

            var marta = contesto.Dipendenti.Single();

            Assert.Null(marta.TeamId);
            Assert.Null(marta.OreSettimanali);
        }
    }
}
