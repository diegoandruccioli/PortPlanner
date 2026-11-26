using System;
using System.Collections.Generic;
using System.Linq;
using PortPlanner.Domain;
using PortPlanner.Services;

namespace PortPlanner.Infrastructure
{
    // Dati di partenza del dominio (utenti, lavoratori, team, siti, barche e turni).
    // Tutti i nomi sono inventati e tutte le date sono calcolate a partire da "oggi", così la demo
    // è coerente in qualsiasi giorno. Il contenuto riprende gli esempi del prototipo.
    public partial class DataGenerator
    {
        // Stessa password per tutti gli utenti del seed. Vale solo per la demo.
        public const string PasswordDiProva = "Prova123";

        // Chi c'è nell'organico: il supervisore, i lavoratori e i team, per nome.
        private class Organico
        {
            public Utente Supervisore { get; set; }
            public Dictionary<string, Dipendente> Persone { get; } = new();
            public Dictionary<string, Team> Team { get; } = new();
        }

        // Riempie il database una sola volta. La data passata è il "oggi" della demo:
        // l'app usa quella di sistema, i test una data fissa.
        public static void Initialize(PortPlannerDbContext context, DateOnly oggi)
        {
            if (context.Utenti.Any())
            {
                return; // Il seed è già stato fatto.
            }

            var organico = CreaOrganico(oggi);
            var siti = CreaPianificazione(organico, oggi);

            context.Utenti.Add(organico.Supervisore);
            context.Team.AddRange(organico.Team.Values);
            context.Dipendenti.AddRange(organico.Persone.Values);
            context.Siti.AddRange(siti);

            context.SaveChanges();
        }
    }
}
