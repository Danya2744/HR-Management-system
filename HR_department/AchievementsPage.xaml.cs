using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace HR_department
{
    public partial class AchievementsPage : Page
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;
        private DataView _achievementsView;

        public AchievementsPage(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;

            DateFromPicker.SelectedDateChanged += (s, e) => ApplyFilters();
            DateToPicker.SelectedDateChanged += (s, e) => ApplyFilters();

            LoadAchievements();
        }

        private void LoadAchievements()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT AchievementDate, AchievementType, Description, Reward, Status
                                   FROM Achievements 
                                   WHERE EmployeeID = @EmployeeID
                                   ORDER BY AchievementDate DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmployeeID", _employeeId);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    _achievementsView = dataTable.DefaultView;
                    AchievementsDataGrid.ItemsSource = _achievementsView;
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при загрузке данных о достижениях: {ex.Message}", false).ShowDialog();
            }
        }

        private void ApplyFilters()
        {
            if (_achievementsView == null) return;

            string filter = "";

            if (DateFromPicker.SelectedDate != null)
            {
                filter += $"AchievementDate >= #{DateFromPicker.SelectedDate.Value.ToString("yyyy-MM-dd")}#";
            }

            if (DateToPicker.SelectedDate != null)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += $"AchievementDate <= #{DateToPicker.SelectedDate.Value.ToString("yyyy-MM-dd")}#";
            }

            _achievementsView.RowFilter = filter;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            DateFromPicker.SelectedDate = null;
            DateToPicker.SelectedDate = null;

            if (_achievementsView != null)
                _achievementsView.RowFilter = "";
        }
    }
}