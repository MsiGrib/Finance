using Microsoft.AspNetCore.Components;
using Web.StateServices;

namespace Web.Layout
{
    public partial class NavPanel
    {
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required AuthStateService AuthState { get; set; }
        [Inject] public required UserStorage UserStorage { get; set; }

        [Parameter] public bool IsOpen { get; set; }

        [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

        protected override void OnInitialized()
        {
            AuthState.OnAuthStateChanged += StateHasChanged;
        }

        public void Dispose()
        {
            AuthState.OnAuthStateChanged -= StateHasChanged;
        }

        private void NavigationToHome()
        {
            Navigation.NavigateTo("/");
        }

        private void NavigationToLogIn()
        {
            Navigation.NavigateTo("/LogIn");
        }

        private void NavigationToReport()
        {
            Navigation.NavigateTo("/Report");
        }

        private void NavigationToFinance()
        {
            Navigation.NavigateTo("/Finance");
        }

        private async void LogOut()
        {
            await UserStorage.Clear();
            AuthState.IsAuthenticated = false;
            AuthState.NotifyStateChanged();
            Navigation.NavigateTo("/LogIn");
        }
    }
}
