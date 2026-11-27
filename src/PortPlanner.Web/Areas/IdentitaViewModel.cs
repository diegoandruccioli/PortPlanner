using System;
using System.Linq;
using PortPlanner.Domain;

namespace PortPlanner.Web.Areas
{
    // Chi è collegato, letto dal cookie. Lo mettono a disposizione delle view i controller autenticati.
    public class IdentitaViewModel
    {
        public static string VIEWDATA_IDENTITACORRENTE_KEY = "IdentitaUtenteCorrente";

        public string EmailUtenteCorrente { get; set; }

        public string NomeCompleto { get; set; }

        public Profilo Profilo { get; set; }

        // Vuoto per il Supervisore, che non è un lavoratore.
        public Guid? DipendenteId { get; set; }

        // Vuoto per il Supervisore e per i lavoratori senza team.
        public Guid? TeamId { get; set; }

        // Iniziali della prima e dell'ultima parola del nome, per il cerchio nel menu utente.
        public string Iniziali
        {
            get
            {
                var parole = NomeCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var iniziali = parole.Length > 1 ? $"{parole[0][0]}{parole[^1][0]}" : $"{parole.FirstOrDefault()?[0]}";
                return iniziali.ToUpperInvariant();
            }
        }
    }
}
