using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("E-mail")]
        [MaxLength(70)]
        public string EmailAddress { get; set; }
        [Required]
        [DisplayName("Query")]
        [MaxLength(255)]
        public string UserQuery { get; set; }
    }
}
