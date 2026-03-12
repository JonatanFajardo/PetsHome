using PetsHome.Common;
using System.Threading.Tasks;

namespace PetsHome.Logic.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<RequestResult> AddAsync(T entity);

        Task<RequestResult> EditAsync(T entity);

        Task<RequestResult> RemoveAsync(int id);

    }
}