using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorStores.Application.DTOs;
using MotorStores.Application.Features.Cheques.Queries;
using MotorStores.Application.Features.UserIds.Commands;
using MotorStores.Application.Features.UserIds.Queries;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Helpers;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;
    private readonly TokenService _tokens;
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly EmailService _email;
    private readonly IMediator _mediator;

    public AuthController(
        UserManager<AppUser> users,
        SignInManager<AppUser> signIn,
        TokenService tokens,
        ApplicationDbContext db,
        IConfiguration cfg,
        EmailService email,
        IMediator mediator)
    {
        _users = users;
        _signIn = signIn;
        _tokens = tokens;
        _db = db;
        _cfg = cfg;
        _email = email;
        _mediator = mediator;
    }

    public record RegisterDto(string Username, string Email, string Password, string Role, int? CreatedBy);
    public record LoginDto(string Username, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUserName = await _users.Users
            .FirstOrDefaultAsync(u => u.UserName == dto.Username);
        if (existingUserName != null)
            return Unauthorized("Username has been used");

        var existingEmail = await _users.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingEmail != null)
            return Unauthorized("Email has been used");

        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };
        var result = await _users.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var newUser = await _users.Users
            .FirstOrDefaultAsync(u => u.UserName == dto.Username);

        if (newUser == null)
            return BadRequest("User not found.");

        string? resolvedCreatedBy = null;
        if (dto.CreatedBy.HasValue && dto.CreatedBy.Value != 0)
        {
            var creatorUser = await _mediator.Send(new GetUserByIdQuery { Id = dto.CreatedBy.Value });
            resolvedCreatedBy = creatorUser?.UserId;
        }

        var userIdDto = await _mediator.Send(new CreateUserIdCommand
        {
            UserId = newUser.Id,
            Role = dto.Role,
            CreatedBy = resolvedCreatedBy ?? null
        });

        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {

        var user = await _users.Users
            .FirstOrDefaultAsync(u => u.UserName == dto.Username || u.Email == dto.Username);
        if (user == null) return Unauthorized("Invalid Username");


        var check = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!check.Succeeded) return Unauthorized("Invalid Password");


        var cheque = await _mediator.Send(new GetIdByUserQuery { UserId = user.Id });


        var access = await _tokens.CreateAccessTokenAsync(user, cheque);

        return Ok(new
        {
            accessToken = access,
            email = user.Email,
            username = user.UserName,
            id = cheque?.Id,
            role = cheque?.Role
        });
    }


[HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        
    {
        string confirmedEmail;
        string confirmedUserName;
        AppUser confirmedUser;

        var user = await _users.FindByEmailAsync(email);
        if (user == null)
        {
            var user1 = await _users.Users.FirstOrDefaultAsync(u => u.UserName == email);
            if (user1 == null)
                return Unauthorized("User cannot be found");

            confirmedEmail = user1.Email;
            confirmedUser = user1;
            confirmedUserName = user1.UserName;
        }
        else
        {
            confirmedEmail = user.Email;
            confirmedUser = user;
            confirmedUserName = user.UserName;
        }


        var resetToken = await _users.GeneratePasswordResetTokenAsync(confirmedUser);
        if (resetToken != null)
        {
            var random = new Random();
            int otp = random.Next(1000, 9999);

            var emailResponse = await _email.SendEmailAsync(
                confirmedEmail,
                "Janasiri Motor Stores – OTP Verification",
                $@"
                <div style='font-family:Segoe UI, Helvetica, Arial, sans-serif; background-color:#f4f6f8; padding:20px;'>
                    <div style='max-width:600px; margin:auto; background:#ffffff; border-radius:8px; padding:30px; box-shadow:0 4px 10px rgba(0,0,0,0.05);'>
                        <h2 style='color:#1e88e5; text-align:center; margin-bottom:10px;'>Janasiri Motor Stores</h2>
                        <h4 style='color:#444; text-align:center; margin-top:0;'>One-Time Password (OTP) Verification</h4>
                        <p style='font-size:15px; color:#333; margin-top:25px;'>
                            Dear Customer,<br><br>
                            To continue with your request, please use the following One-Time Password (OTP):
                        </p>
                        <div style='text-align:center; margin:30px 0;'>
                            <span style='display:inline-block; background-color:#ffffff; color:#1e88e5; font-size:24px; font-weight:bold; letter-spacing:3px; padding:12px 25px; border-radius:6px;'>
                                {otp}
                            </span>
                        </div>
                        <p style='font-size:14px; color:#555;'>
                            This code will expire in <strong>5 minutes</strong>. Please do not share it with anyone.
                        </p>
                        <hr style='margin:30px 0; border:none; border-top:1px solid #ddd;' />
                        <p style='font-size:12px; color:#777; text-align:center;'>
                            © {DateTime.Now.Year} Janasiri Motor Stores<br/>
                            Secure. Reliable. Trusted Service.
                        </p>
                    </div>
                </div>",
                otp,
                confirmedUserName
            );

           if (emailResponse.status == true) {
                string encryptedOtp = CryptoHelper.Encrypt(emailResponse.otp.ToString());
                return Ok(new
                    {
                    email = emailResponse.email,
                    newToken = resetToken,
                    otp = encryptedOtp,
                    username = emailResponse.username
                });
                }
                else { return Unauthorized("Email send failed"); }
        }
        else return BadRequest("Please try again.");
    }
    public record ResetPasswordDto(string Email, string Token, string NewPassword);

    // ── UPDATE USER ───────────────────────────────────────────────────────────────
    [HttpPut("users/{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(UpdateUserIdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserIdCommand command)
    {
      

        try
        {
            command.Id = id;

            var existingUserIdDto = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (existingUserIdDto == null)
                return NotFound($"User with ID {id} not found.");

            var identityUser = await _users.FindByIdAsync(existingUserIdDto.UserId);
            if (identityUser == null)
                return NotFound($"Identity user not found.");
            var updated = await _mediator.Send(command);

            return Ok(new
            {
                id = id,
                username = identityUser.UserName,
                email = identityUser.Email,
                role = updated.Role,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Invalid email.");

        var result = await _users.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password has been reset successfully." });
    }


    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email)
    {
        try { 
        var Email = await _users.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (Email == null) {

            var random = new Random();
            int otp = random.Next(1000, 9999);

           var emailResponse = await _email.SendEmailAsync(
                email,
                "Janasiri Motor Stores – Email Verification Code",
                $@"
                    <div style='font-family:Segoe UI, Helvetica, Arial, sans-serif; background-color:#f4f6f8; padding:20px;'>
                        <div style='max-width:600px; margin:auto; background:#ffffff; border-radius:8px; padding:30px; box-shadow:0 4px 10px rgba(0,0,0,0.05);'>
                            <h2 style='color:#1e88e5; text-align:center; margin-bottom:10px;'>Janasiri Motor Stores</h2>
                            <h4 style='color:#444; text-align:center; margin-top:0;'>Email Verification</h4>
                            <p style='font-size:15px; color:#333; margin-top:25px;'>
                                Dear Customer,<br><br>
                                Thank you for choosing <strong>Janasiri Motor Stores</strong>.<br/>
                                To verify your email address and complete your registration, please use the following verification code:
                            </p>
                            <div style='text-align:center; margin:30px 0;'>
                                <span style='display:inline-block; background-color:#ffffff; color:#1e88e5; font-size:24px; font-weight:bold; letter-spacing:3px; padding:12px 25px; border-radius:6px; border:2px solid #1e88e5;'>
                                    {otp}
                                </span>
                            </div>
                            <p style='font-size:14px; color:#555;'>
                                This code will expire in <strong>5 minutes</strong>.<br/>
                                Please enter it on the verification page to confirm your email address.<br/>
                                Do not share this code with anyone for security reasons.
                            </p>
                            <hr style='margin:30px 0; border:none; border-top:1px solid #ddd;' />
                            <p style='font-size:12px; color:#777; text-align:center;'>
                                © {DateTime.Now.Year} Janasiri Motor Stores<br/>
                                Secure. Reliable. Trusted Service.
                            </p>
                        </div>
                    </div>",
                otp,
                "Register (first-time user)"
            );
                if (emailResponse.status == true) {
                    string encryptedOtp = CryptoHelper.Encrypt(emailResponse.otp.ToString());
                    return Ok(new
                    {
                        email = email,
                        otp = encryptedOtp
                    });
                }
                else { return Unauthorized("Email send failed"); }



            }
        else { return Unauthorized("Email has been used"); }
    }
        catch (Exception ex)
    {

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An unexpected error occurred.",
                error = ex.Message
            });
        }
    }
    [HttpGet("users/my-users")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyUsers()
    {
        // ✅ Get the logged-in user's Identity ID from token
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(loggedInUserId))
            return Unauthorized("User not logged in.");

        // ✅ Get ALL UserIdDtos
        var allUserIds = await _mediator.Send(new GetAllUserIdsQuery());

        // ✅ Filter only users whose CreatedBy == logged-in user's Identity Id
        var myUsers = allUserIds
            .Where(u => u.CreatedBy == loggedInUserId)
            .ToList();

        // ✅ Enrich with Identity user info
        var result = new List<object>();
        foreach (var userIdDto in myUsers)
        {
            var identityUser = await _users.FindByIdAsync(userIdDto.UserId);
            if (identityUser == null) continue;

            result.Add(new
            {
                id = userIdDto.Id,
                userId = userIdDto.UserId,
                username = identityUser.UserName,
                email = identityUser.Email,
                role = userIdDto.Role,
                createdBy = userIdDto.CreatedBy,
            });
        }

        return Ok(result);
    }

    // ── DELETE USER ───────────────────────────────────────────────────────────
    [HttpDelete("users/{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // ✅ Get the UserIdDto by int Id to find the Identity UserId string
        var userIdDto = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (userIdDto == null)
            return NotFound($"User with ID {id} not found.");

        // ✅ Find the Identity user
        var identityUser = await _users.FindByIdAsync(userIdDto.UserId);
        if (identityUser == null)
            return NotFound($"Identity user not found.");

        // ✅ Delete from UserIds table via MediatR
        var deleted = await _mediator.Send(new DeleteUserIdCommand { Id = id });
        if (!deleted)
            return BadRequest("Failed to delete user record.");

        // ✅ Delete from Identity
        var result = await _users.DeleteAsync(identityUser);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }

[HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
        {
            var rt = await _db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == token);
            if (rt != null && rt.IsActive)
            {
                rt.Revoked = DateTime.UtcNow;
                rt.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _db.SaveChangesAsync();
            }
            Response.Cookies.Delete("refreshToken", new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
        }
        return Ok();
    }
}
