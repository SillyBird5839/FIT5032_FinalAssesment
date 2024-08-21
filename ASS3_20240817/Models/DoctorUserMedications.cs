using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASS2_20240802.Models
{
    public class DoctorUserMedication
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }

        [MaxLength(128)]
        public string PrescribingDoctorUserId { get; set; }

        [MaxLength(256)]
        public string MedicationName { get; set; }

        public string MedicationDescription { get; set; }

        [MaxLength(256)]
        public string DosageInstructions { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
