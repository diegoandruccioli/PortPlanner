using System;
using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Domain
{
    // Uno slot in cui un team non deve lavorare su quella barca. Un giorno intero bloccato sono quattro righe.
    // Lo decide il supervisore, dalla gestione della barca (3.0) o dal menu dello slot (2.0).
    [PrimaryKey(nameof(BarcaId), nameof(Data), nameof(Slot), nameof(TeamId))]
    public class SlotNonLavorativo
    {
        public Guid BarcaId { get; set; }
        public Barca Barca { get; set; }

        public DateOnly Data { get; set; }
        public Slot Slot { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }
    }
}
