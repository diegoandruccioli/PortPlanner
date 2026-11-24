using PortPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;
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

        public DbSet<User> Users { get; set; }
    }
}
