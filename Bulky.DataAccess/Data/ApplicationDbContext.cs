using Bulky.Models;
using Microsoft.EntityFrameworkCore;

// basic configuration for the EntityFrameWorkCore(EFC),
// any updations or manipulations with the database is done in this ApplicationDbContext file
namespace Bulky.DataAccess.Data
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
        public DbSet<Contact> Contact { get; set; }


        //using the built-in OnModelCreating from EFC to seed the data to the database, 
        //only one 'OnModelCreating()' method can be declared.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Thriller", DisplayOrder = 4} );

            //for testing purpose, in reality we need to get data from the form
            //modelBuilder.Entity<Contact>().HasData(
            //    new Contact { Id = 1, EmailAddress = "hello@gmail.com", UserQuery = "help!!" });
        }
     

    }
}
