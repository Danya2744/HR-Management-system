using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_department
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ContactInfo { get; set; }
        public string Education { get; set; }
        public DateTime HireDate { get; set; }
        public int PositionID { get; set; }
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Login { get; set; }
    }
}
