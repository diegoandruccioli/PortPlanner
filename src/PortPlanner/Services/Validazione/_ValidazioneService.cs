namespace PortPlanner.Services.Validazione
{
    // Gli avvisi contrattuali sui turni di una persona. Gli avvisi non bloccano mai un'assegnazione.
    public partial class ValidazioneService
    {
        PortPlannerDbContext _dbContext;

        public ValidazioneService(PortPlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
