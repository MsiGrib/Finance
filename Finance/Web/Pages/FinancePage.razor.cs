using Cryptograf;
using DataModel.DataBase;
using DataModel.ModelsRequest;
using DataModel.ModelsResponse;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Net;
using Web.Helpers;
using Web.StateServices;

namespace Web.Pages
{
    public partial class FinancePage
    {
        [Inject] public required IDialogService DialogService { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required UniversalApiManager UniversalApiManager { get; set; }
        [Inject] public required BasicConfiguration BasicConfiguration { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required AuthStateService AuthState { get; set; }
        [Inject] public required ICryptoService CriptoService { get; set; }

        private MainBoardResponse _mainBoardData;

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

                string urlMainBoard = $"{BasicConfiguration.ApiUrl}api/Finance/MainBoard";
                var response = await UniversalApiManager.GetAsync<ApiResponse<MainBoardResponse>>(BasicConfiguration.ApiName, urlMainBoard, await UserStorage.GetToken());

                if (response == null)
                {
                    Snackbar.ShowNotification("Уведомление", "Произошла ошибка на стороне сервера!");
                }
                else if (response.StatusCode == (int)HttpStatusCode.OK)
                {
                    _mainBoardData = response.Data;

                    foreach (var item in _mainBoardData.MainBoards)
                    {
                        item.Item2.Name = CriptoService.DecryptString(item.Item2.Name);
                        item.Item2.SubName = CriptoService.DecryptString(item.Item2.SubName);
                        item.Item2.Currency = CriptoService.DecryptString(item.Item2.Currency);
                        item.Item2.ImageBase64 = CriptoService.DecryptString(item.Item2.ImageBase64);
                    }

                    StateHasChanged();
                }
                else
                {
                    Snackbar.ShowNotification("Уведомление", response.Message);
                }
            }
        }
    }
}
