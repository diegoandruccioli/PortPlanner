using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;

namespace PortPlanner.Services.Validazione
{
    // Cosa succede se questa persona lavora in questo slot.
    public class ValutaTurnoQuery
    {
        public Guid DipendenteId { get; set; }
        public DateOnly Data { get; set; }
        public Slot Slot { get; set; }
    }

    public class AvvisoContrattualeDTO
    {
        public TipoAvviso Tipo { get; set; }
        public GravitaAvviso Gravita { get; set; }

        // Già pronto da mostrare, con le ore e i turni coinvolti.
        public string Testo { get; set; }
    }

    public partial class ValidazioneService
    {
        // Gli avvisi causati dal turno indicato, nessuno se il turno va bene. Serve sia per i candidati
        // (la persona non è ancora nello slot) sia per i turni già assegnati (la persona c'è già).
        public async Task<List<AvvisoContrattualeDTO>> Query(ValutaTurnoQuery qry)
        {
            // Si guardano i turni su ogni sito e ogni team, perché il vincolo è sulla persona (D22).
            // Si caricano tutti: un blocco può estendersi su più giorni e una finestra fissa lo taglierebbe.
            var turniEsistenti = (await _dbContext.AssegnazioniPersone
                    .Where(p => p.DipendenteId == qry.DipendenteId && p.NonSvolto == false)
                    .Select(p => new { p.AssegnazioneTeam.Data, p.AssegnazioneTeam.Slot })
                    .ToListAsync())
                .Select(t => (t.Data, t.Slot));

            return RegoleContrattuali.Valuta(turniEsistenti, qry.Data, qry.Slot);
        }
    }
}
