using DataModel.DataBase;
using DataModel.DataStructures;
using InternalApi.EntityGateWay;

namespace InternalApi.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO?> GetUserByIdAsync(string idGuid)
        {
            if (string.IsNullOrEmpty(idGuid))
            {
                throw new Exception("Пустой id!");
            }

            if (!Guid.TryParse(idGuid, out var id))
            {
                throw new Exception("Не получилось спарсить Guid!");
            }

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<bool> IsExistsRegistrUserAsync(string login, string email)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(email))
            {
                throw new Exception("Пустые данные!");
            }
            var users = await _userRepository.GetAllAsync();

            return users.Any(x => x.Login == login && x.Email == email);
        }

        public async Task<Pair<UserDTO, bool>> RegistrationUserAsync(string login, string password, string numberPhone, string lastName,
            string firstName, string email, DateTime birthday)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(numberPhone) || string.IsNullOrEmpty(lastName)
                || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email))
            {
                throw new Exception("Пустые данные!");
            }

            var user = new UserDTO
            {
                Id = Guid.NewGuid(),
                Login = login,
                Password = HashUtility.HashPassword(password),
                Email = email,
            };

            bool status = await _userRepository.AddAsync(user);

            return new Pair<UserDTO?, bool>(user, status);
        }

        public async Task<UserDTO?> AuthorizationUserAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Пустые данные!");
            }

            var user = await _userRepository.GetByKredsAsync(login);

            if (user == null)
            {
                return null;
            }

            if (!HashUtility.VerifyPassword(password, user.Password))
            {
                return null;
            }

            return user;
        }
    }
}
