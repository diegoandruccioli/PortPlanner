using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortPlanner.Domain;

namespace PortPlanner.Services.Pianificazione
{
    // Relazioni di siti, barche e turni.
    public class BarcaConfigurazione : IEntityTypeConfiguration<Barca>
    {
        public void Configure(EntityTypeBuilder<Barca> builder)
        {
            // Siti e barche: una barca sta su un solo sito.
            builder
                .HasOne(b => b.Sito)
                .WithMany(s => s.Barche)
                .HasForeignKey(b => b.SitoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class GestioneBarcaConfigurazione : IEntityTypeConfiguration<GestioneBarca>
    {
        public void Configure(EntityTypeBuilder<GestioneBarca> builder)
        {
            // La gestione ha come chiave la barca: al massimo una per barca. Se il team sparisce, la barca resta senza gestore.
            builder
                .HasOne(g => g.Barca)
                .WithOne(b => b.Gestione)
                .HasForeignKey<GestioneBarca>(g => g.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(g => g.TeamGestore)
                .WithMany()
                .HasForeignKey(g => g.TeamGestoreId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class AssegnazioneTeamConfigurazione : IEntityTypeConfiguration<AssegnazioneTeam>
    {
        public void Configure(EntityTypeBuilder<AssegnazioneTeam> builder)
        {
            builder
                .HasOne(a => a.Barca)
                .WithMany(b => b.AssegnazioniTeam)
                .HasForeignKey(a => a.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(a => a.Team)
                .WithMany()
                .HasForeignKey(a => a.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un solo team per barca, giorno e slot. Il database in memoria ignora gli indici univoci:
            // lo dichiaro per documentare la regola, ma il controllo vero lo farà il servizio.
            builder
                .HasIndex(a => new { a.BarcaId, a.Data, a.Slot })
                .IsUnique();
        }
    }

    public class SlotNonLavorativoConfigurazione : IEntityTypeConfiguration<SlotNonLavorativo>
    {
        public void Configure(EntityTypeBuilder<SlotNonLavorativo> builder)
        {
            builder
                .HasOne(s => s.Barca)
                .WithMany(b => b.SlotNonLavorativi)
                .HasForeignKey(s => s.BarcaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AssegnazionePersonaConfigurazione : IEntityTypeConfiguration<AssegnazionePersona>
    {
        public void Configure(EntityTypeBuilder<AssegnazionePersona> builder)
        {
            builder
                .HasOne(p => p.AssegnazioneTeam)
                .WithMany(a => a.Persone)
                .HasForeignKey(p => p.AssegnazioneTeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un lavoratore con turni non si cancella da qui: prima vanno tolti i turni (lo farà il servizio).
            builder
                .HasOne(p => p.Dipendente)
                .WithMany()
                .HasForeignKey(p => p.DipendenteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
