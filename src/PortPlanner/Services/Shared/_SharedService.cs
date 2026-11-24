namespace PortPlanner.Services.Shared
{
    public partial class SharedService
    {
        PortPlannerDbContext _dbContext;

        public SharedService(PortPlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
