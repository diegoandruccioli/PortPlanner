using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PortPlanner.Domain;

namespace PortPlanner.Services.Validazione
{
    // Le regole contrattuali sui turni di una persona, senza database: ricevono i turni e restituiscono gli avvisi.
    // Il tempo è continuo (D19): gli slot contigui, anche a cavallo di mezzanotte, formano un blocco di lavoro.
    public static class RegoleContrattuali
    {
        public const int OreDiRiposoMinime = 11;

        // Due slot di fila, cioè 12 ore di lavoro continuo, sono già un avviso.
        public const int OreConsecutiveDaSegnalare = 12;

        private class Blocco
        {
            public long Inizio { get; set; }
            public long Fine { get; set; }
            public List<(DateOnly Data, Slot Slot)> Turni { get; } = new List<(DateOnly, Slot)>();
        }

        // Valuta cosa succede a una persona se lavora nello slot indicato, dati gli altri turni che ha già.
        // Se il turno è già tra quelli esistenti non viene contato due volte. Gli avvisi riguardano
        // solo il blocco che contiene il turno e i riposi subito prima e subito dopo.
        public static List<AvvisoContrattualeDTO> Valuta(IEnumerable<(DateOnly Data, Slot Slot)> turniEsistenti, DateOnly data, Slot slot)
        {
            var turni = turniEsistenti.Append((Data: data, Slot: slot)).Distinct().OrderBy(t => OraAssoluta(t.Data, t.Slot)).ToList();
            var blocchi = CreaBlocchi(turni);

            var inizioDelTurno = OraAssoluta(data, slot);
            var indice = blocchi.FindIndex(b => b.Inizio <= inizioDelTurno && inizioDelTurno < b.Fine);
            var blocco = blocchi[indice];

            var avvisi = new List<AvvisoContrattualeDTO>();

            var ore = blocco.Fine - blocco.Inizio;
            if (ore >= OreConsecutiveDaSegnalare)
            {
                var elenco = string.Join(" e ", blocco.Turni.Select(t => Descrivi(t.Data, t.Slot)));
                avvisi.Add(NuovoAvviso(TipoAvviso.TurniConsecutivi, $"{ore} h di lavoro consecutive ({elenco})"));
            }

            if (indice > 0)
                AggiungiRiposoSeBreve(avvisi, blocchi[indice - 1], blocco);
            if (indice < blocchi.Count - 1)
                AggiungiRiposoSeBreve(avvisi, blocco, blocchi[indice + 1]);

            return avvisi;
        }

        // Fonde in un blocco gli slot che iniziano esattamente quando finisce il precedente.
        private static List<Blocco> CreaBlocchi(List<(DateOnly Data, Slot Slot)> turniOrdinati)
        {
            var blocchi = new List<Blocco>();
            foreach (var turno in turniOrdinati)
            {
                var inizio = OraAssoluta(turno.Data, turno.Slot);
                var ultimo = blocchi.Count > 0 ? blocchi[blocchi.Count - 1] : null;

                if (ultimo == null || ultimo.Fine != inizio)
                {
                    ultimo = new Blocco { Inizio = inizio };
                    blocchi.Add(ultimo);
                }

                ultimo.Fine = inizio + SlotExtensions.OreDiUnoSlot;
                ultimo.Turni.Add(turno);
            }
            return blocchi;
        }

        private static void AggiungiRiposoSeBreve(List<AvvisoContrattualeDTO> avvisi, Blocco prima, Blocco dopo)
        {
            var riposo = dopo.Inizio - prima.Fine;
            if (riposo >= OreDiRiposoMinime)
                return;

            var ultimo = prima.Turni[prima.Turni.Count - 1];
            var primo = dopo.Turni[0];
            avvisi.Add(NuovoAvviso(TipoAvviso.RiposoGiornaliero,
                $"Riposo di {riposo} h tra il turno {Descrivi(ultimo.Data, ultimo.Slot)} e il turno {Descrivi(primo.Data, primo.Slot)}: ne servono almeno {OreDiRiposoMinime}"));
        }

        private static AvvisoContrattualeDTO NuovoAvviso(TipoAvviso tipo, string testo) =>
            new AvvisoContrattualeDTO { Tipo = tipo, Gravita = GravitaAvviso.Normale, Testo = testo };

        // Ore trascorse da un'origine comune: serve solo a confrontare e sottrarre, mai a mostrare.
        private static long OraAssoluta(DateOnly data, Slot slot) => (long)data.DayNumber * 24 + slot.OraInizio();

        private static string Descrivi(DateOnly data, Slot slot) =>
            $"{data.ToString("dd/MM", CultureInfo.InvariantCulture)} {slot.OraInizio()}–{slot.OraFine()}";
    }
}
