using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HR_department
{
    public partial class AdminVacationRequestsWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private ObservableCollection<VacationRequest> _allVacationRequests = new ObservableCollection<VacationRequest>();
        private CollectionView _vacationRequestsView;

        public AdminVacationRequestsWindow()
        {
            InitializeComponent();
            LoadStatuses();
            LoadVacationRequestsData();
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID, StatusName FROM LeaveStatus";
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

        private void LoadVacationRequestsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                                vr.RequestID, 
                                vr.EmployeeID, 
                                s.LastName, 
                                s.FirstName, 
                                s.MiddleName,
                                vr.StartDate, 
                                vr.EndDate, 
                                vr.VacationType, 
                                vr.Comment,
                                ls.StatusName AS Status,
                                ls.StatusID,
                                vr.CreatedDate
                            FROM 
                                VacationRequests vr
                            JOIN 
                                Staff s ON vr.EmployeeID = s.EmployeeID
                            JOIN
                                LeaveStatus ls ON vr.StatusID = ls.StatusID
                            ORDER BY 
                                CASE WHEN ls.StatusName = 'На рассмотрении' THEN 0 ELSE 1 END,
                                vr.CreatedDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allVacationRequests.Clear();
                        while (reader.Read())
                        {
                            var vacationRequest = new VacationRequest
                            {
                                RequestID = reader.GetInt32(reader.GetOrdinal("RequestID")),
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                VacationType = reader.GetString(reader.GetOrdinal("VacationType")),
                                Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Comment")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                            };
                            vacationRequest.OriginalStatusID = vacationRequest.StatusID;
                            _allVacationRequests.Add(vacationRequest);
                        }

                        _vacationRequestsView = (CollectionView)CollectionViewSource.GetDefaultView(_allVacationRequests);
                        VacationRequestsDataGrid.ItemsSource = _vacationRequestsView;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки запросов на отпуск: {ex.Message}", false).ShowDialog();
            }
        }

        private void ApplyFilters()
        {
            if (_vacationRequestsView == null) return;

            _vacationRequestsView.Filter = item =>
            {
                var request = item as VacationRequest;
                if (request == null) return false;

                bool nameMatches = true;
                bool statusMatches = true;

                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    string searchText = SearchTextBox.Text.ToLower();
                    nameMatches = request.FullName.ToLower().Contains(searchText);
                }

                if (StatusFilterComboBox.SelectedValue != null)
                {
                    int statusId = (int)StatusFilterComboBox.SelectedValue;
                    statusMatches = request.StatusID == statusId;
                }

                return nameMatches && statusMatches;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            StatusFilterComboBox.SelectedIndex = -1;
            ApplyFilters();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (VacationRequestsDataGrid.SelectedItem is VacationRequest selectedRequest)
            {
                selectedRequest.StatusID = GetStatusIdByName("Одобрено");
                selectedRequest.Status = "Одобрено";
                VacationRequestsDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите запрос на отпуск для одобрения.", false).ShowDialog();
            }
        }

        private void ApproveWithChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (VacationRequestsDataGrid.SelectedItem is VacationRequest selectedRequest)
            {
                var editWindow = new EditVacationWindow(selectedRequest);
                editWindow.StartDatePicker.SelectedDate = DateTime.Today.AddMonths(1);

                if (editWindow.ShowDialog() == true)
                {
                    selectedRequest.StartDate = editWindow.NewStartDate;
                    selectedRequest.EndDate = editWindow.NewEndDate;
                    selectedRequest.StatusID = GetStatusIdByName("Одобрено с правками");
                    selectedRequest.Status = "Одобрено с правками";
                    VacationRequestsDataGrid.Items.Refresh();
                }
            }
            else
            {
                new CustomBox("Выберите запрос на отпуск для одобрения с правками.", false).ShowDialog();
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (VacationRequestsDataGrid.SelectedItem is VacationRequest selectedRequest)
            {
                selectedRequest.StatusID = GetStatusIdByName("Отклонено");
                selectedRequest.Status = "Отклонено";
                VacationRequestsDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите запрос на отпуск для отклонения.", false).ShowDialog();
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
                var changedRequests = _allVacationRequests.Where(vr => vr.StatusID != vr.OriginalStatusID).ToList();
                if (!changedRequests.Any())
                {
                    new CustomBox("Нет изменений для сохранения.", false).ShowDialog();
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    foreach (var request in changedRequests)
                    {
                        string query = @"UPDATE VacationRequests 
                                        SET StatusID = @StatusID,
                                            StartDate = @StartDate,
                                            EndDate = @EndDate
                                        WHERE RequestID = @RequestID";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@StatusID", request.StatusID);
                            command.Parameters.AddWithValue("@RequestID", request.RequestID);
                            command.Parameters.AddWithValue("@StartDate", request.StartDate);
                            command.Parameters.AddWithValue("@EndDate", request.EndDate);
                            command.ExecuteNonQuery();
                        }
                        request.OriginalStatusID = request.StatusID;
                    }
                }

                new CustomBox("Изменения сохранены успешно!", false).ShowDialog();
                LoadVacationRequestsData();
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при сохранении изменений: {ex.Message}", false).ShowDialog();
            }
        }

        private void VacationRequestsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VacationRequestsDataGrid.SelectedItem is VacationRequest selectedRequest)
            {
                var detailsWindow = new VacationRequestDetails(selectedRequest);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allVacationRequests != null && _allVacationRequests.Any(vr => vr.StatusID != vr.OriginalStatusID))
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

    public class VacationRequest
    {
        public int RequestID { get; set; }
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VacationType { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public int OriginalStatusID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}