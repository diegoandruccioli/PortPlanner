using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace PortPlanner.Domain
{
    // Account per entrare nell'app. Il Supervisore ha solo questo; ogni lavoratore ha anche un Dipendente.
    public class Utente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Deve essere unica, ma con il database in memoria EF non lo controlla: ci pensa il servizio.
        public string Email { get; set; }

        // SHA-256 in Base64 senza sale, come nel template. Va bene solo per la demo.
        public string PasswordHash { get; set; }

        public string Nome { get; set; }
        public string Cognome { get; set; }

        public bool IsSupervisore { get; set; }

        // Vuoto per il Supervisore.
        public Dipendente Dipendente { get; set; }

        public static string CalcolaHash(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public bool IsMatchWithPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            return PasswordHash == CalcolaHash(password);
        }
    }
}
