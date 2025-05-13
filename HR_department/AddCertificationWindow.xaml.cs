using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HR_department
{
    public partial class AddCertificationWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";

        public int SelectedEmployeeID { get; private set; }
        public DateTime CertificationDate { get; private set; }

        public AddCertificationWindow()
        {
            InitializeComponent();
            LoadEmployees();
            CertificationDatePicker.SelectedDate = DateTime.Today.AddDays(14);
            CertificationDatePicker.DisplayDateStart = DateTime.Today.AddDays(7);
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT 
                                    EmployeeID, 
                                    LastName, 
                                    FirstName, 
                                    MiddleName,
                                    LastName + ' ' + FirstName + ' ' + ISNULL(MiddleName, '') AS FullName
                                FROM Staff
                                ORDER BY LastName, FirstName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable employeesTable = new DataTable();
                        employeesTable.Load(command.ExecuteReader());
                        EmployeeComboBox.ItemsSource = employeesTable.DefaultView;
                        EmployeeComboBox.DisplayMemberPath = "FullName";
                        EmployeeComboBox.SelectedValuePath = "EmployeeID";
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка загрузки сотрудников: {ex.Message}", false).ShowDialog();
            }
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            if (EmployeeComboBox.SelectedValue == null)
            {
                EmployeeComboBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                EmployeeComboBox.ClearValue(ComboBox.BorderBrushProperty);
            }

            if (CertificationDatePicker.SelectedDate == null ||
                CertificationDatePicker.SelectedDate < DateTime.Today.AddDays(7))
            {
                CertificationDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                CertificationDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (!isValid)
            {
                var message = "Пожалуйста, заполните все поля правильно:\n" +
                             "- Выберите сотрудника\n" +
                             "- Дата аттестации должна быть не раньше чем через неделю от сегодняшней даты";

                var customBox = new CustomBox(message, false);
                customBox.Owner = this;
                customBox.ShowDialog();
            }

            return isValid;
        }

        private bool CertificationExists(int employeeId, DateTime certificationDate)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT COUNT(*) FROM Certifications 
                                   WHERE EmployeeID = @EmployeeID 
                                   AND CertificationDate = @CertificationDate
                                   AND StatusID = 1"; 

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employeeId);
                        command.Parameters.AddWithValue("@CertificationDate", certificationDate);

                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                new CustomBox($"Ошибка проверки существующей аттестации: {ex.Message}", false).ShowDialog();
                return true; 
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            SelectedEmployeeID = (int)EmployeeComboBox.SelectedValue;
            CertificationDate = CertificationDatePicker.SelectedDate.Value;
            if (CertificationExists(SelectedEmployeeID, CertificationDate))
            {
                new CustomBox("Для выбранного сотрудника уже запланирована аттестация на эту дату.", false).ShowDialog();
                return;
            }

            var confirmBox = new CustomBox("Вы уверены, что хотите назначить аттестацию?", true);
            confirmBox.Owner = this;
            if (confirmBox.ShowDialog() == true && confirmBox.Result)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = @"INSERT INTO Certifications 
                                       (EmployeeID, CertificationDate, Result, Status, StatusID, Recommendations)
                                       VALUES (@EmployeeID, @CertificationDate, 'Не завершено', 'Запланирована', 1, '')";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", SelectedEmployeeID);
                            command.Parameters.AddWithValue("@CertificationDate", CertificationDate);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                DialogResult = true;
                                Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    new CustomBox($"Ошибка при назначении аттестации: {ex.Message}", false).ShowDialog();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedValue != null ||
                CertificationDatePicker.SelectedDate != DateTime.Today.AddDays(14))
            {
                var confirmBox = new CustomBox("Вы действительно хотите закрыть окно? Все несохраненные данные будут потеряны.", true);
                confirmBox.Owner = this;
                if (confirmBox.ShowDialog() == true && confirmBox.Result)
                {
                    DialogResult = false;
                    Close();
                }
            }
            else
            {
                DialogResult = false;
                Close();
            }
        }
    }
}