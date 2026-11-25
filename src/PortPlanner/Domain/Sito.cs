using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Un posto dove lavora una gru. Ce ne sono quattro, fissi.
    public class Sito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Nome { get; set; }

        // Posizione nelle schermate: Sito 1 per primo, Sito 4 per ultimo.
        public int Ordine { get; set; }

        public ICollection<Barca> Barche { get; set; } = new List<Barca>();
    }
}
