using ApiTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ApiTest.Services
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string requesterLogin, string requesterPassword);
        Task<string> UpdateUserAsync(string login, string password, string newName, int newGender, DateTime? newBirthday, string requesterLogin, string requesterPassword);
        Task<string> UpdatePasswordAsync(string login, string newPassword, string requesterLogin, string requesterPassword);
        Task<string> UpdateLoginAsync(string login, string newLogin, string requesterLogin, string requesterPassword);
        Task<(List<User>, string)> GetAllActiveUsersAsync(string requesterLogin, string requesterPassword);
        Task<(UserDto, string)> GetUserByLoginAsync(string login, string requesterLogin, string requesterPassword);
        Task<(User, string)> GetUserByLoginAndPasswordAsync(string login, string password);
        Task<(List<User>, string)> GetUsersOlderThanAgeAsync(int age, string requesterLogin, string requesterPassword);
        Task<string> DeleteUserAsync(string login, bool softDelete, string requesterLogin, string requesterPassword);
        Task<string> RestoreUserAsync(string login, string requesterLogin, string requesterPassword);
    }
}
