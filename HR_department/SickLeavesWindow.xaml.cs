using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HR_department
{
    public partial class SickLeavesWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;
        private DataView _sickLeavesView;

        public SickLeavesWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;

            StatusFilterComboBox.SelectionChanged += (s, e) => ApplyFilters();

            SickLeavesDataGrid.PreviewMouseDoubleClick += (s, e) => e.Handled = true;

            LoadStatuses();
            LoadSickLeaves();
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID, StatusName FROM LeaveStatus WHERE StatusName <> 'Одобрено с правками'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable statusTable = new DataTable();
                    adapter.Fill(statusTable);

                    StatusFilterComboBox.ItemsSource = statusTable.DefaultView;
                    StatusFilterComboBox.DisplayMemberPath = "StatusName";
                    StatusFilterComboBox.SelectedValuePath = "StatusID";
                    StatusFilterComboBox.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки статусов: {ex.Message}", false).ShowDialog();
            }
        }

        private void LoadSickLeaves()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT sl.CreatedDate, sl.StartDate, sl.EndDate, sl.Reason, 
                                    ls.StatusName AS Status, ls.StatusID
                                   FROM SickLeaves sl
                                   JOIN LeaveStatus ls ON sl.StatusID = ls.StatusID
                                   WHERE sl.EmployeeID = @EmployeeID
                                   ORDER BY sl.CreatedDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    _sickLeavesView = dataTable.DefaultView;
                    SickLeavesDataGrid.ItemsSource = _sickLeavesView;
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при загрузке данных о больничных листах: {ex.Message}", false).ShowDialog();
            }
        }

        private void ApplyFilters()
        {
            if (_sickLeavesView == null) return;

            string filter = "";
            if (StatusFilterComboBox.SelectedValue != null)
            {
                int statusId = (int)StatusFilterComboBox.SelectedValue;
                filter = $"StatusID = {statusId}";
            }

            _sickLeavesView.RowFilter = filter;
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = -1;
            if (_sickLeavesView != null)
                _sickLeavesView.RowFilter = "";
        }

        private void AddSickLeaveButton_Click(object sender, RoutedEventArgs e)
        {
            var addSickLeaveWindow = new AddSickLeaveWindow(_employeeId);
            addSickLeaveWindow.Owner = this;
            if (addSickLeaveWindow.ShowDialog() == true)
            {
                LoadSickLeaves();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}