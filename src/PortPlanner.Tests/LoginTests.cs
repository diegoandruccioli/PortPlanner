using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Infrastructure;
using PortPlanner.Services;
using PortPlanner.Services.Shared;

namespace PortPlanner.Tests
{
    // Prova il login sul seed: chi entra e con quale profilo.
    public class LoginTests
    {
        private static readonly DateOnly Oggi = new(2026, 11, 17);

        private static PortPlannerDbContext ContestoConSeed()
        {
            var options = new DbContextOptionsBuilder<PortPlannerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var contesto = new PortPlannerDbContext(options);
            DataGenerator.Initialize(contesto, Oggi);
            return contesto;
        }

        private static Task<UtenteAutenticatoDTO> Accedi(PortPlannerDbContext contesto, string email, string password = DataGenerator.PasswordDiProva) =>
            new SharedService(contesto).Query(new CheckLoginCredentialsQuery { Email = email, Password = password });

        [Fact]
        public async Task Il_supervisore_entra_senza_essere_un_lavoratore()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "mario.rossi@portplanner.it");

            Assert.Equal(Profilo.Supervisore, utente.Profilo);
            Assert.Equal("Mario", utente.Nome);
            Assert.Equal("Rossi", utente.Cognome);
            Assert.Null(utente.DipendenteId);
            Assert.Null(utente.TeamId);
        }

        [Fact]
        public async Task Il_capogruppo_entra_con_il_suo_team()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "anna.conti@portplanner.it");

            var team2 = contesto.Team.Single(t => t.Nome == "Team 2");
            Assert.Equal(Profilo.Capogruppo, utente.Profilo);
            Assert.Equal(team2.Id, utente.TeamId);
            Assert.Equal(team2.CapogruppoId, utente.DipendenteId);
        }

        [Fact]
        public async Task Un_dipendente_di_un_team_entra_come_dipendente()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "sara.fontana@portplanner.it");

            Assert.Equal(Profilo.Dipendente, utente.Profilo);
            Assert.Equal(contesto.Team.Single(t => t.Nome == "Team 2").Id, utente.TeamId);
            Assert.NotNull(utente.DipendenteId);
        }

        [Fact]
        public async Task Chi_non_ha_un_team_entra_come_dipendente_senza_team()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "marta.pini@portplanner.it");

            Assert.Equal(Profilo.Dipendente, utente.Profilo);
            Assert.NotNull(utente.DipendenteId);
            Assert.Null(utente.TeamId);
        }

        [Fact]
        public async Task Il_lavoratore_a_chiamata_entra_come_dipendente()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "luigi.marini@portplanner.it");

            Assert.Equal(Profilo.Dipendente, utente.Profilo);
        }

        [Fact]
        public async Task Tutti_gli_utenti_del_seed_entrano_con_la_password_di_prova()
        {
            using var contesto = ContestoConSeed();

            var profili = new List<Profilo>();
            foreach (var email in contesto.Utenti.Select(u => u.Email).ToList())
                profili.Add((await Accedi(contesto, email)).Profilo);

            Assert.Equal(26, profili.Count);
            Assert.Equal(1, profili.Count(p => p == Profilo.Supervisore));
            Assert.Equal(5, profili.Count(p => p == Profilo.Capogruppo));
            Assert.Equal(20, profili.Count(p => p == Profilo.Dipendente));
        }

        [Fact]
        public async Task Il_capogruppo_cambia_quando_cambia_il_team()
        {
            // Il profilo si calcola dal database a ogni login, non è un campo: nominando un altro capogruppo
            // Anna Conti torna dipendente e il nuovo capogruppo entra come capogruppo.
            using var contesto = ContestoConSeed();
            var team2 = contesto.Team.Single(t => t.Nome == "Team 2");
            team2.CapogruppoId = contesto.Dipendenti.Single(d => d.Utente.Cognome == "Moro").Id;
            contesto.SaveChanges();

            Assert.Equal(Profilo.Dipendente, (await Accedi(contesto, "anna.conti@portplanner.it")).Profilo);
            Assert.Equal(Profilo.Capogruppo, (await Accedi(contesto, "luca.moro@portplanner.it")).Profilo);
        }

        [Fact]
        public async Task L_email_si_confronta_senza_maiuscole_e_senza_spazi()
        {
            using var contesto = ContestoConSeed();

            var utente = await Accedi(contesto, "  Mario.ROSSI@PortPlanner.it ");

            Assert.Equal("mario.rossi@portplanner.it", utente.Email);
        }

        [Theory]
        [InlineData("Sbagliata")]
        [InlineData("prova123")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Con_la_password_sbagliata_o_vuota_non_si_entra(string password)
        {
            using var contesto = ContestoConSeed();

            await Assert.ThrowsAsync<LoginException>(() => Accedi(contesto, "mario.rossi@portplanner.it", password));
        }

        [Theory]
        [InlineData("nessuno@portplanner.it")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Con_un_email_inesistente_o_vuota_non_si_entra(string email)
        {
            using var contesto = ContestoConSeed();

            await Assert.ThrowsAsync<LoginException>(() => Accedi(contesto, email));
        }

        [Fact]
        public async Task Email_e_password_sbagliate_danno_lo_stesso_messaggio()
        {
            // Non si deve capire se un'email esiste.
            using var contesto = ContestoConSeed();

            var conEmailSbagliata = await Assert.ThrowsAsync<LoginException>(() => Accedi(contesto, "nessuno@portplanner.it"));
            var conPasswordSbagliata = await Assert.ThrowsAsync<LoginException>(() => Accedi(contesto, "mario.rossi@portplanner.it", "Sbagliata"));

            Assert.Equal(conEmailSbagliata.Message, conPasswordSbagliata.Message);
        }

        [Fact]
        public async Task Un_account_che_non_e_ne_supervisore_ne_lavoratore_non_entra()
        {
            using var contesto = ContestoConSeed();
            contesto.Utenti.Add(new Utente { Email = "orfano@portplanner.it", PasswordHash = Utente.CalcolaHash("Prova123"), Nome = "Orfano", Cognome = "Senzaruolo" });
            contesto.SaveChanges();

            var ex = await Assert.ThrowsAsync<LoginException>(() => Accedi(contesto, "orfano@portplanner.it"));

            Assert.Equal("Email o password errate", ex.Message);
        }
    }
}
