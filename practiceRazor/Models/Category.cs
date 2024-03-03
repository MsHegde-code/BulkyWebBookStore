using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace practiceRazor.Models
{
    public class Category
    {
        [Required]
        [Range(1,40)]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        public int DisplayOrder {  get; set; }
    }
}
