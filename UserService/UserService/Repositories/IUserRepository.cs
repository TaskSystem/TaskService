using UserService.Models;

namespace UserService.Repositories
{
    public interface IUserRepository 
    {
        
            IEnumerable<User> GetAllUsers();
            User GetUserById(Guid id);
            void AddUser(User user);
            void DeleteUser(Guid id);
            void UpdateUser(User updatedUser);
    }
}
