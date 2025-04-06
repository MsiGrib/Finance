using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        public required NavigationManager Navigation { get; set; }
        public MudTheme _mudTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Blue.Default,
                Secondary = Colors.BlueGray.Default
            }
        };

        private bool _navOpen = false;
    }
}
