using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
using System.IO;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly AzureBlobService _blobService;

        public ProfileController(
            RaceDayDbContext context,
            AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/profile/me
        [HttpGet("me")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                user.Role,
                user.ProfilePictureUrl
            });
        }

        // PUT: api/profile/me
        [HttpPut("me")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    message = "First name, last name and email are required."
                });
            }

            string email = request.Email.Trim();

            bool emailExists = await _context.Users.AnyAsync(
                u => u.Email == email && u.UserID != userId);

            if (emailExists)
                return BadRequest(new
                {
                    message = "Email is already in use."
                });

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = email;
            user.Phone = request.Phone?.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }

        // POST: api/profile/picture
        [HttpPost("picture")]
        [RoleAuthorize("Organiser", "Participant")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfilePicture(
            IFormFile file)
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            if (file == null || file.Length == 0)
                return BadRequest(new
                {
                    message = "Please select a picture."
                });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new
                {
                    message = "Picture must not exceed 5 MB."
                });

            string extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedTypes = new Dictionary<string, string>
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".webp", "image/webp" }
            };

            if (!allowedTypes.TryGetValue(
                extension, out string? expectedContentType))
            {
                return BadRequest(new
                {
                    message = "Only JPG, JPEG, PNG and WEBP pictures are allowed."
                });
            }

            if (!string.Equals(
                file.ContentType,
                expectedContentType,
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "The uploaded file type is invalid."
                });
            }

            try
            {
                using var stream = file.OpenReadStream();

                string fileName =
                    await _blobService.UploadProfilePictureAsync(
                        stream,
                        expectedContentType,
                        extension);

                user.ProfilePictureUrl =
                    $"/api/profile/picture/{fileName}";

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profile picture uploaded successfully.",
                    profilePictureUrl = user.ProfilePictureUrl
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Unable to upload profile picture. Please try again."
                });
            }
        }

        // GET: api/profile/picture/{fileName}
        [HttpGet("picture/{fileName}")]
        [RoleAuthorize("Organiser", "Participant")]
        public async Task<IActionResult> GetProfilePicture(
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                fileName != Path.GetFileName(fileName))
            {
                return BadRequest(new
                {
                    message = "Invalid picture name."
                });
            }

            try
            {
                var picture =
                    await _blobService.DownloadProfilePictureAsync(fileName);

                return File(
                    picture.Content,
                    picture.ContentType);
            }
            catch (Exception)
            {
                return NotFound(new
                {
                    message = "Profile picture not found."
                });
            }
        }
    }
}