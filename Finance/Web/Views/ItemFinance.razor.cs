using DataModel.DataBase;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Charts;

namespace Web.Views
{
    public partial class ItemFinance
    {
        [Parameter] public string Name { get; set; }
        [Parameter] public string SubName { get; set; }
        [Parameter] public string Price { get; set; }
        [Parameter] public string ImageBase64 { get; set; }
        [Parameter] public List<PlotDTO> Plots { get; set; }

        private string _imageBase64;
        private int _index = -1;
        private ChartOptions _options = new ChartOptions();
        private AxisChartOptions _axisChartOptions = new AxisChartOptions();
        private List<ChartSeries> _series = new List<ChartSeries>();
        private string[] _xAxisLabels;

        protected override async Task OnParametersSetAsync()
        {
            _imageBase64 = ImageBase64;
            _series = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Price", Data = Plots.OrderBy(p => p.Date).Select(p => (double)p.Price).ToArray(), ShowDataMarkers = true },
            };
            _options = new ChartOptions()
            {
                
            };
            _axisChartOptions = new AxisChartOptions()
            {
                MatchBoundsToSize = true,
            };
            _xAxisLabels = Plots
                .OrderBy(p => p.Date)
                .Select(p => p.Date.ToString("dd.MM.yy"))
                .ToArray();
        }
    }
}
