using Microsoft.AspNetCore.Components;

namespace Web.Views
{
    public partial class ItemFinance
    {
        [Parameter] public string Name { get; set; }
        [Parameter] public string SubName { get; set; }
        [Parameter] public decimal Price { get; set; }
        [Parameter] public List<double> ChartData { get; set; } = new List<double>();
        [Parameter] public List<string> ChartLabels { get; set; } = new List<string>();
        [Parameter] public byte[] ImageData { get; set; }

        private string GetImageSrc()
        {
            if (ImageData == null || ImageData.Length == 0)
                return string.Empty;

            var base64 = Convert.ToBase64String(ImageData);
            return $"data:image/png;base64,{base64}";
        }
    }
}
