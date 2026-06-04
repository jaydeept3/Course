using System.Collections.Concurrent;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public IReadOnlyCollection<User> GetAll()
    {
        return _users.Values
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToArray();
    }

    public User? GetById(Guid id)
    {
        return _users.TryGetValue(id, out var user) ? user : null;
    }

    public User Create(User user)
    {
        _users[user.Id] = user;
        return user;
    }

    public bool Update(User user)
    {
        if (!_users.ContainsKey(user.Id))
        {
            return false;
        }

        _users[user.Id] = user;
        return true;
    }

    public bool Delete(Guid id)
    {
        return _users.TryRemove(id, out _);
    }
}
