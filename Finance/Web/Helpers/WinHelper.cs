using Microsoft.AspNetCore.Components;
using MudBlazor;
using Web.UIComponents;

namespace Web.Helpers
{
    public static class WinHelper
    {
        public static async Task ShowDialogAsync(this IDialogService dialogService, string title, string message)
        {
            var parameters = new DialogParameters
            {
                { "Title", title },
                { "Message", message }
            };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
            };

            await dialogService.ShowAsync<DialogWindow>("", parameters, options);
        }

        public static void ShowNotification(this ISnackbar snackbar, string title, string message)
        {
            snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;

            RenderFragment customContent = builder =>
            {
                builder.OpenComponent<NotificationWindow>(0);
                builder.AddAttribute(1, "Title", title);
                builder.AddAttribute(2, "Message", message);
                builder.CloseComponent();
            };

            snackbar.Add(customContent, Severity.Normal, config =>
            {
                config.RequireInteraction = false;
                config.VisibleStateDuration = 4000;
                config.SnackbarVariant = Variant.Filled;
            });
        }
    }
}
