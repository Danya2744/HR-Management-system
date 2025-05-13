using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace HR_department
{
    public partial class Employees : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _currentEmployeeId;
        private readonly MainWindow _authWindow;

        public Employees(int employeeId, MainWindow authWindow = null)
        {
            InitializeComponent();
            _currentEmployeeId = employeeId;
            _authWindow = authWindow;
            this.Closing += Employees_Closing;
        }

        private void Employees_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Создаем новое окно авторизации, если оно не было передано
            var authWindow = _authWindow ?? new MainWindow();
            authWindow.Show();
        }

        // Остальные методы остаются без изменений
        private void Information_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                        command.Parameters.AddWithValue("@EmployeeID", _currentEmployeeId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                        string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                    BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                    ContactInfo = reader.IsDBNull(reader.GetOrdinal("ContactInfo")) ?
                                        string.Empty : reader.GetString(reader.GetOrdinal("ContactInfo")),
                                    Education = reader.GetString(reader.GetOrdinal("Education")),
                                    HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                                    Department = reader.GetString(reader.GetOrdinal("Department")),
                                    Position = reader.GetString(reader.GetOrdinal("Position")),
                                    Login = reader.IsDBNull(reader.GetOrdinal("Login")) ?
                                        string.Empty : reader.GetString(reader.GetOrdinal("Login"))
                                };
                                var employeeInfoWindow = new EmployeeInfo(employee, _currentEmployeeId);
                                employeeInfoWindow.ShowDialog();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomBox customMessageBox = new CustomBox($"Ошибка при загрузке данных об отпусках: {ex.Message}", false);
                customMessageBox.ShowDialog();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sickLeavesWindow = new SickLeavesWindow(_currentEmployeeId);
            sickLeavesWindow.Owner = this;
            sickLeavesWindow.ShowDialog();
        }

        private void VacationButton_Click(object sender, RoutedEventArgs e)
        {
            var vacationWindow = new VacationWindow(_currentEmployeeId);
            vacationWindow.Owner = this;
            vacationWindow.ShowDialog();
        }

        private void CertificationButton_Click(object sender, RoutedEventArgs e)
        {
            var certificationWindow = new CertificationWindow(_currentEmployeeId);
            certificationWindow.Owner = this;
            certificationWindow.ShowDialog();
        }

        private void AchievementsButton_Click(object sender, RoutedEventArgs e)
        {
            var achievementsWindow = new AchievementsWindow(_currentEmployeeId);
            achievementsWindow.Owner = this;
            achievementsWindow.ShowDialog();
        }
    }
}