using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	// in this class we implement the ICategory repository, which consists of the dedicated implementation of this entity
	// this class inherits the Repository class of Category class to access the base(common to all) methods which are implemented in 'Repository' already

	// as we know the class which we are implementing, we do not use generic (<T> class) here, using 'Category' class which is inherited
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private readonly ApplicationDbContext _db;
		public CategoryRepository(ApplicationDbContext db) : base(db) //using the same db to all the base class
		{
			_db = db;
		}


		//implementing the methods of the ICategoryRepository which are dedicated methods of this class
		public void Save()
		{
			_db.SaveChanges();
		}

		public void Update(Category category)
		{
			_db.Update(category);
		}
	}
}
