using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Api.Models
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Assignments = new HashSet<Assignment>();
            Calendars = new HashSet<Calendar>();
        }

        [Key]
        public Guid UserId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        [StringLength(100)]
        public string Password { get; set; } = null!;
        public string Image { get; set; } = null!;

        [InverseProperty("User")]
        public virtual ICollection<Assignment> Assignments { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Calendar> Calendars { get; set; }
    }
}
