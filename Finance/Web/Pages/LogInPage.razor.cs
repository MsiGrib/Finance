using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using Web.Helpers;

namespace Web.Pages
{
    public partial class LogInPage
    {
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required IDialogService DialogService { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required UniversalApiManager UniversalApiManager { get; set; }
        [Inject] public required BasicConfiguration BasicConfiguration { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }

        private string _login = string.Empty;
        private string _password = string.Empty;

        private async Task OnLogIn()
        {
            var request = new AuthorizationRequest
            {
                Login = _login,
                Password = _password
            };

            string url = $"{BasicConfiguration.ApiUrl}api/Auth/Authorization";
            var response = await UniversalApiManager.PostAsync<AuthorizationRequest, ApiResponse<AuthorizationResponse>>(BasicConfiguration.ApiName, url, request);

            if (response == null)
            {
                Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                Snackbar.ShowNotification("Уведомление", response.Message);

                await UserStorage.SetToken(response.Data.Token);
                await UserStorage.SetExpirationToken(response.Data.ExpirationTimeToken.ToString());

                Navigation.NavigateTo("/Finance");
            }
            else
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
            }
        }

        private void OnSignIn()
        {
            Navigation.NavigateTo("/SignIn");
        }
    }
}
