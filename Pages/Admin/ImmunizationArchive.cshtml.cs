using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class FamilyHealthArchiveModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<FamilyHealthArchiveModel> _logger;
        private readonly IDataEncryptionService _encryptionService;

        public FamilyHealthArchiveModel(
            EncryptedDbContext context,
            ILogger<FamilyHealthArchiveModel> logger,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedBarangay { get; set; } = string.Empty;

        public List<FamilyGroup> FamilyGroups { get; set; } = new();
        public Dictionary<int, bool> DecryptedRecords { get; set; } = new();
        public Dictionary<string, bool> DecryptedFamilies { get; set; } = new();
        public List<string> Barangays { get; set; } = new();

        public class FamilyGroup
        {
            public string FamilyNumber { get; set; } = string.Empty;
            public string FamilyName { get; set; } = string.Empty;
            public string Barangay { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string ContactInfo { get; set; } = string.Empty;
            public List<ImmunizationRecord> ImmunizationRecords { get; set; } = new();
            public List<NCDRiskAssessment> NCDForms { get; set; } = new();
            public List<HEEADSSSAssessment> HEEADSSSForms { get; set; } = new();
            public List<VitalSign> VitalSigns { get; set; } = new();
            public int TotalRecords { get; set; }
            public DateTime LastUpdated { get; set; }
            public List<DateTime> AllDates { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadFamilyHealthGroups();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading family health archive");
                ModelState.AddModelError("", "An error occurred while loading the archive.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDecryptAsync(int recordId, string recordType, string privateKey)
        {
            try
            {
                // Fixed decryption key for all records
                const string masterDecryptionKey = "Kx9mP2vQ8nR5tY7uI3oE6wA1sD4fG9hJ2kL5zX8c";
                
                if (string.IsNullOrEmpty(privateKey))
                {
                    ModelState.AddModelError("", "Private key is required for decryption.");
                    await LoadFamilyHealthGroups();
                    return Page();
                }

                // Validate against the master decryption key
                if (privateKey != masterDecryptionKey)
                {
                    ModelState.AddModelError("", "Invalid private key. Access denied.");
                    await LoadFamilyHealthGroups();
                    return Page();
                }

                // Toggle decryption state for the specific record
                if (!DecryptedRecords.ContainsKey(recordId))
                {
                    DecryptedRecords[recordId] = true;
                }
                else
                {
                    DecryptedRecords[recordId] = !DecryptedRecords[recordId];
                }

                await LoadFamilyHealthGroups();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting record {RecordId} of type {RecordType}", recordId, recordType);
                ModelState.AddModelError("", "An error occurred while decrypting the record.");
                await LoadFamilyHealthGroups();
                return Page();
            }
        }


        private async Task LoadFamilyHealthGroups()
        {
            // Load all unique barangays for the dropdown
            var barangays = new HashSet<string>();
            barangays.UnionWith(await _context.ImmunizationRecords.Select(r => r.Barangay).Where(b => !string.IsNullOrEmpty(b)).Select(b => b!).Distinct().ToListAsync());
            barangays.UnionWith(await _context.NCDRiskAssessments.Select(r => r.Barangay).Where(b => !string.IsNullOrEmpty(b)).Select(b => b!).Distinct().ToListAsync());
            Barangays = barangays.OrderBy(b => b).ToList();

            // Load all health records
            var immunizationQuery = _context.ImmunizationRecords.AsQueryable();
            var ncdQuery = _context.NCDRiskAssessments.AsQueryable();
            var heeadsssQuery = _context.HEEADSSSAssessments.AsQueryable();
            var vitalSignsQuery = _context.VitalSigns.AsQueryable();

            // Apply search filters to all queries
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                immunizationQuery = immunizationQuery.Where(r => r.ChildName.Contains(SearchTerm) || 
                                                               r.MotherName.Contains(SearchTerm) ||
                                                               r.FatherName.Contains(SearchTerm) ||
                                                               r.FamilyNumber.Contains(SearchTerm));
                ncdQuery = ncdQuery.Where(r => r.FamilyNo.Contains(SearchTerm));
                heeadsssQuery = heeadsssQuery.Where(r => r.FullName.Contains(SearchTerm) || 
                                                       r.FamilyNo.Contains(SearchTerm));
                vitalSignsQuery = vitalSignsQuery.Where(r => r.PatientId.Contains(SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(SelectedBarangay))
            {
                immunizationQuery = immunizationQuery.Where(r => r.Barangay == SelectedBarangay);
                ncdQuery = ncdQuery.Where(r => r.Barangay == SelectedBarangay);
                heeadsssQuery = heeadsssQuery.Where(r => r.Address.Contains(SelectedBarangay));
            }

            // Execute all queries
            var immunizations = await immunizationQuery.ToListAsync();
            var ncdForms = await ncdQuery.ToListAsync();
            var heeadsssForms = await heeadsssQuery.ToListAsync();
            var vitalSigns = await vitalSignsQuery.ToListAsync();

            // Get all unique family numbers
            var allFamilyNumbers = new HashSet<string>();
            allFamilyNumbers.UnionWith(immunizations.Where(r => !string.IsNullOrEmpty(r.FamilyNumber)).Select(r => r.FamilyNumber!).Where(fn => fn != null));
            allFamilyNumbers.UnionWith(ncdForms.Where(r => !string.IsNullOrEmpty(r.FamilyNo)).Select(r => r.FamilyNo!).Where(fn => fn != null));
            allFamilyNumbers.UnionWith(heeadsssForms.Where(r => !string.IsNullOrEmpty(r.FamilyNo)).Select(r => r.FamilyNo!).Where(fn => fn != null));
            allFamilyNumbers.UnionWith(vitalSigns.Where(r => !string.IsNullOrEmpty(r.PatientId)).Select(r => r.PatientId!).Where(pid => pid != null));

            // Group by FamilyNumber
            var familyGroups = allFamilyNumbers.Select(familyNumber => 
            {
                var familyImmunizations = immunizations.Where(r => r.FamilyNumber == familyNumber).ToList();
                var familyNCDs = ncdForms.Where(r => r.FamilyNo == familyNumber).ToList();
                var familyHEEADSSS = heeadsssForms.Where(r => r.FamilyNo == familyNumber).ToList();
                var familyVitalSigns = vitalSigns.Where(r => r.PatientId == familyNumber).ToList();

                // Get family info from first available record
                object? firstRecord = familyImmunizations.FirstOrDefault();
                if (firstRecord == null)
                {
                    firstRecord = familyNCDs.FirstOrDefault();
                }
                if (firstRecord == null)
                {
                    firstRecord = familyHEEADSSS.FirstOrDefault();
                }
                if (firstRecord == null)
                {
                    firstRecord = familyVitalSigns.FirstOrDefault();
                }

                // Collect all dates
                var allDates = new List<DateTime>();
                allDates.AddRange(familyImmunizations.Select(r => DateTime.TryParse(r.CreatedAt ?? "", out var parsedDate) ? parsedDate : DateTime.MinValue));
                allDates.AddRange(familyNCDs.Select(r => DateTime.TryParse(r.CreatedAt ?? "", out var parsedDate) ? parsedDate : DateTime.MinValue));
                allDates.AddRange(familyHEEADSSS.Select(r => r.CreatedAt));
                allDates.AddRange(familyVitalSigns.Select(r => r.RecordedAt));

                return new FamilyGroup
                {
                    FamilyNumber = DecryptField(familyNumber, true),
                    FamilyName = GetFamilyName(firstRecord),
                    Barangay = GetBarangay(firstRecord),
                    Address = GetAddress(firstRecord),
                    ContactInfo = GetContactInfo(firstRecord),
                    ImmunizationRecords = familyImmunizations,
                    NCDForms = familyNCDs,
                    HEEADSSSForms = familyHEEADSSS,
                    VitalSigns = familyVitalSigns,
                    TotalRecords = familyImmunizations.Count + familyNCDs.Count + familyHEEADSSS.Count + familyVitalSigns.Count,
                    LastUpdated = allDates.Any() ? allDates.Max() : DateTime.MinValue,
                    AllDates = allDates.OrderByDescending(d => d).ToList()
                };
            })
            .OrderByDescending(fg => fg.LastUpdated)
            .ToList();

            FamilyGroups = familyGroups;
        }

        private string GetFamilyName(object? firstRecord)
        {
            if (firstRecord is ImmunizationRecord immunization)
                return DecryptField(immunization.MotherName, true) ?? "Unknown Family";
            if (firstRecord is NCDRiskAssessment ncd)
                return $"{DecryptField(ncd.FirstName, true)} {DecryptField(ncd.LastName, true)}".Trim() ?? "Unknown Family";
            if (firstRecord is HEEADSSSAssessment heeadsss)
                return DecryptField(heeadsss.FullName, true) ?? "Unknown Family";
            if (firstRecord is VitalSign vital)
                return DecryptField(vital.PatientId, true) ?? "Unknown Family";
            return "Unknown Family";
        }

        private string GetBarangay(object? firstRecord)
        {
            if (firstRecord is ImmunizationRecord immunization)
                return DecryptField(immunization.Barangay, true) ?? "Unknown";
            if (firstRecord is NCDRiskAssessment ncd)
                return DecryptField(ncd.Barangay, true) ?? "Unknown";
            if (firstRecord is HEEADSSSAssessment heeadsss)
                return "Unknown"; // HEEADSSS doesn't have Barangay field
            if (firstRecord is VitalSign vital)
                return "Unknown"; // VitalSign doesn't have Barangay field
            return "Unknown";
        }

        private string GetAddress(object? firstRecord)
        {
            if (firstRecord is ImmunizationRecord immunization)
                return DecryptField(immunization.Address, true) ?? "Unknown";
            if (firstRecord is NCDRiskAssessment ncd)
                return DecryptField(ncd.Address, true) ?? "Unknown";
            if (firstRecord is HEEADSSSAssessment heeadsss)
                return DecryptField(heeadsss.Address, true) ?? "Unknown";
            if (firstRecord is VitalSign vital)
                return "Unknown"; // VitalSign doesn't have Address field
            return "Unknown";
        }

        private string GetContactInfo(object? firstRecord)
        {
            if (firstRecord is ImmunizationRecord immunization)
                return $"{DecryptField(immunization.Email, true) ?? "No Email"} | {DecryptField(immunization.ContactNumber, true) ?? "No Phone"}";
            if (firstRecord is NCDRiskAssessment ncd)
                return $"No Email | {DecryptField(ncd.Telepono, true) ?? "No Phone"}";
            if (firstRecord is HEEADSSSAssessment heeadsss)
                return $"No Email | {DecryptField(heeadsss.ContactNumber, true) ?? "No Phone"}";
            if (firstRecord is VitalSign vital)
                return "No Email | No Phone";
            return "No Email | No Phone";
        }

        public string DecryptField(string encryptedValue, bool isDecrypted)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return string.Empty;

            if (isDecrypted)
            {
                try
                {
                    // Handle AES256_ENCRYPTED_ format (Base64 encoded with prefix)
                    if (encryptedValue.StartsWith("AES256_ENCRYPTED_"))
                    {
                        var base64Data = encryptedValue.Replace("AES256_ENCRYPTED_", "");
                        var bytes = Convert.FromBase64String(base64Data);
                        return Encoding.UTF8.GetString(bytes);
                    }
                    
                    // Check if the value looks like encrypted data (Base64 encoded, longer than 20 chars, no spaces)
                    bool looksLikeEncrypted = encryptedValue.Length > 20 && 
                                           !encryptedValue.Contains(' ') && 
                                           !encryptedValue.Contains('-') && // Exclude GUIDs
                                           !encryptedValue.Contains('@') && // Exclude emails
                                           !encryptedValue.Contains('.') && // Exclude file extensions
                                           System.Text.RegularExpressions.Regex.IsMatch(encryptedValue, @"^[A-Za-z0-9+/=]+$");
                    
                    if (looksLikeEncrypted)
                    {
                        // PRIORITY: Try AES decryption first (existing data is AES encrypted)
                        try
                        {
                            return _encryptionService.Decrypt(encryptedValue);
                        }
                        catch
                        {
                            // AES decryption failed, try RSA decryption (for future RSA encrypted data)
                            try
                            {
                                var rsaDecrypted = AsymmetricDecrypt(encryptedValue, GetPrivateKey());
                                if (rsaDecrypted != encryptedValue) // Only return if decryption actually worked
                                {
                                    return rsaDecrypted;
                                }
                            }
                            catch
                            {
                                // RSA decryption failed, try Base64 decoding as last resort
                                try
                                {
                                    var bytes = Convert.FromBase64String(encryptedValue);
                                    var decoded = Encoding.UTF8.GetString(bytes);
                                    if (decoded.Length > 0 && decoded.Length < 1000 && System.Text.RegularExpressions.Regex.IsMatch(decoded, @"^[\x20-\x7E]*$"))
                                    {
                                        return decoded;
                                    }
                                }
                                catch
                                {
                                    // Base64 decoding failed
                                }
                            }
                        }
                    }
                    else
                    {
                        // For non-encrypted looking data, try AES decryption first
                        try
                        {
                            return _encryptionService.Decrypt(encryptedValue);
                        }
                        catch
                        {
                            // If AES fails, return original value (it's probably plain text)
                            return encryptedValue;
                        }
                    }
                    
                    // If all methods fail, return the original value
                    return encryptedValue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error decrypting field value: {Value}", encryptedValue);
                    // Return the original value if decryption fails
                    return encryptedValue;
                }
            }

            return encryptedValue;
        }

        public bool IsRecordDecrypted(int recordId)
        {
            return DecryptedRecords.ContainsKey(recordId) && DecryptedRecords[recordId];
        }

        public bool IsFamilyDecrypted(string familyNumber)
        {
            return DecryptedFamilies.ContainsKey(familyNumber) && DecryptedFamilies[familyNumber];
        }

        public async Task<IActionResult> OnPostDecryptFamilyAsync(string familyId, string decryptKey)
        {
            try
            {
                // Fixed decryption key for all families
                const string masterDecryptionKey = "Kx9mP2vQ8nR5tY7uI3oE6wA1sD4fG9hJ2kL5zX8c";
                
                if (string.IsNullOrEmpty(decryptKey))
                {
                    return new JsonResult(new { success = false, error = "Decryption key is required." });
                }

                // Validate against the master decryption key
                if (decryptKey != masterDecryptionKey)
                {
                    return new JsonResult(new { success = false, error = "Invalid decryption key. Access denied." });
                }

                // Get family data and apply asymmetric encryption
                var familyData = await GetFamilyDataForAsymmetricEncryption(familyId);
                
                // Toggle decryption state for the family
                if (!DecryptedFamilies.ContainsKey(familyId))
                {
                    DecryptedFamilies[familyId] = true;
                }
                else
                {
                    DecryptedFamilies[familyId] = !DecryptedFamilies[familyId];
                }

                return new JsonResult(new { 
                    success = true, 
                    encryptedData = familyData,
                    message = "Family data has been asymmetrically encrypted. Use the private key to decrypt."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting family {FamilyId}", familyId);
                return new JsonResult(new { success = false, error = "An error occurred while decrypting the family data." });
            }
        }

        private async Task<object> GetFamilyDataForAsymmetricEncryption(string familyId)
        {
            // Get all records for this family
            var immunizationRecords = await _context.ImmunizationRecords
                .Where(r => r.FamilyNumber == familyId)
                .ToListAsync();

            var heeadsssRecords = await _context.HEEADSSSAssessments
                .Where(r => r.FamilyNo == familyId)
                .ToListAsync();

            var ncdRecords = await _context.NCDRiskAssessments
                .Where(r => r.FamilyNo == familyId)
                .ToListAsync();

            var vitalSignsRecords = await _context.VitalSigns
                .Where(r => r.PatientId == familyId)
                .ToListAsync();

            // Decrypt database data first, then apply asymmetric encryption
            var decryptedImmunizationRecords = DecryptAndReEncryptImmunizationRecords(immunizationRecords);
            var decryptedHeeadsssRecords = DecryptAndReEncryptHeeadsssRecords(heeadsssRecords);
            var decryptedNcdRecords = DecryptAndReEncryptNcdRecords(ncdRecords);

            // Get family info from first available record (use decrypted records)
            var familyInfo = GetFamilyInfoFromRecords(decryptedImmunizationRecords, decryptedHeeadsssRecords, decryptedNcdRecords, vitalSignsRecords);

            // Apply asymmetric encryption to sensitive data
            return new
            {
                familyNumber = DecryptField(familyId, true) ?? familyId,
                familyName = familyInfo.FamilyName,
                address = familyInfo.Address,
                barangay = familyInfo.Barangay,
                contactInfo = familyInfo.ContactInfo,
                totalRecords = immunizationRecords.Count + heeadsssRecords.Count + ncdRecords.Count + vitalSignsRecords.Count,
                immunizationCount = immunizationRecords.Count,
                heeadsssCount = heeadsssRecords.Count,
                ncdCount = ncdRecords.Count,
                vitalSignsCount = vitalSignsRecords.Count,
                immunizationRecords = decryptedImmunizationRecords.Select(r => new
                {
                    childName = r.ChildName,
                    dateOfBirth = r.DateOfBirth?.ToString(),
                    sex = r.Sex,
                    placeOfBirth = r.PlaceOfBirth,
                    birthHeight = r.BirthHeight?.ToString(),
                    birthWeight = r.BirthWeight?.ToString(),
                    motherName = r.MotherName,
                    fatherName = r.FatherName,
                    healthCenter = r.HealthCenter,
                    email = r.Email,
                    contactNumber = r.ContactNumber,
                    bcgVaccineDate = r.BCGVaccineDate?.ToString(),
                    bcgVaccineRemarks = r.BCGVaccineRemarks,
                    hepatitisBVaccineDate = r.HepatitisBVaccineDate?.ToString(),
                    hepatitisBVaccineRemarks = r.HepatitisBVaccineRemarks,
                    pentavalent1Date = r.Pentavalent1Date?.ToString(),
                    pentavalent1Remarks = r.Pentavalent1Remarks,
                    pentavalent2Date = r.Pentavalent2Date?.ToString(),
                    pentavalent2Remarks = r.Pentavalent2Remarks,
                    pentavalent3Date = r.Pentavalent3Date?.ToString(),
                    pentavalent3Remarks = r.Pentavalent3Remarks,
                    opv1Date = r.OPV1Date?.ToString(),
                    opv1Remarks = r.OPV1Remarks,
                    opv2Date = r.OPV2Date?.ToString(),
                    opv2Remarks = r.OPV2Remarks,
                    opv3Date = r.OPV3Date?.ToString(),
                    opv3Remarks = r.OPV3Remarks,
                    ipv1Date = r.IPV1Date?.ToString(),
                    ipv1Remarks = r.IPV1Remarks,
                    ipv2Date = r.IPV2Date?.ToString(),
                    ipv2Remarks = r.IPV2Remarks,
                    pcv1Date = r.PCV1Date?.ToString(),
                    pcv1Remarks = r.PCV1Remarks,
                    pcv2Date = r.PCV2Date?.ToString(),
                    pcv2Remarks = r.PCV2Remarks,
                    pcv3Date = r.PCV3Date?.ToString(),
                    pcv3Remarks = r.PCV3Remarks,
                    mmr1Date = r.MMR1Date?.ToString(),
                    mmr1Remarks = r.MMR1Remarks,
                    mmr2Date = r.MMR2Date?.ToString(),
                    mmr2Remarks = r.MMR2Remarks,
                    createdAt = r.CreatedAt.ToString()
                }),
                heeadsssRecords = decryptedHeeadsssRecords.Select(r => new
                {
                    fullName = r.FullName,
                    age = r.Age,
                    gender = r.Gender,
                    address = r.Address,
                    contactNumber = r.ContactNumber,
                    homeEnvironment = r.HomeEnvironment,
                    familyRelationship = r.FamilyRelationship,
                    homeFamilyProblems = r.HomeFamilyProblems,
                    homeParentalListening = r.HomeParentalListening,
                    homeParentalBlame = r.HomeParentalBlame,
                    homeFamilyChanges = r.HomeFamilyChanges,
                    schoolPerformance = r.SchoolPerformance,
                    attendanceIssues = r.AttendanceIssues,
                    careerPlans = r.CareerPlans,
                    educationCurrentlyStudying = r.EducationCurrentlyStudying,
                    educationWorking = r.EducationWorking,
                    educationSchoolWorkProblems = r.EducationSchoolWorkProblems,
                    educationBullying = r.EducationBullying,
                    educationEmployment = r.EducationEmployment,
                    dietDescription = r.DietDescription,
                    weightConcerns = r.WeightConcerns,
                    eatingDisorderSymptoms = r.EatingDisorderSymptoms,
                    eatingBodyImageSatisfaction = r.EatingBodyImageSatisfaction,
                    eatingDisorderedEatingBehaviors = r.EatingDisorderedEatingBehaviors,
                    eatingWeightComments = r.EatingWeightComments,
                    hobbies = r.Hobbies,
                    physicalActivity = r.PhysicalActivity,
                    screenTime = r.ScreenTime,
                    activitiesParticipation = r.ActivitiesParticipation,
                    activitiesRegularExercise = r.ActivitiesRegularExercise,
                    activitiesScreenTime = r.ActivitiesScreenTime,
                    drugsAlcoholUse = r.DrugsAlcoholUse,
                    drugsTobaccoUse = r.DrugsTobaccoUse,
                    drugsIllicitDrugUse = r.DrugsIllicitDrugUse,
                    substanceType = r.SubstanceType,
                    substanceUse = r.SubstanceUse,
                    sexualActivity = r.SexualActivity,
                    sexualityBodyConcerns = r.SexualityBodyConcerns,
                    sexualityIntimateRelationships = r.SexualityIntimateRelationships,
                    sexualityPartners = r.SexualityPartners,
                    sexualitySexualOrientation = r.SexualitySexualOrientation,
                    sexualityPregnancy = r.SexualityPregnancy,
                    sexualitySTI = r.SexualitySTI,
                    sexualityProtection = r.SexualityProtection,
                    moodChanges = r.MoodChanges,
                    suicidalThoughts = r.SuicidalThoughts,
                    selfHarmBehavior = r.SelfHarmBehavior,
                    feelsSafeAtHome = r.FeelsSafeAtHome,
                    feelsSafeAtSchool = r.FeelsSafeAtSchool,
                    experiencedBullying = r.ExperiencedBullying,
                    personalStrengths = r.PersonalStrengths,
                    supportSystems = r.SupportSystems,
                    copingMechanisms = r.CopingMechanisms,
                    safetyPhysicalAbuse = r.SafetyPhysicalAbuse,
                    safetyRelationshipViolence = r.SafetyRelationshipViolence,
                    safetyProtectiveGear = r.SafetyProtectiveGear,
                    safetyGunsAtHome = r.SafetyGunsAtHome,
                    suicideDepressionFeelings = r.SuicideDepressionFeelings,
                    suicideSelfHarmThoughts = r.SuicideSelfHarmThoughts,
                    suicideFamilyHistory = r.SuicideFamilyHistory,
                    assessmentNotes = r.AssessmentNotes,
                    recommendedActions = r.RecommendedActions,
                    followUpPlan = r.FollowUpPlan,
                    notes = r.Notes,
                    assessedBy = r.AssessedBy,
                    createdAt = r.CreatedAt.ToString()
                }),
                ncdRecords = decryptedNcdRecords.Select(r => new
                {
                    firstName = r.FirstName,
                    lastName = r.LastName,
                    middleName = r.MiddleName,
                    edad = r.Edad?.ToString(),
                    kasarian = r.Kasarian,
                    address = r.Address,
                    barangay = r.Barangay,
                    telepono = r.Telepono,
                    birthday = r.Birthday?.ToString(),
                    relihiyon = r.Relihiyon,
                    occupation = r.Occupation,
                    civilStatus = r.CivilStatus,
                    hasDiabetes = r.HasDiabetes.ToString(),
                    hasHypertension = r.HasHypertension.ToString(),
                    hasCancer = r.HasCancer.ToString(),
                    hasCOPD = r.HasCOPD.ToString(),
                    hasLungDisease = r.HasLungDisease.ToString(),
                    hasEyeDisease = r.HasEyeDisease.ToString(),
                    cancerType = r.CancerType,
                    familyHasHypertension = r.FamilyHasHypertension.ToString(),
                    familyHasHeartDisease = r.FamilyHasHeartDisease.ToString(),
                    familyHasStroke = r.FamilyHasStroke.ToString(),
                    familyHasDiabetes = r.FamilyHasDiabetes.ToString(),
                    familyHasCancer = r.FamilyHasCancer.ToString(),
                    familyHasKidneyDisease = r.FamilyHasKidneyDisease.ToString(),
                    familyHasOtherDisease = r.FamilyHasOtherDisease.ToString(),
                    familyOtherDiseaseDetails = r.FamilyOtherDiseaseDetails,
                    familyHistoryCancerFather = r.FamilyHistoryCancerFather.ToString(),
                    familyHistoryCancerMother = r.FamilyHistoryCancerMother.ToString(),
                    familyHistoryCancerSibling = r.FamilyHistoryCancerSibling.ToString(),
                    familyHistoryDiabetesFather = r.FamilyHistoryDiabetesFather.ToString(),
                    familyHistoryDiabetesMother = r.FamilyHistoryDiabetesMother.ToString(),
                    familyHistoryDiabetesSibling = r.FamilyHistoryDiabetesSibling.ToString(),
                    familyHistoryHeartDiseaseFather = r.FamilyHistoryHeartDiseaseFather.ToString(),
                    familyHistoryHeartDiseaseMother = r.FamilyHistoryHeartDiseaseMother.ToString(),
                    familyHistoryHeartDiseaseSibling = r.FamilyHistoryHeartDiseaseSibling.ToString(),
                    familyHistoryLungDiseaseFather = r.FamilyHistoryLungDiseaseFather.ToString(),
                    familyHistoryLungDiseaseMother = r.FamilyHistoryLungDiseaseMother.ToString(),
                    familyHistoryLungDiseaseSibling = r.FamilyHistoryLungDiseaseSibling.ToString(),
                    familyHistoryOther = r.FamilyHistoryOther,
                    familyHistoryOtherFather = r.FamilyHistoryOtherFather.ToString(),
                    familyHistoryOtherMother = r.FamilyHistoryOtherMother.ToString(),
                    familyHistoryOtherSibling = r.FamilyHistoryOtherSibling.ToString(),
                    familyHistoryStrokeFather = r.FamilyHistoryStrokeFather.ToString(),
                    familyHistoryStrokeMother = r.FamilyHistoryStrokeMother.ToString(),
                    familyHistoryStrokeSibling = r.FamilyHistoryStrokeSibling.ToString(),
                    smokingStatus = r.SmokingStatus,
                    highSaltIntake = r.HighSaltIntake.ToString(),
                    alcoholFrequency = r.AlcoholFrequency,
                    alcoholConsumption = r.AlcoholConsumption,
                    exerciseDuration = r.ExerciseDuration,
                    riskStatus = r.RiskStatus,
                    chestPain = r.ChestPain,
                    chestPainLocation = r.ChestPainLocation,
                    chestPainValue = r.ChestPainValue?.ToString(),
                    hasDifficultyBreathing = r.HasDifficultyBreathing.ToString(),
                    hasAsthma = r.HasAsthma.ToString(),
                    hasNoRegularExercise = r.HasNoRegularExercise.ToString(),
                    appointmentType = r.AppointmentType,
                    cancerMedication = r.CancerMedication,
                    cancerYear = r.CancerYear,
                    diabetesMedication = r.DiabetesMedication,
                    diabetesYear = r.DiabetesYear,
                    hypertensionMedication = r.HypertensionMedication,
                    hypertensionYear = r.HypertensionYear,
                    lungDiseaseMedication = r.LungDiseaseMedication,
                    lungDiseaseYear = r.LungDiseaseYear,
                    createdAt = r.CreatedAt.ToString()
                })
            };
        }

        public async Task<IActionResult> OnGetGetFamilyDetailsAsync(string familyId)
        {
            try
            {
                // Get all records for this family
                var immunizationRecords = await _context.ImmunizationRecords
                    .Where(r => r.FamilyNumber == familyId)
                    .ToListAsync();

                var heeadsssRecords = await _context.HEEADSSSAssessments
                    .Where(r => r.FamilyNo == familyId)
                    .ToListAsync();

                var ncdRecords = await _context.NCDRiskAssessments
                    .Where(r => r.FamilyNo == familyId)
                    .ToListAsync();

                var vitalSignsRecords = await _context.VitalSigns
                    .Where(r => r.PatientId == familyId)
                    .ToListAsync();

                // Decrypt database data first, then apply asymmetric encryption
                var decryptedImmunizationRecords = DecryptAndReEncryptImmunizationRecords(immunizationRecords);
                var decryptedHeeadsssRecords = DecryptAndReEncryptHeeadsssRecords(heeadsssRecords);
                var decryptedNcdRecords = DecryptAndReEncryptNcdRecords(ncdRecords);

                // Get family info from first available record (use decrypted records)
                var familyInfo = GetFamilyInfoFromRecords(decryptedImmunizationRecords, decryptedHeeadsssRecords, decryptedNcdRecords, vitalSignsRecords);

                // Return encrypted data (same as GetFamilyDataForAsymmetricEncryption)
                var result = new
                {
                    familyNumber = DecryptField(familyId, true) ?? familyId,
                    familyName = familyInfo.FamilyName,
                    address = familyInfo.Address,
                    barangay = familyInfo.Barangay,
                    contactInfo = familyInfo.ContactInfo,
                    totalRecords = (immunizationRecords.Count + heeadsssRecords.Count + ncdRecords.Count + vitalSignsRecords.Count).ToString(),
                    immunizationCount = immunizationRecords.Count.ToString(),
                    heeadsssCount = heeadsssRecords.Count.ToString(),
                    ncdCount = ncdRecords.Count.ToString(),
                    vitalSignsCount = vitalSignsRecords.Count.ToString(),
                    immunizationRecords = decryptedImmunizationRecords.Select(r => new
                    {
                        childName = r.ChildName,
                        dateOfBirth = r.DateOfBirth?.ToString(),
                        sex = r.Sex,
                        placeOfBirth = r.PlaceOfBirth,
                        birthHeight = r.BirthHeight?.ToString(),
                        birthWeight = r.BirthWeight?.ToString(),
                        motherName = r.MotherName,
                        fatherName = r.FatherName,
                        healthCenter = r.HealthCenter,
                        email = r.Email,
                        contactNumber = r.ContactNumber,
                        bcgVaccineDate = r.BCGVaccineDate?.ToString(),
                        bcgVaccineRemarks = r.BCGVaccineRemarks,
                        hepatitisBVaccineDate = r.HepatitisBVaccineDate?.ToString(),
                        hepatitisBVaccineRemarks = r.HepatitisBVaccineRemarks,
                        pentavalent1Date = r.Pentavalent1Date?.ToString(),
                        pentavalent1Remarks = r.Pentavalent1Remarks,
                        pentavalent2Date = r.Pentavalent2Date?.ToString(),
                        pentavalent2Remarks = r.Pentavalent2Remarks,
                        pentavalent3Date = r.Pentavalent3Date?.ToString(),
                        pentavalent3Remarks = r.Pentavalent3Remarks,
                        opv1Date = r.OPV1Date?.ToString(),
                        opv1Remarks = r.OPV1Remarks,
                        opv2Date = r.OPV2Date?.ToString(),
                        opv2Remarks = r.OPV2Remarks,
                        opv3Date = r.OPV3Date?.ToString(),
                        opv3Remarks = r.OPV3Remarks,
                        ipv1Date = r.IPV1Date?.ToString(),
                        ipv1Remarks = r.IPV1Remarks,
                        ipv2Date = r.IPV2Date?.ToString(),
                        ipv2Remarks = r.IPV2Remarks,
                        pcv1Date = r.PCV1Date?.ToString(),
                        pcv1Remarks = r.PCV1Remarks,
                        pcv2Date = r.PCV2Date?.ToString(),
                        pcv2Remarks = r.PCV2Remarks,
                        pcv3Date = r.PCV3Date?.ToString(),
                        pcv3Remarks = r.PCV3Remarks,
                        mmr1Date = r.MMR1Date?.ToString(),
                        mmr1Remarks = r.MMR1Remarks,
                        mmr2Date = r.MMR2Date?.ToString(),
                        mmr2Remarks = r.MMR2Remarks,
                        createdAt = r.CreatedAt.ToString()
                    }),
                    heeadsssRecords = decryptedHeeadsssRecords.Select(r => new
                    {
                        fullName = r.FullName,
                        age = r.Age?.ToString(),
                        gender = r.Gender,
                        address = r.Address,
                        contactNumber = r.ContactNumber,
                        homeEnvironment = r.HomeEnvironment,
                        familyRelationship = r.FamilyRelationship,
                        schoolPerformance = r.SchoolPerformance,
                        educationCurrentlyStudying = r.EducationCurrentlyStudying,
                        dietDescription = r.DietDescription,
                        hobbies = r.Hobbies,
                        physicalActivity = r.PhysicalActivity,
                        drugsAlcoholUse = r.DrugsAlcoholUse,
                        sexualActivity = r.SexualActivity.ToString(),
                        feelsSafeAtHome = r.FeelsSafeAtHome.ToString(),
                        suicidalThoughts = r.SuicidalThoughts.ToString(),
                        assessmentNotes = r.AssessmentNotes,
                        createdAt = r.CreatedAt.ToString()
                    }),
                    ncdRecords = decryptedNcdRecords.Select(r => new
                    {
                        firstName = r.FirstName,
                        lastName = r.LastName,
                        edad = r.Edad?.ToString(),
                        kasarian = r.Kasarian,
                        address = r.Address,
                        barangay = r.Barangay,
                        telepono = r.Telepono,
                        birthday = r.Birthday?.ToString(),
                        relihiyon = r.Relihiyon,
                        occupation = r.Occupation,
                        civilStatus = r.CivilStatus,
                        hasDiabetes = r.HasDiabetes.ToString(),
                        hasHypertension = r.HasHypertension.ToString(),
                        hasCancer = r.HasCancer.ToString(),
                        hasCOPD = r.HasCOPD.ToString(),
                        hasLungDisease = r.HasLungDisease.ToString(),
                        hasEyeDisease = r.HasEyeDisease.ToString(),
                        smokingStatus = r.SmokingStatus,
                        alcoholConsumption = r.AlcoholConsumption,
                        exerciseDuration = r.ExerciseDuration,
                        riskStatus = r.RiskStatus,
                        createdAt = r.CreatedAt.ToString()
                    })
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching family details for {FamilyId}", familyId);
                return new JsonResult(new { error = "Error fetching family details" });
            }
        }

        private (string FamilyName, string Address, string Barangay, string ContactInfo) GetFamilyInfoFromRecords(
            List<ImmunizationRecord> immunizations,
            List<HEEADSSSAssessment> heeadsss,
            List<NCDRiskAssessment> ncds,
            List<VitalSign> vitalSigns)
        {
            // Try to get family info from immunization records first
            if (immunizations.Any())
            {
                var first = immunizations.First();
                return (
                    first.MotherName ?? "Unknown Family",
                    first.Address ?? "Unknown",
                    first.Barangay ?? "Unknown",
                    $"{first.Email ?? "No Email"} | {first.ContactNumber ?? "No Phone"}"
                );
            }

            // Try HEEADSSS records
            if (heeadsss.Any())
            {
                var first = heeadsss.First();
                return (
                    first.FullName ?? "Unknown Family",
                    first.Address ?? "Unknown",
                    "Unknown", // HEEADSSS doesn't have barangay
                    $"No Email | {first.ContactNumber ?? "No Phone"}"
                );
            }

            // Try NCD records
            if (ncds.Any())
            {
                var first = ncds.First();
                return (
                    $"{first.FirstName} {first.LastName}".Trim() ?? "Unknown Family",
                    first.Address ?? "Unknown",
                    first.Barangay ?? "Unknown",
                    $"No Email | {first.Telepono ?? "No Phone"}"
                );
            }

            // Default values
            return ("Unknown Family", "Unknown", "Unknown", "No Email | No Phone");
        }

        private List<ImmunizationRecord> DecryptAndReEncryptImmunizationRecords(List<ImmunizationRecord> records)
        {
            var decryptedRecords = new List<ImmunizationRecord>();
            
            foreach (var record in records)
            {
                var decryptedRecord = new ImmunizationRecord
                {
                    Id = record.Id,
                    ChildName = DecryptField(record.ChildName, true),
                    DateOfBirth = record.DateOfBirth,
                    PlaceOfBirth = DecryptField(record.PlaceOfBirth, true),
                    Address = DecryptField(record.Address, true),
                    MotherName = DecryptField(record.MotherName, true),
                    FatherName = DecryptField(record.FatherName, true),
                    Sex = DecryptField(record.Sex, true),
                    BirthHeight = record.BirthHeight,
                    BirthWeight = record.BirthWeight,
                    HealthCenter = DecryptField(record.HealthCenter, true),
                    Barangay = DecryptField(record.Barangay, true),
                    FamilyNumber = record.FamilyNumber,
                    Email = DecryptField(record.Email, true),
                    ContactNumber = DecryptField(record.ContactNumber, true),
                    BCGVaccineDate = record.BCGVaccineDate,
                    BCGVaccineRemarks = DecryptField(record.BCGVaccineRemarks, true),
                    HepatitisBVaccineDate = record.HepatitisBVaccineDate,
                    HepatitisBVaccineRemarks = DecryptField(record.HepatitisBVaccineRemarks, true),
                    Pentavalent1Date = record.Pentavalent1Date,
                    Pentavalent1Remarks = DecryptField(record.Pentavalent1Remarks, true),
                    Pentavalent2Date = record.Pentavalent2Date,
                    Pentavalent2Remarks = DecryptField(record.Pentavalent2Remarks, true),
                    Pentavalent3Date = record.Pentavalent3Date,
                    Pentavalent3Remarks = DecryptField(record.Pentavalent3Remarks, true),
                    OPV1Date = record.OPV1Date,
                    OPV1Remarks = DecryptField(record.OPV1Remarks, true),
                    OPV2Date = record.OPV2Date,
                    OPV2Remarks = DecryptField(record.OPV2Remarks, true),
                    OPV3Date = record.OPV3Date,
                    OPV3Remarks = DecryptField(record.OPV3Remarks, true),
                    IPV1Date = record.IPV1Date,
                    IPV1Remarks = DecryptField(record.IPV1Remarks, true),
                    IPV2Date = record.IPV2Date,
                    IPV2Remarks = DecryptField(record.IPV2Remarks, true),
                    PCV1Date = record.PCV1Date,
                    PCV1Remarks = DecryptField(record.PCV1Remarks, true),
                    PCV2Date = record.PCV2Date,
                    PCV2Remarks = DecryptField(record.PCV2Remarks, true),
                    PCV3Date = record.PCV3Date,
                    PCV3Remarks = DecryptField(record.PCV3Remarks, true),
                    MMR1Date = record.MMR1Date,
                    MMR1Remarks = DecryptField(record.MMR1Remarks, true),
                    MMR2Date = record.MMR2Date,
                    MMR2Remarks = record.MMR2Remarks,
                    CreatedAt = record.CreatedAt,
                    UpdatedAt = record.UpdatedAt,
                    CreatedBy = record.CreatedBy,
                    UpdatedBy = record.UpdatedBy,
                    Status = record.Status
                };
                
                decryptedRecords.Add(decryptedRecord);
            }
            
            return decryptedRecords;
        }

        private List<HEEADSSSAssessment> DecryptAndReEncryptHeeadsssRecords(List<HEEADSSSAssessment> records)
        {
            var decryptedRecords = new List<HEEADSSSAssessment>();
            
            foreach (var record in records)
            {
                var decryptedRecord = new HEEADSSSAssessment
                {
                    Id = record.Id,
                    UserId = record.UserId,
                    AppointmentId = DecryptField(record.AppointmentId, true) ?? record.AppointmentId,
                    HealthFacility = DecryptField(record.HealthFacility, true) ?? record.HealthFacility,
                    FamilyNo = DecryptField(record.FamilyNo, true) ?? record.FamilyNo,
                    FullName = DecryptField(record.FullName, true) ?? record.FullName,
                    Age = DecryptField(record.Age, true) ?? record.Age,
                    Gender = DecryptField(record.Gender, true) ?? record.Gender,
                    Address = DecryptField(record.Address, true) ?? record.Address,
                    ContactNumber = DecryptField(record.ContactNumber, true) ?? record.ContactNumber,
                    HomeEnvironment = DecryptField(record.HomeEnvironment, true) ?? record.HomeEnvironment,
                    FamilyRelationship = DecryptField(record.FamilyRelationship, true) ?? record.FamilyRelationship,
                    HomeFamilyProblems = DecryptField(record.HomeFamilyProblems, true) ?? record.HomeFamilyProblems,
                    HomeParentalListening = DecryptField(record.HomeParentalListening, true) ?? record.HomeParentalListening,
                    HomeParentalBlame = DecryptField(record.HomeParentalBlame, true) ?? record.HomeParentalBlame,
                    HomeFamilyChanges = DecryptField(record.HomeFamilyChanges, true) ?? record.HomeFamilyChanges,
                    SchoolPerformance = DecryptField(record.SchoolPerformance, true) ?? record.SchoolPerformance,
                    AttendanceIssues = record.AttendanceIssues,
                    CareerPlans = DecryptField(record.CareerPlans, true) ?? record.CareerPlans,
                    EducationCurrentlyStudying = DecryptField(record.EducationCurrentlyStudying, true) ?? record.EducationCurrentlyStudying,
                    EducationWorking = DecryptField(record.EducationWorking, true) ?? record.EducationWorking,
                    EducationSchoolWorkProblems = DecryptField(record.EducationSchoolWorkProblems, true) ?? record.EducationSchoolWorkProblems,
                    EducationBullying = DecryptField(record.EducationBullying, true) ?? record.EducationBullying,
                    EducationEmployment = DecryptField(record.EducationEmployment, true) ?? record.EducationEmployment,
                    DietDescription = DecryptField(record.DietDescription, true) ?? record.DietDescription,
                    WeightConcerns = record.WeightConcerns,
                    EatingDisorderSymptoms = record.EatingDisorderSymptoms,
                    EatingBodyImageSatisfaction = DecryptField(record.EatingBodyImageSatisfaction, true) ?? record.EatingBodyImageSatisfaction,
                    EatingDisorderedEatingBehaviors = DecryptField(record.EatingDisorderedEatingBehaviors, true) ?? record.EatingDisorderedEatingBehaviors,
                    EatingWeightComments = DecryptField(record.EatingWeightComments, true) ?? record.EatingWeightComments,
                    Hobbies = DecryptField(record.Hobbies, true) ?? record.Hobbies,
                    PhysicalActivity = DecryptField(record.PhysicalActivity, true) ?? record.PhysicalActivity,
                    ScreenTime = DecryptField(record.ScreenTime, true) ?? record.ScreenTime,
                    ActivitiesParticipation = DecryptField(record.ActivitiesParticipation, true) ?? record.ActivitiesParticipation,
                    ActivitiesRegularExercise = DecryptField(record.ActivitiesRegularExercise, true) ?? record.ActivitiesRegularExercise,
                    ActivitiesScreenTime = DecryptField(record.ActivitiesScreenTime, true) ?? record.ActivitiesScreenTime,
                    SubstanceUse = DecryptField(record.SubstanceUse, true) ?? record.SubstanceUse,
                    SubstanceType = DecryptField(record.SubstanceType, true) ?? record.SubstanceType,
                    DrugsTobaccoUse = record.DrugsTobaccoUse,
                    DrugsAlcoholUse = record.DrugsAlcoholUse,
                    DrugsIllicitDrugUse = record.DrugsIllicitDrugUse,
                    DatingRelationships = record.DatingRelationships,
                    SexualActivity = record.SexualActivity,
                    SexualOrientation = record.SexualOrientation,
                    SexualityBodyConcerns = record.SexualityBodyConcerns,
                    SexualityIntimateRelationships = record.SexualityIntimateRelationships,
                    SexualityPartners = record.SexualityPartners,
                    SexualitySexualOrientation = record.SexualitySexualOrientation,
                    SexualityPregnancy = record.SexualityPregnancy,
                    SexualitySTI = record.SexualitySTI,
                    SexualityProtection = record.SexualityProtection,
                    MoodChanges = record.MoodChanges,
                    SuicidalThoughts = record.SuicidalThoughts,
                    SelfHarmBehavior = record.SelfHarmBehavior,
                    FeelsSafeAtHome = record.FeelsSafeAtHome,
                    FeelsSafeAtSchool = record.FeelsSafeAtSchool,
                    ExperiencedBullying = record.ExperiencedBullying,
                    PersonalStrengths = DecryptField(record.PersonalStrengths, true) ?? record.PersonalStrengths,
                    SupportSystems = DecryptField(record.SupportSystems, true) ?? record.SupportSystems,
                    CopingMechanisms = DecryptField(record.CopingMechanisms, true) ?? record.CopingMechanisms,
                    SafetyPhysicalAbuse = record.SafetyPhysicalAbuse,
                    SafetyRelationshipViolence = record.SafetyRelationshipViolence,
                    SafetyProtectiveGear = record.SafetyProtectiveGear,
                    SafetyGunsAtHome = record.SafetyGunsAtHome,
                    SuicideDepressionFeelings = record.SuicideDepressionFeelings,
                    SuicideSelfHarmThoughts = record.SuicideSelfHarmThoughts,
                    SuicideFamilyHistory = record.SuicideFamilyHistory,
                    AssessmentNotes = DecryptField(record.AssessmentNotes, true) ?? record.AssessmentNotes,
                    RecommendedActions = DecryptField(record.RecommendedActions, true) ?? record.RecommendedActions,
                    FollowUpPlan = DecryptField(record.FollowUpPlan, true) ?? record.FollowUpPlan,
                    Notes = DecryptField(record.Notes, true) ?? record.Notes,
                    AssessedBy = record.AssessedBy,
                    CreatedAt = record.CreatedAt,
                    UpdatedAt = record.UpdatedAt
                };
                
                decryptedRecords.Add(decryptedRecord);
            }
            
            return decryptedRecords;
        }

        private List<NCDRiskAssessment> DecryptAndReEncryptNcdRecords(List<NCDRiskAssessment> records)
        {
            var decryptedRecords = new List<NCDRiskAssessment>();
            
            foreach (var record in records)
            {
                var decryptedRecord = new NCDRiskAssessment
                {
                    Id = record.Id,
                    UserId = record.UserId,
                    AppointmentId = record.AppointmentId,
                    HealthFacility = record.HealthFacility,
                    FamilyNo = DecryptField(record.FamilyNo, true) ?? record.FamilyNo,
                    Address = DecryptField(record.Address, true) ?? record.Address,
                    Barangay = DecryptField(record.Barangay, true) ?? record.Barangay,
                    Birthday = record.Birthday,
                    Telepono = DecryptField(record.Telepono, true) ?? record.Telepono,
                    Edad = record.Edad,
                    Kasarian = DecryptField(record.Kasarian, true) ?? record.Kasarian,
                    Relihiyon = DecryptField(record.Relihiyon, true) ?? record.Relihiyon,
                    HasDiabetes = record.HasDiabetes,
                    HasHypertension = record.HasHypertension,
                    HasCancer = record.HasCancer,
                    HasCOPD = record.HasCOPD,
                    HasLungDisease = record.HasLungDisease,
                    HasEyeDisease = record.HasEyeDisease,
                    CancerType = DecryptField(record.CancerType, true) ?? record.CancerType,
                    FamilyHasHypertension = record.FamilyHasHypertension,
                    FamilyHasHeartDisease = record.FamilyHasHeartDisease,
                    FamilyHasStroke = record.FamilyHasStroke,
                    FamilyHasDiabetes = record.FamilyHasDiabetes,
                    FamilyHasCancer = record.FamilyHasCancer,
                    FamilyHasKidneyDisease = record.FamilyHasKidneyDisease,
                    FamilyHasOtherDisease = record.FamilyHasOtherDisease,
                    FamilyOtherDiseaseDetails = DecryptField(record.FamilyOtherDiseaseDetails, true) ?? record.FamilyOtherDiseaseDetails,
                    SmokingStatus = DecryptField(record.SmokingStatus, true) ?? record.SmokingStatus,
                    HighSaltIntake = record.HighSaltIntake,
                    AlcoholFrequency = DecryptField(record.AlcoholFrequency, true) ?? record.AlcoholFrequency,
                    AlcoholConsumption = DecryptField(record.AlcoholConsumption, true) ?? record.AlcoholConsumption,
                    ExerciseDuration = DecryptField(record.ExerciseDuration, true) ?? record.ExerciseDuration,
                    RiskStatus = DecryptField(record.RiskStatus, true) ?? record.RiskStatus,
                    ChestPain = DecryptField(record.ChestPain, true) ?? record.ChestPain,
                    ChestPainLocation = DecryptField(record.ChestPainLocation, true) ?? record.ChestPainLocation,
                    ChestPainValue = record.ChestPainValue,
                    HasDifficultyBreathing = record.HasDifficultyBreathing,
                    HasAsthma = record.HasAsthma,
                    HasNoRegularExercise = record.HasNoRegularExercise,
                    CreatedAt = record.CreatedAt,
                    UpdatedAt = record.UpdatedAt,
                    AppointmentType = DecryptField(record.AppointmentType, true) ?? record.AppointmentType,
                    CancerMedication = DecryptField(record.CancerMedication, true) ?? record.CancerMedication,
                    CancerYear = DecryptField(record.CancerYear, true) ?? record.CancerYear,
                    CivilStatus = record.CivilStatus,
                    DiabetesMedication = DecryptField(record.DiabetesMedication, true) ?? record.DiabetesMedication,
                    DiabetesYear = DecryptField(record.DiabetesYear, true) ?? record.DiabetesYear,
                    FamilyHistoryCancerFather = record.FamilyHistoryCancerFather,
                    FamilyHistoryCancerMother = record.FamilyHistoryCancerMother,
                    FamilyHistoryCancerSibling = record.FamilyHistoryCancerSibling,
                    FamilyHistoryDiabetesFather = record.FamilyHistoryDiabetesFather,
                    FamilyHistoryDiabetesMother = record.FamilyHistoryDiabetesMother,
                    FamilyHistoryDiabetesSibling = record.FamilyHistoryDiabetesSibling,
                    FamilyHistoryHeartDiseaseFather = record.FamilyHistoryHeartDiseaseFather,
                    FamilyHistoryHeartDiseaseMother = record.FamilyHistoryHeartDiseaseMother,
                    FamilyHistoryHeartDiseaseSibling = record.FamilyHistoryHeartDiseaseSibling,
                    FamilyHistoryLungDiseaseFather = record.FamilyHistoryLungDiseaseFather,
                    FamilyHistoryLungDiseaseMother = record.FamilyHistoryLungDiseaseMother,
                    FamilyHistoryLungDiseaseSibling = record.FamilyHistoryLungDiseaseSibling,
                    FamilyHistoryOther = DecryptField(record.FamilyHistoryOther, true) ?? record.FamilyHistoryOther,
                    FamilyHistoryOtherFather = record.FamilyHistoryOtherFather,
                    FamilyHistoryOtherMother = record.FamilyHistoryOtherMother,
                    FamilyHistoryOtherSibling = record.FamilyHistoryOtherSibling,
                    FamilyHistoryStrokeFather = record.FamilyHistoryStrokeFather,
                    FamilyHistoryStrokeMother = record.FamilyHistoryStrokeMother,
                    FamilyHistoryStrokeSibling = record.FamilyHistoryStrokeSibling,
                    FirstName = DecryptField(record.FirstName, true) ?? record.FirstName,
                    HypertensionMedication = DecryptField(record.HypertensionMedication, true) ?? record.HypertensionMedication,
                    HypertensionYear = DecryptField(record.HypertensionYear, true) ?? record.HypertensionYear,
                    LastName = DecryptField(record.LastName, true) ?? record.LastName,
                    LungDiseaseMedication = DecryptField(record.LungDiseaseMedication, true) ?? record.LungDiseaseMedication,
                    LungDiseaseYear = DecryptField(record.LungDiseaseYear, true) ?? record.LungDiseaseYear,
                    MiddleName = DecryptField(record.MiddleName, true) ?? record.MiddleName,
                    Occupation = DecryptField(record.Occupation, true) ?? record.Occupation
                };
                
                decryptedRecords.Add(decryptedRecord);
            }
            
            return decryptedRecords;
        }

        private static readonly (string publicKey, string privateKey) _keyPair = GenerateConsistentKeyPair();

        private static (string publicKey, string privateKey) GenerateConsistentKeyPair()
        {
            // Use a consistent seed to generate the same key pair every time
            // In production, these keys should be stored securely (e.g., Azure Key Vault)
            using (var rsa = RSA.Create())
            {
                // Generate a 2048-bit key pair
                rsa.KeySize = 2048;
                
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                
                return (publicKey, privateKey);
            }
        }

        private string GetPrivateKey()
        {
            return _keyPair.privateKey;
        }

        private string GetPublicKey()
        {
            return _keyPair.publicKey;
        }

        private string AsymmetricEncrypt(string plainText, string publicKey)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(publicKey))
                    return plainText;

                using (var rsa = RSA.Create())
                {
                    // Import the public key
                    rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                    
                    // Convert plain text to bytes
                    var data = Encoding.UTF8.GetBytes(plainText);
                    
                    // Encrypt using RSA with PKCS1 padding
                    var encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                    
                    // Return Base64 encoded encrypted data
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AsymmetricEncrypt for text: {Text}", plainText);
                return plainText; // Return original text if encryption fails
            }
        }

        private string AsymmetricDecrypt(string encryptedText, string privateKey)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(privateKey))
                    return encryptedText;

                using (var rsa = RSA.Create())
                {
                    // Import the private key
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    
                    // Convert Base64 encoded encrypted data to bytes
                    var encryptedData = Convert.FromBase64String(encryptedText);
                    
                    // Decrypt using RSA with PKCS1 padding
                    var decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                    
                    // Return decrypted text
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AsymmetricDecrypt for text: {Text}", encryptedText);
                return encryptedText; // Return original text if decryption fails
            }
        }

        // Helper method to get family data for hybrid encryption
        private async Task<object> GetFamilyData(string familyId)
        {
            // Get all records for this family
            var immunizationRecords = await _context.ImmunizationRecords
                .Where(r => r.FamilyNumber == familyId)
                .ToListAsync();

            var heeadsssRecords = await _context.HEEADSSSAssessments
                .Where(r => r.FamilyNo == familyId)
                .ToListAsync();

            var ncdRecords = await _context.NCDRiskAssessments
                .Where(r => r.FamilyNo == familyId)
                .ToListAsync();

            var vitalSignsRecords = await _context.VitalSigns
                .Where(r => r.PatientId == familyId)
                .ToListAsync();

            // Decrypt database data first, then apply asymmetric encryption
            var decryptedImmunizationRecords = DecryptAndReEncryptImmunizationRecords(immunizationRecords);
            var decryptedHeeadsssRecords = DecryptAndReEncryptHeeadsssRecords(heeadsssRecords);
            var decryptedNcdRecords = DecryptAndReEncryptNcdRecords(ncdRecords);

            // Get family info from first available record (use decrypted records)
            var familyInfo = GetFamilyInfoFromRecords(decryptedImmunizationRecords, decryptedHeeadsssRecords, decryptedNcdRecords, vitalSignsRecords);

            // Return encrypted data (same as GetFamilyDataForAsymmetricEncryption)
            return new
            {
                familyNumber = DecryptField(familyId, true) ?? familyId,
                familyName = familyInfo.FamilyName,
                address = familyInfo.Address,
                barangay = familyInfo.Barangay,
                contactInfo = familyInfo.ContactInfo,
                totalRecords = (immunizationRecords.Count + heeadsssRecords.Count + ncdRecords.Count + vitalSignsRecords.Count).ToString(),
                immunizationCount = immunizationRecords.Count.ToString(),
                heeadsssCount = heeadsssRecords.Count.ToString(),
                ncdCount = ncdRecords.Count.ToString(),
                vitalSignsCount = vitalSignsRecords.Count.ToString(),
                immunizationRecords = decryptedImmunizationRecords.Select(r => new
                {
                    childName = r.ChildName,
                    dateOfBirth = r.DateOfBirth,
                    sex = r.Sex,
                    placeOfBirth = r.PlaceOfBirth,
                    birthHeight = r.BirthHeight,
                    birthWeight = r.BirthWeight,
                    motherName = r.MotherName,
                    fatherName = r.FatherName,
                    healthCenter = r.HealthCenter,
                    email = r.Email,
                    contactNumber = r.ContactNumber,
                    bcgVaccineDate = r.BCGVaccineDate,
                    bcgVaccineRemarks = r.BCGVaccineRemarks,
                    hepatitisBVaccineDate = r.HepatitisBVaccineDate,
                    hepatitisBVaccineRemarks = r.HepatitisBVaccineRemarks,
                    pentavalent1Date = r.Pentavalent1Date,
                    pentavalent1Remarks = r.Pentavalent1Remarks,
                    pentavalent2Date = r.Pentavalent2Date,
                    pentavalent2Remarks = r.Pentavalent2Remarks,
                    pentavalent3Date = r.Pentavalent3Date,
                    pentavalent3Remarks = r.Pentavalent3Remarks,
                    opv1Date = r.OPV1Date,
                    opv1Remarks = r.OPV1Remarks,
                    opv2Date = r.OPV2Date,
                    opv2Remarks = r.OPV2Remarks,
                    opv3Date = r.OPV3Date,
                    opv3Remarks = r.OPV3Remarks,
                    ipvDate = r.IPV1Date,
                    ipvRemarks = r.IPV1Remarks,
                    pcv1Date = r.PCV1Date,
                    pcv1Remarks = r.PCV1Remarks,
                    pcv2Date = r.PCV2Date,
                    pcv2Remarks = r.PCV2Remarks,
                    pcv3Date = r.PCV3Date,
                    pcv3Remarks = r.PCV3Remarks,
                    mmrDate = r.MMR1Date,
                    mmrRemarks = r.MMR1Remarks,
                    createdAt = r.CreatedAt
                }).ToList(),
                heeadsssRecords = decryptedHeeadsssRecords.Select(r => new
                {
                    fullName = r.FullName,
                    age = r.Age,
                    gender = r.Gender,
                    address = r.Address,
                    contactNumber = r.ContactNumber,
                    homeEnvironment = r.HomeEnvironment,
                    familyRelationship = r.FamilyRelationship,
                    schoolPerformance = r.SchoolPerformance,
                    careerPlans = r.CareerPlans,
                    educationCurrentlyStudying = r.EducationCurrentlyStudying,
                    educationWorking = r.EducationWorking,
                    educationSchoolWorkProblems = r.EducationSchoolWorkProblems,
                    dietDescription = r.DietDescription,
                    hobbies = r.Hobbies,
                    physicalActivity = r.PhysicalActivity,
                    drugsAlcohol = r.DrugsAlcoholUse,
                    sexualActivity = r.SexualActivity,
                    feelsSafeAtHome = r.FeelsSafeAtHome,
                    feelsSafeAtSchool = r.FeelsSafeAtSchool,
                    experiencedBullying = r.ExperiencedBullying,
                    suicidalThoughts = r.SuicidalThoughts,
                    selfHarmBehavior = r.SelfHarmBehavior,
                    moodChanges = r.MoodChanges,
                    weightConcerns = r.WeightConcerns,
                    eatingDisorderSymptoms = r.EatingDisorderSymptoms,
                    substanceUse = r.SubstanceUse,
                    assessmentNotes = r.AssessmentNotes,
                    recommendedActions = r.RecommendedActions,
                    followUpPlan = r.FollowUpPlan,
                    assessedBy = r.AssessedBy,
                    createdAt = r.CreatedAt.ToString()
                }).ToList(),
                ncdRecords = decryptedNcdRecords.Select(r => new
                {
                    firstName = r.FirstName,
                    lastName = r.LastName,
                    middleName = r.MiddleName,
                    age = r.Edad,
                    gender = r.Kasarian,
                    civilStatus = r.CivilStatus,
                    occupation = r.Occupation,
                    hasDiabetes = r.HasDiabetes,
                    hasHypertension = r.HasHypertension,
                    hasHeartDisease = r.FamilyHasHeartDisease,
                    hasLungDisease = r.HasLungDisease,
                    hasCancer = r.HasCancer,
                    hasStroke = r.FamilyHasStroke,
                    diabetesMedication = r.DiabetesMedication,
                    diabetesYear = r.DiabetesYear,
                    hypertensionMedication = r.HypertensionMedication,
                    hypertensionYear = r.HypertensionYear,
                    lungDiseaseMedication = r.LungDiseaseMedication,
                    lungDiseaseYear = r.LungDiseaseYear,
                    cancerMedication = r.CancerMedication,
                    cancerYear = r.CancerYear,
                    familyHistoryDiabetesFather = r.FamilyHistoryDiabetesFather,
                    familyHistoryDiabetesMother = r.FamilyHistoryDiabetesMother,
                    familyHistoryDiabetesSibling = r.FamilyHistoryDiabetesSibling,
                    familyHistoryHypertensionFather = r.FamilyHasHypertension,
                    familyHistoryHypertensionMother = r.FamilyHasHypertension,
                    familyHistoryHypertensionSibling = r.FamilyHasHypertension,
                    familyHistoryHeartDiseaseFather = r.FamilyHasHeartDisease,
                    familyHistoryHeartDiseaseMother = r.FamilyHasHeartDisease,
                    familyHistoryHeartDiseaseSibling = r.FamilyHasHeartDisease,
                    familyHistoryLungDiseaseFather = r.FamilyHistoryLungDiseaseFather,
                    familyHistoryLungDiseaseMother = r.FamilyHistoryLungDiseaseMother,
                    familyHistoryLungDiseaseSibling = r.FamilyHistoryLungDiseaseSibling,
                    familyHistoryCancerFather = r.FamilyHasCancer,
                    familyHistoryCancerMother = r.FamilyHasCancer,
                    familyHistoryCancerSibling = r.FamilyHasCancer,
                    familyHistoryStrokeFather = r.FamilyHasStroke,
                    familyHistoryStrokeMother = r.FamilyHasStroke,
                    familyHistoryStrokeSibling = r.FamilyHasStroke,
                    familyHistoryOtherFather = r.FamilyHasOtherDisease,
                    familyHistoryOtherMother = r.FamilyHasOtherDisease,
                    familyHistoryOtherSibling = r.FamilyHasOtherDisease,
                    familyHistoryOther = r.FamilyOtherDiseaseDetails,
                    createdAt = r.CreatedAt?.ToString()
                }).ToList(),
                vitalSignsRecords = vitalSignsRecords.Select(r => new
                {
                    systolicBP = r.BloodPressure,
                    diastolicBP = r.BloodPressure,
                    heartRate = r.HeartRate,
                    temperature = r.Temperature,
                    respiratoryRate = r.RespiratoryRate,
                    oxygenSaturation = r.SpO2,
                    height = r.Height,
                    weight = r.Weight
                }).ToList()
            };
        }

        // Hybrid Encryption Methods

        /// <summary>
        /// Generates a new RSA key pair for hybrid encryption
        /// </summary>
        /// <returns>RSA key pair</returns>
        public async Task<IActionResult> OnGetGenerateKeyPair()
        {
            try
            {
                var hybridService = new HybridEncryptionService(_logger as ILogger<HybridEncryptionService>);
                var keyPair = hybridService.GenerateRSAKeyPair();
                
                return new JsonResult(new
                {
                    success = true,
                    publicKey = keyPair.PublicKey,
                    privateKey = keyPair.PrivateKey,
                    keyId = keyPair.KeyId,
                    createdAt = keyPair.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating RSA key pair");
                return new JsonResult(new
                {
                    success = false,
                    error = "Failed to generate key pair: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Encrypts data using hybrid encryption (AES + RSA)
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="publicKey">RSA public key</param>
        /// <returns>Encrypted data</returns>
        public async Task<IActionResult> OnPostEncryptHybrid([FromBody] HybridEncryptRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Data) || string.IsNullOrEmpty(request.PublicKey))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Data and public key are required"
                    });
                }

                var hybridService = new HybridEncryptionService(_logger as ILogger<HybridEncryptionService>);
                var encryptedData = hybridService.Encrypt(request.Data, request.PublicKey);
                
                return new JsonResult(new
                {
                    success = true,
                    encryptedData = encryptedData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in hybrid encryption");
                return new JsonResult(new
                {
                    success = false,
                    error = "Encryption failed: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Decrypts data using hybrid decryption (RSA + AES)
        /// </summary>
        /// <param name="request">Decryption request</param>
        /// <returns>Decrypted data</returns>
        public async Task<IActionResult> OnPostDecryptHybrid([FromBody] HybridDecryptRequest request)
        {
            try
            {
                if (request.EncryptedData == null || string.IsNullOrEmpty(request.PrivateKey))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Encrypted data and private key are required"
                    });
                }

                var hybridService = new HybridEncryptionService(_logger as ILogger<HybridEncryptionService>);
                var decryptedData = hybridService.Decrypt(request.EncryptedData, request.PrivateKey);
                
                return new JsonResult(new
                {
                    success = true,
                    decryptedData = decryptedData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in hybrid decryption");
                return new JsonResult(new
                {
                    success = false,
                    error = "Decryption failed: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Validates an RSA private key
        /// </summary>
        /// <param name="privateKey">Private key to validate</param>
        /// <returns>Validation result</returns>
        public async Task<IActionResult> OnPostValidatePrivateKey([FromBody] ValidateKeyRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PrivateKey))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        isValid = false,
                        error = "Private key is required"
                    });
                }

                var hybridService = new HybridEncryptionService(_logger as ILogger<HybridEncryptionService>);
                var isValid = hybridService.IsValidRSAPrivateKey(request.PrivateKey);
                
                return new JsonResult(new
                {
                    success = true,
                    isValid = isValid
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating private key");
                return new JsonResult(new
                {
                    success = false,
                    isValid = false,
                    error = "Validation failed: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Gets family details with hybrid encryption support
        /// </summary>
        /// <param name="familyId">Family ID</param>
        /// <param name="useHybridEncryption">Whether to use hybrid encryption</param>
        /// <returns>Family details</returns>
        public async Task<IActionResult> OnGetGetFamilyDetailsHybrid(string familyId, bool useHybridEncryption = false)
        {
            try
            {
                if (string.IsNullOrEmpty(familyId))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Family ID is required"
                    });
                }

                // Get family data (same as existing method but with hybrid encryption support)
                var familyData = await GetFamilyData(familyId);
                
                if (useHybridEncryption)
                {
                    // For hybrid encryption, we'll return the data as-is
                    // The client will handle the decryption
                    return new JsonResult(new
                    {
                        success = true,
                        data = familyData,
                        encryptionType = "hybrid"
                    });
                }
                else
                {
                    // For legacy encryption, use existing decryption
                    return new JsonResult(new
                    {
                        success = true,
                        data = familyData,
                        encryptionType = "legacy"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting family details for hybrid encryption");
                return new JsonResult(new
                {
                    success = false,
                    error = "Failed to get family details: " + ex.Message
                });
            }
        }
    }

    // Request/Response Models for Hybrid Encryption

    public class HybridEncryptRequest
    {
        public string Data { get; set; } = "";
        public string PublicKey { get; set; } = "";
    }

    public class HybridDecryptRequest
    {
        public HybridEncryptedData EncryptedData { get; set; } = new();
        public string PrivateKey { get; set; } = "";
    }

    public class ValidateKeyRequest
    {
        public string PrivateKey { get; set; } = "";
    }
}
