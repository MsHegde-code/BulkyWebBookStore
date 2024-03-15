using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }
        public string Description {  get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        [DisplayName("List Price")]
        [Range(1,1000)]
        public int ListPrice {  get; set; }

        [Required]
        [DisplayName("Price for 1-50")]
        [Range(1, 1000)]
        public int Price { get; set; }

        [Required]
        [DisplayName("Price for 50-100")]
        [Range(1, 1000)]
        public int Price50 { get; set; }

        [Required]
        [DisplayName("Price for 100+")]
        [Range(1, 1000)]
        public int Price100 { get; set; }

        //adding reference/foriegn key relation, 
        // (as we want to relate this entity with Category table's 'ID' we create a )
        [DisplayName("Category ID")]
        public int CategoryId { get; set; } 

        [ForeignKey("CategoryId")] // the "Name" denotes that the above property is the FK of this table

		[ValidateNever]
		public Category Category { get; set; } // navigation property, to tell the EFC about FK
                                               //that this table has foriegn key to category table


        [ValidateNever]
        public List<ProductImage> ProductImage { get; set; }
    }
}
