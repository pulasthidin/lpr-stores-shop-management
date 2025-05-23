using LPRStoresAPI.DTOs;
using LPRStoresAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; // For IConfiguration
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq; // Required for Select
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "User with this email already exists." };
            }

            var newUser = new ApplicationUser
            {
                UserName = registerDto.Email, // Typically UserName is Email or a separate Username
                Email = registerDto.Email,
                // EmailConfirmed = true // Optional: auto-confirm or implement email confirmation flow
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto { IsSuccess = false, Message = $"Registration failed: {errors}" };
            }
            
            // Optional: Add user to a default role here if needed
            // await _userManager.AddToRoleAsync(newUser, "User");

            return new AuthResponseDto { IsSuccess = true, Message = "User registered successfully." };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password." };
            }

            var tokenString = await GenerateJwtTokenAsync(user); // Changed to await
            return new AuthResponseDto 
            { 
                IsSuccess = true, 
                Token = tokenString, 
                Email = user.Email ?? "N/A" // Ensure Email is not null
            };
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user) // Changed to async Task<string>
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];
            var durationInHours = _configuration["JwtSettings:DurationInHours"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(durationInHours))
            {
                throw new InvalidOperationException("JWT settings (Key, Issuer, Audience, DurationInHours) are not configured properly in appsettings.json.");
            }
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                new Claim(ClaimTypes.NameIdentifier, user.Id) 
            };
            
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(durationInHours)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
