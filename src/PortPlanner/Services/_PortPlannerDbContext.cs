using PortPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;
using PortPlanner.Services.Shared;

namespace PortPlanner.Services
{
    public class PortPlannerDbContext : DbContext
    {
        public PortPlannerDbContext()
        {
        }

        public PortPlannerDbContext(DbContextOptions<PortPlannerDbContext> options) : base(options)
        {
            DataGenerator.InitializeUsers(this);
        }

        // User è quello del template: resta finché il login non passa a Utente (PR sul login).
        public DbSet<User> Users { get; set; }

        public DbSet<Utente> Utenti { get; set; }
        public DbSet<Dipendente> Dipendenti { get; set; }
        public DbSet<DipendenteRuolo> DipendentiRuoli { get; set; }
        public DbSet<Patente> Patenti { get; set; }
        public DbSet<Team> Team { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Un Utente ha al più un Dipendente (il Supervisore non ne ha).
            modelBuilder.Entity<Dipendente>()
                .HasOne(d => d.Utente)
                .WithOne(u => u.Dipendente)
                .HasForeignKey<Dipendente>(d => d.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un lavoratore sta in un solo team; senza team = TeamId vuoto.
            modelBuilder.Entity<Dipendente>()
                .HasOne(d => d.Team)
                .WithMany(t => t.Membri)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            // Il capogruppo è un membro del team: niente cascade per evitare il ciclo Team <-> Dipendente.
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Capogruppo)
                .WithMany()
                .HasForeignKey(t => t.CapogruppoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DipendenteRuolo>()
                .HasOne(r => r.Dipendente)
                .WithMany(d => d.Ruoli)
                .HasForeignKey(r => r.DipendenteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patente>()
                .HasOne(p => p.Dipendente)
                .WithMany(d => d.Patenti)
                .HasForeignKey(p => p.DipendenteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
