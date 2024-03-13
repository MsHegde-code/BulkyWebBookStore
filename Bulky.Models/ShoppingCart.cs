using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        [Range(1,1000,ErrorMessage ="enter value between 1-1000")]
        public int Count { get; set; } //count of items



        public int ProductId { get; set; }//productId of items
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }


        //to know which userId owns this cart items
        public string ApplicationUserId { get; set; }//userId
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


        [NotMapped]
        public double Price { get; set; }
        //when we use "Not Mapped", the EFC will not create a column in the Db
    }
}
