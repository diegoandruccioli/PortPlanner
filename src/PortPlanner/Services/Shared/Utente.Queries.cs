using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Infrastructure;

namespace PortPlanner.Services.Shared
{
    public class CheckLoginCredentialsQuery
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Chi ha fatto il login: i dati che finiscono nel cookie.
    public class UtenteAutenticatoDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public Profilo Profilo { get; set; }

        // Vuoti per il Supervisore, che non è un lavoratore.
        public Guid? DipendenteId { get; set; }

        // Vuoto per il Supervisore e per i lavoratori senza team.
        public Guid? TeamId { get; set; }
    }

    public partial class SharedService
    {
        // Lo stesso messaggio vale per email inesistente, password errata e account non valido:
        // così non si capisce quali email esistono.
        private const string MessaggioCredenziali = "Email o password errate";

        // Controlla email e password e calcola il profilo di chi entra.
        // Lancia LoginException se le credenziali non sono valide.
        public async Task<UtenteAutenticatoDTO> Query(CheckLoginCredentialsQuery qry)
        {
            var email = qry.Email?.Trim().ToLowerInvariant();

            var utente = await _dbContext.Utenti
                .Include(u => u.Dipendente)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (utente == null || utente.IsMatchWithPassword(qry.Password) == false)
                throw new LoginException(MessaggioCredenziali);

            var risultato = new UtenteAutenticatoDTO
            {
                Id = utente.Id,
                Email = utente.Email,
                Nome = utente.Nome,
                Cognome = utente.Cognome,
            };

            if (utente.IsSupervisore)
            {
                risultato.Profilo = Profilo.Supervisore;
                return risultato;
            }

            // Un account che non è né supervisore né lavoratore non ha niente da fare nell'app.
            if (utente.Dipendente == null)
                throw new LoginException(MessaggioCredenziali);

            var dipendente = utente.Dipendente;
            var isCapogruppo = await _dbContext.Team.AnyAsync(t => t.CapogruppoId == dipendente.Id);

            risultato.Profilo = isCapogruppo ? Profilo.Capogruppo : Profilo.Dipendente;
            risultato.DipendenteId = dipendente.Id;
            risultato.TeamId = dipendente.TeamId;
            return risultato;
        }
    }
}
