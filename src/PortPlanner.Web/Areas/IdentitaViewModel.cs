using System;
using PortPlanner.Domain;
using PortPlanner.Web.Infrastructure;

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

        public string GravatarUrl
        {
            get
            {
                return EmailUtenteCorrente.ToGravatarUrl(ToGravatarUrlExtension.DefaultGravatar.Identicon, null);
            }
        }
    }
}
