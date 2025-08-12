using System;
using System.Collections.Generic;
using System.Windows;

namespace KarvinenApp
{
    public partial class SettingsWindow : Window
    {
        public Dictionary<string, string> UpdatedPaths { get; private set; }

        public SettingsWindow(Dictionary<string, string> currentPaths)
        {
            InitializeComponent();
            UpdatedPaths = new Dictionary<string, string>(currentPaths);

            // Load current paths into textboxes using the correct keys
            txtPath1.Text = currentPaths["Database Search"];
            txtPath2.Text = currentPaths["Log Analyzer"];
            txtPath3.Text = currentPaths["File Processor"];
            txtPath4.Text = currentPaths["System Report"];
            txtPath5.Text = currentPaths["Data Sync"];
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            UpdatedPaths = new Dictionary<string, string>
            {
                {"Database Search", txtPath1.Text},
                {"Log Analyzer", txtPath2.Text},
                {"File Processor", txtPath3.Text},
                {"System Report", txtPath4.Text},
                {"Data Sync", txtPath5.Text}
            };

            DialogResult = true;
            Close();
        }
    }
}