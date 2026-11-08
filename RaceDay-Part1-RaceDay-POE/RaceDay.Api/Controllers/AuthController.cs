using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(
            RaceDayDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();
            string role = request.Role.Trim();

            // Check that the role is valid.
            if (role != "Organiser" && role != "Participant")
            {
                return BadRequest(new
                {
                    message = "Role must be Organiser or Participant."
                });
            }

            // Check whether the email is already registered.
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message = "An account with this email already exists."
                });
            }

            // Create the new user.
            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = role,
                Phone = request.Phone
            };

            // Hash the password before saving it.
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                userId = user.UserID,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            // Find the user by email.
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Verify the supplied password.
            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Save the user's ID and role in the session.
            HttpContext.Session.SetInt32("UserId", user.UserID);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok(new
            {
                message = "Login successful.",
                userId = user.UserID,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role
            });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                message = "Logout successful."
            });
        }
    }

    // Data required when registering a user.
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }
    }

    // Data required when logging in.
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

