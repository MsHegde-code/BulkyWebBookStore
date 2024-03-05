using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
	public class ProductVM
	{
        // we need Product model reference for the data fields
        public Product Product { get; set; }


        //Category list for the dropdown menu
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
