using CourseEnrollBlaze.Server.Authentication;
using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Shared.Entities;
using CourseEnrollBlaze.Shared.Models;
using CourseEnrollBlaze.Shared.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollBlaze.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;

        public UserAccountController(IUserAccountService userAccountService,JwtAuthenticationManager jwtAuthenticationManager)
        {
            _userAccountService = userAccountService;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            { 
                if (registerRequest.Password != registerRequest.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and confirm password do not match");
                    return BadRequest(ModelState);
                }
                var userAccount = new UserAccount
                {
                    IsEmailConfirmed = true,
                    Role = registerRequest.Role,
                    Password = registerRequest.Password,
                    Email = registerRequest.Email,
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                };
                await _userAccountService.RegisterAsync(userAccount);
                var user = await _userAccountService.LoginAsync(registerRequest.Email, registerRequest.Password);
                if (user != null)
                {
                    var token = _jwtAuthenticationManager.GenerateToken(user);
                    var userSession = new UserSession
                    {
                        Email = user.Email,
                        ExpiresIn = 50000,
                        ExpiresInTimeStamp = DateTime.Now.AddSeconds(50000),
                        FirstName = user.FirstName,
                        Id = user.Id.ToString(),
                        LastName = user.LastName,
                        Role = user.Role,
                        Token = token

                    };
                    return Ok(userSession);
                }
                else
                {
                    return Unauthorized("Invalid email or password");
                } 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userAccountService.LoginAsync(loginRequest.Email, loginRequest.Password);
                if (user != null)
                {
                    var token = _jwtAuthenticationManager.GenerateToken(user);
                    var userSession = new UserSession
                    {
                        Email = user.Email,
                        ExpiresIn = 50000,
                        ExpiresInTimeStamp = DateTime.Now.AddSeconds(50000),
                        FirstName = user.FirstName,
                        Id = user.Id.ToString(),
                        LastName = user.LastName,
                        Role = user.Role,
                        Token = token

                    };
                    return Ok(userSession);
                }
                else
                {
                    return Unauthorized("Invalid email or password");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userAccountService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userAccountService.GetUserByIdAsync(id);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserAccount user)
        {
            try
            {
                var result = await _userAccountService.UpdateUserAsync(user);
                if (result)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _userAccountService.DeleteUserAsync(id);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}