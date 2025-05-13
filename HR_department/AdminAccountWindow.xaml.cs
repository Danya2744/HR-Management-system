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
    public partial class AdminAccountWindow : Window
    {
        private readonly int _adminId;
        private const string ConnectionString = "Server=localhost\\MSSQLSERVER1;Database=HR_department;Trusted_Connection=True;TrustServerCertificate=True";

        public AdminAccountWindow(int adminId)
        {
            InitializeComponent();
            _adminId = adminId;
            ContentFrame.Navigate(new EmployeeInfoPage(_adminId, ConnectionString));

            SetActiveButton(PersonalInfoButton);
        }

        private void SetActiveButton(Button activeButton)
        {
            PersonalInfoButton.Style = (Style)FindResource("NavButtonStyle");
            SickLeavesButton.Style = (Style)FindResource("NavButtonStyle");
            VacationsButton.Style = (Style)FindResource("NavButtonStyle");
            CertificationsButton.Style = (Style)FindResource("NavButtonStyle");
            AchievementsButton.Style = (Style)FindResource("NavButtonStyle");
            activeButton.Style = (Style)FindResource("ActiveNavButtonStyle");
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SetActiveButton(button);
                switch (button.Name)
                {
                    case "PersonalInfoButton":
                        ContentFrame.Navigate(new EmployeeInfoPage(_adminId, ConnectionString));
                        break;
                    case "SickLeavesButton":
                        ContentFrame.Navigate(new SickLeavesPage(_adminId));
                        break;
                    case "VacationsButton":
                        ContentFrame.Navigate(new VacationPage(_adminId));
                        break;
                    case "CertificationsButton":
                        ContentFrame.Navigate(new CertificationPage(_adminId));
                        break;
                    case "AchievementsButton":
                        ContentFrame.Navigate(new AchievementsPage(_adminId));
                        break;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
