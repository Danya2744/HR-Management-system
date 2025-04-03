using System;
using System.Linq;
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

        private void Add_user_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved) return;

            e.Cancel = true;
            ShowCustomMessageBox("Необходимо сохранить данные перед выходом!", "Предупреждение", false);
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID, Name_status FROM Status_user";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    StatusComboBox.Items.Clear();
                    while (reader.Read())
                    {
                        StatusComboBox.Items.Add(new
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                    StatusComboBox.DisplayMemberPath = "Name";
                    StatusComboBox.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка загрузки статусов: {ex.Message}", "Ошибка", false);
            }
        }

        public bool ValidateUserInputs(string login, string password, object status)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                ShowCustomMessageBox("Логин не может быть пустым!", "Ошибка", false);
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowCustomMessageBox("Пароль не может быть пустым!", "Ошибка", false);
                return false;
            }

            if (status == null)
            {
                ShowCustomMessageBox("Необходимо выбрать статус пользователя!", "Ошибка", false);
                return false;
            }

            if (login.Length > 50)
            {
                ShowCustomMessageBox("Логин не может превышать 50 символов!", "Ошибка", false);
                return false;
            }

            if (password.Length > 15)
            {
                ShowCustomMessageBox("Пароль не должен содержать более 15 символов!", "Ошибка", false);
                return false;
            }

            if (password.Length < 6)
            {
                ShowCustomMessageBox("Пароль должен содержать не менее 6 символов!", "Ошибка", false);
                return false;
            }

            return true;
        }

        public bool SaveUser(int employeeId, string login, string password, object status)
        {
            if (!ValidateUserInputs(login, password, status))
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Login_user = @Login";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Login", login);
                    int exists = (int)checkCommand.ExecuteScalar();

                    if (exists > 0)
                    {
                        ShowCustomMessageBox("Пользователь с таким логином уже существует!", "Ошибка", false);
                        return false;
                    }

                    string query = @"INSERT INTO Users 
                                    (Login_user, Password_user, EmployeeID, StatusID)
                                    VALUES 
                                    (@Login, @Password, @EmployeeID, @StatusID)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);
                    command.Parameters.AddWithValue("@StatusID", (status as dynamic)?.Id ?? 0);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                ShowCustomMessageBox($"Ошибка при создании пользователя: {ex.Message}", "Ошибка", false);
                return false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveUser(_employeeId, LoginTextBox.Text, PasswordBox.Password, StatusComboBox.SelectedItem))
            {
                _isSaved = true;
                ShowCustomMessageBox("Пользователь успешно создан!", "Успех", false);
                Staff staff = new Staff();
                staff.Show();
                this.Close();
            }
        }

        private void ShowCustomMessageBox(string message, string title, bool showCancelButton = true)
        {
            CustomBox customBox = new CustomBox(message, showCancelButton)
            {
                Title = title
            };
            customBox.ShowDialog();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomMessageBox("Необходимо сохранить данные!", "Предупреждение", false);
        }
    }
}