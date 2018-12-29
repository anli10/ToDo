using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToDo.Enum
{
    public enum Status
    {
        [Display(Name = "Not Started")]
        Not_Started,
        [Display(Name = "In Progress")]
        In_progress,
        Completed
    }
}