using ApiTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _dbContext;

        public UserService(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private async Task<bool> IsAdminChecker(string requesterLogin, string requesterPassword)
        {
            var requester = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == requesterLogin && u.Password == requesterPassword);

            if (requester == null || !requester.Admin)
            {
                return false;
            }

            return true;
        }

        public async Task<string> CreateUserAsync(string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var newUser = new User
                {
                    Login = login,
                    Password = password,
                    Name = name,
                    Gender = gender,
                    Birthday = birthday,
                    Admin = isAdmin,
                    CreatedBy = requesterLogin,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = requesterLogin,
                    ModifiedOn = DateTime.Now,
                };

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return "Пользователь успешно создан.";
            }

            return "Ошибка доступа.";
        }

        public async Task<string> DeleteUserAsync(string login, bool softDelete, string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var userToDelete = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (userToDelete == null)
                {
                    return "Пользователь не найден.";
                }

                if (softDelete)
                {
                    userToDelete.ModifiedBy = requesterLogin;
                    userToDelete.ModifiedOn = DateTime.Now;
                    userToDelete.RevokedOn = DateTime.Now;
                    userToDelete.RevokedBy = requesterLogin;
                }
                else
                {
                    _dbContext.Users.Remove(userToDelete);
                }

                await _dbContext.SaveChangesAsync();

                return "Пользователь успешно удален.";
            }

            return "Ошибка доступа.";
        }

        public async Task<(List<User>, string)> GetAllActiveUsersAsync(string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var activeUsers = await _dbContext.Users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn).ToListAsync();

                return (activeUsers, "ok");
            }

            return (null, "Ошибка доступа.");
        }

        public async Task<(User, string)> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return (null, "Пользователь не найден");
            }

            if (user.RevokedOn != null)
            {
                return (null, "Пользователь не найден");
            }

            return (user, "ok");
        }

        public async Task<(UserDto, string)> GetUserByLoginAsync(string login, string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (user == null)
                {
                    return (null, "ok"); 
                }

                var userDto = new UserDto
                {
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                };

                return (userDto, "ok");
            }

            return (null, "Ошибка доступа.");
        }

        public async Task<(List<User>, string)> GetUsersOlderThanAgeAsync(int age, string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var currentDate = DateTime.Now;

                var minBirthdate = currentDate.AddYears(-age);

                var usersOlderThanAge = await _dbContext.Users.Where(u => u.Birthday <= minBirthdate).ToListAsync();

                return (usersOlderThanAge, "ok");
            }

            return (null, "Ошибка доступа.");
        }

        public async Task<string> RestoreUserAsync(string login, string requesterLogin, string requesterPassword)
        {
            if (await IsAdminChecker(requesterLogin, requesterPassword))
            {
                var userToRestore = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (userToRestore == null)
                {
                    return "Пользователь не найден.";
                }

                if (userToRestore.RevokedOn == null)
                {
                    return "Пользователь не удален или удален полностью.";
                }

                userToRestore.RevokedOn = null;
                userToRestore.RevokedBy = null;

                await _dbContext.SaveChangesAsync();

                return "Пользователь успешно восстановлен.";
            }

            return "Ошибка доступа.";
        }

        public async Task<string> UpdateLoginAsync(string currentLogin, string newLogin, string requesterLogin, string requesterPassword)
        {
            var requester = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == requesterLogin && u.Password == requesterPassword);
            if (requester == null || (!requester.Admin && requester.Login != currentLogin))
            {
                return "Ошибка доступа.";
            }

            var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == currentLogin);
            if (userToUpdate == null)
            {
                return "Пользователь не найден.";
            }

            if (await _dbContext.Users.AnyAsync(u => u.Login == newLogin))
            {
                return "Новый логин уже занят.";
            }

            userToUpdate.Login = newLogin;
            await _dbContext.SaveChangesAsync();

            return "Логин успешно обновлен.";
        }

        public async Task<string> UpdatePasswordAsync(string login, string newPassword, string requesterLogin, string requesterPassword)
        {
            var requester = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == requesterLogin && u.Password == requesterPassword);
            if (requester == null || (!requester.Admin && requester.Login != login))
            {
                return "Ошибка доступа.";
            }

            var userToUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (userToUpdate == null)
            {
                return "Пользователь не найден.";
            }

            userToUpdate.Password = newPassword;
            await _dbContext.SaveChangesAsync();

            return "Пароль успешно обновлен.";
        }

        public async Task<string> UpdateUserAsync(string login, string password, string newName, int newGender, DateTime? newBirthday, string requesterLogin, string requesterPassword)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
            if (user == null)
            {
                return "Пользователь не найден.";
            }

            var requester = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == requesterLogin && u.Password == requesterPassword);
            if (requester == null || (!requester.Admin && requester.Login != login))
            {
                return "Ошибка доступа.";
            }

            if (user.RevokedOn != null)
            {
                return "Пользователь не активен.";
            }

            user.Name = newName;
            user.Gender = newGender;
            user.Birthday = newBirthday;

            await _dbContext.SaveChangesAsync();

            return "Данные пользователя успешно обновлены.";
        }
    }
}
