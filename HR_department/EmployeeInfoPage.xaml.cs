using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HR_department
{
    /// <summary>
    /// Логика взаимодействия для EmployeeInfoPage.xaml
    /// </summary>
    public partial class EmployeeInfoPage : Page
    {
        private readonly int _employeeId;
        private readonly string _connectionString;

        public EmployeeInfoPage(int employeeId, string connectionString)
        {
            InitializeComponent();
            _employeeId = employeeId;
            _connectionString = connectionString;
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT 
                                    s.LastName, 
                                    s.FirstName, 
                                    s.MiddleName, 
                                    s.BirthDate,
                                    s.ContactInfo,
                                    s.Education,
                                    s.HireDate,
                                    d.DepartmentName AS Department, 
                                    p.PositionName AS Position,
                                    u.Login_user AS Login
                                FROM 
                                    Staff s
                                JOIN 
                                    Departments d ON s.DepartmentID = d.DepartmentID
                                JOIN 
                                    Positions p ON s.PositionID = p.PositionID
                                JOIN
                                    Users u ON s.EmployeeID = u.EmployeeID
                                WHERE 
                                    s.EmployeeID = @EmployeeID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LastNameTextBox.Text = reader["LastName"].ToString();
                                FirstNameTextBox.Text = reader["FirstName"].ToString();
                                MiddleNameTextBox.Text = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader["MiddleName"].ToString();
                                BirthDateTextBox.Text = Convert.ToDateTime(reader["BirthDate"]).ToShortDateString();
                                PhoneTextBox.Text = reader["ContactInfo"].ToString();
                                EducationTextBox.Text = reader["Education"].ToString();
                                HireDateTextBox.Text = Convert.ToDateTime(reader["HireDate"]).ToShortDateString();
                                DepartmentTextBox.Text = reader["Department"].ToString();
                                PositionTextBox.Text = reader["Position"].ToString();
                                EmailTextBox.Text = string.IsNullOrEmpty(reader["Login"].ToString()) ?
                                    "не указан" : $"{reader["Login"]}@edu.fa.ru";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomBox customMessageBox = new CustomBox($"Ошибка при загрузке данных: {ex.Message}", false);
                customMessageBox.ShowDialog();
            }
        }
    }
}
