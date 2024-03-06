using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;


namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db; // as we need to perform CRUD operation on db

		//creating a field of type DbSet<T>
		internal DbSet<T> DbSet; // in generic implementation we will not know which class is being operated, so we use DbSet<> 
								 // it represent a collection of entities 
		public Repository(ApplicationDbContext db)
        {
            _db = db;
			DbSet = _db.Set<T>();
			
        }
        public void Add(T entity)
		{
			DbSet.Add(entity);
		}
								//LINQ expression		//include() populating properties based on FK
		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			//_db.Categories == DbSet

			IQueryable<T> query = DbSet; // we need to perform the LINQ operations on DbSet, so we assign it to IQueryable<>
			query = query.Where(filter);// apply the LINQ operation on the query

			if (!string.IsNullOrWhiteSpace(includeProperties))
			{
				foreach (var property in includeProperties
					.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}
			return query.FirstOrDefault();//retrieve the first occurance
		}

		public IEnumerable<T> GetAll(string? includeProperties = null)
		{
			// remember, when retriving a record, we need to have a query

			IQueryable<T> query = DbSet;
			if (!string.IsNullOrWhiteSpace(includeProperties))
			{
				foreach (var property in includeProperties
					.Split(',',StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}

			
			return query.ToList();
		}

		public void Remove(T entity)
		{
			DbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			DbSet.RemoveRange(entities);
		}
	}
}
