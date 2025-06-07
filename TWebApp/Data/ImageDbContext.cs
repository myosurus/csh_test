using Microsoft.EntityFrameworkCore;

namespace TWebApp.Data
{
    public class ImageDbContext : DbContext
    {

        protected readonly IConfiguration Configuration;
        //public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options) { }

        public ImageDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Configuration.GetConnectionString("userConnection"));
        }
        public DbSet<Image> Images { get; set; }
    }
}
