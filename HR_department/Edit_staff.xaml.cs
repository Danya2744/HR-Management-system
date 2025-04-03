using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;

namespace HR_department
{
    public partial class Edit_staff : Window
    {
        private const string ConnectionString = "Server=localhost;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        public Employee SelectedEmployee { get; set; }

        public Edit_staff(Employee employee)
        {
            InitializeComponent();
            SelectedEmployee = employee;
            LastNameTextBox.Text = SelectedEmployee.LastName;
            FirstNameTextBox.Text = SelectedEmployee.FirstName;
            MiddleNameTextBox.Text = SelectedEmployee.MiddleName;
            BirthDatePicker.SelectedDate = SelectedEmployee.BirthDate;
            EducationTextBox.Text = SelectedEmployee.Education;
            HireDatePicker.SelectedDate = SelectedEmployee.HireDate;

            if (!string.IsNullOrEmpty(SelectedEmployee.ContactInfo))
            {
                var digits = new string(SelectedEmployee.ContactInfo.Where(char.IsDigit).ToArray());
                if (digits.Length >= 11) 
                {
                    PhoneTextBox.Text = digits.Substring(1); 
                }
                else if (digits.Length == 10) 
                {
                    PhoneTextBox.Text = digits;
                }
            }

            LoadDepartments();
            LoadPositions();
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT DepartmentID as Id, DepartmentName as Name FROM Departments";
                    SqlCommand command = new SqlCommand(query, connection);

                    var departments = new System.Collections.ObjectModel.ObservableCollection<Department>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }

                    DepartmentComboBox.ItemsSource = departments;
                    DepartmentComboBox.SelectedValue = SelectedEmployee.DepartmentID;
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки отделов: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private void LoadPositions()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT PositionID as Id, PositionName as Name FROM Positions";
                    SqlCommand command = new SqlCommand(query, connection);

                    var positions = new System.Collections.ObjectModel.ObservableCollection<Position>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            positions.Add(new Position
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }

                    PositionComboBox.ItemsSource = positions;
                    PositionComboBox.SelectedValue = SelectedEmployee.PositionID;
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки должностей: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                BirthDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(EducationTextBox.Text) ||
                HireDatePicker.SelectedDate == null ||
                PositionComboBox.SelectedItem == null ||
                DepartmentComboBox.SelectedItem == null)
            {
                var errorMessageBox = new CustomBox("Все поля обязательны для заполнения!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (LastNameTextBox.Text.Length > 50)
            {
                var errorMessageBox = new CustomBox("Фамилия не может превышать 50 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (FirstNameTextBox.Text.Length > 50)
            {
                var errorMessageBox = new CustomBox("Имя не может превышать 50 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (!string.IsNullOrEmpty(MiddleNameTextBox.Text) && MiddleNameTextBox.Text.Length > 50)
            {
                var errorMessageBox = new CustomBox("Отчество не может превышать 50 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (PhoneTextBox.Text.Length != 10 || !PhoneTextBox.Text.All(char.IsDigit))
            {
                var errorMessageBox = new CustomBox("Номер телефона должен содержать ровно 10 цифр!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            DateTime minBirthDate = DateTime.Today.AddYears(-18);
            if (BirthDatePicker.SelectedDate > minBirthDate)
            {
                var errorMessageBox = new CustomBox("Сотрудник должен быть старше 18 лет!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                SelectedEmployee.LastName = LastNameTextBox.Text;
                SelectedEmployee.FirstName = FirstNameTextBox.Text;
                SelectedEmployee.MiddleName = MiddleNameTextBox.Text;
                SelectedEmployee.BirthDate = BirthDatePicker.SelectedDate.Value;
                SelectedEmployee.Education = EducationTextBox.Text;
                SelectedEmployee.HireDate = HireDatePicker.SelectedDate.Value;
                SelectedEmployee.PositionID = (int)PositionComboBox.SelectedValue;
                SelectedEmployee.DepartmentID = (int)DepartmentComboBox.SelectedValue;

                SelectedEmployee.ContactInfo = $"+7({PhoneTextBox.Text.Substring(0, 3)})" +
                                            $"{PhoneTextBox.Text.Substring(3, 3)}-" +
                                            $"{PhoneTextBox.Text.Substring(6, 2)}-" +
                                            $"{PhoneTextBox.Text.Substring(8, 2)}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Staff 
                                   SET LastName = @LastName, 
                                       FirstName = @FirstName, 
                                       MiddleName = @MiddleName,
                                       BirthDate = @BirthDate,
                                       ContactInfo = @ContactInfo,
                                       Education = @Education,
                                       HireDate = @HireDate,
                                       PositionID = @PositionID,
                                       DepartmentID = @DepartmentID
                                   WHERE EmployeeID = @EmployeeID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LastName", SelectedEmployee.LastName);
                    command.Parameters.AddWithValue("@FirstName", SelectedEmployee.FirstName);
                    command.Parameters.AddWithValue("@MiddleName",
                        string.IsNullOrEmpty(SelectedEmployee.MiddleName) ? DBNull.Value : (object)SelectedEmployee.MiddleName);
                    command.Parameters.AddWithValue("@BirthDate", SelectedEmployee.BirthDate);
                    command.Parameters.AddWithValue("@ContactInfo", SelectedEmployee.ContactInfo);
                    command.Parameters.AddWithValue("@Education", SelectedEmployee.Education);
                    command.Parameters.AddWithValue("@HireDate", SelectedEmployee.HireDate);
                    command.Parameters.AddWithValue("@PositionID", SelectedEmployee.PositionID);
                    command.Parameters.AddWithValue("@DepartmentID", SelectedEmployee.DepartmentID);
                    command.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);

                    command.ExecuteNonQuery();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка при обновлении данных: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}