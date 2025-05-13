using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HR_department
{
    public partial class VacationPage : Page
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;
        private DataView _allVacationsView;

        public VacationPage(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;

            StatusFilterComboBox.SelectionChanged += (s, e) => ApplyFilters();

            VacationDataGrid.PreviewMouseDoubleClick += (s, e) => e.Handled = true;

            LoadStatuses();
            LoadVacations();
        }

        private void LoadStatuses()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT StatusName FROM LeaveStatus";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable statusTable = new DataTable();
                    adapter.Fill(statusTable);

                    StatusFilterComboBox.Items.Clear();
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все статусы", IsSelected = true });

                    foreach (DataRow row in statusTable.Rows)
                    {
                        StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = row["StatusName"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки статусов: {ex.Message}", false).ShowDialog();
            }
        }

        private void LoadVacations()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT vr.CreatedDate, vr.StartDate, vr.EndDate, 
                                    vr.VacationType, ls.StatusName AS Status, 
                                    vr.Comment, vr.StatusID
                                   FROM VacationRequests vr
                                   JOIN LeaveStatus ls ON vr.StatusID = ls.StatusID
                                   WHERE vr.EmployeeID = @EmployeeID
                                   ORDER BY vr.CreatedDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    _allVacationsView = dataTable.DefaultView;
                    VacationDataGrid.ItemsSource = _allVacationsView;
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при загрузке данных об отпусках: {ex.Message}", false).ShowDialog();
            }
        }

        private void ApplyFilters()
        {
            if (_allVacationsView == null) return;

            var selectedItem = StatusFilterComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string selectedStatus = selectedItem.Content.ToString();

            if (selectedStatus == "Все статусы")
            {
                _allVacationsView.RowFilter = "";
            }
            else
            {
                _allVacationsView.RowFilter = $"Status = '{selectedStatus}'";
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = 0;
            if (_allVacationsView != null)
                _allVacationsView.RowFilter = "";
        }

        private void AddVacationButton_Click(object sender, RoutedEventArgs e)
        {
            var addVacationWindow = new AddVacationWindow(_employeeId);
            addVacationWindow.Owner = Window.GetWindow(this);
            if (addVacationWindow.ShowDialog() == true)
            {
                LoadVacations();
            }
        }
    }
}