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
    public partial class AchievementDetailsWindow : Window
    {
        public string AchievementType { get; set; }
        public string Description { get; set; }
        public string Reward { get; set; }
        public string Status { get; set; }

        public AchievementDetailsWindow(EmployeeAchievement achievement)
        {
            InitializeComponent();
            DataContext = achievement;

            AchievementType = achievement.AchievementType;
            Description = achievement.Description;
            Reward = achievement.Reward;
            Status = achievement.Status;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
