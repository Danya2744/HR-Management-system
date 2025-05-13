using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HR_department
{
    public partial class AddAchievementWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";

        public int SelectedEmployeeID { get; private set; }
        public DateTime AchievementDate { get; private set; }
        public string AchievementType { get; private set; }
        public string Description { get; private set; }
        public string Reward { get; private set; }
        public string Status { get; internal set; }

        public AddAchievementWindow()
        {
            InitializeComponent();
            LoadEmployees();
            AchievementDatePicker.SelectedDate = DateTime.Today;

            AchievementTypeTextBox.TextChanged += ValidateInput;
            RewardTextBox.TextChanged += ValidateInput;
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT EmployeeID, 
                                            LastName + ' ' + FirstName + ' ' + ISNULL(MiddleName, '') AS FullName
                                     FROM Staff
                                     ORDER BY LastName, FirstName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dt = new DataTable();
                        dt.Load(command.ExecuteReader());
                        EmployeeComboBox.ItemsSource = dt.DefaultView;
                        EmployeeComboBox.DisplayMemberPath = "FullName";
                        EmployeeComboBox.SelectedValuePath = "EmployeeID";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private void ValidateInput(object sender, TextChangedEventArgs e)
        {
            ClearValidationErrors();
            if (sender is TextBox textBox)
            {
                if (textBox == AchievementTypeTextBox && textBox.Text.Length < 5)
                    textBox.BorderBrush = Brushes.Red;
                else if (textBox == RewardTextBox && textBox.Text.Length < 20)
                    textBox.BorderBrush = Brushes.Red;
            }
        }

        private bool ValidateFields()
        {
            bool isValid = true;
            ClearValidationErrors();

            if (EmployeeComboBox.SelectedItem == null)
            {
                EmployeeComboBox.BorderBrush = Brushes.Red;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(AchievementTypeTextBox.Text) ||
                AchievementTypeTextBox.Text.Length < 5 ||
                AchievementTypeTextBox.Text.Length > 50)
            {
                AchievementTypeTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(RewardTextBox.Text) ||
                RewardTextBox.Text.Length < 20 ||
                RewardTextBox.Text.Length > 50)
            {
                RewardTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }

            if (!isValid)
            {
                ShowError("Пожалуйста, заполните все обязательные поля:\n" +
                          "• Выберите сотрудника\n" +
                          "• Тип достижения: 5-50 символов\n" +
                          "• Награда: 20-50 символов");
            }

            return isValid;
        }

        private void ClearValidationErrors()
        {
            EmployeeComboBox.ClearValue(ComboBox.BorderBrushProperty);
            AchievementTypeTextBox.ClearValue(TextBox.BorderBrushProperty);
            RewardTextBox.ClearValue(TextBox.BorderBrushProperty);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            var confirmResult = new CustomBox("Вы уверены, что хотите сохранить достижение?", true) { Owner = this };
            if (confirmResult.ShowDialog() == true && confirmResult.Result)
            {
                try
                {
                    SelectedEmployeeID = (int)EmployeeComboBox.SelectedValue;
                    AchievementDate = AchievementDatePicker.SelectedDate ?? DateTime.Today;
                    AchievementType = AchievementTypeTextBox.Text.Trim();
                    Description = DescriptionTextBox.Text.Trim();
                    Reward = RewardTextBox.Text.Trim();

                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = @"INSERT INTO Achievements 
                                        (EmployeeID, AchievementDate, AchievementType, Description, Reward, Status)
                                        VALUES (@EmployeeID, @Date, @Type, @Description, @Reward, @Status)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", SelectedEmployeeID);
                            command.Parameters.AddWithValue("@Date", AchievementDate);
                            command.Parameters.AddWithValue("@Type", AchievementType);
                            command.Parameters.AddWithValue("@Description",
                                string.IsNullOrWhiteSpace(Description) ? DBNull.Value : (object)Description);
                            command.Parameters.AddWithValue("@Reward",
                                string.IsNullOrWhiteSpace(Reward) ? DBNull.Value : (object)Reward);
                            command.Parameters.AddWithValue("@Status", "Присуждена");

                            command.ExecuteNonQuery();
                        }
                    }

                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    ShowError($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDataEntered())
            {
                var confirmResult = new CustomBox("Все несохраненные данные будут потеряны. Продолжить?", true) { Owner = this };
                if (confirmResult.ShowDialog() != true || !confirmResult.Result) return;
            }
            DialogResult = false;
            Close();
        }

        private bool IsDataEntered()
        {
            return EmployeeComboBox.SelectedItem != null ||
                   !string.IsNullOrWhiteSpace(AchievementTypeTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(RewardTextBox.Text);
        }

        private void ShowError(string message)
        {
            new CustomBox(message, false) { Owner = this }.ShowDialog();
        }
    }
}