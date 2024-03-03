using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface ICategoryRepository : IRepository<Category>
	{
		// inherting the IRepository on dedicated 'category' class, as wkt this interface belongs to Category class
		// this interface declares the methods which are dedicated to Category class
		void Save();
		void Update(Category category);
	}
}
