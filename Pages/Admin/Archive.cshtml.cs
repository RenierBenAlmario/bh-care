using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Text.Json;

namespace Barangay.Pages.Admin
{
    public class FamilyData
    {
        public string FamilyNumber { get; set; } = "";
        public List<object> ImmunizationRecords { get; set; } = new();
        public List<object> HEEADSSSRecords { get; set; } = new();
        public List<object> NCDRecords { get; set; } = new();
        public List<object> VitalSignsRecords { get; set; } = new();
        public string LastUpdated { get; set; } = "";
        public string MotherName { get; set; } = "";
        public string FatherName { get; set; } = "";
        public string Address { get; set; } = "";
        public string ContactNumber { get; set; } = "";
    }

    public class ArchiveModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ArchiveModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ImmunizationRecord> ImmunizationRecords { get; set; } = new();
        public List<HEEADSSSAssessment> HEEADSSSAssessments { get; set; } = new();
        public List<NCDRiskAssessment> NCDRiskAssessments { get; set; } = new();
        public List<VitalSign> VitalSigns { get; set; } = new();

        public async Task OnGetAsync()
        {
            ImmunizationRecords = await _context.ImmunizationRecords
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            HEEADSSSAssessments = await _context.HEEADSSSAssessments
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            NCDRiskAssessments = await _context.NCDRiskAssessments
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            VitalSigns = await _context.VitalSigns
                .OrderByDescending(r => r.RecordedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetFamilyDetailsAsync(string familyId)
        {
            try
            {
                // Fetch all records for this family
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

                // Process records to return readable data
                var processedImmunizationRecords = ProcessImmunizationRecords(immunizationRecords);
                var processedHeeadsssRecords = ProcessHeeadsssRecords(heeadsssRecords);
                var processedNcdRecords = ProcessNcdRecords(ncdRecords);
                var processedVitalSignsRecords = ProcessVitalSignsRecords(vitalSignsRecords);

                // Get family info
                var familyInfo = GetFamilyInfo(immunizationRecords, heeadsssRecords, ncdRecords, vitalSignsRecords);

                // Calculate statistics
                var totalRecords = processedImmunizationRecords.Count + processedHeeadsssRecords.Count + 
                                 processedNcdRecords.Count + processedVitalSignsRecords.Count;

                var stats = new
                {
                    total = totalRecords,
                    immunization = processedImmunizationRecords.Count,
                    heeadsss = processedHeeadsssRecords.Count,
                    ncd = processedNcdRecords.Count,
                    vitalSigns = processedVitalSignsRecords.Count,
                    familyInfo = familyInfo
                };

                return new JsonResult(new
                {
                    success = true,
                    stats = stats,
                    data = new
                    {
                        immunization = processedImmunizationRecords,
                        heeadsss = processedHeeadsssRecords,
                        ncd = processedNcdRecords,
                        vitalSigns = processedVitalSignsRecords
                    },
                    familyId = familyId,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        private List<object> ProcessImmunizationRecords(List<ImmunizationRecord> records)
        {
            var processedRecords = new List<object>();

            foreach (var record in records)
            {
                var processedRecord = new
                {
                    Id = record.Id,
                    ChildName = record.ChildName ?? "N/A",
                    DateOfBirth = record.DateOfBirth ?? "N/A",
                    PlaceOfBirth = record.PlaceOfBirth ?? "N/A",
                    Address = record.Address ?? "N/A",
                    MotherName = record.MotherName ?? "N/A",
                    FatherName = record.FatherName ?? "N/A",
                    Sex = record.Sex ?? "N/A",
                    BirthHeight = record.BirthHeight ?? "N/A",
                    BirthWeight = record.BirthWeight ?? "N/A",
                    HealthCenter = record.HealthCenter ?? "N/A",
                    Barangay = record.Barangay ?? "N/A",
                    FamilyNumber = record.FamilyNumber ?? "N/A",
                    Email = record.Email ?? "N/A",
                    ContactNumber = record.ContactNumber ?? "N/A",
                    BCGVaccineDate = record.BCGVaccineDate ?? "N/A",
                    BCGVaccineRemarks = record.BCGVaccineRemarks ?? "N/A",
                    HepatitisBVaccineDate = record.HepatitisBVaccineDate ?? "N/A",
                    HepatitisBVaccineRemarks = record.HepatitisBVaccineRemarks ?? "N/A",
                    Pentavalent1Date = record.Pentavalent1Date ?? "N/A",
                    Pentavalent1Remarks = record.Pentavalent1Remarks ?? "N/A",
                    Pentavalent2Date = record.Pentavalent2Date ?? "N/A",
                    Pentavalent2Remarks = record.Pentavalent2Remarks ?? "N/A",
                    Pentavalent3Date = record.Pentavalent3Date ?? "N/A",
                    Pentavalent3Remarks = record.Pentavalent3Remarks ?? "N/A",
                    OPV1Date = record.OPV1Date ?? "N/A",
                    OPV1Remarks = record.OPV1Remarks ?? "N/A",
                    OPV2Date = record.OPV2Date ?? "N/A",
                    OPV2Remarks = record.OPV2Remarks ?? "N/A",
                    OPV3Date = record.OPV3Date ?? "N/A",
                    OPV3Remarks = record.OPV3Remarks ?? "N/A",
                    IPV1Date = record.IPV1Date ?? "N/A",
                    IPV1Remarks = record.IPV1Remarks ?? "N/A",
                    IPV2Date = record.IPV2Date ?? "N/A",
                    IPV2Remarks = record.IPV2Remarks ?? "N/A",
                    PCV1Date = record.PCV1Date ?? "N/A",
                    PCV1Remarks = record.PCV1Remarks ?? "N/A",
                    PCV2Date = record.PCV2Date ?? "N/A",
                    PCV2Remarks = record.PCV2Remarks ?? "N/A",
                    PCV3Date = record.PCV3Date ?? "N/A",
                    PCV3Remarks = record.PCV3Remarks ?? "N/A",
                    MMR1Date = record.MMR1Date ?? "N/A",
                    MMR1Remarks = record.MMR1Remarks ?? "N/A",
                    MMR2Date = record.MMR2Date ?? "N/A",
                    MMR2Remarks = record.MMR2Remarks ?? "N/A",
                    CreatedAt = record.CreatedAt ?? "N/A",
                    UpdatedAt = record.UpdatedAt ?? "N/A",
                    CreatedBy = record.CreatedBy ?? "N/A",
                    UpdatedBy = record.UpdatedBy ?? "N/A",
                    Status = record.Status ?? "N/A"
                };

                processedRecords.Add(processedRecord);
            }

            return processedRecords;
        }

        private List<object> ProcessHeeadsssRecords(List<HEEADSSSAssessment> records)
        {
            var processedRecords = new List<object>();

            foreach (var record in records)
            {
                var processedRecord = new
                {
                    Id = record.Id,
                    UserId = record.UserId ?? "N/A",
                    AppointmentId = record.AppointmentId ?? "N/A",
                    HealthFacility = record.HealthFacility ?? "N/A",
                    FamilyNo = record.FamilyNo ?? "N/A",
                    FullName = record.FullName ?? "N/A",
                    Age = record.Age ?? "N/A",
                    Gender = record.Gender ?? "N/A",
                    Address = record.Address ?? "N/A",
                    ContactNumber = record.ContactNumber ?? "N/A",
                    HomeEnvironment = record.HomeEnvironment ?? "N/A",
                    FamilyRelationship = record.FamilyRelationship ?? "N/A",
                    HomeFamilyProblems = record.HomeFamilyProblems ?? "N/A",
                    HomeParentalListening = record.HomeParentalListening ?? "N/A",
                    HomeParentalBlame = record.HomeParentalBlame ?? "N/A",
                    HomeFamilyChanges = record.HomeFamilyChanges ?? "N/A",
                    SchoolPerformance = record.SchoolPerformance ?? "N/A",
                    AttendanceIssues = record.AttendanceIssues ?? "N/A",
                    CareerPlans = record.CareerPlans ?? "N/A",
                    EducationCurrentlyStudying = record.EducationCurrentlyStudying ?? "N/A",
                    EducationWorking = record.EducationWorking ?? "N/A",
                    EducationSchoolWorkProblems = record.EducationSchoolWorkProblems ?? "N/A",
                    EducationBullying = record.EducationBullying ?? "N/A",
                    EducationEmployment = record.EducationEmployment ?? "N/A",
                    DietDescription = record.DietDescription ?? "N/A",
                    WeightConcerns = record.WeightConcerns ?? "N/A",
                    EatingDisorderSymptoms = record.EatingDisorderSymptoms ?? "N/A",
                    EatingBodyImageSatisfaction = record.EatingBodyImageSatisfaction ?? "N/A",
                    EatingDisorderedEatingBehaviors = record.EatingDisorderedEatingBehaviors ?? "N/A",
                    EatingWeightComments = record.EatingWeightComments ?? "N/A",
                    Hobbies = record.Hobbies ?? "N/A",
                    PhysicalActivity = record.PhysicalActivity ?? "N/A",
                    ScreenTime = record.ScreenTime ?? "N/A",
                    ActivitiesParticipation = record.ActivitiesParticipation ?? "N/A",
                    ActivitiesRegularExercise = record.ActivitiesRegularExercise ?? "N/A",
                    ActivitiesScreenTime = record.ActivitiesScreenTime ?? "N/A",
                    SubstanceUse = record.SubstanceUse ?? "N/A",
                    SubstanceType = record.SubstanceType ?? "N/A",
                    DrugsTobaccoUse = record.DrugsTobaccoUse ?? "N/A",
                    DrugsAlcoholUse = record.DrugsAlcoholUse ?? "N/A",
                    DrugsIllicitDrugUse = record.DrugsIllicitDrugUse ?? "N/A",
                    DatingRelationships = record.DatingRelationships ?? "N/A",
                    SexualActivity = record.SexualActivity ?? "N/A",
                    SexualOrientation = record.SexualOrientation ?? "N/A",
                    SexualityBodyConcerns = record.SexualityBodyConcerns ?? "N/A",
                    SexualityIntimateRelationships = record.SexualityIntimateRelationships ?? "N/A",
                    SexualityPartners = record.SexualityPartners ?? "N/A",
                    SexualitySexualOrientation = record.SexualitySexualOrientation ?? "N/A",
                    SexualityPregnancy = record.SexualityPregnancy ?? "N/A",
                    SexualitySTI = record.SexualitySTI ?? "N/A",
                    SexualityProtection = record.SexualityProtection ?? "N/A",
                    MoodChanges = record.MoodChanges ?? "N/A",
                    SuicidalThoughts = record.SuicidalThoughts ?? "N/A",
                    SelfHarmBehavior = record.SelfHarmBehavior ?? "N/A",
                    FeelsSafeAtHome = record.FeelsSafeAtHome ?? "N/A",
                    FeelsSafeAtSchool = record.FeelsSafeAtSchool ?? "N/A",
                    ExperiencedBullying = record.ExperiencedBullying ?? "N/A",
                    PersonalStrengths = record.PersonalStrengths ?? "N/A",
                    SupportSystems = record.SupportSystems ?? "N/A",
                    CopingMechanisms = record.CopingMechanisms ?? "N/A",
                    SafetyPhysicalAbuse = record.SafetyPhysicalAbuse ?? "N/A",
                    SafetyRelationshipViolence = record.SafetyRelationshipViolence ?? "N/A",
                    SafetyProtectiveGear = record.SafetyProtectiveGear ?? "N/A",
                    SafetyGunsAtHome = record.SafetyGunsAtHome ?? "N/A",
                    SuicideDepressionFeelings = record.SuicideDepressionFeelings ?? "N/A",
                    SuicideSelfHarmThoughts = record.SuicideSelfHarmThoughts ?? "N/A",
                    SuicideFamilyHistory = record.SuicideFamilyHistory ?? "N/A",
                    AssessmentNotes = record.AssessmentNotes ?? "N/A",
                    RecommendedActions = record.RecommendedActions ?? "N/A",
                    FollowUpPlan = record.FollowUpPlan ?? "N/A",
                    Notes = record.Notes ?? "N/A",
                    AssessedBy = record.AssessedBy ?? "N/A",
                    CreatedAt = record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = record.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"
                };

                processedRecords.Add(processedRecord);
            }

            return processedRecords;
        }

        private List<object> ProcessNcdRecords(List<NCDRiskAssessment> records)
        {
            var processedRecords = new List<object>();

            foreach (var record in records)
            {
                var processedRecord = new
                {
                    Id = record.Id,
                    UserId = record.UserId ?? "N/A",
                    AppointmentId = record.AppointmentId?.ToString() ?? "N/A",
                    HealthFacility = record.HealthFacility ?? "N/A",
                    FamilyNo = record.FamilyNo ?? "N/A",
                    FirstName = record.FirstName ?? "N/A",
                    MiddleName = record.MiddleName ?? "N/A",
                    LastName = record.LastName ?? "N/A",
                    Address = record.Address ?? "N/A",
                    Barangay = record.Barangay ?? "N/A",
                    Birthday = record.Birthday ?? "N/A",
                    Telepono = record.Telepono ?? "N/A",
                    Edad = record.Edad?.ToString() ?? "N/A",
                    Kasarian = record.Kasarian ?? "N/A",
                    Relihiyon = record.Relihiyon ?? "N/A",
                    CivilStatus = record.CivilStatus ?? "N/A",
                    Occupation = record.Occupation ?? "N/A",
                    HasDiabetes = record.HasDiabetes,
                    DiabetesYear = record.DiabetesYear?.ToString() ?? "N/A",
                    DiabetesMedication = record.DiabetesMedication ?? "N/A",
                    HasHypertension = record.HasHypertension,
                    HypertensionYear = record.HypertensionYear?.ToString() ?? "N/A",
                    HypertensionMedication = record.HypertensionMedication ?? "N/A",
                    HasCancer = record.HasCancer,
                    CancerType = record.CancerType ?? "N/A",
                    CancerYear = record.CancerYear?.ToString() ?? "N/A",
                    CancerMedication = record.CancerMedication ?? "N/A",
                    HasLungDisease = record.HasLungDisease,
                    LungDiseaseYear = record.LungDiseaseYear?.ToString() ?? "N/A",
                    LungDiseaseMedication = record.LungDiseaseMedication ?? "N/A",
                    HasCOPD = record.HasCOPD,
                    HasEyeDisease = record.HasEyeDisease,
                    HasNoRegularExercise = record.HasNoRegularExercise,
                    FamilyHistoryDiabetesFather = record.FamilyHistoryDiabetesFather,
                    FamilyHistoryDiabetesMother = record.FamilyHistoryDiabetesMother,
                    FamilyHistoryDiabetesSibling = record.FamilyHistoryDiabetesSibling,
                    FamilyHistoryCancerFather = record.FamilyHistoryCancerFather,
                    FamilyHistoryCancerMother = record.FamilyHistoryCancerMother,
                    FamilyHistoryCancerSibling = record.FamilyHistoryCancerSibling,
                    FamilyHistoryStrokeFather = record.FamilyHistoryStrokeFather,
                    FamilyHistoryStrokeMother = record.FamilyHistoryStrokeMother,
                    FamilyHistoryStrokeSibling = record.FamilyHistoryStrokeSibling,
                    FamilyHistoryOtherFather = record.FamilyHistoryOtherFather,
                    FamilyHistoryOtherMother = record.FamilyHistoryOtherMother,
                    FamilyHistoryOtherSibling = record.FamilyHistoryOtherSibling,
                    CreatedAt = record.CreatedAt ?? "N/A",
                    UpdatedAt = record.UpdatedAt ?? "N/A",
                    AppointmentType = record.AppointmentType ?? "N/A",
                    SmokingStatus = record.SmokingStatus ?? "N/A",
                    RiskStatus = record.RiskStatus ?? "N/A"
                };

                processedRecords.Add(processedRecord);
            }

            return processedRecords;
        }

        private List<object> ProcessVitalSignsRecords(List<VitalSign> records)
        {
            var processedRecords = new List<object>();

            foreach (var record in records)
            {
                var processedRecord = new
                {
                    Id = record.Id,
                    PatientId = record.PatientId ?? "N/A",
                    BloodPressure = record.BloodPressure ?? "N/A",
                    HeartRate = record.HeartRate ?? "N/A",
                    Temperature = record.Temperature ?? "N/A",
                    RespiratoryRate = record.RespiratoryRate ?? "N/A",
                    SpO2 = record.SpO2 ?? "N/A",
                    Height = record.Height ?? "N/A",
                    Weight = record.Weight ?? "N/A",
                    RecordedAt = record.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                    Notes = record.Notes ?? "N/A"
                };

                processedRecords.Add(processedRecord);
            }

            return processedRecords;
        }

        private object GetFamilyInfo(
            List<ImmunizationRecord> immunizationRecords,
            List<HEEADSSSAssessment> heeadsssRecords,
            List<NCDRiskAssessment> ncdRecords,
            List<VitalSign> vitalSignsRecords)
        {
            // Get family info from the first available record
            if (immunizationRecords.Any())
            {
                var firstRecord = immunizationRecords.First();
                return new
                {
                    familyNumber = firstRecord.FamilyNumber,
                    motherName = firstRecord.MotherName ?? "Unknown",
                    fatherName = firstRecord.FatherName ?? "Unknown",
                    address = firstRecord.Address ?? "Unknown",
                    barangay = firstRecord.Barangay ?? "Unknown",
                    contactNumber = firstRecord.ContactNumber ?? "Unknown"
                };
            }

            if (heeadsssRecords.Any())
            {
                var firstRecord = heeadsssRecords.First();
                return new
                {
                    familyNumber = firstRecord.FamilyNo ?? "Unknown",
                    fullName = firstRecord.FullName ?? "Unknown",
                    address = firstRecord.Address ?? "Unknown",
                    contactNumber = firstRecord.ContactNumber ?? "Unknown"
                };
            }

            if (ncdRecords.Any())
            {
                var firstRecord = ncdRecords.First();
                return new
                {
                    familyNumber = firstRecord.FamilyNo ?? "Unknown",
                    firstName = firstRecord.FirstName ?? "Unknown",
                    lastName = firstRecord.LastName ?? "Unknown",
                    address = firstRecord.Address ?? "Unknown",
                    barangay = firstRecord.Barangay ?? "Unknown"
                };
            }

            if (vitalSignsRecords.Any())
            {
                var firstRecord = vitalSignsRecords.First();
                return new
                {
                    patientId = firstRecord.PatientId ?? "Unknown",
                    recordedAt = firstRecord.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            return new
            {
                familyNumber = "Unknown",
                message = "No family information available"
            };
        }
    }
}
