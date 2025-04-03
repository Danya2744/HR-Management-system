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

        public Staff()
        {
            InitializeComponent();
            LoadStaffData();
        }

        private void LoadStaffData()
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
                                    d.DepartmentName AS Department, 
                                    p.PositionName AS Position,
                                    s.PositionID,
                                    s.DepartmentID
                                FROM 
                                    Staff s
                                JOIN 
                                    Departments d ON s.DepartmentID = d.DepartmentID
                                JOIN 
                                    Positions p ON s.PositionID = p.PositionID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Employee> employees = new List<Employee>();
                        while (reader.Read())
                        {
                            employees.Add(new Employee
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
                                Position = reader.GetString(reader.GetOrdinal("Position"))
                            });
                        }

                        StaffDataGrid.ItemsSource = employees;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessageBox = new CustomBox($"Ошибка загрузки сотрудников: {ex.Message}", false);
                errorMessageBox.ShowDialog();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StaffDataGrid.ItemsSource is List<Employee> employees)
            {
                var searchText = SearchTextBox.Text.ToLower();
                var filteredData = employees
                    .Where(emp => emp.LastName.ToLower().Contains(searchText) ||
                                  emp.FirstName.ToLower().Contains(searchText) ||
                                  (emp.MiddleName != null && emp.MiddleName.ToLower().Contains(searchText)) ||
                                  emp.Department.ToLower().Contains(searchText) ||
                                  emp.Position.ToLower().Contains(searchText))
                    .ToList();

                StaffDataGrid.ItemsSource = filteredData;
            }
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
            Admin admin = new Admin();
            admin.Show();
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
                var errorMessageBox = new CustomBox("Выберите сотрудника для удаления.", false);
                errorMessageBox.ShowDialog();
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
                    var errorMessageBox = new CustomBox($"Ошибка при удалении сотрудника: {ex.Message}", false);
                    errorMessageBox.ShowDialog();
                }
            }
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = StaffDataGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                var errorMessageBox = new CustomBox("Выберите сотрудника для редактирования.", false);
                errorMessageBox.ShowDialog();
                return;
            }

            var editWindow = new Edit_staff(selectedEmployee);
            if (editWindow.ShowDialog() == true)
            {
                LoadStaffData();
            }
        }
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ContactInfo { get; set; }
        public string Education { get; set; }
        public DateTime HireDate { get; set; }
        public int PositionID { get; set; }
        public int DepartmentID { get; set; }

        public string Department { get; set; }
        public string Position { get; set; }
    }
}