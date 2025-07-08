
using Microsoft.EntityFrameworkCore;
namespace Paying_Hub.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<States>States { get; set; }
        public DbSet<Cities> Cities { get; set; }
    }
}
