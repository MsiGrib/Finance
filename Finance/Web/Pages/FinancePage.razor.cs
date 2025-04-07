using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Web.Pages
{
    public partial class FinancePage
    {
        [Inject] public required IDialogService DialogService { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required UniversalApiManager UniversalApiManager { get; set; }
        [Inject] public required BasicConfiguration BasicConfiguration { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
            }
        }




    }
}
