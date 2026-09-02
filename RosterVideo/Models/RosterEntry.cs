using System;
using System.ComponentModel.DataAnnotations;

namespace RosterVideo.Models
{
    public class RosterEntry
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Major { get; set; }

        [Required]
        [StringLength(100)]
        public string Shortcut { get; set; } = string.Empty;

        [StringLength(200)]
        public string? WhereUsed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
