using DataModel.DataBase;

namespace InternalApi.Service
{
    public interface IPlotService
    {
        public Task<List<PlotDTO>> GetPlotsByTableIdAsync(long id);
    }
}
