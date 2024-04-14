using ApiTest.Models;
using ApiTest.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace ApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.CreateUserAsync(user.Login, user.Password, user.Name, user.Gender, user.Birthday, user.Admin, requesterLogin, requesterPassword);
            return Ok(result);
        }

        [HttpDelete("delete/{login}")]
        public async Task<IActionResult> DeleteUser(string login, bool softDelete, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.DeleteUserAsync(login, softDelete, requesterLogin, requesterPassword);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveUsers(string requesterLogin, string requesterPassword)
        {
            var result = await _userService.GetAllActiveUsersAsync(requesterLogin, requesterPassword);
            if (result.Item2 == "ok")
            {
                return Ok(result.Item1);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpGet("olderThan/{age}")]
        public async Task<IActionResult> GetUsersOlderThanAge(int age, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.GetUsersOlderThanAgeAsync(age, requesterLogin, requesterPassword);
            if (result.Item2 == "ok")
            {
                return Ok(result.Item1);
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }

        [HttpPost("restore/{login}")]
        public async Task<IActionResult> RestoreUser(string login, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.RestoreUserAsync(login, requesterLogin, requesterPassword);
            return Ok(result);
        }

        [HttpPost("updateLogin")]
        public async Task<IActionResult> UpdateLogin(string currentLogin, string newLogin, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.UpdateLoginAsync(currentLogin, newLogin, requesterLogin, requesterPassword);
            return Ok(result);
        }

        [HttpPost("updatePassword")]
        public async Task<IActionResult> UpdatePassword(string login, string newPassword, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.UpdatePasswordAsync(login, newPassword, requesterLogin, requesterPassword);
            return Ok(result);
        }

        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser(string login, string password, string newName, int newGender, DateTime? newBirthday, string requesterLogin, string requesterPassword)
        {
            var result = await _userService.UpdateUserAsync(login, password, newName, newGender, newBirthday, requesterLogin, requesterPassword);
            return Ok(result);
        }
    }
}
