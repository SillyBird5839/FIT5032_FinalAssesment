namespace ASS2_20240802.Models
{
    public class DoctorViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Major { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DoctorRatingViewModel
    {
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Major { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public double? AverageScore { get; set; }
    }

        public class MedicationViewModel
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string UserEmail { get; set; }
            public string PrescribingDoctorUserId { get; set; }
            public string PrescribingDoctorEmail { get; set; }
            public string MedicationName { get; set; }
            public string MedicationDescription { get; set; }
            public string DosageInstructions { get; set; }

            public bool CanEdit { get; set; }
    }
    
}
