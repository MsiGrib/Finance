using Blazored.LocalStorage;

namespace Web
{
    public class UserStorage
    {
        private readonly ILocalStorageService _localStorage;

        public UserStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetToken(string token)
        {
            await _localStorage.SetItemAsync("token", token);
        }

        public async Task<string?> GetToken()
        {
            return await _localStorage.GetItemAsync<string>("token");
        }

        public async Task SetExpirationToken(string expiration)
        {
            await _localStorage.SetItemAsync("expirationToken", expiration);
        }

        public async Task<string?> GetExpirationToken()
        {
            return await _localStorage.GetItemAsync<string>("expirationToken");
        }

        public async Task Clear()
        {
            await _localStorage.RemoveItemAsync("token");
            await _localStorage.RemoveItemAsync("expiration");
        }
    }
}
