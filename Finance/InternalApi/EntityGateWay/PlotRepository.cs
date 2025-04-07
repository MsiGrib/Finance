using DataModel.DataBase;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.EntityGateWay
{
    public class PlotRepository : IRepository<PlotDTO, long>
    {
        private readonly FinanceDBContext _context;

        public PlotRepository(FinanceDBContext context)
        {
            _context = context;
        }

        public async Task<List<PlotDTO>> GetAllAsync()
        {
            return await _context.Plots
                .ToListAsync();
        }

        public async Task<PlotDTO?> GetByIdAsync(long id)
        {
            return await _context.Plots
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(PlotDTO entity)
        {
            await _context.Plots.AddAsync(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var plot = await GetByIdAsync(id);
            if (plot != null)
            {
                _context.Plots.Remove(plot);
                int result = await _context.SaveChangesAsync();

                return result > 0;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(PlotDTO entity)
        {
            _context.Update(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
