using BilgeShop.Data.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Repository
{
	public interface IRepository<TEntity> where TEntity : class
	{
		void Add(TEntity entity);
		void Delete(int Id);
		void Delete(TEntity entity);
		void Update(TEntity entity);
		TEntity GetById(int Id);
		TEntity Get(Expression<Func<TEntity,bool>> predicate);
		IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);
	}
}

//Linq için değişken tipi ->   Expression<Func<TEntity,bool>>
// Get( x => x.Name.Contains("Mustafa") bu şekilde alabilirz artık.

