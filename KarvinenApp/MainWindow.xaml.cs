using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json; // Add this for JsonConvert

namespace KarvinenApp
{
    public partial class MainWindow : Window
    {
        private List<Process> _runningProcesses = new List<Process>(); // Track multiple processes
        private Dictionary<string, string> _processPaths;

        public MainWindow()
        {
            InitializeComponent();
            LoadPaths();
        }

        private void LoadPaths()
        {
            try
            {
                string configPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "process_config.json");

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _processPaths = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                else
                {
                    _processPaths = new Dictionary<string, string>
                    {
                        {"Database Search", ""},
                        {"Log Analyzer", ""},
                        {"File Processor", ""},
                        {"System Report", ""},
                        {"Data Sync", ""}
                    };
                    SavePaths(); // Create initial config
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}");
                _processPaths = new Dictionary<string, string>();
            }
        }

        private void SavePaths()
        {
            try
            {
                string configPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "process_config.json");

                File.WriteAllText(configPath,
                    JsonConvert.SerializeObject(_processPaths, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}");
            }
        }

        private void DatabaseBackup_Click(object sender, RoutedEventArgs e)
        {
            ActiveScriptButton.Content = ((Button)sender).Content;
            OriginalButtonPanel.Visibility = Visibility.Collapsed;
            ActiveButtonPanel.Visibility = Visibility.Visible;

            StartSqlProcesses(); // Renamed to reflect multiple processes
        }

        private void StartSqlProcesses()
        {
            try
            {
                // Use actual paths without @ and extra quotes
                string[] processPaths = new string[]
                {
                    @"C:\Users\Olli\source\repos\KarvinenRuotsiPöytäkirjaHaku\KarvinenRuotsiPöytäkirjaHaku\bin\Debug\net8.0\KarvinenRuotsiPöytäkirjaHaku.exe",
                    @"C:\Path\To\Process2.exe",
                    @"C:\Path\To\Process3.exe",
                    @"C:\Path\To\Process4.exe",
                    @"C:\Path\To\Process5.exe"
                };

                CleanupProcesses(); // Stop any existing processes

                foreach (string processPath in processPaths)
                {
                    if (File.Exists(processPath))
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = processPath,
                                UseShellExecute = true,
                                CreateNoWindow = false
                            }
                        };

                        if (process.Start())
                        {
                            _runningProcesses.Add(process);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {processPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start processes: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                ResetUI();
            }
        }

        private void CleanupProcesses()
        {
            foreach (var process in _runningProcesses)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill();
                    process.Dispose();
                }
                catch { /* Ignore cleanup errors */ }
            }
            _runningProcesses.Clear();
        }

        private void QuitScript_Click(object sender, RoutedEventArgs e)
        {
            CleanupProcesses();
            ResetUI();
        }

        private void ResetUI()
        {
            ActiveButtonPanel.Visibility = Visibility.Collapsed;
            OriginalButtonPanel.Visibility = Visibility.Visible;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_processPaths);
            if (settingsWindow.ShowDialog() == true)
            {
                _processPaths = settingsWindow.UpdatedPaths;
                SavePaths();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{((Button)sender).Content} clicked");
        }
    }
}