using System;
using System.Collections.Generic;

namespace KBT.WebAPI.Training.Example.Entities.Demo
{
    public partial class User
    {
        public int UserKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public int? EmployeeKey { get; set; }

        public virtual Employee EmployeeKeyNavigation { get; set; }
    }
}
