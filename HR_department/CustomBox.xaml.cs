using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HR_department
{
    /// <summary>
    /// Логика взаимодействия для CustomBox.xaml
    /// </summary>
    public partial class CustomBox : Window
    {
        public bool Result { get; private set; } = false;

        public CustomBox(string message, bool showCancelButton = true)
        {
            InitializeComponent();
            MessageText.Text = message;

            if (!showCancelButton)
            {
                CancelButton.Visibility = Visibility.Collapsed;
                OkButton.Margin = new Thickness(0);
                OkButton.Content = "ОК";
                ButtonsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
