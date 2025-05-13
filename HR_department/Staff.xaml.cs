using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;

namespace HR_department
{
    public partial class Staff : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private List<Employee> _allEmployees = new List<Employee>();

        public Staff()
        {
            InitializeComponent();
            LoadStaffData();
            DepartmentFilterComboBox.SelectionChanged += DepartmentFilterComboBox_SelectionChanged;
        }

        private void LoadStaffData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string departmentsQuery = "SELECT DepartmentID, DepartmentName FROM Departments";
                    using (SqlCommand deptCommand = new SqlCommand(departmentsQuery, connection))
                    {
                        DataTable deptTable = new DataTable();
                        deptTable.Load(deptCommand.ExecuteReader());
                        DepartmentFilterComboBox.ItemsSource = deptTable.DefaultView;
                        DepartmentFilterComboBox.SelectedIndex = -1;
                    }
                    string query = @"SELECT 
                                    s.EmployeeID, 
                                    s.LastName, 
                                    s.FirstName, 
                                    s.MiddleName, 
                                    s.BirthDate,
                                    s.ContactInfo,
                                    s.Education,
                                    s.HireDate,
                                    d.DepartmentName AS Department, 
                                    p.PositionName AS Position,
                                    s.PositionID,
                                    s.DepartmentID,
                                    u.Login_user AS Login
                                FROM 
                                    Staff s
                                JOIN 
                                    Departments d ON s.DepartmentID = d.DepartmentID
                                JOIN 
                                    Positions p ON s.PositionID = p.PositionID
                                LEFT JOIN
                                    Users u ON s.EmployeeID = u.EmployeeID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        _allEmployees = new List<Employee>();
                        while (reader.Read())
                        {
                            _allEmployees.Add(new Employee
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("MiddleName")),
                                BirthDate = reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                ContactInfo = reader.IsDBNull(reader.GetOrdinal("ContactInfo")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("ContactInfo")),
                                Education = reader.GetString(reader.GetOrdinal("Education")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                                PositionID = reader.GetInt32(reader.GetOrdinal("PositionID")),
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                Department = reader.GetString(reader.GetOrdinal("Department")),
                                Position = reader.GetString(reader.GetOrdinal("Position")),
                                Login = reader.IsDBNull(reader.GetOrdinal("Login")) ?
                                    string.Empty : reader.GetString(reader.GetOrdinal("Login"))
                            });
                        }

                        StaffDataGrid.ItemsSource = _allEmployees;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки сотрудников: {ex.Message}", false).ShowDialog();
            }
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

        private void DepartmentFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allEmployees == null || _allEmployees.Count == 0)
                return;

            var filtered = _allEmployees.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filtered = filtered.Where(emp =>
                    emp.LastName.ToLower().Contains(searchText) ||
                    emp.FirstName.ToLower().Contains(searchText) ||
                    (emp.MiddleName != null && emp.MiddleName.ToLower().Contains(searchText)));
            }

            if (DepartmentFilterComboBox.SelectedValue != null &&
                DepartmentFilterComboBox.SelectedValue is int deptId)
            {
                filtered = filtered.Where(emp => emp.DepartmentID == deptId);
            }

            StaffDataGrid.ItemsSource = filtered.ToList();
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            DepartmentFilterComboBox.SelectedIndex = -1;
            StaffDataGrid.ItemsSource = _allEmployees;
        }

        private void StaffDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StaffDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var detailsWindow = new EmployeeDetails(selectedEmployee);
                detailsWindow.ShowDialog();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_staff_Click(object sender, RoutedEventArgs e)
        {
            Add_staff add_Staff = new Add_staff();
            add_Staff.ShowDialog();
            LoadStaffData();
        }

        private void Delete_staff_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = StaffDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                new CustomBox("Выберите сотрудника для удаления.", false).ShowDialog();
                return;
            }

            var confirmMessageBox = new CustomBox(
                $"Вы уверены, что хотите удалить сотрудника {selectedEmployee.LastName} {selectedEmployee.FirstName}?",
                true);
            confirmMessageBox.ShowDialog();

            if (confirmMessageBox.Result)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Staff WHERE EmployeeID = @EmployeeID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@EmployeeID", selectedEmployee.EmployeeID);
                        command.ExecuteNonQuery();
                    }

                    LoadStaffData();
                }
                catch (Exception ex)
                {
                    new CustomBox($"Ошибка при удалении сотрудника: {ex.Message}", false).ShowDialog();
                }
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = StaffDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                new CustomBox("Выберите сотрудника для редактирования.", false).ShowDialog();
                return;
            }

            var editWindow = new Edit_staff(selectedEmployee);
            if (editWindow.ShowDialog() == true)
            {
                LoadStaffData();
            }
        }
    }
}