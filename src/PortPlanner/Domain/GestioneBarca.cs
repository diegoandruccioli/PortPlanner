using System;
using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Domain
{
    // Livello "3.0": un team gestisce la barca per tutto il periodo e il suo capogruppo sceglie quando lavorare.
    // La chiave è la barca stessa, così una barca ha al massimo una gestione.
    [PrimaryKey(nameof(BarcaId))]
    public class GestioneBarca
    {
        public Guid BarcaId { get; set; }
        public Barca Barca { get; set; }

        // Vuoto finché il supervisore non sceglie un team.
        public Guid? TeamGestoreId { get; set; }
        public Team TeamGestore { get; set; }
    }
}
