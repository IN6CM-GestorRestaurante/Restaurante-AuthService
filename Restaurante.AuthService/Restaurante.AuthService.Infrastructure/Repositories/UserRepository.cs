using Microsoft.EntityFrameworkCore;
using Restaurante.AuthService.Application.Interfaces;
using Restaurante.AuthService.Domain.Entities;
using Restaurante.AuthService.Infrastructure.Data;

namespace Restaurante.AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    // Inyectamos el DbContext
    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // Entity Framework va a la base de datos a buscar el usuario
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }

    public async Task<User?> GetByVerificationOtpAsync(string otp)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.VerificationOtp == otp);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }

    public async Task<User?> GetByRefreshTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
    }

    public async Task AddAsync(User user)
    {
        // Agrega la entidad a la memoria de EF
        await _context.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<(List<User> Users, int TotalCount)> GetAllAsync(int page, int limit, string? companyMongoId = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(companyMongoId))
        {
            query = query.Where(u => u.CompanyMongoId == companyMongoId);
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task SaveChangesAsync()
    {
        // Impacta los cambios reales en PostgreSQL
        await _context.SaveChangesAsync();
    }
}