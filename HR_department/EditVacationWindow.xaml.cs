using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HR_department
{
    public partial class EditVacationWindow : Window
    {
        public DateTime NewStartDate { get; private set; }
        public DateTime NewEndDate { get; private set; }
        public DateTime EarliestStartDate { get; } = DateTime.Today.AddMonths(1);

        public EditVacationWindow(VacationRequest request)
        {
            InitializeComponent();
            StartDatePicker.DisplayDateStart = EarliestStartDate;
            StartDatePicker.SelectedDate = request.StartDate;
            EndDatePicker.SelectedDate = request.EndDate;
        }

        private bool ValidateDates()
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

            if (EndDatePicker.SelectedDate == null ||
                (StartDatePicker.SelectedDate != null && EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate))
            {
                EndDatePicker.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                EndDatePicker.ClearValue(DatePicker.BorderBrushProperty);
            }

            if (!isValid)
            {
                new CustomBox("Пожалуйста, укажите корректные даты:\n" +
                              "- Дата окончания должна быть позже даты начала", false).ShowDialog();
            }

            return isValid;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateDates()) return;

            NewStartDate = StartDatePicker.SelectedDate.Value;
            NewEndDate = EndDatePicker.SelectedDate.Value;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
    }
}