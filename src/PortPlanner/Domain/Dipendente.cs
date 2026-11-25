using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortPlanner.Domain
{
    // Un lavoratore del porto. Nome, cognome ed email stanno sull'Utente collegato.
    public class Dipendente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid UtenteId { get; set; }
        public Utente Utente { get; set; }

        public string Telefono { get; set; }

        public TipoContratto TipoContratto { get; set; }

        // 36 o 38 per chi è a contratto, vuote per chi è a chiamata.
        public int? OreSettimanali { get; set; }

        public DateOnly DataInizioContratto { get; set; }

        // Se è vuota il contratto è a tempo indeterminato.
        public DateOnly? DataFineContratto { get; set; }

        // Serve per raggruppare i membri nel team, per ordinare gli elenchi e come ruolo predefinito nei turni.
        public Ruolo RuoloPrincipale { get; set; }

        // Un lavoratore sta in un solo team. Se è vuoto è senza team.
        public Guid? TeamId { get; set; }
        public Team Team { get; set; }

        // Gli altri ruoli che può fare, oltre a quello principale.
        public ICollection<DipendenteRuolo> Ruoli { get; set; } = new List<DipendenteRuolo>();

        public ICollection<Patente> Patenti { get; set; } = new List<Patente>();
    }
}
