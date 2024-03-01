using BulkyWeb_RazorPage.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb_RazorPage.Data
{
    public class ApplicationDbContext : DbContext
    {
        // what ever we config options we add in appDbContext that needs to passed into the base class "DbContext" also
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // this single line of code creates the table in the database with the help of EFC
        public DbSet<Category> Categories { get; set; }


        //using the built-in OnModelCreating from EFC to seed the data to the database, 
        //only one 'OnModelCreating()' method can be declared.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Thriller", DisplayOrder = 4 });
        }


    }

}
