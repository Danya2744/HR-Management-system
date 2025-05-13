using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HR_department
{
    public partial class EmployeeInfo : Window
    {
        private readonly Employee _employee;
        private readonly int _employeeId; 

        public EmployeeInfo(Employee employee, int employeeId)
        {
            InitializeComponent();
            _employee = employee;
            _employeeId = employeeId; 
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            if (_employee != null)
            {
                LastNameTextBox.Text = _employee.LastName;
                FirstNameTextBox.Text = _employee.FirstName;
                MiddleNameTextBox.Text = _employee.MiddleName;
                BirthDateTextBox.Text = _employee.BirthDate.ToShortDateString();
                PhoneTextBox.Text = _employee.ContactInfo;
                EducationTextBox.Text = _employee.Education;
                HireDateTextBox.Text = _employee.HireDate.ToShortDateString();
                PositionTextBox.Text = _employee.PositionName;
                DepartmentTextBox.Text = _employee.DepartmentName;
                EmailTextBox.Text = string.IsNullOrEmpty(_employee.Login) ?
                    "не указан" : $"{_employee.Login}@edu.fa.ru";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
