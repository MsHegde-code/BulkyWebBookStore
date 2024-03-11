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
		T Get(Expression<Func<T, bool>> filter, // returns a single record using LINQ expression in parameter
				string? includeProperties = null,//for include properties, as its not mandatory to have a include properties
				bool tracked = false			//for EFC object tracking
                ); 
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,
								string? includeProperties = null); // returns set of records, hence IEnumerable<T> is the return type
							//for include properties
	}
}
