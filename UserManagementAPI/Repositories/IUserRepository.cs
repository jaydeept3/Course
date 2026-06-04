using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public interface IUserRepository
{
    IReadOnlyCollection<User> GetAll();
    User? GetById(Guid id);
    User Create(User user);
    bool Update(User user);
    bool Delete(Guid id);
}
