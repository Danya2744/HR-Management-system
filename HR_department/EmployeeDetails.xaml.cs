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
    /// <summary>
    /// Логика взаимодействия для EmployeeDetails.xaml
    /// </summary>
    public partial class EmployeeDetails : Window
    {
        public EmployeeDetails(Employee employee)
        {
            InitializeComponent();
            //LastNameTextBox.Text = employee.LastName;
            //FirstNameTextBox.Text = employee.FirstName;
            //MiddleNameTextBox.Text = employee.MiddleName;
            //BirthDateTextBox.Text = employee.BirthDate;
            //PhoneTextBox.Text = employee.Phone;
            //EmailTextBox.Text = employee.Email;
            //EducationTextBox.Text = employee.Education;
            //HireDateTextBox.Text = employee.HireDate;
            //PositionTextBox.Text = employee.Position;
            //DepartmentTextBox.Text = employee.Department;
        }
    }
}
