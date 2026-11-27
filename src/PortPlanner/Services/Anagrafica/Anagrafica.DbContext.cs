using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;

namespace PortPlanner.Services
{
    public partial class PortPlannerDbContext
    {
        public DbSet<Utente> Utenti { get; set; }
        public DbSet<Dipendente> Dipendenti { get; set; }
        public DbSet<DipendenteRuolo> DipendentiRuoli { get; set; }
        public DbSet<Patente> Patenti { get; set; }
        public DbSet<Team> Team { get; set; }
    }
}
