using System;
using System.Collections.Generic;

namespace DiChoThue.AuthenticationService.Models
{
    public class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public short RoleId { get; set; }
        public string RoleDesc { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
