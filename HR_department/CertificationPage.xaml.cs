using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace HR_department
{
    public partial class CertificationPage : Page
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;
        private DataView _certificationsView;

        public CertificationPage(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;

            StatusFilterComboBox.SelectionChanged += (s, e) => ApplyFilters();
            LoadStatuses();
            LoadCertifications();
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusID, StatusName FROM CertificationStatuses";
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

        private void LoadCertifications()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT c.CertificationDate, c.Result, cs.StatusName AS Status, 
                                   c.Recommendations, cs.StatusID
                                   FROM Certifications c
                                   JOIN CertificationStatuses cs ON c.StatusID = cs.StatusID
                                   WHERE c.EmployeeID = @EmployeeID
                                   ORDER BY 
                                       CASE 
                                           WHEN cs.StatusName = 'Запланирована' THEN 1
                                           WHEN cs.StatusName = 'Пройдена' THEN 2
                                           WHEN cs.StatusName = 'Не пройдена' THEN 3
                                           ELSE 4
                                       END,
                                   c.CertificationDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    _certificationsView = dataTable.DefaultView;
                    CertificationDataGrid.ItemsSource = _certificationsView;
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при загрузке данных аттестации: {ex.Message}", false).ShowDialog();
            }
        }

        private void ApplyFilters()
        {
            if (_certificationsView == null) return;

            string filter = "";
            if (StatusFilterComboBox.SelectedValue != null)
            {
                int statusId = (int)StatusFilterComboBox.SelectedValue;
                filter = $"StatusID = {statusId}";
            }

            _certificationsView.RowFilter = filter;
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = -1;
            if (_certificationsView != null)
                _certificationsView.RowFilter = "";
        }
    }
}