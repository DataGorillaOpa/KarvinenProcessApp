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

            StartSqlProcessesKunta(); // Renamed to reflect multiple processes
        }

        private void StartSqlProcessesKunta()
        {
            try
            {
                Dictionary<string, string> processPaths = GetDefaultPaths();
                CleanupProcesses();

                int totalProcesses = processPaths.Count;
                int completedProcesses = 0;

                foreach (var kvp in processPaths)
                {
                    string processName = kvp.Key;
                    string processPath = kvp.Value;

                    if (File.Exists(processPath))
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = processPath,
                                UseShellExecute = true,
                                CreateNoWindow = false
                            },
                            EnableRaisingEvents = true
                        };

                        process.Exited += (sender, args) =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                completedProcesses++;
                                UpdateProgress(completedProcesses, totalProcesses);

                                if (completedProcesses == totalProcesses)
                                {
                                    AllProcessesCompleted();
                                }
                            });
                        };

                        if (process.Start())
                        {
                            _runningProcesses.Add(process);
                            ActiveScriptButton.Content = $"Running {processName}...";
                        }
                    }
                    else
                    {
                        MessageBox.Show($"File not found: {processPath}");
                        completedProcesses++; // Count missing files as "completed"
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

        private void UpdateProgress(int completed, int total)
        {
            ActiveScriptButton.Content = $"Progress: {completed}/{total} completed";
            // Optional: Update a progress bar if you have one
            // progressBar.Value = (double)completed/total * 100;
        }

        private void AllProcessesCompleted()
        {
            //MessageBox.Show("All processes have finished!", "Completion",
                          //MessageBoxButton.OK, MessageBoxImage.Information);
            ResetUI();
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
        private Dictionary<string, string> GetDefaultPaths()
        {
            return new Dictionary<string, string>
    {
        {"Database Search", @"C:\Users\OlliSalminen\Documents\PöytäkirjaHakuAutomation\PoytakirjaTool\FilterPoytakirjas\bin\Debug\FilterPoytakirjas.exe"},
        {"Log Analyzer", @"C:\Users\OlliSalminen\Documents\PöytäkirjaHakuAutomation\PoytakirjaTool - Copy\FilterPoytakirjas\bin\Debug\FilterPoytakirjas.exe"},
        {"File Processor", @"C:\Users\OlliSalminen\Documents\PöytäkirjaHakuAutomation\PoytakirjaTool - Copy - Copy\FilterPoytakirjas\bin\Debug\FilterPoytakirjas.exe"},
        {"System Report", @"C:\Users\OlliSalminen\Documents\PöytäkirjaHakuAutomation\PoytakirjaTool - Copy (2)\FilterPoytakirjas\bin\Debug\FilterPoytakirjas.exe"},
        {"Data Sync", @"C:\Users\OlliSalminen\Documents\PöytäkirjaHakuAutomation\PoytakirjaTool - Copy (3)\FilterPoytakirjas\bin\Debug\FilterPoytakirjas.exe"}
    };
        }
    }
}