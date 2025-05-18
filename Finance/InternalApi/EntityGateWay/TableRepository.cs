using DataModel.DataBase;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.EntityGateWay
{
    public class TableRepository : IRepository<TableDTO, long>
    {
        private readonly FinanceDBContext _context;

        public TableRepository(FinanceDBContext context)
        {
            _context = context;
        }

        public async Task<List<TableDTO>> GetAllAsync()
        {
            return await _context.Tables
                .Include(x => x.Plots)
                .ToListAsync();
        }

        public async Task<TableDTO?> GetByIdAsync(long id)
        {
            return await _context.Tables
                .AsNoTracking()
                .Include(x => x.Plots)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(TableDTO entity)
        {
            await _context.Tables.AddAsync(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var table = await GetByIdAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                int result = await _context.SaveChangesAsync();

                return result > 0;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(TableDTO entity)
        {
            _context.Update(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
