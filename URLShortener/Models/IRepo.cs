using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace URLShortener.Models
{
    public interface IRepo<TEntity> where TEntity: class
    {
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        //Allows the running of lamba-style LINQ queries like the typical Entity Framework does:
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
