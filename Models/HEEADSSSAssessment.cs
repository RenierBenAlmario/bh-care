using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Barangay.Attributes;

namespace Barangay.Models
{
    public class HEEADSSSAssessment
    {
        [Key]
        public int Id { get; set; }
        
        public string? UserId { get; set; }
        
        [Encrypted]
        public string? AppointmentId { get; set; }
        
        [Encrypted]
        public string? HealthFacility { get; set; }
        
        [Encrypted]
        public string? FamilyNo { get; set; }
        
        // Personal Information 
        [Encrypted]
        public string? FullName { get; set; }
        
        [NotMapped]
        public DateTime? Birthday { get; set; }
        
        [Encrypted]
        public string? Age { get; set; }

        [Encrypted]
        public string? Gender { get; set; }

        [Encrypted]
        public string? Address { get; set; }

        [Encrypted]
        public string? ContactNumber { get; set; }
        
        // HOME
        [Encrypted]
        public string? HomeEnvironment { get; set; }
        
        [Encrypted]
        public string? FamilyRelationship { get; set; }
        
        [Encrypted]
        public string? HomeFamilyProblems { get; set; }
        
        [Encrypted]
        public string? HomeParentalListening { get; set; }
        
        [Encrypted]
        public string? HomeParentalBlame { get; set; }
        
        [Encrypted]
        public string? HomeFamilyChanges { get; set; }
        
        // EDUCATION
        [Encrypted]
        public string? SchoolPerformance { get; set; }
        
        [Encrypted]
        public string? AttendanceIssues { get; set; }
        
        [Encrypted]
        public string? CareerPlans { get; set; }
        
        [Encrypted]
        public string? EducationCurrentlyStudying { get; set; }
        
        [Encrypted]
        public string? EducationWorking { get; set; }
        
        [Encrypted]
        public string? EducationSchoolWorkProblems { get; set; }
        
        [Encrypted]
        public string? EducationBullying { get; set; }
        
        [Encrypted]
        public string? EducationEmployment { get; set; }
        
        // EATING HABITS
        [Encrypted]
        public string? DietDescription { get; set; }
        
        [Encrypted]
        public string? WeightConcerns { get; set; }
        
        [Encrypted]
        public string? EatingDisorderSymptoms { get; set; }
        
        [Encrypted]
        public string? EatingBodyImageSatisfaction { get; set; }
        
        [Encrypted]
        public string? EatingDisorderedEatingBehaviors { get; set; }
        
        [Encrypted]
        public string? EatingWeightComments { get; set; }
        
        // ACTIVITIES
        [Encrypted]
        public string? Hobbies { get; set; }
        
        [Encrypted]
        public string? PhysicalActivity { get; set; }
        
        [Encrypted]
        public string? ScreenTime { get; set; }
        
        [Encrypted]
        public string? ActivitiesParticipation { get; set; }
        
        [Encrypted]
        public string? ActivitiesRegularExercise { get; set; }
        
        [Encrypted]
        public string? ActivitiesScreenTime { get; set; }
        
        // DRUGS
        [Encrypted]
        public string? SubstanceUse { get; set; }
        
        [Encrypted]
        public string? SubstanceType { get; set; }
        
        [Encrypted]
        public string? DrugsTobaccoUse { get; set; }
        
        [Encrypted]
        public string? DrugsAlcoholUse { get; set; }
        
        [Encrypted]
        public string? DrugsIllicitDrugUse { get; set; }
        
        // SEXUALITY
        [Encrypted]
        public string? DatingRelationships { get; set; }
        
        [Encrypted]
        public string? SexualActivity { get; set; }
        
        [Encrypted]
        public string? SexualOrientation { get; set; }
        
        [Encrypted]
        public string? SexualityBodyConcerns { get; set; }
        
        [Encrypted]
        public string? SexualityIntimateRelationships { get; set; }
        
        [Encrypted]
        public string? SexualityPartners { get; set; }
        
        [Encrypted]
        public string? SexualitySexualOrientation { get; set; }
        
        [Encrypted]
        public string? SexualityPregnancy { get; set; }
        
        [Encrypted]
        public string? SexualitySTI { get; set; }
        
        [Encrypted]
        public string? SexualityProtection { get; set; }
        
        // SUICIDE/DEPRESSION
        [Encrypted]
        public string? MoodChanges { get; set; }
        
        [Encrypted]
        public string? SuicidalThoughts { get; set; }
        
        [Encrypted]
        public string? SelfHarmBehavior { get; set; }
        
        // SAFETY
        [Encrypted]
        public string? FeelsSafeAtHome { get; set; }
        
        [Encrypted]
        public string? FeelsSafeAtSchool { get; set; }
        
        [Encrypted]
        public string? ExperiencedBullying { get; set; }
        
        // STRENGTHS
        [Encrypted]
        public string? PersonalStrengths { get; set; }
        
        [Encrypted]
        public string? SupportSystems { get; set; }
        
        [Encrypted]
        public string? CopingMechanisms { get; set; }
        
        // SAFETY/WEAPONS/VIOLENCE
        [Encrypted]
        public string? SafetyPhysicalAbuse { get; set; }
        
        [Encrypted]
        public string? SafetyRelationshipViolence { get; set; }
        
        [Encrypted]
        public string? SafetyProtectiveGear { get; set; }
        
        [Encrypted]
        public string? SafetyGunsAtHome { get; set; }
        
        // SUICIDE/DEPRESSION
        [Encrypted]
        public string? SuicideDepressionFeelings { get; set; }
        
        [Encrypted]
        public string? SuicideSelfHarmThoughts { get; set; }
        
        [Encrypted]
        public string? SuicideFamilyHistory { get; set; }
        
        // Assessment Information
        [Encrypted]
        public string? AssessmentNotes { get; set; }
        
        [Encrypted]
        public string? RecommendedActions { get; set; }
        
        [Encrypted]
        public string? FollowUpPlan { get; set; }
        
        [Encrypted]
        public string? Notes { get; set; }
        
        [Encrypted]
        public string? AssessedBy { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
} 