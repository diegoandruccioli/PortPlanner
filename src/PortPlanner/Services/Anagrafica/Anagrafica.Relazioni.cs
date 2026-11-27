using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortPlanner.Domain;

namespace PortPlanner.Services.Anagrafica
{
    // Relazioni di utenti, lavoratori e team.
    public class DipendenteConfigurazione : IEntityTypeConfiguration<Dipendente>
    {
        public void Configure(EntityTypeBuilder<Dipendente> builder)
        {
            // Un Utente ha al più un Dipendente (il Supervisore non ne ha).
            builder
                .HasOne(d => d.Utente)
                .WithOne(u => u.Dipendente)
                .HasForeignKey<Dipendente>(d => d.UtenteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un lavoratore sta in un solo team; senza team = TeamId vuoto.
            builder
                .HasOne(d => d.Team)
                .WithMany(t => t.Membri)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class TeamConfigurazione : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            // Il capogruppo è un membro del team: niente cascade per evitare il ciclo Team <-> Dipendente.
            builder
                .HasOne(t => t.Capogruppo)
                .WithMany()
                .HasForeignKey(t => t.CapogruppoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class DipendenteRuoloConfigurazione : IEntityTypeConfiguration<DipendenteRuolo>
    {
        public void Configure(EntityTypeBuilder<DipendenteRuolo> builder)
        {
            builder
                .HasOne(r => r.Dipendente)
                .WithMany(d => d.Ruoli)
                .HasForeignKey(r => r.DipendenteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PatenteConfigurazione : IEntityTypeConfiguration<Patente>
    {
        public void Configure(EntityTypeBuilder<Patente> builder)
        {
            builder
                .HasOne(p => p.Dipendente)
                .WithMany(d => d.Patenti)
                .HasForeignKey(p => p.DipendenteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
