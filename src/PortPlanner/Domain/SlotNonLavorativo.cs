using System;
using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Domain
{
    // Uno slot di una barca in cui il team gestore non deve lavorare. Lo decide il supervisore,
    // dalla gestione della barca (3.0) o dal menu dello slot (2.0). Un giorno bloccato sono quattro righe.
    //
    // Il blocco appartiene alla barca, non a un team: se cambia il team gestore i blocchi restano.
    // Il supervisore può comunque inserire un team in uno slot bloccato con un'AssegnazioneTeam:
    // l'assegnazione puntuale vince e la riga di blocco resta (se l'inserimento viene tolto, lo slot torna bloccato).
    [PrimaryKey(nameof(BarcaId), nameof(Data), nameof(Slot))]
    public class SlotNonLavorativo
    {
        public Guid BarcaId { get; set; }
        public Barca Barca { get; set; }

        public DateOnly Data { get; set; }
        public Slot Slot { get; set; }
    }
}
