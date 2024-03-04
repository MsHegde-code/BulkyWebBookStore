using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models
{
    public class Category
    {
        [Key] //data annotation, it says that the ID is the primary key of this table
        public int Id { get; set; } // primary key of the category table (CategoryId can be used to represent the primary key)

        [Required] // sets not null in the DB
        [DisplayName("Category Name")]
        [MaxLength(20)] //validation for maximum allowed characters
        public string Name { get; set; } // name of the category

        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order value must be from 1 to 100")] //maximum of 100 or minimum of 1 is allowed to enter
        public int DisplayOrder { get; set; }
    }
}
