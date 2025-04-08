using DataModel.DataBase;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.EntityGateWay
{
    public class MainBoardRepository : IRepository<MainBoardDTO, long>
    {
        private readonly FinanceDBContext _context;

        public MainBoardRepository(FinanceDBContext context)
        {
            _context = context;
        }

        public async Task<List<MainBoardDTO>> GetAllAsync()
        {
            return await _context.MainBoards
                .Include(x => x.Table)
                .ThenInclude(y => y.Plot)
                .ToListAsync();
        }

        public async Task<MainBoardDTO?> GetByIdAsync(long id)
        {
            return await _context.MainBoards
                .AsNoTracking()
                .Include(x => x.Table)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(MainBoardDTO entity)
        {
            await _context.MainBoards.AddAsync(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(long id)
        {
            var mainBoard = await GetByIdAsync(id);
            if (mainBoard != null)
            {
                _context.MainBoards.Remove(mainBoard);
                int result = await _context.SaveChangesAsync();

                return result > 0;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(MainBoardDTO entity)
        {
            _context.Update(entity);
            int result = await _context.SaveChangesAsync();

            return result > 0;
        }
    }
}
