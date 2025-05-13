using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace HR_department
{
    public partial class Add_user : Window
    {
        private readonly int _employeeId;
        private const string ConnectionString = "Server=localhost;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private bool _isSaved = false;

        public Add_user(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadStatuses();
            this.Closing += Add_user_Closing;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private void Add_user_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved) return;

            var confirmBox = new CustomBox("Вы уверены, что хотите закрыть окно без сохранения? Все несохраненные данные будут потеряны.", true);
            if (confirmBox.ShowDialog() == true)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
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
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки статусов: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                var errorMessageBox = new CustomBox("Логин не может быть пустым!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                var errorMessageBox = new CustomBox("Пароль не может быть пустым!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                var errorMessageBox = new CustomBox("Необходимо выбрать статус пользователя!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (LoginTextBox.Text.Length < 12)
            {
                var errorMessageBox = new CustomBox("Логин должен содержать не менее 12 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (LoginTextBox.Text.Length > 30)
            {
                var errorMessageBox = new CustomBox("Логин не может превышать 30 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (PasswordBox.Password.Length < 6)
            {
                var errorMessageBox = new CustomBox("Пароль должен содержать не менее 6 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            if (PasswordBox.Password.Length > 15)
            {
                var errorMessageBox = new CustomBox("Пароль не должен содержать более 15 символов!", false);
                errorMessageBox.ShowDialog();
                return false;
            }

            return true;
        }

        private bool SaveUser()
        {
            if (!ValidateInputs())
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string checkLoginQuery = "SELECT COUNT(*) FROM Users WHERE Login_user = @Login";
                    SqlCommand checkLoginCommand = new SqlCommand(checkLoginQuery, connection);
                    checkLoginCommand.Parameters.AddWithValue("@Login", LoginTextBox.Text);
                    int loginExists = (int)checkLoginCommand.ExecuteScalar();

                    if (loginExists > 0)
                    {
                        var errorMessageBox = new CustomBox("Пользователь с таким логином уже существует!", false);
                        errorMessageBox.ShowDialog();
                        return false;
                    }

                    string checkEmployeeQuery = "SELECT COUNT(*) FROM Users WHERE EmployeeID = @EmployeeID";
                    SqlCommand checkEmployeeCommand = new SqlCommand(checkEmployeeQuery, connection);
                    checkEmployeeCommand.Parameters.AddWithValue("@EmployeeID", _employeeId);
                    int userExists = (int)checkEmployeeCommand.ExecuteScalar();

                    if (userExists > 0)
                    {
                        var errorMessageBox = new CustomBox("Для этого сотрудника уже существует учетная запись!", false);
                        errorMessageBox.ShowDialog();
                        return false;
                    }

                    SqlCommand command = new SqlCommand("CreateUserWithHashedPassword", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Login", LoginTextBox.Text);
                    command.Parameters.AddWithValue("@PlainPassword", PasswordBox.Password);
                    command.Parameters.AddWithValue("@StatusID", StatusComboBox.SelectedValue);
                    command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка при создании пользователя: {ex.Message}", false);
                errorMessageBox.ShowDialog();
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var confirmBox = new CustomBox("Вы уверены, что хотите создать нового пользователя?", true);
            if (confirmBox.ShowDialog() != true) return;

            if (SaveUser())
            {
                _isSaved = true;
                var successMessageBox = new CustomBox("Пользователь успешно создан!", false);
                successMessageBox.ShowDialog();
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var confirmBox = new CustomBox("Вы уверены, что хотите отменить создание пользователя? Все введенные данные будут потеряны.", true);
            if (confirmBox.ShowDialog() == true)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}