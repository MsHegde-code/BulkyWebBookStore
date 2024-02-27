using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

// basic configuration for the EntityFrameWorkCore(EFC)
namespace BulkyWeb.Data
{
    // every class must implement the DbContext, which is the built-in class of the EFC which is in Microsoft.Entity.FrameWorkCore package
    public class ApplicationDbContext : DbContext
    {
        // what ever we config options we add in appDbContext that needs to passed into the base class "DbContext" also
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
            
        }

        // this single line of code creates the table in the database with the help of EFC
        public DbSet<Category> Categories{ get; set; }
    }
}
