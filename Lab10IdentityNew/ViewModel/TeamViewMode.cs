using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;

namespace ToDo.ViewModel
{
    public class TeamViewMode
    {
        public Team Team { get; set; }
        public IEnumerable<SelectListItem> AllUsers { get; set; }

        public TeamViewMode(){
            AllUsers = new List<SelectListItem>();
        }

        private List<string> _selectedUsers;
        public List<string> SelectedUsers
        {
            get
            {
                if (_selectedUsers  == null)
                {
                    _selectedUsers = Team.ApplicationUsers.Select(u => u.Id).ToList();
                }
                return _selectedUsers;
            }
            set { _selectedUsers = value; }
        }
    }
}