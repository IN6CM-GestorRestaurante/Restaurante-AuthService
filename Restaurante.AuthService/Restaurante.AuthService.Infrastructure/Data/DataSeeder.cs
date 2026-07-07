using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restaurante.AuthService.Domain.Entities;
using Restaurante.AuthService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Restaurante.AuthService.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AuthDbContext context, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            // Seed de usuarios SOLO si no existen
            if (!await context.Users.AnyAsync())
            {
                var adminPassword = configuration["SeedSettings:AdminPassword"] ?? "Admin123!";
                
                var users = new List<User>();

                // 1. Super Admin
                users.Add(new User
                {
                    Username = "superadmin",
                    Email = "admin@restaurante.local",
                    PasswordHash = passwordHasher.Hash(adminPassword),
                    Role = "SUPER_ADMIN",
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                });

                // IDs estáticos de MongoDB (para que coincidan exactamente con la base de Node.js)
                var camperoCompanyId = "c00000000000000000000001";
                var camperoBranch1Id = "b00000000000000000000011";
                var camperoBranch2Id = "b00000000000000000000012";

                var mcdoCompanyId = "c00000000000000000000002";
                var mcdoBranch1Id = "b00000000000000000000021";
                var mcdoBranch2Id = "b00000000000000000000022";

                // Contraseñas por defecto para pruebas
                var defaultAdminPass = passwordHasher.Hash("Admin123!");
                var defaultUserPass = passwordHasher.Hash("Password123!");

                // ==========================================
                // POLLO CAMPERO
                // ==========================================
                // Admin
                users.Add(CreateUser("e00000000000000000000010", "Juan", "Bautista", "jbautista", "admin@campero.com", "55511111", "COMPANY_ADMIN", camperoCompanyId, null, defaultAdminPass));
                
                // Branch 1
                users.Add(CreateUser("e00000000000000000000011", "Luis", "Perez", "lperez", "manager1@campero.com", "55511112", "BRANCH_MANAGER", camperoCompanyId, camperoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000012", "Sofia", "Castro", "scastro", "waiter1@campero.com", "55511113", "WAITER", camperoCompanyId, camperoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000013", "Mario", "Ruiz", "mruiz", "chef1@campero.com", "55511114", "CHEF", camperoCompanyId, camperoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000014", "Ana", "Lopez", "alopez", "cashier1@campero.com", "55511115", "CASHIER", camperoCompanyId, camperoBranch1Id, defaultUserPass));
                
                // Branch 2
                users.Add(CreateUser("e00000000000000000000015", "Carlos", "Giron", "cgiron", "manager2@campero.com", "55511116", "BRANCH_MANAGER", camperoCompanyId, camperoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000016", "Diana", "Morales", "dmorales", "waiter2@campero.com", "55511117", "WAITER", camperoCompanyId, camperoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000017", "Pedro", "Gomez", "pgomez", "chef2@campero.com", "55511118", "CHEF", camperoCompanyId, camperoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000018", "Maria", "Sosa", "msosa", "cashier2@campero.com", "55511119", "CASHIER", camperoCompanyId, camperoBranch2Id, defaultUserPass));

                // ==========================================
                // MC DONALDS
                // ==========================================
                // Admin
                users.Add(CreateUser("e00000000000000000000020", "Ronald", "Mac", "rmac", "admin@mcdonalds.com", "55522221", "COMPANY_ADMIN", mcdoCompanyId, null, defaultAdminPass));
                
                // Branch 1
                users.Add(CreateUser("e00000000000000000000021", "Oscar", "Pinto", "opinto", "manager1@mcdonalds.com", "55522222", "BRANCH_MANAGER", mcdoCompanyId, mcdoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000022", "Elena", "Cruz", "ecruz", "waiter1@mcdonalds.com", "55522223", "WAITER", mcdoCompanyId, mcdoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000023", "Hugo", "Leon", "hleon", "chef1@mcdonalds.com", "55522224", "CHEF", mcdoCompanyId, mcdoBranch1Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000024", "Rosa", "Mendez", "rmendez", "cashier1@mcdonalds.com", "55522225", "CASHIER", mcdoCompanyId, mcdoBranch1Id, defaultUserPass));
                
                // Branch 2
                users.Add(CreateUser("e00000000000000000000025", "Ivan", "Salas", "isalas", "manager2@mcdonalds.com", "55522226", "BRANCH_MANAGER", mcdoCompanyId, mcdoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000026", "Laura", "Vargas", "lvargas", "waiter2@mcdonalds.com", "55522227", "WAITER", mcdoCompanyId, mcdoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000027", "Julio", "Rios", "jrios", "chef2@mcdonalds.com", "55522228", "CHEF", mcdoCompanyId, mcdoBranch2Id, defaultUserPass));
                users.Add(CreateUser("e00000000000000000000028", "Sara", "Vega", "svega", "cashier2@mcdonalds.com", "55522229", "CASHIER", mcdoCompanyId, mcdoBranch2Id, defaultUserPass));

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }

        private static User CreateUser(string mongoId, string name, string surname, string username, string email, string phone, string role, string companyMongoId, string? branchMongoId, string passwordHash)
        {
            return new User
            {
                MongoId = mongoId,
                Name = name,
                Surname = surname,
                Username = username,
                Email = email,
                Phone = phone,
                Role = role,
                CompanyMongoId = companyMongoId,
                BranchMongoId = branchMongoId,
                PasswordHash = passwordHash,
                IsActive = true,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
