using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Livello "2.0": quale team lavora su una barca in un dato giorno e slot.
    // Se c'è sia un'assegnazione del supervisore sia una scelta del capogruppo, vince la più precisa.
    public class AssegnazioneTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid BarcaId { get; set; }
        public Barca Barca { get; set; }

        public DateOnly Data { get; set; }
        public Slot Slot { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }

        public OrigineAssegnazione Origine { get; set; }

        // Le persone del team che lavorano davvero in quello slot (livello "2.1").
        public ICollection<AssegnazionePersona> Persone { get; set; } = new List<AssegnazionePersona>();
    }
}
