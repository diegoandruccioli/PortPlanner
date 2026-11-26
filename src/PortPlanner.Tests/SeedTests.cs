using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Infrastructure;
using PortPlanner.Services;

namespace PortPlanner.Tests
{
    // Controlla i dati di partenza della demo. Il database in memoria non fa rispettare vincoli né indici univoci,
    // quindi le regole del dominio (un solo team per slot, una persona su un solo sito per slot, barche che non
    // si sovrappongono) si verificano qui sul seed e, più avanti, nei servizi.
    public class SeedTests
    {
        private static readonly DateOnly Oggi = new(2026, 11, 17);

        // Date diverse per verificare che il seed sia coerente in qualsiasi giorno, anche a cavallo di un anno bisestile.
        public static IEnumerable<object[]> DateDiOggi => new[]
        {
            new object[] { new DateOnly(2026, 11, 17) },
            new object[] { new DateOnly(2027, 12, 29) },
            new object[] { new DateOnly(2028, 2, 27) },
        };

        private static PortPlannerDbContext ContestoConSeed(DateOnly oggi)
        {
            var options = new DbContextOptionsBuilder<PortPlannerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var contesto = new PortPlannerDbContext(options);
            DataGenerator.Initialize(contesto, oggi);
            return contesto;
        }

        // Una riga per ogni persona assegnata a uno slot, con il sito in cui lavora.
        private static List<(Guid DipendenteId, DateOnly Data, Slot Slot, Guid SitoId)> TurniDelleSingolePersone(PortPlannerDbContext contesto) =>
            contesto.AssegnazioniPersone
                .Include(p => p.AssegnazioneTeam).ThenInclude(a => a.Barca)
                .AsEnumerable()
                .Select(p => (p.DipendenteId, p.AssegnazioneTeam.Data, p.AssegnazioneTeam.Slot, p.AssegnazioneTeam.Barca.SitoId))
                .ToList();

        // --- Le quattro regole del dominio ---

        [Theory]
        [MemberData(nameof(DateDiOggi))]
        public void Su_ogni_barca_c_e_un_solo_team_per_data_e_slot(DateOnly oggi)
        {
            using var contesto = ContestoConSeed(oggi);

            var doppioni = contesto.AssegnazioniTeam
                .AsEnumerable()
                .GroupBy(a => (a.BarcaId, a.Data, a.Slot))
                .Where(g => g.Count() > 1)
                .ToList();

            Assert.Empty(doppioni);
        }

        [Theory]
        [MemberData(nameof(DateDiOggi))]
        public void Nessuna_persona_lavora_in_due_siti_nello_stesso_slot(DateOnly oggi)
        {
            using var contesto = ContestoConSeed(oggi);

            var inDueSiti = TurniDelleSingolePersone(contesto)
                .GroupBy(t => (t.DipendenteId, t.Data, t.Slot))
                .Where(g => g.Select(t => t.SitoId).Distinct().Count() > 1)
                .ToList();

            Assert.Empty(inDueSiti);
        }

        [Fact]
        public void Una_persona_non_compare_due_volte_nello_stesso_slot()
        {
            // Vale anche sullo stesso sito: un lavoratore fa un solo ruolo per slot.
            using var contesto = ContestoConSeed(Oggi);

            var doppioni = TurniDelleSingolePersone(contesto)
                .GroupBy(t => (t.DipendenteId, t.Data, t.Slot))
                .Where(g => g.Count() > 1)
                .ToList();

            Assert.Empty(doppioni);
        }

        [Theory]
        [MemberData(nameof(DateDiOggi))]
        public void Ogni_capogruppo_e_membro_del_suo_team(DateOnly oggi)
        {
            using var contesto = ContestoConSeed(oggi);

            var team = contesto.Team.Include(t => t.Membri).ToList();

            Assert.Equal(5, team.Count);
            Assert.All(team, t =>
            {
                Assert.Contains(t.Membri, m => m.Id == t.CapogruppoId);
                Assert.Equal(t.Id, contesto.Dipendenti.Single(d => d.Id == t.CapogruppoId).TeamId);
            });
        }

        [Theory]
        [MemberData(nameof(DateDiOggi))]
        public void Le_barche_dello_stesso_sito_non_si_sovrappongono(DateOnly oggi)
        {
            using var contesto = ContestoConSeed(oggi);

            var barche = contesto.Barche.AsEnumerable();

            Assert.All(barche, b => Assert.True(b.Partenza >= b.Arrivo, $"{b.Nome}: la partenza precede l'arrivo"));

            foreach (var delSito in barche.GroupBy(b => b.SitoId))
            {
                var inOrdine = delSito.OrderBy(b => b.Arrivo).ToList();
                for (var i = 1; i < inOrdine.Count; i++)
                    Assert.True(inOrdine[i - 1].Partenza < inOrdine[i].Arrivo,
                        $"{inOrdine[i - 1].Nome} parte dopo l'arrivo di {inOrdine[i].Nome} sullo stesso sito");
            }
        }

        // --- Coerenza dei dati ---

        [Fact]
        public void Il_seed_si_puo_eseguire_due_volte_senza_duplicare_i_dati()
        {
            using var contesto = ContestoConSeed(Oggi);
            var lavoratori = contesto.Dipendenti.Count();
            var assegnazioni = contesto.AssegnazioniTeam.Count();

            DataGenerator.Initialize(contesto, Oggi);

            Assert.Equal(lavoratori, contesto.Dipendenti.Count());
            Assert.Equal(assegnazioni, contesto.AssegnazioniTeam.Count());
        }

        [Fact]
        public void Le_date_sono_relative_a_oggi()
        {
            var primo = ContestoConSeed(Oggi);
            var dopo = ContestoConSeed(Oggi.AddDays(10));

            var barcaA = primo.Barche.Single(b => b.Nome == "Barca A");
            var barcaAUnaSettimanaDopo = dopo.Barche.Single(b => b.Nome == "Barca A");

            Assert.Equal(Oggi, barcaA.Arrivo);
            Assert.Equal(Oggi.AddDays(3), barcaA.Partenza);
            Assert.Equal(Oggi.AddDays(10), barcaAUnaSettimanaDopo.Arrivo);
            Assert.Equal(Oggi.AddDays(7), primo.Barche.Single(b => b.Nome == "Barca D").Arrivo);
        }

        [Fact]
        public void Gli_utenti_hanno_email_uniche_e_la_stessa_password_di_prova()
        {
            using var contesto = ContestoConSeed(Oggi);

            var utenti = contesto.Utenti.ToList();

            Assert.Equal(26, utenti.Count);
            Assert.Equal(utenti.Count, utenti.Select(u => u.Email).Distinct().Count());
            Assert.All(utenti, u => Assert.True(u.IsMatchWithPassword(DataGenerator.PasswordDiProva), u.Email));
            Assert.All(utenti, u => Assert.EndsWith("@portplanner.it", u.Email));
        }

        [Fact]
        public void Il_supervisore_e_uno_solo_e_non_e_un_lavoratore()
        {
            using var contesto = ContestoConSeed(Oggi);

            var supervisore = contesto.Utenti.Include(u => u.Dipendente).Single(u => u.IsSupervisore);

            Assert.Equal("Mario", supervisore.Nome);
            Assert.Null(supervisore.Dipendente);
            Assert.Equal(25, contesto.Dipendenti.Count());
            Assert.All(contesto.Dipendenti.Include(d => d.Utente), d => Assert.False(d.Utente.IsSupervisore));
        }

        [Fact]
        public void Le_ore_settimanali_ci_sono_solo_per_chi_e_a_contratto()
        {
            using var contesto = ContestoConSeed(Oggi);

            var lavoratori = contesto.Dipendenti.ToList();

            Assert.All(lavoratori.Where(d => d.TipoContratto == TipoContratto.AContratto),
                d => Assert.Contains(d.OreSettimanali, new int?[] { 36, 38 }));
            Assert.All(lavoratori.Where(d => d.TipoContratto == TipoContratto.AChiamata),
                d => Assert.Null(d.OreSettimanali));
        }

        [Fact]
        public void Chi_e_assegnato_a_un_ruolo_lo_ha_abilitato_o_e_operaio()
        {
            using var contesto = ContestoConSeed(Oggi);

            var fuoriRuolo = contesto.AssegnazioniPersone
                .Include(p => p.Dipendente).ThenInclude(d => d.Ruoli)
                .AsEnumerable()
                .Where(p => p.Ruolo != Ruolo.Operaio
                         && p.Dipendente.RuoloPrincipale != p.Ruolo
                         && p.Dipendente.Ruoli.All(r => r.Ruolo != p.Ruolo))
                .ToList();

            Assert.Empty(fuoriRuolo);
        }

        [Fact]
        public void Nessuna_riga_di_ruolo_abilitato_e_operaio_o_uguale_al_principale()
        {
            using var contesto = ContestoConSeed(Oggi);

            var extra = contesto.Dipendenti.Include(d => d.Ruoli).AsEnumerable().SelectMany(d => d.Ruoli.Select(r => (d.RuoloPrincipale, r.Ruolo)));

            Assert.All(extra, e =>
            {
                Assert.NotEqual(Ruolo.Operaio, e.Ruolo);
                Assert.NotEqual(e.RuoloPrincipale, e.Ruolo);
            });
        }

        // --- Dati di esempio che servono alle schermate ---

        [Fact]
        public void Il_seed_ha_cinque_barche_su_quattro_siti_e_il_sito_3_e_libero_oggi()
        {
            using var contesto = ContestoConSeed(Oggi);

            Assert.Equal(4, contesto.Siti.Count());
            Assert.Equal(5, contesto.Barche.Count());

            var sito3 = contesto.Siti.Include(s => s.Barche).Single(s => s.Ordine == 3);
            Assert.DoesNotContain(sito3.Barche, b => b.Arrivo <= Oggi && Oggi <= b.Partenza);
        }

        [Fact]
        public void Il_team_2_gestisce_la_barca_A_con_la_griglia_delle_specifiche()
        {
            using var contesto = ContestoConSeed(Oggi);

            var barcaA = contesto.Barche
                .Include(b => b.Gestione).ThenInclude(g => g.TeamGestore)
                .Include(b => b.AssegnazioniTeam)
                .Include(b => b.SlotNonLavorativi)
                .Single(b => b.Nome == "Barca A");

            Assert.Equal("Team 2", barcaA.Gestione.TeamGestore.Nome);
            Assert.Equal(7, barcaA.AssegnazioniTeam.Count);
            Assert.Equal(3, barcaA.AssegnazioniTeam.Count(a => a.Origine == OrigineAssegnazione.Capogruppo));
            Assert.Equal(4, barcaA.AssegnazioniTeam.Count(a => a.Origine == OrigineAssegnazione.Supervisore));
            Assert.Equal(5, barcaA.SlotNonLavorativi.Count);

            // Ven 20: il capogruppo ha scelto 6-12 e 12-18.
            var venerdi = barcaA.AssegnazioniTeam.Where(a => a.Data == Oggi.AddDays(3)).Select(a => a.Slot).OrderBy(s => s);
            Assert.Equal(new[] { Slot.Dalle6Alle12, Slot.Dalle12Alle18 }, venerdi);
        }

        [Fact]
        public void Il_supervisore_impone_il_team_2_nello_slot_di_giovedi_sulla_barca_che_gestisce()
        {
            // Gio 19, 12-18: lo slot è deciso dal supervisore, non dal capogruppo.
            using var contesto = ContestoConSeed(Oggi);

            var imposta = contesto.AssegnazioniTeam
                .Include(a => a.Team)
                .Include(a => a.Barca)
                .Single(a => a.Barca.Nome == "Barca A" && a.Data == Oggi.AddDays(2) && a.Slot == Slot.Dalle12Alle18);

            Assert.Equal("Team 2", imposta.Team.Nome);
            Assert.Equal(OrigineAssegnazione.Supervisore, imposta.Origine);
        }

        [Fact]
        public void Il_seed_contiene_un_caso_per_ogni_anomalia_di_anagrafica()
        {
            using var contesto = ContestoConSeed(Oggi);

            var stati = contesto.Patenti.AsEnumerable().GroupBy(p => p.StatoAl(Oggi)).ToDictionary(g => g.Key, g => g.Count());

            Assert.Equal(1, stati[StatoPatente.Scaduta]);
            Assert.Equal(1, stati[StatoPatente.InScadenza]);

            var neri = contesto.Dipendenti.Include(d => d.Patenti).Single(d => d.Utente.Cognome == "Neri");
            Assert.Equal(StatoPatente.Scaduta, neri.Patenti.Single(p => p.Tipo == TipoPatente.Muletto).StatoAl(Oggi));

            var lavoratori = contesto.Dipendenti.ToList();
            Assert.Single(lavoratori, d => d.DataFineContratto < Oggi);
            Assert.Single(lavoratori, d => d.DataInizioContratto > Oggi);
            Assert.Single(lavoratori, d => d.TeamId == null);
            Assert.Single(lavoratori, d => d.TipoContratto == TipoContratto.AChiamata);
        }

        [Fact]
        public void Luigi_Marini_e_a_chiamata_e_ha_la_patente_del_muletto_valida()
        {
            using var contesto = ContestoConSeed(Oggi);

            var luigi = contesto.Dipendenti.Include(d => d.Patenti).Single(d => d.Utente.Cognome == "Marini");

            Assert.Equal(TipoContratto.AChiamata, luigi.TipoContratto);
            Assert.Equal(StatoPatente.Valida, luigi.Patenti.Single(p => p.Tipo == TipoPatente.Muletto).StatoAl(Oggi));
        }

        [Fact]
        public void Il_seed_contiene_assegnazioni_con_anomalia_per_le_schermate()
        {
            using var contesto = ContestoConSeed(Oggi);

            var persone = contesto.AssegnazioniPersone
                .Include(p => p.Dipendente).ThenInclude(d => d.Patenti)
                .Include(p => p.Dipendente).ThenInclude(d => d.Utente)
                .Include(p => p.AssegnazioneTeam)
                .AsEnumerable()
                .ToList();

            // Ruolo con patente che non c'è o è scaduta.
            bool SenzaPatenteValida(AssegnazionePersona p)
            {
                var tipo = p.Ruolo switch { Ruolo.Gruista => TipoPatente.Gru, Ruolo.Muletista => TipoPatente.Muletto, _ => (TipoPatente?)null };
                if (tipo == null) return false;
                var patente = p.Dipendente.Patenti.SingleOrDefault(x => x.Tipo == tipo);
                return patente == null || patente.StatoAl(Oggi) == StatoPatente.Scaduta;
            }

            var senzaPatente = persone.Where(SenzaPatenteValida).Select(p => p.Dipendente.Utente.Cognome).Distinct().OrderBy(c => c).ToList();
            Assert.Equal(new[] { "Fiore", "Neri" }, senzaPatente);

            // Una persona che non è del team dello slot (un'aggiunta del supervisore).
            Assert.Contains(persone, p => p.Dipendente.TeamId != p.AssegnazioneTeam.TeamId && p.Dipendente.TeamId != null);
            Assert.Contains(persone, p => p.Dipendente.TeamId == null);

            // Contratto scaduto assegnato a uno slot.
            Assert.Contains(persone, p => p.Dipendente.DataFineContratto < Oggi);
        }

        [Fact]
        public void Marco_Bruno_finisce_alle_24_e_riparte_subito_alle_0_del_giorno_dopo()
        {
            using var contesto = ContestoConSeed(Oggi);

            var turni = contesto.AssegnazioniPersone
                .Where(p => p.Dipendente.Utente.Cognome == "Bruno")
                .Select(p => new { p.AssegnazioneTeam.Data, p.AssegnazioneTeam.Slot })
                .AsEnumerable()
                .OrderBy(t => t.Data).ThenBy(t => t.Slot)
                .ToList();

            // Riposo di zero ore: un turno 18-24 seguito da uno 0-6 il giorno successivo.
            var senzaRiposo = turni.Any(prima => prima.Slot == Slot.Dalle18Alle24
                && turni.Any(dopo => dopo.Data == prima.Data.AddDays(1) && dopo.Slot == Slot.Dalle0Alle6));

            Assert.True(senzaRiposo);
        }

        [Fact]
        public void Giorgio_Neri_lavora_dodici_ore_di_fila_il_venerdi()
        {
            using var contesto = ContestoConSeed(Oggi);

            var venerdi = contesto.AssegnazioniPersone
                .Where(p => p.Dipendente.Utente.Cognome == "Neri" && p.AssegnazioneTeam.Data == Oggi.AddDays(3))
                .Select(p => p.AssegnazioneTeam.Slot)
                .AsEnumerable()
                .OrderBy(s => s)
                .ToList();

            Assert.Equal(new[] { Slot.Dalle6Alle12, Slot.Dalle12Alle18 }, venerdi);
        }

        [Fact]
        public void Luigi_Marini_lavora_mercoledi_sulla_A_venerdi_sulla_B_e_lunedi_sulla_C_alle_12_18()
        {
            using var contesto = ContestoConSeed(Oggi);

            var turni = contesto.AssegnazioniPersone
                .Where(p => p.Dipendente.Utente.Cognome == "Marini")
                .Select(p => new { p.AssegnazioneTeam.Barca.Nome, p.AssegnazioneTeam.Data, p.AssegnazioneTeam.Slot })
                .AsEnumerable()
                .OrderBy(t => t.Data)
                .ToList();

            Assert.Equal(
                new[] { ("Barca A", 1), ("Barca B", 3), ("Barca C", 6) },
                turni.Select(t => (t.Nome, t.Data.DayNumber - Oggi.DayNumber)));
            Assert.All(turni, t => Assert.Equal(Slot.Dalle12Alle18, t.Slot));
        }
    }
}
