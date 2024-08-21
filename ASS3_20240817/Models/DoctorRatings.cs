using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASS2_20240802.Models
{
    public class DoctorRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RatingId { get; set; }

        [StringLength(128)]
        public string DoctorId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public int? Score { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
