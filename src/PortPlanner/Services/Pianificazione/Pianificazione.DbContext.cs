using Microsoft.EntityFrameworkCore;
using PortPlanner.Domain;

namespace PortPlanner.Services
{
    public partial class PortPlannerDbContext
    {
        public DbSet<Sito> Siti { get; set; }
        public DbSet<Barca> Barche { get; set; }
        public DbSet<GestioneBarca> GestioniBarche { get; set; }
        public DbSet<AssegnazioneTeam> AssegnazioniTeam { get; set; }
        public DbSet<SlotNonLavorativo> SlotNonLavorativi { get; set; }
        public DbSet<AssegnazionePersona> AssegnazioniPersone { get; set; }
    }
}
