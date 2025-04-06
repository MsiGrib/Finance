using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using Web.Helpers;

namespace Web.Pages
{
    public partial class SignInPage
    {
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required IDialogService DialogService { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required UniversalApiManager UniversalApiManager { get; set; }
        [Inject] public required BasicConfiguration BasicConfiguration { get; set; }

        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _repeatPassword = string.Empty;
        private string _email = string.Empty;

        private void OnBack()
        {
            Navigation.NavigateTo("/LogIn");
        }

        private async Task OnSignIn()
        {
            if (_password != _repeatPassword)
            {
                await DialogService.ShowDialogAsync("Ошибка", "Пароли не совпадают!");
                return;
            }

            var request = new RegistrationRequest
            {
                Login = _login,
                Password = _password,
                Email = _email,
            };

            string url = $"{BasicConfiguration.ApiUrl}api/Auth/Registration";
            var response = await UniversalApiManager.PostAsync<RegistrationRequest, BaseResponse>(BasicConfiguration.ApiName, url, request);

            if (response == null)
            {
                Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
                Navigation.NavigateTo("/LogIn");
            }
            else
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
            }
        }
    }
}
