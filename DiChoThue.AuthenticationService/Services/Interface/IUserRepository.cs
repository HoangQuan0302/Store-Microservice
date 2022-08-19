using DiChoThue.AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiChoThue.AuthenticationService.Services.Interface
{
    public interface IUserRepository
    {
        Task<ActionResult<User>> GetUser(int id);
        Task<ActionResult<User>> GetUserDetails(int id);
        Task<ActionResult<UserWithToken>> Login([FromBody] User user);
        Task<ActionResult<UserWithToken>> RegisterUser([FromBody] User user);
        Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest);
        Task<ActionResult<User>> GetUserByAccessToken([FromBody] string accessToken);
        bool ValidateRefreshToken(User user,string refreshToken);
        Task<User> GetUserFromAccessToken(string accessToken);
        RefreshToken GenerateRefreshToken();
        string GenerateAccessToken(int userId);
        Task<ActionResult<User>> UpdateUser(int id,User user);
        Task<ActionResult<User>> DeleteUser(int id);
        Task<ActionResult<User>> PostUser(User user);
        bool UserExists(int id);

    }
}
