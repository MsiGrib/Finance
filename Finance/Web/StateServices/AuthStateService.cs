namespace Web.StateServices
{
    public class AuthStateService
    {
        private bool _isAuthenticated;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    OnAuthStateChanged?.Invoke();
                }
            }
        }

        public event Action OnAuthStateChanged;

        public void NotifyStateChanged() => OnAuthStateChanged?.Invoke();
    }
}
