using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToDo.Models
{
    public class Team
    {
       public Team()
       {
           ApplicationUsers = new HashSet<ApplicationUser>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        //public int[] SelectedUsers { get; set; }
        //public IEnumerable<ApplicationUser> Users { get; set; }
    }
}