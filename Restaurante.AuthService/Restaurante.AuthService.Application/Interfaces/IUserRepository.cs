using Restaurante.AuthService.Domain.Entities;

namespace Restaurante.AuthService.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailVerificationTokenAsync(string token);
    Task<User?> GetByPasswordResetTokenAsync(string token);
    Task<User?> GetByRefreshTokenAsync(string token);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<(List<User> Users, int TotalCount)> GetAllAsync(int page, int limit, string? companyMongoId = null);
    Task SaveChangesAsync();
}