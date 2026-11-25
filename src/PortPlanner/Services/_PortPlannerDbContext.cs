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

        public DbSet<Sito> Siti { get; set; }
        public DbSet<Barca> Barche { get; set; }
        public DbSet<GestioneBarca> GestioniBarche { get; set; }
        public DbSet<AssegnazioneTeam> AssegnazioniTeam { get; set; }
        public DbSet<SlotNonLavorativo> SlotNonLavorativi { get; set; }
        public DbSet<AssegnazionePersona> AssegnazioniPersone { get; set; }

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

            // Siti e barche: una barca sta su un solo sito.
            modelBuilder.Entity<Barca>()
                .HasOne(b => b.Sito)
                .WithMany(s => s.Barche)
                .HasForeignKey(b => b.SitoId)
                .OnDelete(DeleteBehavior.Restrict);

            // La gestione ha come chiave la barca: al massimo una per barca. Se il team sparisce, la barca resta senza gestore.
            modelBuilder.Entity<GestioneBarca>()
                .HasOne(g => g.Barca)
                .WithOne(b => b.Gestione)
                .HasForeignKey<GestioneBarca>(g => g.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GestioneBarca>()
                .HasOne(g => g.TeamGestore)
                .WithMany()
                .HasForeignKey(g => g.TeamGestoreId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AssegnazioneTeam>()
                .HasOne(a => a.Barca)
                .WithMany(b => b.AssegnazioniTeam)
                .HasForeignKey(a => a.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssegnazioneTeam>()
                .HasOne(a => a.Team)
                .WithMany()
                .HasForeignKey(a => a.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un solo team per barca, giorno e slot. Il database in memoria ignora gli indici univoci:
            // lo dichiaro per documentare la regola, ma il controllo vero lo farà il servizio.
            modelBuilder.Entity<AssegnazioneTeam>()
                .HasIndex(a => new { a.BarcaId, a.Data, a.Slot })
                .IsUnique();

            modelBuilder.Entity<SlotNonLavorativo>()
                .HasOne(s => s.Barca)
                .WithMany(b => b.SlotNonLavorativi)
                .HasForeignKey(s => s.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SlotNonLavorativo>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssegnazionePersona>()
                .HasOne(p => p.AssegnazioneTeam)
                .WithMany(a => a.Persone)
                .HasForeignKey(p => p.AssegnazioneTeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un lavoratore con turni non si cancella da qui: prima vanno tolti i turni (lo farà il servizio).
            modelBuilder.Entity<AssegnazionePersona>()
                .HasOne(p => p.Dipendente)
                .WithMany()
                .HasForeignKey(p => p.DipendenteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
