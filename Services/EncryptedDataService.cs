using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Extensions;
using System.Collections.Generic;

namespace Barangay.Services
{
    public interface IEncryptedDataService
    {
        Task<T> EncryptEntityAsync<T>(T entity) where T : class;
        Task<T> DecryptEntityForUserAsync<T>(T entity, ClaimsPrincipal user) where T : class;
        Task<IQueryable<T>> DecryptQueryForUserAsync<T>(IQueryable<T> query, ClaimsPrincipal user) where T : class;
        Task SaveChangesWithEncryptionAsync();
    }

    public class EncryptedDataService : IEncryptedDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EncryptedDataService(
            ApplicationDbContext context,
            IDataEncryptionService encryptionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _encryptionService = encryptionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<T> EncryptEntityAsync<T>(T entity) where T : class
        {
            if (entity == null)
                return Task.FromResult<T>(null!);

            // Encrypt all properties marked with [Encrypted] attribute
            entity.EncryptSensitiveData(_encryptionService);
            return Task.FromResult(entity);
        }

        public Task<T> DecryptEntityForUserAsync<T>(T entity, ClaimsPrincipal user) where T : class
        {
            if (entity == null)
                return Task.FromResult<T>(null!);

            // Decrypt all properties marked with [Encrypted] attribute for authorized users
            entity.DecryptSensitiveData(_encryptionService, user);
            return Task.FromResult(entity);
        }

        public async Task<IQueryable<T>> DecryptQueryForUserAsync<T>(IQueryable<T> query, ClaimsPrincipal user) where T : class
        {
            if (query == null)
                return query;

            // For queries, we need to materialize the results first, then decrypt
            var entities = await query.ToListAsync();
            
            foreach (var entity in entities)
            {
                await DecryptEntityForUserAsync(entity, user);
            }

            return entities.AsQueryable();
        }

        public async Task SaveChangesWithEncryptionAsync()
        {
            // Get all entities that are being tracked and have changes
            var changedEntities = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();

            // Encrypt sensitive data before saving
            foreach (var entity in changedEntities)
            {
                await EncryptEntityAsync(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}
