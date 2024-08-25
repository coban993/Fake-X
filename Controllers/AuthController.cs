using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI2.Data;
using DotnetAPI2.DTOs;
using DotnetAPI2.Helpers;
using DotnetAPI2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly ReusableSQL _reusableSQL;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSQL = new ReusableSQL(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserRegistrationDTO, UserComplete>();
                }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserRegistrationDTO userToRegister)
        {
            if (userToRegister.Password == userToRegister.PasswordConfirm)
            {
                string sql = @"
                    SELECT Email
                    FROM TutorialAppSchema.Auth
                    WHERE Email = '" + userToRegister.Email
                    + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sql);
                if (existingUsers.Count() == 0)
                {
                    UserLoginDTO userToSetPassword = new UserLoginDTO()
                    {
                        Email = userToRegister.Email,
                        Password = userToRegister.Password
                    };

                    if (_authHelper.SetPassword(userToSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userToRegister);
                        userComplete.Active = true;

                        if (_reusableSQL.UpsertUser(userComplete))
                        {
                            return Ok();
                        }

                        throw new Exception("Failed to add user");
                    }

                    throw new Exception("Failed to register user!");
                }

                throw new Exception("User aready exist!");
            }

            throw new Exception("Passwords do not match!");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserLoginDTO userToSetPassword)
        {
            if (_authHelper.SetPassword(userToSetPassword))
            {
                return Ok();
            }

            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userToLogin)
        {
            string sqlHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
                @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userToLogin.Email, DbType.String);

            UserLoginConfirmationDTO userLoginConfirmation = _dapper
                .LoadDataSingleWithParameters<UserLoginConfirmationDTO>(sqlHashAndSalt, sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userToLogin.Password, userLoginConfirmation.PasswordSalt);

            if (!passwordHash.SequenceEqual(userLoginConfirmation.PasswordHash))
            {
                return StatusCode(401, "Incorrect password!");
            }

            string userIdSql = @"
                SELECT UserId
                FROM TutorialAppSchema.Users
                WHERE Email = '" + userToLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql = @"
                SELECT UserId
                FROM TutorialAppSchema.Users
                WHERE UserId = " + userId;

            int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userIdFromDb)}
            });
        }
    }
}