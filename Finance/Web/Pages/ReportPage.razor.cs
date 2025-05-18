using Cryptograf;
using DataModel.ModelsResponse;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Microsoft.JSInterop.WebAssembly;
using MudBlazor;
using System.Net;
using Web.Helpers;
using Web.StateServices;

namespace Web.Pages
{
    public partial class ReportPage
    {
        [Inject] public required IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public required IDialogService DialogService { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required UniversalApiManager UniversalApiManager { get; set; }
        [Inject] public required BasicConfiguration BasicConfiguration { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required AuthStateService AuthState { get; set; }
        [Inject] public required ICryptoService CriptoService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (string.IsNullOrEmpty(await UserStorage.GetToken()))
                {
                    await UserStorage.Clear();
                    AuthState.IsAuthenticated = false;
                    AuthState.NotifyStateChanged();
                    Navigation.NavigateTo("/LogIn");
                    return;
                }

                if (!await UserStorage.CheckTokenExpiration())
                {
                    await UserStorage.Clear();
                    AuthState.IsAuthenticated = false;
                    AuthState.NotifyStateChanged();
                    Navigation.NavigateTo("/LogIn");
                    return;
                }
            }
        }

        private async void DownloadWord()
        {
            if (string.IsNullOrEmpty(await UserStorage.GetToken()))
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            if (!await UserStorage.CheckTokenExpiration())
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            string urlMainBoard = $"{BasicConfiguration.ApiUrl}api/Report/Word";
            var response = await UniversalApiManager.GetAsync<ApiResponse<FileResponse>>(BasicConfiguration.ApiName, urlMainBoard, await UserStorage.GetToken());

            if (response == null)
            {
                Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                using var streamRef = new DotNetStreamReference(stream: new MemoryStream(response.Data.Bytes));
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", CriptoService.DecryptString(response.Data.FileName), streamRef);
            }
            else
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
            }
        }

        private async void DownloadExcel()
        {
            if (string.IsNullOrEmpty(await UserStorage.GetToken()))
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            if (!await UserStorage.CheckTokenExpiration())
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            string urlMainBoard = $"{BasicConfiguration.ApiUrl}api/Report/Excel";
            var response = await UniversalApiManager.GetAsync<ApiResponse<FileResponse>>(BasicConfiguration.ApiName, urlMainBoard, await UserStorage.GetToken());

            if (response == null)
            {
                Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                using var streamRef = new DotNetStreamReference(stream: new MemoryStream(response.Data.Bytes));
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", CriptoService.DecryptString(response.Data.FileName), streamRef);
            }
            else
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
            }
        }

        private async void DownloadJson()
        {
            if (string.IsNullOrEmpty(await UserStorage.GetToken()))
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            if (!await UserStorage.CheckTokenExpiration())
            {
                await UserStorage.Clear();
                AuthState.IsAuthenticated = false;
                AuthState.NotifyStateChanged();
                Navigation.NavigateTo("/LogIn");
                return;
            }

            string urlMainBoard = $"{BasicConfiguration.ApiUrl}api/Report/Json";
            var response = await UniversalApiManager.GetAsync<ApiResponse<FileResponse>>(BasicConfiguration.ApiName, urlMainBoard, await UserStorage.GetToken());

            if (response == null)
            {
                Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
            }
            else if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                using var streamRef = new DotNetStreamReference(stream: new MemoryStream(CriptoService.Decrypt(response.Data.Bytes)));
                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", CriptoService.DecryptString(response.Data.FileName), streamRef);
            }
            else
            {
                Snackbar.ShowNotification("Уведомление", response.Message);
            }
        }
    }
}
