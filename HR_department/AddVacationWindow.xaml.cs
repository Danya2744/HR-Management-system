using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HR_department
{
    public partial class AddVacationWindow : Window
    {
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";
        private readonly int _employeeId;
        public DateTime EarliestStartDate { get; } = DateTime.Today.AddMonths(1);

        public AddVacationWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            StartDatePicker.DisplayDateStart = EarliestStartDate;
            StartDatePicker.SelectedDate = EarliestStartDate;
            EndDatePicker.SelectedDate = EarliestStartDate.AddDays(14);
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            if (StartDatePicker.SelectedDate == null || StartDatePicker.SelectedDate < EarliestStartDate)
            {
                StartDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                StartDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (EndDatePicker.SelectedDate == null || EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
            {
                EndDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                EndDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(VacationTypeTextBox.Text))
            {
                VacationTypeTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else if (VacationTypeTextBox.Text.Length < 30)
            {
                VacationTypeTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                VacationTypeTextBox.ClearValue(TextBox.BorderBrushProperty);
            }

            if (!isValid)
            {
                var message = "Пожалуйста, заполните все поля правильно:\n" +
                              "- Дата окончания должна быть позже даты начала\n" +
                              "- Тип отпуска обязателен (минимум 30 символов)";

                var customBox = new CustomBox(message, false);
                customBox.Owner = this;
                customBox.ShowDialog();
            }

            return isValid;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            var confirmBox = new CustomBox("Вы уверены, что хотите отправить заявку на отпуск?", true);
            confirmBox.Owner = this;
            if (confirmBox.ShowDialog() == true && confirmBox.Result)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();

                        int statusId = 0;
                        string statusQuery = "SELECT StatusID FROM LeaveStatus WHERE StatusName = 'На рассмотрении'";
                        using (SqlCommand statusCommand = new SqlCommand(statusQuery, connection))
                        {
                            var result = statusCommand.ExecuteScalar();
                            if (result != null)
                            {
                                statusId = Convert.ToInt32(result);
                            }
                            else
                            {
                                throw new Exception("Статус 'На рассмотрении' не найден в базе данных");
                            }
                        }

                        string query = @"INSERT INTO VacationRequests 
                                       (EmployeeID, StartDate, EndDate, VacationType, Comment, StatusID) 
                                       VALUES (@EmployeeID, @StartDate, @EndDate, @VacationType, @Comment, @StatusID)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EmployeeID", _employeeId);
                            command.Parameters.AddWithValue("@StartDate", StartDatePicker.SelectedDate);
                            command.Parameters.AddWithValue("@EndDate", EndDatePicker.SelectedDate);
                            command.Parameters.AddWithValue("@VacationType", VacationTypeTextBox.Text.Trim());
                            command.Parameters.AddWithValue("@Comment", string.IsNullOrWhiteSpace(CommentTextBox.Text) ? DBNull.Value : (object)CommentTextBox.Text.Trim());
                            command.Parameters.AddWithValue("@StatusID", statusId);

                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                var successBox = new CustomBox("Заявка на отпуск успешно отправлена!", false);
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
            if (StartDatePicker.SelectedDate != EarliestStartDate ||
                EndDatePicker.SelectedDate != EarliestStartDate.AddDays(14) ||
                !string.IsNullOrWhiteSpace(CommentTextBox.Text) ||
                !string.IsNullOrWhiteSpace(VacationTypeTextBox.Text))
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

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate != null && EndDatePicker.SelectedDate != null)
            {
                if (EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
                {
                    EndDatePicker.SelectedDate = StartDatePicker.SelectedDate.Value.AddDays(1);
                }
            }
        }

        private void VacationTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VacationTypeTextBox.Text.Length > 50)
            {
                VacationTypeTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                VacationTypeTextBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
    }
}