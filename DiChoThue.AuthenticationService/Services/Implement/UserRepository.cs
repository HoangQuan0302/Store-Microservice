using Dapper;
using DiChoThue.AuthenticationService.Models;
using DiChoThue.AuthenticationService.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiChoThue.AuthenticationService.Services.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly JWTSettings _jwtsettings;
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration, IOptions<JWTSettings> jwtsettings)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtsettings = jwtsettings.Value;
        }

        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE UserId=@UserId", new { UserId = id });
            if(user==null)
            {
                return null;
            }
            else
            {
                var affected = await connection.ExecuteAsync("DELETE FROM UserAccount WHERE UserId=@UserId", new { UserId = id });
                if (affected == 0)
                {
                    return null;
                }
                return user;
            }
        }

        public string GenerateAccessToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(userId))
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

            return refreshToken;
        }

        public async Task<ActionResult<User>> GetUser(int id)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE UserId=@UserId", new { UserId = id });
            return user;
        }

        public async Task<ActionResult<User>> GetUserByAccessToken([FromBody] string accessToken)
        {
            User user = await GetUserFromAccessToken(accessToken);
            if(user!=null)
            {
                return user;
            }
            return null;
        }

        public async Task<ActionResult<User>> GetUserDetails(int id)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE UserId=@UserId", new { UserId = id });
            var roleuser = await connection.QueryFirstOrDefaultAsync<Role>("SELECT * FROM Role WHERE RoleId=@RoleId", new { RoleId = user.RoleId });
            return new User()
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Source = user.Source,
                Role = new Role() { RoleId = roleuser.RoleId, RoleDesc = roleuser.RoleDesc }
            };
            }

        public async Task<User> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
                    var user= await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE UserId=@UserId", new { UserId= Convert.ToInt32(userId) });
                    var roleuser = await connection.QueryFirstOrDefaultAsync<Role>("SELECT * FROM Role WHERE RoleId=@RoleId", new { RoleId = user.RoleId });
                    return new User() { UserId=Convert.ToInt32(userId),EmailAddress=user.EmailAddress,Password=user.Password,FirstName=user.FirstName,MiddleName=user.MiddleName,LastName=user.LastName,
                    Source=user.Source,Role=new Role() { RoleId=roleuser.RoleId,RoleDesc=roleuser.RoleDesc},
                    };
                }
            }
            catch (Exception)
            {
                return new User();
            }

            return new User();
        }

        public async Task<ActionResult<UserWithToken>> Login([FromBody] User user)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            user = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE EmailAddress=@EmailAddress", new { EmailAddress = user.EmailAddress });
            UserWithToken userWithToken = null;
            if (user != null)
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await connection.ExecuteAsync("UPDATE RefreshToken SET Token=@Token WHERE UserId=@UserId", new { Token = refreshToken.Token,UserId=user.UserId });

                userWithToken = new UserWithToken(user);
                userWithToken.RefreshToken = refreshToken.Token;
            }
            if (userWithToken == null)
            {
                return null;
            }

            //sign your token here here..
            userWithToken.AccessToken = GenerateAccessToken(user.UserId);
            return userWithToken;
        }

        public Task<ActionResult<User>> PostUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ActionResult<UserWithToken>> RefreshToken(RefreshRequest refreshRequest)
        {
            User user = await GetUserFromAccessToken(refreshRequest.AccessToken);

            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                UserWithToken userWithToken = new UserWithToken(user);
                userWithToken.AccessToken = GenerateAccessToken(user.UserId);

                return userWithToken;
            }

            return null;
        }

        public async Task<ActionResult<UserWithToken>> RegisterUser([FromBody] User user)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var userExist = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE EmailAddress=@EmailAddress", new { EmailAddress = user.EmailAddress });
            if(userExist==null)
            {
                await connection.ExecuteAsync("INSERT INTO UserAccount(EmailAddress,Password,FirstName,MiddleName,LastName,RoleId) VALUES(@EmailAddress,@Password,@FirstName,@MiddleName,@LastName,@RoleId); ", new { EmailAddress = user.EmailAddress, Password = user.Password, FirstName = user.FirstName, MiddleName = user.MiddleName, LastName = user.LastName, RoleId = user.RoleId });
            }
            else
            {
                return null;
            }

            //Load user
            var userRes = await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM UserAccount WHERE EmailAddress=@EmailAddress", new { EmailAddress = user.EmailAddress });

            UserWithToken userWithToken = null;

            if (userRes != null)
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                userRes.RefreshTokens.Add(refreshToken);
                await connection.ExecuteAsync("INSERT INTO RefreshToken (UserId,Token,ExpiryDate) VALUES(@UserId,@Token,@ExpiryDate)", new { UserId = userRes.UserId,Token=refreshToken.Token,ExpiryDate=refreshToken.ExpiryDate });

                userWithToken = new UserWithToken(userRes);
                userWithToken.RefreshToken = refreshToken.Token;
            }

            if (userWithToken == null)
            {
                return null;
            }

            //sign your token here here..
            userWithToken.AccessToken = GenerateAccessToken(userRes.UserId);
            return userWithToken;
        }

        public Task<ActionResult<User>> UpdateUser(int id, User user)
        {
            throw new System.NotImplementedException();
        }

        public bool UserExists(int id)
        {
            throw new System.NotImplementedException();
        }

        public  bool ValidateRefreshToken(User user, string refreshToken)
        {
            using var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            RefreshToken refreshTokenUser = connection.QueryFirstOrDefault<RefreshToken>("SELECT * FROM WHERE Token=@Token", new { Token=refreshToken});

            if (refreshTokenUser != null && refreshTokenUser.UserId == user.UserId
                && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }
    }
}
