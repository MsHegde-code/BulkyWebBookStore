using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        //has the details on orders, items, customer
        public OrderHeader OrderHeader { get; set; }

        //list of items 
        public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
    }
}
