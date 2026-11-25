using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Una nave in porto. Sta su un solo sito, dal giorno di arrivo al giorno di partenza (compresi).
    public class Barca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public Guid SitoId { get; set; }
        public Sito Sito { get; set; }

        public DateOnly Arrivo { get; set; }

        // Non può essere prima dell'arrivo, e su un sito deve precedere l'arrivo della barca dopo.
        // EF non lo controlla: sono blocchi veri che deve far rispettare il servizio.
        public DateOnly Partenza { get; set; }

        public TipoMerce TipoMerce { get; set; }

        // In tonnellate.
        public int QuantitaMerce { get; set; }

        // Solo se un team è stato incaricato di gestirla.
        public GestioneBarca Gestione { get; set; }

        public ICollection<AssegnazioneTeam> AssegnazioniTeam { get; set; } = new List<AssegnazioneTeam>();
        public ICollection<SlotNonLavorativo> SlotNonLavorativi { get; set; } = new List<SlotNonLavorativo>();
    }
}
