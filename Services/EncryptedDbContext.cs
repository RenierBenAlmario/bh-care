using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;
using Barangay.Data;
using Barangay.Extensions;
using Barangay.Services;

namespace Barangay.Services
{
    public class EncryptedDbContext : ApplicationDbContext
    {
        private readonly IDataEncryptionService _encryptionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EncryptedDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IDataEncryptionService encryptionService,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _encryptionService = encryptionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override int SaveChanges()
        {
            EncryptSensitiveData();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EncryptSensitiveData();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void EncryptSensitiveData()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // Special handling for VitalSign entities
                if (entry.Entity is Models.VitalSign vitalSign)
                {
                    Console.WriteLine($"Encrypting VitalSign entity - Temperature: {vitalSign.Temperature}, BloodPressure: {vitalSign.BloodPressure}");
                    vitalSign.EncryptVitalSignData(_encryptionService);
                    Console.WriteLine($"After encryption - EncryptedTemperature: {vitalSign.EncryptedTemperature}, EncryptedBloodPressure: {vitalSign.EncryptedBloodPressure}");
                }
                else
                {
                    // Use the standard encryption for other entities
                    if (entry.Entity is Models.ImmunizationRecord immunizationRecord)
                    {
                        Console.WriteLine($"Encrypting ImmunizationRecord entity - ChildName: {immunizationRecord.ChildName}, DateOfBirth: {immunizationRecord.DateOfBirth}");
                        immunizationRecord.EncryptSensitiveData(_encryptionService);
                        Console.WriteLine($"After encryption - ChildName: {immunizationRecord.ChildName}, DateOfBirth: {immunizationRecord.DateOfBirth}");
                    }
                    else if (entry.Entity is Models.ImmunizationShortcutForm shortcutForm)
                    {
                        Console.WriteLine($"Encrypting ImmunizationShortcutForm entity - ChildName: {shortcutForm.ChildName}, PreferredDate: {shortcutForm.PreferredDate}");
                        shortcutForm.EncryptSensitiveData(_encryptionService);
                        Console.WriteLine($"After encryption - ChildName: {shortcutForm.ChildName}, PreferredDate: {shortcutForm.PreferredDate}");
                    }
                    else if (entry.Entity is Models.HEEADSSSAssessment heeadsss)
                    {
                        Console.WriteLine($"Encrypting HEEADSSS entity - FullName: {heeadsss.FullName}, Gender: {heeadsss.Gender}");
                        heeadsss.EncryptSensitiveData(_encryptionService);
                        Console.WriteLine($"After encryption - FullName: {heeadsss.FullName}, Gender: {heeadsss.Gender}");
                    }
                    else
                    {
                        entry.Entity.EncryptSensitiveData(_encryptionService);
                    }
                }
            }
        }

        public override async ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class
        {
            var entity = await base.FindAsync<TEntity>(keyValues);
            if (entity != null)
            {
                DecryptSensitiveData(entity);
            }
            return entity;
        }

        public override TEntity? Find<TEntity>(params object?[]? keyValues) where TEntity : class
        {
            var entity = base.Find<TEntity>(keyValues);
            if (entity != null)
            {
                DecryptSensitiveData(entity);
            }
            return entity;
        }

        private void DecryptSensitiveData<TEntity>(TEntity entity) where TEntity : class
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && _encryptionService.CanUserDecrypt(user))
            {
                // Special handling for VitalSign entities
                if (entity is Models.VitalSign vitalSign)
                {
                    vitalSign.DecryptVitalSignData(_encryptionService, user);
                }
                else
                {
                    // Use the standard decryption for other entities
                    entity.DecryptSensitiveData(_encryptionService, user);
                }
            }
        }
    }
}
