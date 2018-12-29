using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ToDo.Enum;

namespace ToDo.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public Status Status { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }


        public virtual ICollection<Comment> Comments { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}