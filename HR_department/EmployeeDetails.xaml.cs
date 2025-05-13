using System;
using System.Data.SqlClient;
using System.Windows;

namespace HR_department
{
    public partial class EmployeeDetails : Window
    {
        public EmployeeDetails(Employee employee)
        {
            InitializeComponent();
            LoadEmployeeData(employee);
        }

        private void LoadEmployeeData(Employee employee)
        {
            LastNameTextBox.Text = employee.LastName;
            FirstNameTextBox.Text = employee.FirstName;
            MiddleNameTextBox.Text = employee.MiddleName;
            BirthDateTextBox.Text = employee.BirthDate.ToShortDateString();
            PhoneTextBox.Text = employee.ContactInfo;
            EducationTextBox.Text = employee.Education;
            HireDateTextBox.Text = employee.HireDate.ToShortDateString();
            PositionTextBox.Text = employee.PositionName;
            DepartmentTextBox.Text = employee.DepartmentName;
            EmailTextBox.Text = $"{employee.Login}@edu.fa.ru";
        }

        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
    }
}