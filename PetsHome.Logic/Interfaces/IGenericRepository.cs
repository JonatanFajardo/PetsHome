using System.Threading.Tasks;

namespace PetsHome.Logic.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> AddAsync(T entity);

        Task<bool> EditAsync(T entity);

        Task<bool> RemoveAsync(int id);

    }
}