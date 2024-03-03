using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	// as the operations might be different for different entities, we declare this interface as generic <T>
	// this interface consists of all base(common to all operations)
	public interface IRepository<T> where T : class
	{
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);//remove the set of records
		T Get(Expression<Func<T, bool>> filter);// returns a single record using LINQ expression in parameter
		IEnumerable<T> GetAll(); // returns set of records, hence IEnumerable<T> is the return type
	}
}
