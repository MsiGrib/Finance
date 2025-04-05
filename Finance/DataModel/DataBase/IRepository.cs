using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.DataBase
{
    public interface IRepository<T1, T2>
        where T1 : class
        where T2 : struct
    {
        Task<List<T1>> GetAllAsync();
        Task<T1?> GetByIdAsync(T2 id);
        Task AddAsync(T1 entity);
        Task UpdateAsync(T1 entity);
        Task DeleteByIdAsync(T2 id);
    }
}
