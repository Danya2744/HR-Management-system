﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HR_department
{
    public partial class Admin : Window
    {
        private readonly int _adminId;
        private MainWindow _authWindow;
        private readonly bool _isManager;

        public Admin(int adminId, MainWindow authWindow = null, bool isManager = false)
        {
            InitializeComponent();
            _adminId = adminId;
            _authWindow = authWindow;
            _isManager = isManager;

            if (_isManager)
            {
                this.Title = "Руководитель";
            }

            this.Closing += Admin_Closing;
        }

        private void Admin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var authWindow = new MainWindow();
            authWindow.Show();
        }

        private void staff_Click(object sender, RoutedEventArgs e)
        {
            var staff = new Staff(_isManager, _adminId); 
            staff.Owner = this;
            staff.ShowDialog();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new AdminAccountWindow(_adminId);
            accountWindow.Owner = this;
            accountWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_authWindow != null)
            {
                _authWindow.Show();
            }
        }

        private void SickLeavesButton_Click(object sender, RoutedEventArgs e)
        {
            var sickLeavesWindow = new AdminSickLeavesWindow();
            sickLeavesWindow.Owner = this;
            sickLeavesWindow.ShowDialog();
        }

        private void VacationButton_Click(object sender, RoutedEventArgs e)
        {
            var vacationwindow = new AdminVacationRequestsWindow();
            vacationwindow.Owner = this;
            vacationwindow.ShowDialog();
        }

        private void CertificationButton_Click(object sender, RoutedEventArgs e)
        {
            var certWindow = new AdminCertificationsWindow();
            certWindow.Owner = this;
            certWindow.ShowDialog();
        }

        private void AchievementButton_Click(object sender, RoutedEventArgs e)
        {
            var achWindow = new AdminAchievementsWindow();
            achWindow.Owner = this;
            achWindow.ShowDialog();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ReportsWindow reportsWindow = new ReportsWindow
            {
                Owner = this
            };
            reportsWindow.Show();
            Hide();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("help.chm");
        }
    }
}
