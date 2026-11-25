using PortPlanner.Domain;

namespace PortPlanner.Tests
{
    public class PatenteTests
    {
        private static readonly DateOnly Oggi = new(2026, 11, 17);

        private static Patente ConScadenza(DateOnly scadenza) =>
            new() { Tipo = TipoPatente.Gru, Scadenza = scadenza };

        [Fact]
        public void Scadenza_lontana_e_valida()
        {
            Assert.Equal(StatoPatente.Valida, ConScadenza(Oggi.AddDays(31)).StatoAl(Oggi));
        }

        [Fact]
        public void Scadenza_entro_trenta_giorni_e_in_scadenza()
        {
            Assert.Equal(StatoPatente.InScadenza, ConScadenza(Oggi.AddDays(30)).StatoAl(Oggi));
            Assert.Equal(StatoPatente.InScadenza, ConScadenza(Oggi.AddDays(1)).StatoAl(Oggi));
        }

        [Fact]
        public void Il_giorno_di_scadenza_la_patente_vale_ancora()
        {
            Assert.Equal(StatoPatente.InScadenza, ConScadenza(Oggi).StatoAl(Oggi));
        }

        [Fact]
        public void Dal_giorno_dopo_la_scadenza_e_scaduta()
        {
            Assert.Equal(StatoPatente.Scaduta, ConScadenza(Oggi.AddDays(-1)).StatoAl(Oggi));
        }
    }
}
