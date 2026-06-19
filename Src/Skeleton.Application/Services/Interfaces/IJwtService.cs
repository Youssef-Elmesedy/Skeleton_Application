using Skeleton.Domain.Entities;

namespace Skeleton.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}
