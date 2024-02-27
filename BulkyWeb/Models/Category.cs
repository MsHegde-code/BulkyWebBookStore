using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key] //data annotation, it says that the ID is the primary key of this table
        public int Id { get; set; } // primary key of the category table (CategoryId can be used to represent the primary key)
        
        [Required] // sets not null in the DB
        public string Name { get; set; } // name of the category
        public int DisplayOrder { get; set; }
    }
}
