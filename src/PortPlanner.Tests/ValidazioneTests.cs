using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Infrastructure;
using PortPlanner.Services;
using PortPlanner.Services.Validazione;

namespace PortPlanner.Tests
{
    // Le regole contrattuali sui turni producono avvisi, mai blocchi. Le regole pure si provano su turni inventati,
    // il servizio sui casi del seed (Marco Bruno e Giorgio Neri).
    public class ValidazioneTests
    {
        private static readonly DateOnly Oggi = new(2026, 11, 17);
        private static readonly DateOnly Venerdi = new(2026, 11, 20);
        private static readonly DateOnly Sabato = new(2026, 11, 21);

        private static (DateOnly Data, Slot Slot) Turno(DateOnly data, Slot slot) => (data, slot);

        private static List<AvvisoContrattualeDTO> Valuta(DateOnly data, Slot slot, params (DateOnly Data, Slot Slot)[] esistenti) =>
            RegoleContrattuali.Valuta(esistenti, data, slot);

        // --- Le regole, senza database ---

        [Fact]
        public void Un_turno_da_solo_non_da_avvisi()
        {
            Assert.Empty(Valuta(Venerdi, Slot.Dalle6Alle12));
        }

        [Fact]
        public void Due_slot_di_fila_nello_stesso_giorno_sono_dodici_ore_consecutive_e_non_un_riposo_breve()
        {
            var avvisi = Valuta(Venerdi, Slot.Dalle12Alle18, Turno(Venerdi, Slot.Dalle6Alle12));

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.TurniConsecutivi, avviso.Tipo);
            Assert.Contains("12 h", avviso.Testo);
        }

        [Fact]
        public void Dalle_18_alle_24_e_dalle_0_alle_6_del_giorno_dopo_sono_un_solo_blocco_da_dodici_ore()
        {
            var avvisi = Valuta(Sabato, Slot.Dalle0Alle6, Turno(Venerdi, Slot.Dalle18Alle24));

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.TurniConsecutivi, avviso.Tipo);
            Assert.Contains("12 h", avviso.Testo);
        }

        [Fact]
        public void Tre_slot_di_fila_sono_diciotto_ore_e_restano_un_avviso_normale()
        {
            var avvisi = Valuta(Venerdi, Slot.Dalle12Alle18, Turno(Venerdi, Slot.Dalle0Alle6), Turno(Venerdi, Slot.Dalle6Alle12));

            var avviso = Assert.Single(avvisi);
            Assert.Contains("18 h", avviso.Testo);
            Assert.Equal(GravitaAvviso.Normale, avviso.Gravita);
        }

        [Fact]
        public void Dalle_12_alle_18_e_dalle_0_alle_6_del_giorno_dopo_danno_sei_ore_di_riposo()
        {
            var avvisi = Valuta(Sabato, Slot.Dalle0Alle6, Turno(Venerdi, Slot.Dalle12Alle18));

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.RiposoGiornaliero, avviso.Tipo);
            Assert.Contains("6 h", avviso.Testo);
        }

        [Fact]
        public void Dalle_6_alle_12_e_dalle_18_alle_24_dello_stesso_giorno_danno_sei_ore_di_riposo()
        {
            var avvisi = Valuta(Venerdi, Slot.Dalle18Alle24, Turno(Venerdi, Slot.Dalle6Alle12));

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.RiposoGiornaliero, avviso.Tipo);
        }

        [Fact]
        public void Con_dodici_ore_di_pausa_non_c_e_avviso_con_sei_si()
        {
            // Gli slot hanno confini ogni 6 ore, quindi la pausa dopo le 18 è di 6 ore (fino alle 0) o di 12 (fino alle 6):
            // il limite di 11 ore separa proprio questi due casi.
            Assert.Empty(Valuta(Sabato, Slot.Dalle6Alle12, Turno(Venerdi, Slot.Dalle12Alle18)));
            Assert.Single(Valuta(Sabato, Slot.Dalle0Alle6, Turno(Venerdi, Slot.Dalle12Alle18)));
        }

        [Fact]
        public void Dalle_6_alle_12_e_dalle_0_alle_6_del_giorno_dopo_hanno_diciotto_ore_di_riposo()
        {
            Assert.Empty(Valuta(Sabato, Slot.Dalle0Alle6, Turno(Venerdi, Slot.Dalle6Alle12)));
        }

        [Fact]
        public void Il_riposo_si_controlla_sia_prima_sia_dopo_il_turno()
        {
            // 12-18 del venerdì e 0-6 del sabato, con in mezzo niente: valutando uno dei due si vede l'altro.
            var valutandoIlPrimo = Valuta(Venerdi, Slot.Dalle12Alle18, Turno(Sabato, Slot.Dalle0Alle6));
            var valutandoIlSecondo = Valuta(Sabato, Slot.Dalle0Alle6, Turno(Venerdi, Slot.Dalle12Alle18));

            Assert.Single(valutandoIlPrimo);
            Assert.Single(valutandoIlSecondo);
        }

        [Fact]
        public void Un_turno_gia_presente_non_viene_contato_due_volte()
        {
            var avvisi = Valuta(Venerdi, Slot.Dalle12Alle18, Turno(Venerdi, Slot.Dalle6Alle12), Turno(Venerdi, Slot.Dalle12Alle18));

            Assert.Single(avvisi);
        }

        [Fact]
        public void Gli_avvisi_di_riposo_e_di_turni_consecutivi_non_sono_mai_forti()
        {
            var avvisi = Valuta(Venerdi, Slot.Dalle12Alle18, Turno(Venerdi, Slot.Dalle6Alle12), Turno(Sabato, Slot.Dalle0Alle6));

            Assert.Equal(2, avvisi.Count);
            Assert.All(avvisi, a => Assert.Equal(GravitaAvviso.Normale, a.Gravita));
        }

        // --- Il servizio, sui casi del seed ---

        private static PortPlannerDbContext ContestoConSeed()
        {
            var options = new DbContextOptionsBuilder<PortPlannerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var contesto = new PortPlannerDbContext(options);
            DataGenerator.Initialize(contesto, Oggi);
            return contesto;
        }

        private static Guid IdDi(PortPlannerDbContext contesto, string cognome) =>
            contesto.Dipendenti.Single(d => d.Utente.Cognome == cognome).Id;

        [Fact]
        public async Task Marco_Bruno_alle_18_24_e_poi_alle_0_6_ha_solo_dodici_ore_consecutive()
        {
            using var contesto = ContestoConSeed();
            var servizio = new ValidazioneService(contesto);

            var avvisi = await servizio.Query(new ValutaTurnoQuery
            {
                DipendenteId = IdDi(contesto, "Bruno"),
                Data = Oggi.AddDays(1),
                Slot = Slot.Dalle18Alle24
            });

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.TurniConsecutivi, avviso.Tipo);
            Assert.Contains("12 h", avviso.Testo);
        }

        [Fact]
        public async Task Giorgio_Neri_il_venerdi_ha_dodici_ore_consecutive_e_riposo_nei_limiti()
        {
            using var contesto = ContestoConSeed();
            var servizio = new ValidazioneService(contesto);

            var avvisi = await servizio.Query(new ValutaTurnoQuery
            {
                DipendenteId = IdDi(contesto, "Neri"),
                Data = Oggi.AddDays(3),
                Slot = Slot.Dalle12Alle18
            });

            var avviso = Assert.Single(avvisi);
            Assert.Equal(TipoAvviso.TurniConsecutivi, avviso.Tipo);
            Assert.Contains("12 h", avviso.Testo);
        }

        [Fact]
        public async Task Un_candidato_non_ancora_assegnato_riceve_gli_avvisi_che_avrebbe()
        {
            using var contesto = ContestoConSeed();
            var servizio = new ValidazioneService(contesto);

            // Anna Conti lavora oggi dalle 6 alle 12: aggiungere le 12-18 le darebbe 12 ore di fila.
            var avvisi = await servizio.Query(new ValutaTurnoQuery
            {
                DipendenteId = IdDi(contesto, "Conti"),
                Data = Oggi,
                Slot = Slot.Dalle12Alle18
            });

            Assert.Equal(TipoAvviso.TurniConsecutivi, Assert.Single(avvisi).Tipo);
        }

        [Fact]
        public async Task Un_turno_segnato_non_svolto_non_conta()
        {
            using var contesto = ContestoConSeed();
            var servizio = new ValidazioneService(contesto);
            var marco = IdDi(contesto, "Bruno");

            // Marco non ha lavorato lo slot 0-6 del giorno dopo: il suo 18-24 torna a essere un turno solo.
            var nonSvolto = contesto.AssegnazioniPersone
                .Where(p => p.DipendenteId == marco && p.AssegnazioneTeam.Data == Oggi.AddDays(2) && p.AssegnazioneTeam.Slot == Slot.Dalle0Alle6)
                .Single();
            nonSvolto.NonSvolto = true;
            await contesto.SaveChangesAsync();

            var avvisi = await servizio.Query(new ValutaTurnoQuery
            {
                DipendenteId = marco,
                Data = Oggi.AddDays(1),
                Slot = Slot.Dalle18Alle24
            });

            Assert.Empty(avvisi);
        }

        [Fact]
        public async Task Il_vincolo_e_sulla_persona_anche_se_i_turni_sono_su_siti_e_team_diversi()
        {
            using var contesto = ContestoConSeed();
            var servizio = new ValidazioneService(contesto);

            // Luigi Marini (a chiamata) lavora venerdì 12-18 sulla Barca B: aggiungere venerdì 6-12 su un altro sito
            // gli darebbe lo stesso 12 ore consecutive.
            var avvisi = await servizio.Query(new ValutaTurnoQuery
            {
                DipendenteId = IdDi(contesto, "Marini"),
                Data = Oggi.AddDays(3),
                Slot = Slot.Dalle6Alle12
            });

            Assert.Equal(TipoAvviso.TurniConsecutivi, Assert.Single(avvisi).Tipo);
        }
    }
}
