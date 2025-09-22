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
                    id = record.Id,
                    childName = record.ChildName ?? "N/A",
                    dateOfBirth = record.DateOfBirth ?? "N/A",
                    placeOfBirth = record.PlaceOfBirth ?? "N/A",
                    address = record.Address ?? "N/A",
                    motherName = record.MotherName ?? "N/A",
                    fatherName = record.FatherName ?? "N/A",
                    sex = record.Sex ?? "N/A",
                    birthHeight = record.BirthHeight ?? "N/A",
                    birthWeight = record.BirthWeight ?? "N/A",
                    healthCenter = record.HealthCenter ?? "N/A",
                    barangay = record.Barangay ?? "N/A",
                    familyNumber = record.FamilyNumber ?? "N/A",
                    email = record.Email ?? "N/A",
                    contactNumber = record.ContactNumber ?? "N/A",
                    bcgVaccineDate = record.BCGVaccineDate ?? "N/A",
                    bcgVaccineRemarks = record.BCGVaccineRemarks ?? "N/A",
                    hepatitisBVaccineDate = record.HepatitisBVaccineDate ?? "N/A",
                    hepatitisBVaccineRemarks = record.HepatitisBVaccineRemarks ?? "N/A",
                    pentavalent1Date = record.Pentavalent1Date ?? "N/A",
                    pentavalent1Remarks = record.Pentavalent1Remarks ?? "N/A",
                    pentavalent2Date = record.Pentavalent2Date ?? "N/A",
                    pentavalent2Remarks = record.Pentavalent2Remarks ?? "N/A",
                    pentavalent3Date = record.Pentavalent3Date ?? "N/A",
                    pentavalent3Remarks = record.Pentavalent3Remarks ?? "N/A",
                    opv1Date = record.OPV1Date ?? "N/A",
                    opv1Remarks = record.OPV1Remarks ?? "N/A",
                    opv2Date = record.OPV2Date ?? "N/A",
                    opv2Remarks = record.OPV2Remarks ?? "N/A",
                    opv3Date = record.OPV3Date ?? "N/A",
                    opv3Remarks = record.OPV3Remarks ?? "N/A",
                    ipv1Date = record.IPV1Date ?? "N/A",
                    ipv1Remarks = record.IPV1Remarks ?? "N/A",
                    ipv2Date = record.IPV2Date ?? "N/A",
                    ipv2Remarks = record.IPV2Remarks ?? "N/A",
                    pcv1Date = record.PCV1Date ?? "N/A",
                    pcv1Remarks = record.PCV1Remarks ?? "N/A",
                    pcv2Date = record.PCV2Date ?? "N/A",
                    pcv2Remarks = record.PCV2Remarks ?? "N/A",
                    pcv3Date = record.PCV3Date ?? "N/A",
                    pcv3Remarks = record.PCV3Remarks ?? "N/A",
                    mmr1Date = record.MMR1Date ?? "N/A",
                    mmr1Remarks = record.MMR1Remarks ?? "N/A",
                    mmr2Date = record.MMR2Date ?? "N/A",
                    mmr2Remarks = record.MMR2Remarks ?? "N/A",
                    createdAt = record.CreatedAt ?? "N/A",
                    updatedAt = record.UpdatedAt ?? "N/A",
                    createdBy = record.CreatedBy ?? "N/A",
                    updatedBy = record.UpdatedBy ?? "N/A",
                    status = record.Status ?? "N/A"
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
                    id = record.Id,
                    userId = record.UserId ?? "N/A",
                    appointmentId = record.AppointmentId ?? "N/A",
                    healthFacility = record.HealthFacility ?? "N/A",
                    familyNo = record.FamilyNo ?? "N/A",
                    fullName = record.FullName ?? "N/A",
                    age = record.Age ?? "N/A",
                    gender = record.Gender ?? "N/A",
                    address = record.Address ?? "N/A",
                    contactNumber = record.ContactNumber ?? "N/A",
                    homeEnvironment = record.HomeEnvironment ?? "N/A",
                    familyRelationship = record.FamilyRelationship ?? "N/A",
                    homeFamilyProblems = record.HomeFamilyProblems ?? "N/A",
                    homeParentalListening = record.HomeParentalListening ?? "N/A",
                    homeParentalBlame = record.HomeParentalBlame ?? "N/A",
                    homeFamilyChanges = record.HomeFamilyChanges ?? "N/A",
                    schoolPerformance = record.SchoolPerformance ?? "N/A",
                    attendanceIssues = record.AttendanceIssues ?? "N/A",
                    careerPlans = record.CareerPlans ?? "N/A",
                    educationCurrentlyStudying = record.EducationCurrentlyStudying ?? "N/A",
                    educationWorking = record.EducationWorking ?? "N/A",
                    educationSchoolWorkProblems = record.EducationSchoolWorkProblems ?? "N/A",
                    educationBullying = record.EducationBullying ?? "N/A",
                    educationEmployment = record.EducationEmployment ?? "N/A",
                    dietDescription = record.DietDescription ?? "N/A",
                    weightConcerns = record.WeightConcerns ?? "N/A",
                    eatingDisorderSymptoms = record.EatingDisorderSymptoms ?? "N/A",
                    eatingBodyImageSatisfaction = record.EatingBodyImageSatisfaction ?? "N/A",
                    eatingDisorderedEatingBehaviors = record.EatingDisorderedEatingBehaviors ?? "N/A",
                    eatingWeightComments = record.EatingWeightComments ?? "N/A",
                    hobbies = record.Hobbies ?? "N/A",
                    physicalActivity = record.PhysicalActivity ?? "N/A",
                    screenTime = record.ScreenTime ?? "N/A",
                    activitiesParticipation = record.ActivitiesParticipation ?? "N/A",
                    activitiesRegularExercise = record.ActivitiesRegularExercise ?? "N/A",
                    activitiesScreenTime = record.ActivitiesScreenTime ?? "N/A",
                    substanceUse = record.SubstanceUse ?? "N/A",
                    substanceType = record.SubstanceType ?? "N/A",
                    drugsTobaccoUse = record.DrugsTobaccoUse ?? "N/A",
                    drugsAlcoholUse = record.DrugsAlcoholUse ?? "N/A",
                    drugsIllicitDrugUse = record.DrugsIllicitDrugUse ?? "N/A",
                    datingRelationships = record.DatingRelationships ?? "N/A",
                    sexualActivity = record.SexualActivity ?? "N/A",
                    sexualOrientation = record.SexualOrientation ?? "N/A",
                    sexualityBodyConcerns = record.SexualityBodyConcerns ?? "N/A",
                    sexualityIntimateRelationships = record.SexualityIntimateRelationships ?? "N/A",
                    sexualityPartners = record.SexualityPartners ?? "N/A",
                    sexualitySexualOrientation = record.SexualitySexualOrientation ?? "N/A",
                    sexualityPregnancy = record.SexualityPregnancy ?? "N/A",
                    sexualitySTI = record.SexualitySTI ?? "N/A",
                    sexualityProtection = record.SexualityProtection ?? "N/A",
                    moodChanges = record.MoodChanges ?? "N/A",
                    suicidalThoughts = record.SuicidalThoughts ?? "N/A",
                    selfHarmBehavior = record.SelfHarmBehavior ?? "N/A",
                    feelsSafeAtHome = record.FeelsSafeAtHome ?? "N/A",
                    feelsSafeAtSchool = record.FeelsSafeAtSchool ?? "N/A",
                    experiencedBullying = record.ExperiencedBullying ?? "N/A",
                    personalStrengths = record.PersonalStrengths ?? "N/A",
                    supportSystems = record.SupportSystems ?? "N/A",
                    copingMechanisms = record.CopingMechanisms ?? "N/A",
                    safetyPhysicalAbuse = record.SafetyPhysicalAbuse ?? "N/A",
                    safetyRelationshipViolence = record.SafetyRelationshipViolence ?? "N/A",
                    safetyProtectiveGear = record.SafetyProtectiveGear ?? "N/A",
                    safetyGunsAtHome = record.SafetyGunsAtHome ?? "N/A",
                    suicideDepressionFeelings = record.SuicideDepressionFeelings ?? "N/A",
                    suicideSelfHarmThoughts = record.SuicideSelfHarmThoughts ?? "N/A",
                    suicideFamilyHistory = record.SuicideFamilyHistory ?? "N/A",
                    assessmentNotes = record.AssessmentNotes ?? "N/A",
                    recommendedActions = record.RecommendedActions ?? "N/A",
                    followUpPlan = record.FollowUpPlan ?? "N/A",
                    notes = record.Notes ?? "N/A",
                    assessedBy = record.AssessedBy ?? "N/A",
                    createdAt = record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    updatedAt = record.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"
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
                    id = record.Id,
                    userId = record.UserId ?? "N/A",
                    appointmentId = record.AppointmentId?.ToString() ?? "N/A",
                    healthFacility = record.HealthFacility ?? "N/A",
                    familyNo = record.FamilyNo ?? "N/A",
                    firstName = record.FirstName ?? "N/A",
                    middleName = record.MiddleName ?? "N/A",
                    lastName = record.LastName ?? "N/A",
                    address = record.Address ?? "N/A",
                    barangay = record.Barangay ?? "N/A",
                    birthday = record.Birthday ?? "N/A",
                    telepono = record.Telepono ?? "N/A",
                    edad = record.Edad?.ToString() ?? "N/A",
                    kasarian = record.Kasarian ?? "N/A",
                    relihiyon = record.Relihiyon ?? "N/A",
                    civilStatus = record.CivilStatus ?? "N/A",
                    occupation = record.Occupation ?? "N/A",
                    hasDiabetes = record.HasDiabetes,
                    diabetesYear = record.DiabetesYear?.ToString() ?? "N/A",
                    diabetesMedication = record.DiabetesMedication ?? "N/A",
                    hasHypertension = record.HasHypertension,
                    hypertensionYear = record.HypertensionYear?.ToString() ?? "N/A",
                    hypertensionMedication = record.HypertensionMedication ?? "N/A",
                    hasCancer = record.HasCancer,
                    cancerType = record.CancerType ?? "N/A",
                    cancerYear = record.CancerYear?.ToString() ?? "N/A",
                    cancerMedication = record.CancerMedication ?? "N/A",
                    hasLungDisease = record.HasLungDisease,
                    lungDiseaseYear = record.LungDiseaseYear?.ToString() ?? "N/A",
                    lungDiseaseMedication = record.LungDiseaseMedication ?? "N/A",
                    hasCOPD = record.HasCOPD,
                    hasEyeDisease = record.HasEyeDisease,
                    hasNoRegularExercise = record.HasNoRegularExercise,
                    familyHistoryDiabetesFather = record.FamilyHistoryDiabetesFather,
                    familyHistoryDiabetesMother = record.FamilyHistoryDiabetesMother,
                    familyHistoryDiabetesSibling = record.FamilyHistoryDiabetesSibling,
                    familyHistoryCancerFather = record.FamilyHistoryCancerFather,
                    familyHistoryCancerMother = record.FamilyHistoryCancerMother,
                    familyHistoryCancerSibling = record.FamilyHistoryCancerSibling,
                    familyHistoryStrokeFather = record.FamilyHistoryStrokeFather,
                    familyHistoryStrokeMother = record.FamilyHistoryStrokeMother,
                    familyHistoryStrokeSibling = record.FamilyHistoryStrokeSibling,
                    familyHistoryOtherFather = record.FamilyHistoryOtherFather,
                    familyHistoryOtherMother = record.FamilyHistoryOtherMother,
                    familyHistoryOtherSibling = record.FamilyHistoryOtherSibling,
                    createdAt = record.CreatedAt ?? "N/A",
                    updatedAt = record.UpdatedAt ?? "N/A",
                    appointmentType = record.AppointmentType ?? "N/A",
                    smokingStatus = record.SmokingStatus ?? "N/A",
                    riskStatus = record.RiskStatus ?? "N/A"
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
                    id = record.Id,
                    patientId = record.PatientId ?? "N/A",
                    bloodPressure = record.BloodPressure ?? "N/A",
                    heartRate = record.HeartRate ?? "N/A",
                    temperature = record.Temperature ?? "N/A",
                    respiratoryRate = record.RespiratoryRate ?? "N/A",
                    o2Saturation = record.SpO2 ?? "N/A",
                    height = record.Height ?? "N/A",
                    weight = record.Weight ?? "N/A",
                    recordedAt = record.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                    notes = record.Notes ?? "N/A"
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
