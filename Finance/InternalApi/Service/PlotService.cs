using DataModel.DataBase;

namespace InternalApi.Service
{
    public class PlotService : IPlotService
    {
        private readonly IRepository<PlotDTO, long> _plotRepository;

        public PlotService(IRepository<PlotDTO, long> plotRepository)
        {
            _plotRepository = plotRepository;
        }

        public async Task<List<PlotDTO>> GetPlotsByTableIdAsync(long id)
        {
            var res = await _plotRepository.GetAllAsync();
            return res.Where(x => x.TableId == id).ToList();
        }
    }
}
