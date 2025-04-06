using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.UIComponents
{
    public partial class DialogWindow
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Message { get; set; }

        private void Submit()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
