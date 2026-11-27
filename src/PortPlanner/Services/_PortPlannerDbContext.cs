using Microsoft.EntityFrameworkCore;

namespace PortPlanner.Services
{
    // I DbSet e le relazioni stanno nella cartella della loro funzione
    // (<Funzione>.DbContext.cs e <Funzione>.Relazioni.cs): qui non si aggiunge niente.
    public partial class PortPlannerDbContext : DbContext
    {
        public PortPlannerDbContext()
        {
        }

        // Il costruttore non semina più il database: lo fa Startup all'avvio (DataGenerator.Initialize),
        // così i test partono da un database vuoto e quelli sul seed scelgono la data di "oggi".
        public PortPlannerDbContext(DbContextOptions<PortPlannerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Trova da sola tutte le classi IEntityTypeConfiguration del progetto.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortPlannerDbContext).Assembly);
        }
    }
}
