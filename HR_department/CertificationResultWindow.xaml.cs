using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HR_department
{
    public partial class CertificationResultWindow : Window
    {
        public string Result { get; private set; } = "Пройдена";
        public string Recommendations { get; private set; }
        private bool _hasUnsavedChanges = false;

        public CertificationResultWindow()
        {
            InitializeComponent();
            RecommendationsTextBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _hasUnsavedChanges = true;
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(ResultTextBox.Text) ||
                ResultTextBox.Text.Length < 7 ||
                ResultTextBox.Text.Length > 50)
            {
                ResultTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                ResultTextBox.ClearValue(TextBox.BorderBrushProperty);
            }

            if (RecommendationsTextBox.Text.Length > 300)
            {
                RecommendationsTextBox.BorderBrush = Brushes.Red;
                isValid = false;
            }
            else
            {
                RecommendationsTextBox.ClearValue(TextBox.BorderBrushProperty);
            }

            if (!isValid)
            {
                var message = "Пожалуйста, проверьте введенные данные:\n" +
                             "- Результат должен содержать от 7 до 50 символов\n" +
                             "- Рекомендации не должны превышать 300 символов";

                new CustomBox(message, false).ShowDialog();
            }

            return isValid;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            Recommendations = RecommendationsTextBox.Text;
            _hasUnsavedChanges = false;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var confirmBox = new CustomBox(
                    "Вы действительно хотите закрыть окно? Все несохраненные данные будут потеряны.",
                    true);
                confirmBox.Owner = this;
                if (confirmBox.ShowDialog() != true || !confirmBox.Result)
                {
                    return;
                }
            }

            DialogResult = false;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var confirmBox = new CustomBox(
                    "Вы действительно хотите закрыть окно? Все несохраненные данные будут потеряны.",
                    true);
                confirmBox.Owner = this;
                if (confirmBox.ShowDialog() != true || !confirmBox.Result)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}