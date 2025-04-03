using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;

namespace HR_department
{
    public partial class Add_staff : Window
    {
        private const string ConnectionString = "Server=localhost;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private bool _isSaved = false;

        public Add_staff()
        {
            InitializeComponent();
            LoadDepartments();
            LoadPositions();
            HireDatePicker.SelectedDate = DateTime.Today;
            this.Closing += Add_staff_Closing;
        }

        private void Add_staff_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved) return;

            var result = new CustomBox("Вы уверены, что хотите выйти? Все несохраненные данные будут потеряны.", true)
            {
                Title = "Подтверждение выхода"
            };
            result.ShowDialog();

            if (!result.Result)
            {
                e.Cancel = true;
            }
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void LettersOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-ЯёЁ\-]+$"))
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

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Department> departments = new List<Department>();
                        while (reader.Read())
                        {
                            departments.Add(new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                        DepartmentComboBox.ItemsSource = departments;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка загрузки отделов: {ex.Message}", "Ошибка");
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

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Position> positions = new List<Position>();
                        while (reader.Read())
                        {
                            positions.Add(new Position
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                        PositionComboBox.ItemsSource = positions;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка загрузки должностей: {ex.Message}", "Ошибка");
            }
        }

        public bool ValidateEmployeeInputs(string lastName, string firstName, string middleName,
                                         DateTime? birthDate, string phone, string education,
                                         DateTime? hireDate, object position, object department)
        {
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) ||
                birthDate == null || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(education) || hireDate == null ||
                position == null || department == null)
            {
                ShowCustomMessageBox("Все поля обязательны для заполнения!", "Ошибка");
                return false;
            }

            if (lastName.Length > 50)
            {
                ShowCustomMessageBox("Фамилия не может превышать 50 символов!", "Ошибка");
                return false;
            }

            if (firstName.Length > 50)
            {
                ShowCustomMessageBox("Имя не может превышать 50 символов!", "Ошибка");
                return false;
            }

            if (!string.IsNullOrEmpty(middleName) && middleName.Length > 50)
            {
                ShowCustomMessageBox("Отчество не может превышать 50 символов!", "Ошибка");
                return false;
            }

            if (phone.Length != 10 || !phone.All(char.IsDigit))
            {
                ShowCustomMessageBox("Номер телефона должен содержать ровно 10 цифр (без +7 и других символов)!", "Ошибка");
                return false;
            }

            if (education.Length > 200)
            {
                ShowCustomMessageBox("Образование не может превышать 200 символов!", "Ошибка");
                return false;
            }

            DateTime minBirthDate = DateTime.Today.AddYears(-18);
            if (birthDate > minBirthDate)
            {
                ShowCustomMessageBox("Сотрудник должен быть старше 18 лет!", "Ошибка");
                return false;
            }

            return true;
        }

        public bool SaveEmployee(string lastName, string firstName, string middleName,
                                DateTime? birthDate, string phone, string education,
                                DateTime? hireDate, object position, object department)
        {
            if (!ValidateEmployeeInputs(lastName, firstName, middleName, birthDate, phone,
                                      education, hireDate, position, department))
            {
                return false;
            }

            try
            {
                string phoneDigits = phone;
                string formattedPhone = $"+7({phoneDigits.Substring(0, 3)}){phoneDigits.Substring(3, 3)}-{phoneDigits.Substring(6, 2)}-{phoneDigits.Substring(8, 2)}";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Staff 
                                   (LastName, FirstName, MiddleName, BirthDate, ContactInfo, Education, HireDate, PositionID, DepartmentID)
                                   VALUES 
                                   (@LastName, @FirstName, @MiddleName, @BirthDate, @ContactInfo, @Education, @HireDate, @PositionID, @DepartmentID);
                                   SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(middleName) ? DBNull.Value : (object)middleName);
                    command.Parameters.AddWithValue("@BirthDate", birthDate);
                    command.Parameters.AddWithValue("@ContactInfo", formattedPhone);
                    command.Parameters.AddWithValue("@Education", education);
                    command.Parameters.AddWithValue("@HireDate", hireDate);
                    command.Parameters.AddWithValue("@PositionID", (position as Position)?.Id ?? 0);
                    command.Parameters.AddWithValue("@DepartmentID", (department as Department)?.Id ?? 0);

                    int employeeId = Convert.ToInt32(command.ExecuteScalar());
                    return employeeId > 0;
                }
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка при сохранении сотрудника: {ex.Message}", "Ошибка");
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveEmployee(
                LastNameTextBox.Text,
                FirstNameTextBox.Text,
                MiddleNameTextBox.Text,
                BirthDatePicker.SelectedDate,
                PhoneTextBox.Text,
                EducationTextBox.Text,
                HireDatePicker.SelectedDate,
                PositionComboBox.SelectedItem,
                DepartmentComboBox.SelectedItem))
            {
                _isSaved = true;
                int employeeId = GetLastEmployeeId();
                Add_user addUserWindow = new Add_user(employeeId);
                addUserWindow.Show();
                this.Close();
            }
        }

        private int GetLastEmployeeId()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT MAX(EmployeeID) FROM Staff";
                SqlCommand command = new SqlCommand(query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void ShowCustomMessageBox(string message, string title)
        {
            CustomBox customBox = new CustomBox(message)
            {
                Title = title
            };
            customBox.ShowDialog();
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
}