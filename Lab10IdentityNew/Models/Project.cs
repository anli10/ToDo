using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ToDo.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public int TeamId { get; set; }
        public virtual Team Team { get; set; }

        public string EditorId { get; set; }
        public virtual ApplicationUser Editor { get; set; }

        public virtual ICollection <Activity> Activities { get; set; }
    }


}