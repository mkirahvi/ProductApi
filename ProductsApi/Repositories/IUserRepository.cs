using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public abstract class Repository<TItem> : IRepository<TItem> where TItem : Item
    {
        protected ItemContext<TItem> _context;

        public void Add(TItem item)
        {
            _context.Create(item).Wait();
        }

        public TItem Find(string id)
        {
            return _context.Get(id).Result;
        }

        public IEnumerable<TItem> GetAll(string query = null)
        {
            return  _context.GetAll().Result;
        }

        public void Remove(string id)
        {
            _context.Remove(id).Wait();
        }

        public void Update(TItem item)
        {
            _context.Update(item).Wait();
        }
    }
}
