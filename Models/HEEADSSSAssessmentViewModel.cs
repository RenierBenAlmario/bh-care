using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class HEEADSSSAssessmentViewModel
    {
        public string? UserId { get; set; }

        public int? AppointmentId { get; set; }

        // Facility Information
        [Display(Name = "Health Facility")]
        public string? HealthFacility { get; set; }
        
        [Display(Name = "Family No")]
        public string? FamilyNo { get; set; }
        
        // Personal Information 
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }
        
        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
        
        [Display(Name = "Age")]
        public string? Age { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Contact Number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? ContactNumber { get; set; }
        
        [Display(Name = "Religion")]
        public string? Religion { get; set; }
        
        // Cancer Type - only required if Cancer is selected
        [Display(Name = "Cancer Type")]
        public string? CancerType { get; set; }
        
        // Appointment Type - optional
        [Display(Name = "Appointment Type")]
        public string? AppointmentType { get; set; }
        
        // Family Other Disease Details - optional
        [Display(Name = "Family Other Disease Details")]
        public string? FamilyOtherDiseaseDetails { get; set; }
        
        // HOME
        [Display(Name = "Home Environment")]
        public string? HomeEnvironment { get; set; }

        [Display(Name = "Family Relationship")]
        public string? FamilyRelationship { get; set; }

        [Display(Name = "Meron bang problema sa inyong pamilya na kung saan ikaw ay maaaring maapektuhan?")]
        public string? HomeFamilyProblems { get; set; }
        
        [Display(Name = "Pinakikinggan ka ba ng iyong magulang o guardian sa tuwing ikaw ay may problema?")]
        public string? HomeParentalListening { get; set; }
        
        [Display(Name = "May pagkakataon bang nasisi iyong magulang?")]
        public string? HomeParentalBlame { get; set; }
        
        [Display(Name = "Sa nakaraang taon, meron bang pagbabagong naganap sa loob ng iyong pamilya?")]
        public string? HomeFamilyChanges { get; set; }
        
        // EDUCATION
        [Display(Name = "School Performance")]
        public string? SchoolPerformance { get; set; }

        [Display(Name = "Attendance Issues")]
        public string? AttendanceIssues { get; set; }

        [Display(Name = "Career Plans")]
        public string? CareerPlans { get; set; }

        [Display(Name = "Ikaw ba ay kasalukuyang nag-aaral?")]
        public string? EducationCurrentlyStudying { get; set; }
        
        [Display(Name = "Ikaw ba ay nagtatrabaho? Kung oo, ano ang iyong trabaho?")]
        public string? EducationWorking { get; set; }
        
        [Display(Name = "May problema ka ba sa iskwela o sa trabaho?")]
        public string? EducationSchoolWorkProblems { get; set; }
        
        [Display(Name = "Nakaranas ka bang ma-api (bully) sa iskwela o sa iba pang pagkakataon?")]
        public string? EducationBullying { get; set; }
        
        [Display(Name = "Education/Employment")]
        public string? EducationEmployment { get; set; }
        
        // EATING HABITS
        [Display(Name = "Diet Description")]
        public string? DietDescription { get; set; }

        [Display(Name = "Weight Concerns")]
        public string? WeightConcerns { get; set; }

        [Display(Name = "Eating Disorder Symptoms")]
        public string? EatingDisorderSymptoms { get; set; }

        [Display(Name = "Kuntento ka ba sa iyong itsura, anyo, o sa iyong kasalukuyang timbang?")]
        public string? EatingBodyImageSatisfaction { get; set; }
        
        [Display(Name = "Sinubukan mo na bang magbawas ng timbang sa pamamagitan ng pagsuka ng kinain (vomiting), uminom ng diet pills o pampadumi (laxatives), o kaya ang sadyang hindi pagkain (starvation)?")]
        public string? EatingDisorderedEatingBehaviors { get; set; }
        
        [Display(Name = "May nagpuna na ba na ikaw ay tumataba o pumapayat?")]
        public string? EatingWeightComments { get; set; }
        
        // ACTIVITIES
        [Display(Name = "Hobbies")]
        public string? Hobbies { get; set; }

        [Display(Name = "Physical Activity")]
        public string? PhysicalActivity { get; set; }

        [Display(Name = "Screen Time")]
        public string? ScreenTime { get; set; }

        [Display(Name = "Mayroon ka bang mga sinasalihang sports/ aktibidad sa iskwela, trabaho o bahay?")]
        public string? ActivitiesParticipation { get; set; }
        
        [Display(Name = "Regular ka bang nag-eehersisyo?")]
        public string? ActivitiesRegularExercise { get; set; }
        
        [Display(Name = "Madalas ka bang gumamit ng mga gadgets/internet/computer?")]
        public string? ActivitiesScreenTime { get; set; }
        
        // DRUGS
        [Display(Name = "Substance Use")]
        public string? SubstanceUse { get; set; }

        [Display(Name = "Substance Type")]
        public string? SubstanceType { get; set; }

        [Display(Name = "Naranasan mo na bang gumamit ng mga sumusunod: Tobacco (Sigarilyo)?")]
        public string? DrugsTobaccoUse { get; set; }
        
        [Display(Name = "Naranasan mo na bang gumamit ng mga sumusunod: Alcohol (Alak)?")]
        public string? DrugsAlcoholUse { get; set; }
        
        [Display(Name = "Naranasan mo na bang gumamit ng mga sumusunod: Street drugs/prohibited drugs?")]
        public string? DrugsIllicitDrugUse { get; set; }
        
        // SEXUALITY
        [Display(Name = "Dating Relationships")]
        public string? DatingRelationships { get; set; }

        [Display(Name = "Sexual Activity")]
        public string? SexualActivity { get; set; }

        [Display(Name = "Sexual Orientation")]
        public string? SexualOrientation { get; set; }

        [Display(Name = "Nababahala ka ba sa iyong kalusugan o mga pagbabago ng iyong pangangatawan?")]
        public string? SexualityBodyConcerns { get; set; }
        
        [Display(Name = "Ikaw ba ay may karanasan na sa pakikipagtalik?")]
        public string? SexualityIntimateRelationships { get; set; }
        
        [Display(Name = "Kung oo, ilan na ang taong nakatalik mo sa nakaraang taon?")]
        public string? SexualityPartners { get; set; }
        
        [Display(Name = "Iniisip mo ba na ikaw ay gay, lesbian, o bisexual?")]
        public string? SexualitySexualOrientation { get; set; }
        
        [Display(Name = "Ikaw ba ay nakaranas nang magbuntis o makabuntis?")]
        public string? SexualityPregnancy { get; set; }
        
        [Display(Name = "Ikaw ba ay nagkaroon na ng nakakahawang sakit dulot ng pakikipagtalik?")]
        public string? SexualitySTI { get; set; }
        
        [Display(Name = "May ginagamit ka bang proteksiyon kapag sa tuwing nakikipagtalik?")]
        public string? SexualityProtection { get; set; }
        
        // SUICIDE/DEPRESSION
        [Display(Name = "Mood Changes")]
        public string? MoodChanges { get; set; }

        [Display(Name = "Suicidal Thoughts")]
        public string? SuicidalThoughts { get; set; }

        [Display(Name = "Self Harm Behavior")]
        public string? SelfHarmBehavior { get; set; }

        // SAFETY
        [Display(Name = "Feels Safe At Home")]
        public string? FeelsSafeAtHome { get; set; }

        [Display(Name = "Feels Safe At School")]
        public string? FeelsSafeAtSchool { get; set; }

        [Display(Name = "Experienced Bullying")]
        public string? ExperiencedBullying { get; set; }

        // STRENGTHS
        [Display(Name = "Personal Strengths")]
        public string? PersonalStrengths { get; set; }

        [Display(Name = "Support Systems")]
        public string? SupportSystems { get; set; }

        [Display(Name = "Coping Mechanisms")]
        public string? CopingMechanisms { get; set; }

        // SAFETY/WEAPONS/VIOLENCE
        [Display(Name = "Ikaw ba ay nakakaranas mahipuan/masuntok/masampal/matulak ng kahit sino?")]
        public string? SafetyPhysicalAbuse { get; set; }
        
        [Display(Name = "Nakaranas ka na bang masaktan o takutin ng iyong karelasyon?")]
        public string? SafetyRelationshipViolence { get; set; }
        
        [Display(Name = "Gumagamit ka ba ng helmet o protective gears sa pagsakay ng motor o iba pang uri ng sasakyan?")]
        public string? SafetyProtectiveGear { get; set; }
        
        [Display(Name = "May kasambahay ka bang nagmamay-ari ng baril o rifle?")]
        public string? SafetyGunsAtHome { get; set; }
        
        // SUICIDE/DEPRESSION
        [Display(Name = "Ikaw ba ay nakakaranas na ng pagkalisa o pagkamalungkutin?")]
        public string? SuicideDepressionFeelings { get; set; }
        
        [Display(Name = "Naisip mo na bang saktan ang iyong sarili o wakasan ang iyong buhay?")]
        public string? SuicideSelfHarmThoughts { get; set; }
        
        [Display(Name = "May miyembro ba sa iyong pamilya na nagtangkang magpakamatay o nakaranas ng matinding kalungkutan o labis na pagkabalisa?")]
        public string? SuicideFamilyHistory { get; set; }
        
        // Assessment Information
        [Display(Name = "Assessment Notes")]
        public string? AssessmentNotes { get; set; }

        [Display(Name = "Recommended Actions")]
        public string? RecommendedActions { get; set; }

        [Display(Name = "Follow Up Plan")]
        public string? FollowUpPlan { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
        
        [Display(Name = "Assessed By")]
        public string? AssessedBy { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
} 


