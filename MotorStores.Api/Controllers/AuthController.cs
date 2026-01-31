using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorStores.Infrastructure.Entities;
using MotorStores.Infrastructure.Helpers;
using MotorStores.Infrastructure.Persistence;
using MotorStores.Infrastructure.Services;

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

    public AuthController(
        UserManager<AppUser> users,
        SignInManager<AppUser> signIn,
        TokenService tokens,
        ApplicationDbContext db,
        IConfiguration cfg,
        EmailService email)
    {
        _users = users;
        _signIn = signIn;
        _tokens = tokens;
        _db = db;
        _cfg = cfg;
        _email = email;
    }

    public record RegisterDto(string Username, string Email, string Password);
    public record LoginDto(string Username, string Password);
    public record ResetPasswordDto(string Email, string Token, string NewPassword);

    // ================= REGISTER =================
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Invalid data");

        if (await _users.Users.AnyAsync(u => u.UserName == dto.Username))
            return Unauthorized("Username has been used");

        if (await _users.Users.AnyAsync(u => u.Email == dto.Email))
            return Unauthorized("Email has been used");

        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _users.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "User created successfully" });
    }

    // ================= LOGIN =================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.Users
            .FirstOrDefaultAsync(u => u.UserName == dto.Username || u.Email == dto.Username);

        if (user == null)
            return Unauthorized("Invalid Username");

        var check = await _signIn.CheckPasswordSignInAsync(user, dto.Password, true);
        if (!check.Succeeded)
            return Unauthorized("Invalid Password");

        var access = await _tokens.CreateAccessTokenAsync(user);

        return Ok(new
        {
            accessToken = access,
            email = user.Email,
            username = user.UserName
        });
    }

    // ================= FORGOT PASSWORD =================
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        AppUser confirmedUser;
        string confirmedEmail;
        string confirmedUserName;

        var user = await _users.FindByEmailAsync(email);
        if (user == null)
        {
            var userByName = await _users.Users.FirstOrDefaultAsync(u => u.UserName == email);
            if (userByName == null)
                return Unauthorized("User cannot be found");

            if (string.IsNullOrWhiteSpace(userByName.Email))
                return Unauthorized("User email not found");

            confirmedUser = userByName;
        }
        else
        {
            confirmedUser = user;
        }

        confirmedEmail = confirmedUser.Email!;
        confirmedUserName = confirmedUser.UserName ?? string.Empty;

        var resetToken = await _users.GeneratePasswordResetTokenAsync(confirmedUser);
        if (string.IsNullOrWhiteSpace(resetToken))
            return BadRequest("Please try again");

        int otp = Random.Shared.Next(1000, 9999);

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

        if (!emailResponse.status)
            return Unauthorized("Email send failed");

        return Ok(new
        {
            email = confirmedEmail,
            newToken = resetToken,
            otp = CryptoHelper.Encrypt(otp.ToString()),
            username = confirmedUserName
        });
    }

    // ================= RESET PASSWORD =================
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Invalid email");

        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Invalid email");

        var result = await _users.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password has been reset successfully." });
    }

    // ================= VERIFY EMAIL =================
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        try
        {
            var exists = await _users.Users.AnyAsync(u => u.Email == email);
            if (exists)
                return Unauthorized("Email has been used");

            int otp = Random.Shared.Next(1000, 9999);

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

            if (!emailResponse.status)
                return Unauthorized("Email send failed");

            return Ok(new
            {
                email,
                otp = CryptoHelper.Encrypt(otp.ToString())
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An unexpected error occurred.",
                error = ex.Message
            });
        }
    }

    // ================= LOGOUT =================
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

            Response.Cookies.Delete("refreshToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
        }

        return Ok();
    }
}
