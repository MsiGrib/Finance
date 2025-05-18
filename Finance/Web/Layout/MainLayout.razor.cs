using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Web.Helpers;

namespace Web.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required IJSRuntime JSRuntime { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }
        [Inject] public required LeaveHelper LeaveHandler { get; set; }

        public MudTheme _mudTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Blue.Default,
                Secondary = Colors.BlueGray.Default
            }
        };

        private bool _navOpen = false;
        private DotNetObjectReference<LeaveHelper>? dotNetRef;

        protected override async Task OnInitializedAsync()
        {
            dotNetRef = DotNetObjectReference.Create(LeaveHandler);
            await JSRuntime.InvokeVoidAsync("registerLeaveHandler", dotNetRef);
        }

        public async ValueTask DisposeAsync()
        {
            dotNetRef?.Dispose();
        }
    }
}
