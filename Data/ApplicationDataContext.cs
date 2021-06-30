using IntroAspNet.Models;
using Microsoft.EntityFrameworkCore;

namespace IntroAspNet.Data
{
    public class ApplicationDataContext: DbContext
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
