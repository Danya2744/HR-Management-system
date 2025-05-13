using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace HR_department
{
    public partial class Edit_staff : Window
    {
        private const string ConnectionString = "Server=localhost;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        public Employee SelectedEmployee { get; set; }
        private bool _isNewUser = true;

        public Edit_staff(Employee employee)
        {
            InitializeComponent();
            SelectedEmployee = employee;

            LoadEmployeeData();
            LoadDepartments();
            LoadPositions();
            LoadUserData();
            LoadStatuses();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void LoadEmployeeData()
        {
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
        }

        private void LoadUserData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT u.Login_user, u.StatusID, s.Name_status 
                                   FROM Users u
                                   LEFT JOIN Status_user s ON u.StatusID = s.StatusID
                                   WHERE u.EmployeeID = @EmployeeID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _isNewUser = false;
                            LoginTextBox.Text = reader["Login_user"].ToString();
                            PasswordTextBox.Text = "[Оставить текущий пароль]";
                            PasswordTextBox.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
                        }
                        else
                        {
                            _isNewUser = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки учетных данных: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID as Id, Name_status as Name FROM Status_user";
                    SqlCommand command = new SqlCommand(query, connection);

                    var statuses = new System.Collections.ObjectModel.ObservableCollection<Status>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statuses.Add(new Status
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }

                    StatusComboBox.ItemsSource = statuses;
                    StatusComboBox.DisplayMemberPath = "Name";
                    StatusComboBox.SelectedValuePath = "Id";

                    if (!_isNewUser)
                    {
                        string getStatusQuery = "SELECT StatusID FROM Users WHERE EmployeeID = @EmployeeID";
                        SqlCommand getStatusCommand = new SqlCommand(getStatusQuery, connection);
                        getStatusCommand.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);
                        var statusId = getStatusCommand.ExecuteScalar();
                        if (statusId != null)
                        {
                            StatusComboBox.SelectedValue = (int)statusId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки статусов: {ex.Message}", false);
                errorMessageBox.ShowDialog();
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
                    DepartmentComboBox.DisplayMemberPath = "Name";
                    DepartmentComboBox.SelectedValuePath = "Id";

                    if (SelectedEmployee.DepartmentID > 0)
                    {
                        DepartmentComboBox.SelectedValue = SelectedEmployee.DepartmentID;
                    }
                    else if (departments.Count > 0)
                    {
                        DepartmentComboBox.SelectedIndex = 0;
                    }
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
                    PositionComboBox.DisplayMemberPath = "Name";
                    PositionComboBox.SelectedValuePath = "Id";

                    if (SelectedEmployee.PositionID > 0)
                    {
                        PositionComboBox.SelectedValue = SelectedEmployee.PositionID;
                    }
                    else if (positions.Count > 0)
                    {
                        PositionComboBox.SelectedIndex = 0;
                    }
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

            if (!string.IsNullOrEmpty(LoginTextBox.Text))
            {
                if (LoginTextBox.Text.Length < 12)
                {
                    var errorMessageBox = new CustomBox("Логин должен содержать не менее 12 символов!", false);
                    errorMessageBox.ShowDialog();
                    return false;
                }

                if (_isNewUser && (string.IsNullOrEmpty(PasswordTextBox.Text) ||
                    PasswordTextBox.Text == "[Оставить текущий пароль]"))
                {
                    var errorMessageBox = new CustomBox("Для нового пользователя необходимо указать пароль!", false);
                    errorMessageBox.ShowDialog();
                    return false;
                }

                if (!_isNewUser && PasswordTextBox.Text != "[Оставить текущий пароль]" &&
                    PasswordTextBox.Text.Length < 6)
                {
                    var errorMessageBox = new CustomBox("Пароль должен содержать не менее 6 символов!", false);
                    errorMessageBox.ShowDialog();
                    return false;
                }

                if (StatusComboBox.SelectedItem == null)
                {
                    var errorMessageBox = new CustomBox("Необходимо выбрать статус пользователя!", false);
                    errorMessageBox.ShowDialog();
                    return false;
                }
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var confirmBox = new CustomBox("Вы уверены, что хотите сохранить изменения? Проверьте все данные перед сохранением.", true);
            if (confirmBox.ShowDialog() != true) return;

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
                    string updateStaffQuery = @"UPDATE Staff 
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

                    SqlCommand updateStaffCommand = new SqlCommand(updateStaffQuery, connection);
                    updateStaffCommand.Parameters.AddWithValue("@LastName", SelectedEmployee.LastName);
                    updateStaffCommand.Parameters.AddWithValue("@FirstName", SelectedEmployee.FirstName);
                    updateStaffCommand.Parameters.AddWithValue("@MiddleName",
                        string.IsNullOrEmpty(SelectedEmployee.MiddleName) ? DBNull.Value : (object)SelectedEmployee.MiddleName);
                    updateStaffCommand.Parameters.AddWithValue("@BirthDate", SelectedEmployee.BirthDate);
                    updateStaffCommand.Parameters.AddWithValue("@ContactInfo", SelectedEmployee.ContactInfo);
                    updateStaffCommand.Parameters.AddWithValue("@Education", SelectedEmployee.Education);
                    updateStaffCommand.Parameters.AddWithValue("@HireDate", SelectedEmployee.HireDate);
                    updateStaffCommand.Parameters.AddWithValue("@PositionID", SelectedEmployee.PositionID);
                    updateStaffCommand.Parameters.AddWithValue("@DepartmentID", SelectedEmployee.DepartmentID);
                    updateStaffCommand.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);

                    updateStaffCommand.ExecuteNonQuery();

                    if (!string.IsNullOrEmpty(LoginTextBox.Text))
                    {
                        if (_isNewUser)
                        {
                            string createUserQuery = @"INSERT INTO Users 
                                (Login_user, Password_user, EmployeeID, StatusID) 
                                VALUES (@Login, @Password, @EmployeeID, @StatusID)";

                            SqlCommand createUserCommand = new SqlCommand(createUserQuery, connection);
                            createUserCommand.Parameters.AddWithValue("@Login", LoginTextBox.Text);
                            createUserCommand.Parameters.AddWithValue("@Password", HashPassword(PasswordTextBox.Text));
                            createUserCommand.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);
                            createUserCommand.Parameters.AddWithValue("@StatusID", StatusComboBox.SelectedValue);
                            createUserCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            string updateUserQuery = @"UPDATE Users SET 
                                Login_user = @Login, 
                                StatusID = @StatusID
                                {0}
                                WHERE EmployeeID = @EmployeeID";

                            if (PasswordTextBox.Text != "[Оставить текущий пароль]")
                            {
                                updateUserQuery = string.Format(updateUserQuery, ", Password_user = @Password");
                            }
                            else
                            {
                                updateUserQuery = string.Format(updateUserQuery, "");
                            }

                            SqlCommand updateUserCommand = new SqlCommand(updateUserQuery, connection);
                            updateUserCommand.Parameters.AddWithValue("@Login", LoginTextBox.Text);
                            updateUserCommand.Parameters.AddWithValue("@StatusID", StatusComboBox.SelectedValue);
                            updateUserCommand.Parameters.AddWithValue("@EmployeeID", SelectedEmployee.EmployeeID);

                            if (PasswordTextBox.Text != "[Оставить текущий пароль]")
                            {
                                updateUserCommand.Parameters.AddWithValue("@Password", HashPassword(PasswordTextBox.Text));
                            }

                            updateUserCommand.ExecuteNonQuery();
                        }
                    }

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка при обновлении данных: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var confirmBox = new CustomBox("Вы уверены, что хотите отменить изменения? Все несохраненные данные будут потеряны.", true);
            if (confirmBox.ShowDialog() == true)
            {
                DialogResult = false;
                Close();
            }
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text == "[Оставить текущий пароль]")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text) && !_isNewUser)
            {
                PasswordTextBox.Text = "[Оставить текущий пароль]";
                PasswordTextBox.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                if (tabControl.SelectedIndex == 0)
                {
                    this.Height = 835.6;
                }
                else
                {
                    this.Height = 650;
                }
            }
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

    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}