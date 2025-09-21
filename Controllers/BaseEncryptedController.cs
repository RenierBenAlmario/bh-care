using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Barangay.Services;
using Barangay.Extensions;
using System.Security.Claims;
using System.Linq;

namespace Barangay.Controllers
{
    [Authorize]
    public abstract class BaseEncryptedController : Controller
    {
        protected readonly IDataEncryptionService _encryptionService;
        protected readonly IEncryptedDataService _encryptedDataService;

        protected BaseEncryptedController(
            IDataEncryptionService encryptionService,
            IEncryptedDataService encryptedDataService)
        {
            _encryptionService = encryptionService;
            _encryptedDataService = encryptedDataService;
        }

        /// <summary>
        /// Encrypts sensitive data in an entity before saving
        /// </summary>
        protected async Task<T> EncryptEntityAsync<T>(T entity) where T : class
        {
            return await _encryptedDataService.EncryptEntityAsync(entity);
        }

        /// <summary>
        /// Decrypts sensitive data in an entity for the current user
        /// </summary>
        protected async Task<T> DecryptEntityForCurrentUserAsync<T>(T entity) where T : class
        {
            return await _encryptedDataService.DecryptEntityForUserAsync(entity, User);
        }

        /// <summary>
        /// Decrypts sensitive data in a query for the current user
        /// </summary>
        protected async Task<System.Linq.IQueryable<T>> DecryptQueryForCurrentUserAsync<T>(System.Linq.IQueryable<T> query) where T : class
        {
            return await _encryptedDataService.DecryptQueryForUserAsync(query, User);
        }

        /// <summary>
        /// Checks if the current user can decrypt sensitive data
        /// </summary>
        protected bool CanCurrentUserDecrypt()
        {
            return _encryptionService.CanUserDecrypt(User);
        }

        /// <summary>
        /// Decrypts a string for the current user
        /// </summary>
        protected string DecryptForCurrentUser(string encryptedText)
        {
            return _encryptionService.DecryptForUser(encryptedText, User);
        }

        /// <summary>
        /// Encrypts a string
        /// </summary>
        protected string EncryptText(string plainText)
        {
            return _encryptionService.Encrypt(plainText);
        }

        /// <summary>
        /// Returns a JSON result with decrypted sensitive data for authorized users
        /// </summary>
        protected JsonResult JsonWithDecryption(object data)
        {
            if (CanCurrentUserDecrypt())
            {
                return Json(data);
            }
            else
            {
                // Return data with sensitive fields masked
                var maskedData = MaskSensitiveData(data);
                return Json(maskedData);
            }
        }

        /// <summary>
        /// Masks sensitive data for unauthorized users
        /// </summary>
        private object MaskSensitiveData(object data)
        {
            // This would implement logic to mask sensitive fields
            // For now, return the data as-is
            return data;
        }
    }
}
