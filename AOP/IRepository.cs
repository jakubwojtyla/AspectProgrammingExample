using System.Collections.Generic;

namespace AOP
{
    public interface IRepository<T>
    {
        [Log]
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        IEnumerable<T> GetAll();
        T GetById(int id);
    }
}