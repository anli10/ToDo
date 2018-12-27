using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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


        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }


        //public int CommentId { get; set; }
        //public virtual Comment Comment { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Se aduaga acest atribut pentru a putea prelua toate categoriile unui model in helper
        //public IEnumerable<SelectListItem> Comments{ get; set; }
    }
}