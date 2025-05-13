using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HR_department
{
    public partial class ReportsWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private ObservableCollection<Employee> _allEmployees = new ObservableCollection<Employee>();

        public ReportsWindow()
        {
            InitializeComponent();
            Loaded += ReportsWindow_Loaded;
        }

        private void ReportsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Today;
                SickLeaveStartDatePicker.SelectedDate = today.AddMonths(-1);
                SickLeaveEndDatePicker.SelectedDate = today;
                VacationStartDatePicker.SelectedDate = today.AddMonths(-1);
                VacationEndDatePicker.SelectedDate = today;
                CertificationStartDatePicker.SelectedDate = today.AddMonths(-1);
                CertificationEndDatePicker.SelectedDate = today;

                // Установка свойств только для чтения
                EmployeesDataGrid.IsReadOnly = true;
                SickLeavesDataGrid.IsReadOnly = true;
                VacationsDataGrid.IsReadOnly = true;
                CertificationsDataGrid.IsReadOnly = true;

                LoadDepartments();
                LoadEmployeesData();

                DepartmentFilterComboBox.SelectionChanged += DepartmentFilterComboBox_SelectionChanged;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при инициализации: {ex.Message}");
            }
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable departmentsTable = new DataTable();
                        departmentsTable.Load(command.ExecuteReader());
                        DepartmentFilterComboBox.ItemsSource = departmentsTable.DefaultView;
                        DepartmentFilterComboBox.DisplayMemberPath = "DepartmentName";
                        DepartmentFilterComboBox.SelectedValuePath = "DepartmentID";
                        DepartmentFilterComboBox.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки отделов: {ex.Message}");
            }
        }

        private void LoadEmployeesData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                        s.EmployeeID,
                        s.LastName, 
                        s.FirstName, 
                        s.MiddleName,
                        s.BirthDate,
                        s.ContactInfo,
                        s.Education,
                        s.HireDate,
                        s.PositionID,
                        s.DepartmentID,
                        p.PositionName,
                        d.DepartmentName
                    FROM 
                        Staff s
                    JOIN 
                        Positions p ON s.PositionID = p.PositionID
                    JOIN
                        Departments d ON s.DepartmentID = d.DepartmentID
                    ORDER BY 
                        d.DepartmentName, s.LastName, s.FirstName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allEmployees.Clear();
                        while (reader.Read())
                        {
                            var employee = new Employee
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                ContactInfo = reader.IsDBNull(reader.GetOrdinal("ContactInfo")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("ContactInfo")),
                                Education = reader.IsDBNull(reader.GetOrdinal("Education")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Education")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                                PositionID = reader.GetInt32(reader.GetOrdinal("PositionID")),
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                PositionName = reader.GetString(reader.GetOrdinal("PositionName")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            };
                            _allEmployees.Add(employee);
                        }

                        EmployeesDataGrid.ItemsSource = _allEmployees;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private void DepartmentFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyDepartmentFilter();
        }

        private void ApplyDepartmentFilter()
        {
            if (_allEmployees == null || _allEmployees.Count == 0)
                return;

            var filtered = _allEmployees.AsEnumerable();

            if (DepartmentFilterComboBox.SelectedValue != null &&
                int.TryParse(DepartmentFilterComboBox.SelectedValue.ToString(), out int departmentId))
            {
                filtered = filtered.Where(emp => emp.DepartmentID == departmentId);
            }

            EmployeesDataGrid.ItemsSource = new ObservableCollection<Employee>(filtered.ToList());
        }

        private void ResetDepartmentFilterButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentFilterComboBox.SelectedIndex = -1;
            EmployeesDataGrid.ItemsSource = _allEmployees;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner?.Show();
        }

        private void LoadSickLeavesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SickLeaveStartDatePicker.SelectedDate == null || SickLeaveEndDatePicker.SelectedDate == null)
                {
                    ShowWarningMessage("Пожалуйста, выберите даты начала и окончания периода");
                    return;
                }

                string query = @"
                    SELECT 
                        s.LastName + ' ' + s.FirstName + ISNULL(' ' + s.MiddleName, '') AS EmployeeFullName,
                        sl.StartDate,
                        sl.EndDate,
                        sl.Reason,
                        ls.StatusName
                    FROM SickLeaves sl
                    JOIN Staff s ON sl.EmployeeID = s.EmployeeID
                    JOIN LeaveStatus ls ON sl.StatusID = ls.StatusID
                    WHERE sl.CreatedDate BETWEEN @StartDate AND @EndDate
                    ORDER BY sl.StartDate";

                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", SickLeaveStartDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@EndDate", SickLeaveEndDatePicker.SelectedDate?.AddDays(1)); // Добавляем день для включения всей даты окончания

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        connection.Open();
                        adapter.Fill(dataTable);

                        Dispatcher.Invoke(() =>
                        {
                            SickLeavesDataGrid.ItemsSource = dataTable.DefaultView;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки больничных: {ex.Message}");
            }
        }

        private void LoadVacationsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VacationStartDatePicker.SelectedDate == null || VacationEndDatePicker.SelectedDate == null)
                {
                    ShowWarningMessage("Пожалуйста, выберите даты начала и окончания периода");
                    return;
                }

                string query = @"
                    SELECT 
                        s.LastName + ' ' + s.FirstName + ISNULL(' ' + s.MiddleName, '') AS EmployeeFullName,
                        vr.StartDate,
                        vr.EndDate,
                        vr.VacationType,
                        ls.StatusName
                    FROM VacationRequests vr
                    JOIN Staff s ON vr.EmployeeID = s.EmployeeID
                    JOIN LeaveStatus ls ON vr.StatusID = ls.StatusID
                    WHERE vr.CreatedDate BETWEEN @StartDate AND @EndDate
                    ORDER BY vr.StartDate";

                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", VacationStartDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@EndDate", VacationEndDatePicker.SelectedDate?.AddDays(1)); // Добавляем день для включения всей даты окончания

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        connection.Open();
                        adapter.Fill(dataTable);

                        Dispatcher.Invoke(() =>
                        {
                            VacationsDataGrid.ItemsSource = dataTable.DefaultView;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки отпусков: {ex.Message}");
            }
        }

        private void LoadCertificationsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CertificationStartDatePicker.SelectedDate == null || CertificationEndDatePicker.SelectedDate == null)
                {
                    ShowWarningMessage("Пожалуйста, выберите даты начала и окончания периода");
                    return;
                }

                string query = @"
                    SELECT 
                        s.LastName + ' ' + s.FirstName + ISNULL(' ' + s.MiddleName, '') AS EmployeeFullName,
                        c.CertificationDate,
                        c.Result,
                        c.Recommendations,
                        cs.StatusName
                    FROM Certifications c
                    JOIN Staff s ON c.EmployeeID = s.EmployeeID
                    JOIN CertificationStatuses cs ON c.StatusID = cs.StatusID
                    WHERE c.CertificationDate BETWEEN @StartDate AND @EndDate
                    ORDER BY c.CertificationDate";

                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", CertificationStartDatePicker.SelectedDate);
                    command.Parameters.AddWithValue("@EndDate", CertificationEndDatePicker.SelectedDate?.AddDays(1)); // Добавляем день для включения всей даты окончания

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        connection.Open();
                        adapter.Fill(dataTable);

                        Dispatcher.Invoke(() =>
                        {
                            CertificationsDataGrid.ItemsSource = dataTable.DefaultView;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки аттестаций: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var errorBox = new CustomBox(message, false);
                errorBox.Owner = this;
                errorBox.ShowDialog();
            });
        }

        private void ShowWarningMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                var warningBox = new CustomBox(message, false);
                warningBox.Owner = this;
                warningBox.ShowDialog();
            });
        }
    }
}