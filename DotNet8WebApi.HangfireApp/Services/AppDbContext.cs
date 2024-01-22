using DotNet8WebApi.HangfireApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet8WebApi.HangfireApp.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<BlogModel> blogs { get; set; }
    }
}
