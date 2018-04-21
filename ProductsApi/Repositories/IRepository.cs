using System.Collections.Generic;
using ProductsApi.Models;

namespace ProductsApi.Repositories
{
    public interface IRepository<TItem> where TItem:Item
    {
        void Add(TItem item);
        IEnumerable<TItem> GetAll(string query = null);
        TItem Find(string id);
        void Remove(string id);
        void Update(TItem item);
    }
}
