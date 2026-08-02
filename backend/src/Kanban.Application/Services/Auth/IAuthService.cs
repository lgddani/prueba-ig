using Kanban.Application.DTOs.Auth;

namespace Kanban.Application.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
