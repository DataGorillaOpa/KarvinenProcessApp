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

namespace KarvinenApp
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Dictionary<string, string> UpdatedPaths { get; private set; }

        public SettingsWindow(Dictionary<string, string> currentPaths)
        {
            InitializeComponent();

            // Load current paths into textboxes
            txtPath1.Text = currentPaths["C:\\Users\\OlliSalminen\\Documents\\PöytäkirjaHaku\\PoytakirjaTool\\FilterPoytakirjas\\bin\\Debug\\FilterPoytakirjas.exe"];
            txtPath2.Text = currentPaths["C:\\Users\\OlliSalminen\\Documents\\PöytäkirjaHaku\\PoytakirjaTool - Copy\\FilterPoytakirjas\\bin\\Debug\\FilterPoytakirjas.exe"];
            txtPath3.Text = currentPaths["C:\\Users\\OlliSalminen\\Documents\\PöytäkirjaHaku\\PoytakirjaTool - Copy - Copy\\FilterPoytakirjas\\bin\\Debug\\FilterPoytakirjas.exe"];
            txtPath4.Text = currentPaths["C:\\Users\\OlliSalminen\\Documents\\PöytäkirjaHaku\\PoytakirjaTool - Copy (2)\\FilterPoytakirjas\\bin\\Debug\\FilterPoytakirjas.exe"];
            txtPath5.Text = currentPaths["C:\\Users\\OlliSalminen\\Documents\\PöytäkirjaHaku\\PoytakirjaTool - Copy (3)\\FilterPoytakirjas\\bin\\Debug\\FilterPoytakirjas.exe"];
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
