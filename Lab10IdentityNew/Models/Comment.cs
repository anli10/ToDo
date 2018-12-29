using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ToDo.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
    }


}