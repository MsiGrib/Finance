using DataModel.DataBase;

namespace InternalApi.EntityGateWay
{
    public interface IUserRepository : IRepository<UserDTO, Guid>
    {
        public Task<UserDTO?> GetByKredsAsync(string login);
    }
}
