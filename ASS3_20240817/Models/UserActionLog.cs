using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASS2_20240802.Models
{
    public class UserActionLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string ActionName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ActionTime { get; set; }
    }

    public class ActionCountViewModel
    {
        public string ActionName { get; set; }
        public int Count { get; set; }
    }
}
