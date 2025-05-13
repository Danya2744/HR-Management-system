using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HR_department
{
    public partial class AdminCertificationsWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private ObservableCollection<Certification> _allCertifications = new ObservableCollection<Certification>();

        public AdminCertificationsWindow()
        {
            InitializeComponent();
            LoadStatuses();
            LoadCertificationsData();

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
                    string query = "SELECT StatusID, StatusName FROM CertificationStatuses ORDER BY StatusID";
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

        private void LoadCertificationsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                                    c.CertificationID, 
                                    c.EmployeeID, 
                                    s.LastName, 
                                    s.FirstName, 
                                    s.MiddleName,
                                    c.CertificationDate, 
                                    c.Result, 
                                    cs.StatusName,
                                    c.Recommendations,
                                    cs.StatusID
                                FROM 
                                    Certifications c
                                JOIN 
                                    Staff s ON c.EmployeeID = s.EmployeeID
                                JOIN
                                    CertificationStatuses cs ON c.StatusID = cs.StatusID
                                ORDER BY 
                                    CASE cs.StatusID
                                        WHEN 1 THEN 0  -- Запланирована
                                        WHEN 2 THEN 1  -- Пройдена
                                        WHEN 3 THEN 2  -- Не пройдена
                                        WHEN 4 THEN 3  -- Отменена
                                        ELSE 4
                                    END,
                                    c.CertificationDate DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allCertifications.Clear();
                        while (reader.Read())
                        {
                            var certification = new Certification
                            {
                                CertificationID = reader.GetInt32(reader.GetOrdinal("CertificationID")),
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                CertificationDate = reader.GetDateTime(reader.GetOrdinal("CertificationDate")),
                                Result = reader.GetString(reader.GetOrdinal("Result")),
                                StatusName = reader.GetString(reader.GetOrdinal("StatusName")),
                                Recommendations = reader.IsDBNull(reader.GetOrdinal("Recommendations")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Recommendations")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"))
                            };
                            certification.OriginalStatusID = certification.StatusID;
                            _allCertifications.Add(certification);
                        }

                        CertificationsDataGrid.ItemsSource = _allCertifications;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки аттестаций: {ex.Message}", false).ShowDialog();
            }
        }

        private void AddCertificationButton_Click(object sender, RoutedEventArgs e)
        {
            var addCertificationWindow = new AddCertificationWindow();
            if (addCertificationWindow.ShowDialog() == true)
            {
                LoadCertificationsData(); 
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
            if (_allCertifications == null || _allCertifications.Count == 0)
                return;

            var filtered = _allCertifications.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(c =>
                    c.LastName.ToLower().Contains(searchText) ||
                    c.FirstName.ToLower().Contains(searchText) ||
                    (!string.IsNullOrEmpty(c.MiddleName) && c.MiddleName.ToLower().Contains(searchText)));
            }

            if (StatusFilterComboBox.SelectedValue != null &&
                StatusFilterComboBox.SelectedValue is int statusId)
            {
                filtered = filtered.Where(c => c.StatusID == statusId);
            }

            CertificationsDataGrid.ItemsSource = filtered.ToList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyFilters();
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            StatusFilterComboBox.SelectedIndex = -1;
            CertificationsDataGrid.ItemsSource = _allCertifications;
        }

        private void MarkAsPassedButton_Click(object sender, RoutedEventArgs e)
        {
            if (CertificationsDataGrid.SelectedItem is Certification selectedCert)
            {
                var resultWindow = new CertificationResultWindow();
                if (resultWindow.ShowDialog() == true)
                {
                    selectedCert.StatusID = 2; 
                    selectedCert.StatusName = "Пройдена";
                    selectedCert.Result = resultWindow.Result;
                    selectedCert.Recommendations = resultWindow.Recommendations;
                    CertificationsDataGrid.Items.Refresh();
                }
            }
            else
            {
                new CustomBox("Выберите аттестацию для отметки как 'Пройдена'.", false).ShowDialog();
            }
        }

        private void MarkAsFailedButton_Click(object sender, RoutedEventArgs e)
        {
            if (CertificationsDataGrid.SelectedItem is Certification selectedCert)
            {
                selectedCert.StatusID = 3; 
                selectedCert.StatusName = "Не пройдена";
                CertificationsDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите аттестацию для отметки как 'Не пройдена'.", false).ShowDialog();
            }
        }

        private void CancelCertificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (CertificationsDataGrid.SelectedItem is Certification selectedCert)
            {
                selectedCert.StatusID = 4; 
                selectedCert.StatusName = "Отменена";
                CertificationsDataGrid.Items.Refresh();
            }
            else
            {
                new CustomBox("Выберите аттестацию для отмены.", false).ShowDialog();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changedCerts = _allCertifications.Where(c => c.StatusID != c.OriginalStatusID).ToList();
                if (!changedCerts.Any())
                {
                    new CustomBox("Нет изменений для сохранения.", false).ShowDialog();
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    foreach (var cert in changedCerts)
                    {
                        string query = @"UPDATE Certifications 
                                        SET StatusID = @StatusID,
                                            Result = @Result,
                                            Recommendations = @Recommendations,
                                            Status = @StatusName
                                        WHERE CertificationID = @CertificationID";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@StatusID", cert.StatusID);
                            command.Parameters.AddWithValue("@StatusName", cert.StatusName);
                            command.Parameters.AddWithValue("@Result", cert.Result);
                            command.Parameters.AddWithValue("@Recommendations", cert.Recommendations);
                            command.Parameters.AddWithValue("@CertificationID", cert.CertificationID);
                            command.ExecuteNonQuery();
                        }
                        cert.OriginalStatusID = cert.StatusID;
                    }
                }

                new CustomBox("Изменения сохранены успешно!", false).ShowDialog();
                LoadCertificationsData();
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка при сохранении изменений: {ex.Message}", false).ShowDialog();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_allCertifications != null && _allCertifications.Any(c => c.StatusID != c.OriginalStatusID))
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

    public class Certification
    {
        public int CertificationID { get; set; }
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string FullName => $"{LastName} {FirstName} {MiddleName}".TrimEnd();
        public DateTime CertificationDate { get; set; }
        public string Result { get; set; }
        public string StatusName { get; set; }
        public int StatusID { get; set; }
        public int OriginalStatusID { get; set; }
        public string Recommendations { get; set; }
    }
}