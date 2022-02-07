using System.Threading.Tasks;

namespace PetsHome.Logic.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //int rtint = 0;
        // public IEnumerable<object> ListAsync();
        //Task<IEnumerable<T>> List();
        //Task<T> Find(int id);
        Task<bool> AddAsync(T entity);
        Task<bool> EditAsync(T entity);
        Task<bool> RemoveAsync(int id);
        //Task<IEnumerable<T>> Detail();
    }
}
