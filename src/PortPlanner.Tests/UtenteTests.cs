using PortPlanner.Domain;

namespace PortPlanner.Tests
{
    public class UtenteTests
    {
        private static Utente ConPassword(string password) =>
            new() { PasswordHash = Utente.CalcolaHash(password) };

        [Fact]
        public void Password_corretta_viene_accettata()
        {
            Assert.True(ConPassword("Prova123").IsMatchWithPassword("Prova123"));
        }

        [Theory]
        [InlineData("prova123")]
        [InlineData("Sbagliata")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Password_errata_o_vuota_viene_rifiutata(string tentativo)
        {
            Assert.False(ConPassword("Prova123").IsMatchWithPassword(tentativo));
        }

        [Fact]
        public void Hash_e_compatibile_con_quello_del_template()
        {
            // SHA-256 di "Prova" in Base64, come nel seed del template.
            Assert.Equal("M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", Utente.CalcolaHash("Prova"));
        }
    }
}
