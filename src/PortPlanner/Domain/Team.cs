using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Una squadra. Ha sempre almeno un membro: il capogruppo.
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Nome { get; set; }

        // Il capogruppo è un incarico fisso, senza sostituto. Può essere un membro qualsiasi del team.
        public Guid CapogruppoId { get; set; }
        public Dipendente Capogruppo { get; set; }

        public ICollection<Dipendente> Membri { get; set; } = new List<Dipendente>();
    }
}
