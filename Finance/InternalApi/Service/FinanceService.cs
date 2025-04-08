using DataModel.DataBase;
using DataModel.DataStructures;
using InternalApi.EntityGateWay;

namespace InternalApi.Service
{
    public class FinanceService
    {
        private readonly PlotRepository _plotRepository;
        private readonly TableRepository _tableRepository;
        private readonly MainBoardRepository _mainBoardRepository;

        public FinanceService(IRepository<PlotDTO, long> plotRepository, IRepository<TableDTO, long> tableRepository, IRepository<MainBoardDTO, long> mainBoardRepository)
        {
            _plotRepository = (PlotRepository)plotRepository;
            _tableRepository = (TableRepository)tableRepository;
            _mainBoardRepository = (MainBoardRepository)mainBoardRepository;
        }

        private async Task<List<Pair<int, TableDTO>>> GetViewFinance()
        {
            var result = new List<Pair<int, TableDTO>>();
            var mainBoards = await _mainBoardRepository.GetAllAsync();







        }
    }
}
