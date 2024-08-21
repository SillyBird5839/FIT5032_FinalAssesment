using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASS2_20240802.Models
{
    public class DoctorInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DoctorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Major { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }
    }
}
