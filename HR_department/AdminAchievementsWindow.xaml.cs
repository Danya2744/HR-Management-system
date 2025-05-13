using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HR_department
{
    public partial class AdminAchievementsWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private ObservableCollection<EmployeeAchievement> _allAchievements = new ObservableCollection<EmployeeAchievement>();

        public AdminAchievementsWindow()
        {
            InitializeComponent();
            LoadAchievementsData();
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void LoadAchievementsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                                a.AchievementID,
                                a.EmployeeID,
                                s.LastName, 
                                s.FirstName, 
                                s.MiddleName,
                                a.AchievementDate, 
                                a.AchievementType, 
                                a.Description,
                                a.Reward,
                                a.Status
                            FROM 
                                Achievements a
                            JOIN 
                                Staff s ON a.EmployeeID = s.EmployeeID
                            ORDER BY 
                                a.AchievementDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allAchievements.Clear();
                        while (reader.Read())
                        {
                            var achievement = new EmployeeAchievement
                            {
                                AchievementID = reader.GetInt32(reader.GetOrdinal("AchievementID")),
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                AchievementDate = reader.GetDateTime(reader.GetOrdinal("AchievementDate")),
                                AchievementType = reader.GetString(reader.GetOrdinal("AchievementType")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                                Reward = reader.IsDBNull(reader.GetOrdinal("Reward")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Reward")),
                                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ?
                                    "Присуждена" : reader.GetString(reader.GetOrdinal("Status"))
                            };
                            _allAchievements.Add(achievement);
                        }
                    }
                }
                AchievementsDataGrid.ItemsSource = _allAchievements;
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки достижений: {ex.Message}", false).ShowDialog();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allAchievements == null || _allAchievements.Count == 0)
                return;

            var filtered = _allAchievements.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(a =>
                    a.LastName.ToLower().Contains(searchText) ||
                    a.FirstName.ToLower().Contains(searchText) ||
                    (!string.IsNullOrEmpty(a.MiddleName) && a.MiddleName.ToLower().Contains(searchText)));
            }

            AchievementsDataGrid.ItemsSource = new ObservableCollection<EmployeeAchievement>(filtered.ToList());
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => ApplyFilters();

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ApplyFilters();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            AchievementsDataGrid.ItemsSource = _allAchievements;
        }

        private void AddAchievementButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddAchievementWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = @"INSERT INTO Achievements 
                                    (EmployeeID, AchievementDate, AchievementType, Description, Reward, Status)
                                    VALUES (@EmployeeID, @AchievementDate, @AchievementType, @Description, @Reward, @Status);
                                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", addWindow.SelectedEmployeeID);
                            command.Parameters.AddWithValue("@AchievementDate", addWindow.AchievementDate);
                            command.Parameters.AddWithValue("@AchievementType", addWindow.AchievementType);
                            command.Parameters.AddWithValue("@Description", addWindow.Description ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Reward", addWindow.Reward ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Status", addWindow.Status ?? "Присуждена");

                            int newId = Convert.ToInt32(command.ExecuteScalar());

                            string staffQuery = "SELECT LastName, FirstName, MiddleName FROM Staff WHERE EmployeeID = @EmployeeID";
                            using (SqlCommand staffCommand = new SqlCommand(staffQuery, connection))
                            {
                                staffCommand.Parameters.AddWithValue("@EmployeeID", addWindow.SelectedEmployeeID);
                                using (SqlDataReader reader = staffCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        var newAchievement = new EmployeeAchievement
                                        {
                                            AchievementID = newId,
                                            EmployeeID = addWindow.SelectedEmployeeID,
                                            LastName = reader.GetString(0),
                                            FirstName = reader.GetString(1),
                                            MiddleName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                            AchievementDate = addWindow.AchievementDate,
                                            AchievementType = addWindow.AchievementType,
                                            Description = addWindow.Description,
                                            Reward = addWindow.Reward,
                                            Status = addWindow.Status ?? "Присуждена"
                                        };
                                        _allAchievements.Insert(0, newAchievement);
                                    }
                                }
                            }
                        }
                    }
                    new CustomBox("Достижение успешно добавлено!", false).ShowDialog();
                    AchievementsDataGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    new CustomBox($"Ошибка при добавлении достижения: {ex.Message}", false).ShowDialog();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AchievementsDataGrid.SelectedItem is EmployeeAchievement selectedAchievement)
            {
                var confirmBox = new CustomBox("Вы уверены, что хотите удалить это достижение?", true);
                confirmBox.ShowDialog();

                if (confirmBox.Result)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Achievements WHERE AchievementID = @AchievementID";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@AchievementID", selectedAchievement.AchievementID);
                                command.ExecuteNonQuery();
                            }
                        }

                        _allAchievements.Remove(selectedAchievement);
                        AchievementsDataGrid.Items.Refresh();
                        new CustomBox("Достижение успешно удалено!", false).ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        new CustomBox($"Ошибка при удалении достижения: {ex.Message}", false).ShowDialog();
                    }
                }
            }
            else
            {
                new CustomBox("Пожалуйста, выберите достижение для удаления.", false).ShowDialog();
            }
        }

        private void AchievementsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AchievementsDataGrid.SelectedItem is EmployeeAchievement selectedAchievement)
            {
                var detailsWindow = new AchievementDetailsWindow(selectedAchievement);
                if (detailsWindow.ShowDialog() == true)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = @"UPDATE Achievements 
                                            SET AchievementType = @AchievementType,
                                                Description = @Description,
                                                Reward = @Reward,
                                                Status = @Status
                                            WHERE AchievementID = @AchievementID";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@AchievementType", detailsWindow.AchievementType);
                                command.Parameters.AddWithValue("@Description", detailsWindow.Description ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Reward", detailsWindow.Reward ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Status", detailsWindow.Status ?? "Присуждена");
                                command.Parameters.AddWithValue("@AchievementID", selectedAchievement.AchievementID);
                                command.ExecuteNonQuery();
                            }
                        }

                        selectedAchievement.AchievementType = detailsWindow.AchievementType;
                        selectedAchievement.Description = detailsWindow.Description;
                        selectedAchievement.Reward = detailsWindow.Reward;
                        selectedAchievement.Status = detailsWindow.Status;

                        AchievementsDataGrid.Items.Refresh();
                        new CustomBox("Изменения сохранены успешно!", false).ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        new CustomBox($"Ошибка при сохранении изменений: {ex.Message}", false).ShowDialog();
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => this.Close();

    }

    public class EmployeeAchievement
    {
        public int AchievementID { get; set; }
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
        public DateTime AchievementDate { get; set; }
        public string AchievementType { get; set; }
        public string Description { get; set; }
        public string Reward { get; set; }
        public string Status { get; set; }
    }
}