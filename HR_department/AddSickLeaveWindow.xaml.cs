using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HR_department
{
    public partial class AddSickLeaveWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;

        public AddSickLeaveWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            StartDatePicker.SelectedDate = DateTime.Today;
            StartDatePicker.DisplayDateStart = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);
            ReasonTextBox.Focus();
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            if (StartDatePicker.SelectedDate == null || StartDatePicker.SelectedDate < DateTime.Today)
            {
                StartDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                StartDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (EndDatePicker.SelectedDate == null || EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
            {
                EndDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                EndDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(ReasonTextBox.Text) || ReasonTextBox.Text.Length < 50)
            {
                ReasonTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                ReasonTextBox.ClearValue(TextBox.BorderBrushProperty);
            }

            if (!isValid)
            {
                var message = "Пожалуйста, заполните все поля правильно:\n" +
                              "- Дата начала должна быть сегодня или позже\n" +
                              "- Дата окончания должна быть позже даты начала\n" +
                              "- Причина должна содержать минимум 50 символов";

                var customBox = new CustomBox(message, false);
                customBox.Owner = this;
                customBox.ShowDialog();
            }

            return isValid;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            var confirmBox = new CustomBox("Вы уверены, что хотите отправить заявку на больничный?", true);
            confirmBox.Owner = this;
            if (confirmBox.ShowDialog() == true && confirmBox.Result)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        string query = @"INSERT INTO SickLeaves 
                                       (EmployeeID, StartDate, EndDate, Reason, StatusID) 
                                       VALUES (@EmployeeID, @StartDate, @EndDate, @Reason, 
                                       (SELECT StatusID FROM LeaveStatus WHERE StatusName = 'На рассмотрении'))";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", _employeeId);
                            command.Parameters.AddWithValue("@StartDate", StartDatePicker.SelectedDate);
                            command.Parameters.AddWithValue("@EndDate", EndDatePicker.SelectedDate);
                            command.Parameters.AddWithValue("@Reason", ReasonTextBox.Text.Trim());

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                var successBox = new CustomBox("Заявка на больничный успешно отправлена!", false);
                                successBox.Owner = this;
                                successBox.ShowDialog();
                                this.DialogResult = true;
                                this.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorBox = new CustomBox($"Ошибка при отправке заявки: {ex.Message}", false);
                    errorBox.Owner = this;
                    errorBox.ShowDialog();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ReasonTextBox.Text) || StartDatePicker.SelectedDate != DateTime.Today || EndDatePicker.SelectedDate != DateTime.Today.AddDays(1))
            {
                var confirmBox = new CustomBox("Вы действительно хотите закрыть окно? Все несохраненные данные будут потеряны.", true);
                confirmBox.Owner = this;
                if (confirmBox.ShowDialog() == true && confirmBox.Result)
                {
                    this.DialogResult = false;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}