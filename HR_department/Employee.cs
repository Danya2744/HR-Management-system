using System;
using System.ComponentModel;

namespace HR_department
{
    public class Employee : INotifyPropertyChanged
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
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public string Login { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
        public string BirthDateString => BirthDate.ToShortDateString();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}