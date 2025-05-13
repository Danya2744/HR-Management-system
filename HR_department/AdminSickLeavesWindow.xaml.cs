using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HR_department
{
    public partial class AdminSickLeavesWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private ObservableCollection<SickLeave> _allSickLeaves = new ObservableCollection<SickLeave>();

        public AdminSickLeavesWindow()
        {
            InitializeComponent();
            LoadStatuses();
            LoadSickLeavesData();

            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            StatusFilterComboBox.SelectionChanged += StatusFilterComboBox_SelectionChanged;
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID, StatusName FROM LeaveStatus WHERE StatusName <> 'Одобрено с правками'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable statusTable = new DataTable();
                        statusTable.Load(command.ExecuteReader());
                        StatusFilterComboBox.ItemsSource = statusTable.DefaultView;
                        StatusFilterComboBox.DisplayMemberPath = "StatusName";
                        StatusFilterComboBox.SelectedValuePath = "StatusID";
                        StatusFilterComboBox.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки статусов: {ex.Message}", false).ShowDialog();
            }
        }

        private void LoadSickLeavesData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                                sl.SickLeaveID, 
                                sl.EmployeeID, 
                                s.LastName, 
                                s.FirstName, 
                                s.MiddleName,
                                sl.StartDate, 
                                sl.EndDate, 
                                sl.Reason, 
                                ls.StatusName AS Status,
                                ls.StatusID,
                                sl.CreatedDate
                            FROM 
                                SickLeaves sl
                            JOIN 
                                Staff s ON sl.EmployeeID = s.EmployeeID
                            JOIN
                                LeaveStatus ls ON sl.StatusID = ls.StatusID
                            ORDER BY 
                                CASE WHEN ls.StatusName = 'На рассмотрении' THEN 0 ELSE 1 END,
                                sl.CreatedDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allSickLeaves.Clear();
                        while (reader.Read())
                        {
                            var sickLeave = new SickLeave
                            {
                                SickLeaveID = reader.GetInt32(reader.GetOrdinal("SickLeaveID")),
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            };
                            sickLeave.OriginalStatusID = sickLeave.StatusID;
                            _allSickLeaves.Add(sickLeave);
                        }

                        SickLeavesDataGrid.ItemsSource = _allSickLeaves;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки больничных листов: {ex.Message}", false).ShowDialog();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allSickLeaves == null || _allSickLeaves.Count == 0)
                return;

            var filtered = _allSickLeaves.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(sl =>
                    sl.LastName.ToLower().Contains(searchText) ||
                    sl.FirstName.ToLower().Contains(searchText) ||
                    (!string.IsNullOrEmpty(sl.MiddleName) && sl.MiddleName.ToLower().Contains(searchText)));
            }

            if (StatusFilterComboBox.SelectedValue != null &&
                StatusFilterComboBox.SelectedValue is int statusId)
            {
                filtered = filtered.Where(sl => sl.StatusID == statusId);
            }

            SickLeavesDataGrid.ItemsSource = filtered.ToList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => ApplyFilters();

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ApplyFilters();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            StatusFilterComboBox.SelectedIndex = -1;
            SickLeavesDataGrid.ItemsSource = _allSickLeaves;
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SickLeavesDataGrid.SelectedItem is SickLeave selectedLeave)
            {
                selectedLeave.StatusID = GetStatusIdByName("Одобрено");
                selectedLeave.Status = "Одобрено";
                SickLeavesDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите больничный лист для одобрения.", false).ShowDialog();
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (SickLeavesDataGrid.SelectedItem is SickLeave selectedLeave)
            {
                selectedLeave.StatusID = GetStatusIdByName("Отклонено");
                selectedLeave.Status = "Отклонено";
                SickLeavesDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите больничный лист для отклонения.", false).ShowDialog();
            }
        }

        private int GetStatusIdByName(string statusName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID FROM LeaveStatus WHERE StatusName = @StatusName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StatusName", statusName);
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка получения ID статуса: {ex.Message}", false).ShowDialog();
                return -1;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changedLeaves = _allSickLeaves.Where(l => l.StatusID != l.OriginalStatusID).ToList();
                if (!changedLeaves.Any())
                {
                    new CustomBox("Нет изменений для сохранения.", false).ShowDialog();
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    foreach (var leave in changedLeaves)
                    {
                        string query = @"UPDATE SickLeaves 
                                        SET StatusID = @StatusID
                                        WHERE SickLeaveID = @SickLeaveID";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@StatusID", leave.StatusID);
                            command.Parameters.AddWithValue("@SickLeaveID", leave.SickLeaveID);
                            command.ExecuteNonQuery();
                        }
                        leave.OriginalStatusID = leave.StatusID;
                    }
                }

                new CustomBox("Изменения сохранены успешно!", false).ShowDialog();
                LoadSickLeavesData();
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при сохранении изменений: {ex.Message}", false).ShowDialog();
            }
        }

        private void SickLeavesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SickLeavesDataGrid.SelectedItem is SickLeave selectedLeave)
            {
                var detailsWindow = new SickLeaveDetails(selectedLeave);
                detailsWindow.ShowDialog();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allSickLeaves != null && _allSickLeaves.Any(sl => sl.StatusID != sl.OriginalStatusID))
            {
                var confirmBox = new CustomBox(
                    "Есть несохраненные изменения. Закрыть без сохранения?",
                    true);
                confirmBox.ShowDialog();

                if (!confirmBox.Result)
                {
                    e.Cancel = true;
                }
            }
        }
    }

    public class SickLeave
    {
        public int SickLeaveID { get; set; }
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public int OriginalStatusID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}