using System;
using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Domain
{
    // Una patente per tipo (Gru o Muletto). Se manca la riga, il lavoratore non ha quella patente.
    [PrimaryKey(nameof(DipendenteId), nameof(Tipo))]
    public class Patente
    {
        // Da quanti giorni prima della scadenza la patente risulta "in scadenza".
        public const int GiorniPreavvisoScadenza = 30;

        public Guid DipendenteId { get; set; }
        public Dipendente Dipendente { get; set; }

        public TipoPatente Tipo { get; set; }

        // Ultimo giorno in cui la patente vale (compreso).
        public DateOnly Scadenza { get; set; }

        // Lo stato non si salva, si calcola rispetto a una data.
        // Il giorno di scadenza la patente vale ancora; è scaduta dal giorno dopo.
        public StatoPatente StatoAl(DateOnly oggi)
        {
            if (Scadenza < oggi) return StatoPatente.Scaduta;
            if (Scadenza <= oggi.AddDays(GiorniPreavvisoScadenza)) return StatoPatente.InScadenza;
            return StatoPatente.Valida;
        }
    }
}
