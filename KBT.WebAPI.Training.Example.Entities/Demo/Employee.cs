using System;
using System.Collections.Generic;

namespace KBT.WebAPI.Training.Example.Entities.Demo
{
    public partial class Employee
    {
        public Employee()
        {
            Users = new HashSet<User>();
        }

        public int EmployeeKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
