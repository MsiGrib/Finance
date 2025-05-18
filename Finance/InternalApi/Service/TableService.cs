using DataModel.DataBase;
using InternalApi.EntityGateWay;

namespace InternalApi.Service
{
    public class TableService : ITableService
    {
        private readonly IRepository<TableDTO, long> _tableRepository;

        public TableService(IRepository<TableDTO, long> tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public async Task<List<TableDTO>> GetTablesAllAsync()
        {
            return await _tableRepository.GetAllAsync();
        }

        public async Task<TableDTO?> GetTableByIdAsync(long id)
        {
            return await _tableRepository.GetByIdAsync(id);
        }
    }
}
