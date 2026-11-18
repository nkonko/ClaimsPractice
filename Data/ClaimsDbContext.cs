using Microsoft.EntityFrameworkCore;
using ClaimsWebApi.Domain;

namespace ClaimsWebApi.Data
{
    public class ClaimsDbContext : DbContext
    {
        public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options)
        {
        }
        public DbSet<Claim> Claims { get; set; }
    }
}
