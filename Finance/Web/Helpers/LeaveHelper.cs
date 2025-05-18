using Microsoft.JSInterop;

namespace Web.Helpers
{
    public class LeaveHelper
    {
        private UserStorage _userStorage;

        public LeaveHelper(UserStorage userStorage)
        {
            _userStorage = userStorage;
        }

        [JSInvokable("OnLeave")]
        public async void OnLeave()
        {
            await _userStorage.Clear();
        }
    }
}
