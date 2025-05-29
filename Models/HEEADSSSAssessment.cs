using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class HEEADSSSAssessment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public int AppointmentId { get; set; }
        
        public string HealthFacility { get; set; }
        
        public string FamilyNo { get; set; }
        
        // Personal Information 
        public string FullName { get; set; }
        
        public DateTime Birthday { get; set; }
        
        public int Age { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }
        
        // HOME
        public string HomeEnvironment { get; set; } = string.Empty;
        
        public string FamilyRelationship { get; set; } = string.Empty;
        
        public string HomeFamilyProblems { get; set; }
        
        public string HomeParentalListening { get; set; }
        
        public string HomeParentalBlame { get; set; }
        
        public string HomeFamilyChanges { get; set; }
        
        // EDUCATION
        public string SchoolPerformance { get; set; } = string.Empty;
        
        public bool AttendanceIssues { get; set; }
        
        public string CareerPlans { get; set; } = string.Empty;
        
        public string EducationCurrentlyStudying { get; set; }
        
        public string EducationWorking { get; set; }
        
        public string EducationSchoolWorkProblems { get; set; }
        
        public string EducationBullying { get; set; }
        
        public string EducationEmployment { get; set; } = string.Empty;
        
        // EATING HABITS
        public string DietDescription { get; set; } = string.Empty;
        
        public bool WeightConcerns { get; set; }
        
        public bool EatingDisorderSymptoms { get; set; }
        
        public string EatingBodyImageSatisfaction { get; set; }
        
        public string EatingDisorderedEatingBehaviors { get; set; }
        
        public string EatingWeightComments { get; set; }
        
        // ACTIVITIES
        public string Hobbies { get; set; } = string.Empty;
        
        public string PhysicalActivity { get; set; } = string.Empty;
        
        public string ScreenTime { get; set; } = string.Empty;
        
        public string ActivitiesParticipation { get; set; }
        
        public string ActivitiesRegularExercise { get; set; }
        
        public string ActivitiesScreenTime { get; set; }
        
        // DRUGS
        public bool SubstanceUse { get; set; }
        
        public string SubstanceType { get; set; } = string.Empty;
        
        public string DrugsTobaccoUse { get; set; }
        
        public string DrugsAlcoholUse { get; set; }
        
        public string DrugsIllicitDrugUse { get; set; }
        
        // SEXUALITY
        public string DatingRelationships { get; set; } = string.Empty;
        
        public bool SexualActivity { get; set; }
        
        public string SexualOrientation { get; set; } = string.Empty;
        
        public string SexualityBodyConcerns { get; set; }
        
        public string SexualityIntimateRelationships { get; set; }
        
        public string SexualityPartners { get; set; }
        
        public string SexualitySexualOrientation { get; set; }
        
        public string SexualityPregnancy { get; set; }
        
        public string SexualitySTI { get; set; }
        
        public string SexualityProtection { get; set; }
        
        // SUICIDE/DEPRESSION
        public bool MoodChanges { get; set; }
        
        public bool SuicidalThoughts { get; set; }
        
        public bool SelfHarmBehavior { get; set; }
        
        // SAFETY
        public bool FeelsSafeAtHome { get; set; }
        
        public bool FeelsSafeAtSchool { get; set; }
        
        public bool ExperiencedBullying { get; set; }
        
        // STRENGTHS
        public string PersonalStrengths { get; set; } = string.Empty;
        
        public string SupportSystems { get; set; } = string.Empty;
        
        public string CopingMechanisms { get; set; } = string.Empty;
        
        // SAFETY/WEAPONS/VIOLENCE
        public string SafetyPhysicalAbuse { get; set; }
        
        public string SafetyRelationshipViolence { get; set; }
        
        public string SafetyProtectiveGear { get; set; }
        
        public string SafetyGunsAtHome { get; set; }
        
        // SUICIDE/DEPRESSION
        public string SuicideDepressionFeelings { get; set; }
        
        public string SuicideSelfHarmThoughts { get; set; }
        
        public string SuicideFamilyHistory { get; set; }
        
        // Assessment Information
        public string AssessmentNotes { get; set; } = string.Empty;
        
        public string RecommendedActions { get; set; } = string.Empty;
        
        public string FollowUpPlan { get; set; } = string.Empty;
        
        public string Notes { get; set; }
        
        public string AssessedBy { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
} 