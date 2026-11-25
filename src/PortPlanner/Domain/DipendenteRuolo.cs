using System;
using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Domain
{
    // Un ruolo che il lavoratore può fare oltre a quello principale.
    // L'Operaio non va mai elencato qui: lo può fare chiunque.
    [PrimaryKey(nameof(DipendenteId), nameof(Ruolo))]
    public class DipendenteRuolo
    {
        public Guid DipendenteId { get; set; }
        public Dipendente Dipendente { get; set; }

        public Ruolo Ruolo { get; set; }
    }
}
