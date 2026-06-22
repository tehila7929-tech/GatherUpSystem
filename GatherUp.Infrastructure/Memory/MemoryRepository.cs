using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Infrastructure.Memory
{
    public class MemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _dataContext = new List<T>();

        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dataContext.Add(entity);
        }

        public T? GetById(int id)
        {
            return _dataContext.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dataContext;
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = GetById(entity.Id);
            if (existing != null)
            {
                _dataContext.Remove(existing);
                _dataContext.Add(entity);
            }
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _dataContext.Remove(entity);
            }
        }
    }
}


