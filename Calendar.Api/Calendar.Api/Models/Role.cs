using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Api.Models
{
    [Table("Role")]
    public partial class Role
    {
        public Role()
        {
            Assignments = new HashSet<Assignment>();
        }

        [Key]
        public Guid RoleId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public int RoleLevel { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
