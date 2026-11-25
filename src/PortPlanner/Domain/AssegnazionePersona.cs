using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Livello "2.1": una persona che lavora nello slot di un team, con il ruolo che fa in quel turno.
    // Non deve per forza essere del team: il supervisore può aggiungere anche persone di altri team o senza team.
    public class AssegnazionePersona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AssegnazioneTeamId { get; set; }
        public AssegnazioneTeam AssegnazioneTeam { get; set; }

        public Guid DipendenteId { get; set; }
        public Dipendente Dipendente { get; set; }

        // Un lavoratore fa un solo ruolo per slot, e non può stare su due siti nello stesso slot.
        // Sono blocchi veri: li controlla il servizio, non il database in memoria.
        public Ruolo Ruolo { get; set; }

        // Il supervisore lo segna su un turno passato che la persona non ha svolto.
        public bool NonSvolto { get; set; }
    }
}
