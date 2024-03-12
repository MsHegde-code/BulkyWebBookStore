using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork 
	{
		//we implement the ICategory, IProduct when we implement IUnitOfWork
		ICategoryRepository Category{ get; }
		IProductRepository Product { get; }
		ICompanyRepository Company { get; }

		//cart
		IShoppingCartRepository ShoppingCart { get; }
		IOrderDetailRepository OrderDetail { get; }
		IOrderHeaderRepository OrderHeader { get; }

		//user
		IApplicationUserRepository ApplicationUser { get; }
		void Save();
	}
}
