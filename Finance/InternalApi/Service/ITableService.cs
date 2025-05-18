using DataModel.DataBase;

namespace InternalApi.Service
{
    public interface ITableService
    {
        public Task<List<TableDTO>> GetTablesAllAsync();
        public Task<TableDTO?> GetTableByIdAsync(long id);
    }
}
