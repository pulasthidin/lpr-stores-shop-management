using LPRStoresAPI.DTOs;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto);
    }
}
