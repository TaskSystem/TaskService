using Microsoft.EntityFrameworkCore;
using System;
using UserService.ApplicationDbContext;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
            private readonly UserDbContext _context;

            public UserRepository(UserDbContext context)
            {
                _context = context;
            }

            public IEnumerable<User> GetAllUsers()
            {
                return _context.Users.ToList();  // Haal gebruikers uit de database
            }

            public User GetUserById(Guid id)
            {
                return _context.Users.FirstOrDefault(u => u.Id == id);  // Haal de gebruiker uit de database
            }

            public void AddUser(User user)
            {
                user.Id = Guid.NewGuid();  // Genereer een nieuwe GUID
                _context.Users.Add(user);
                _context.SaveChanges();  // Opslaan in de database
            }

            public void DeleteUser(Guid id)
            {
                var user = GetUserById(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();  // Opslaan in de database
                }
            }

            public void UpdateUser(User updatedUser)
            {
                _context.Users.Update(updatedUser);  // Update de gebruiker
                _context.SaveChanges();  // Opslaan in de database
            }
        }

}
