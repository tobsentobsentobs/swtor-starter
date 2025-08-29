using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace SWTORStarter
{
    /// <summary>
    /// Einstellungsfenster für die SWTOR Starter Anwendung
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private AppConfig _config;
        private readonly string _configPath;

        /// <summary>
        /// Initialisiert eine neue Instanz der SettingsWindow-Klasse
        /// </summary>
        /// <param name="config">Anwendungskonfiguration</param>
        public SettingsWindow(AppConfig config)
        {
            InitializeComponent();
            _config = config;
            _configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SWTORStarter", "config.json");
            
            LoadSettings();
        }

        /// <summary>
        /// Lädt die aktuellen Einstellungen in die UI
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                AutoStartSteamCheckBox.IsChecked = _config.AutoStartSteam ?? true;
                AutoStartSWTORCheckBox.IsChecked = _config.AutoStartSWTOR ?? true;
                AutoFillPasswordCheckBox.IsChecked = _config.AutoFillPassword ?? true;

                AutoStartTimerCheckBox.IsChecked = _config.AutoStartTimerEnabled ?? false;
                AutoStartTimerDelayTextBox.Text = (_config.AutoStartTimerDelay ?? 5).ToString();
                
                SteamStartDelayTextBox.Text = (_config.SteamStartDelay ?? 30).ToString();
                SWTORLauncherDelayTextBox.Text = (_config.SWTORLauncherDelay ?? 45).ToString();
                PasswordInputDelayTextBox.Text = (_config.PasswordInputDelay ?? 5).ToString();
                
                LauncherRamThresholdTextBox.Text = (_config.LauncherRamThresholdMB ?? 70).ToString();

                switch (_config.SteamStartMode ?? "minimized")
                {
                    case "normal":
                        SteamNormalRadioButton.IsChecked = true;
                        break;
                    case "minimized":
                        SteamMinimizedRadioButton.IsChecked = true;
                        break;
                    case "silent":
                        SteamSilentRadioButton.IsChecked = true;
                        break;
                }

                SteamPathTextBox.Text = _config.SteamPath ?? "Automatisch erkannt";

                SWTORPathTextBox.Text = _config.SWTORPath ?? "Automatisch erkannt";

                UseWaitForInputIdleCheckBox.IsChecked = _config.UseWaitForInputIdle ?? true;
                UseCPUMonitoringCheckBox.IsChecked = _config.UseCPUMonitoring ?? true;
                UseMemoryMonitoringCheckBox.IsChecked = _config.UseMemoryMonitoring ?? true;
                UseWindowStabilityCheckBox.IsChecked = _config.UseWindowStability ?? true;

                DebugModeCheckBox.IsChecked = _config.DebugMode ?? false;
                ShowStatusBoxCheckBox.IsChecked = _config.ShowStatusBox ?? true;
                AutoSaveLogsCheckBox.IsChecked = _config.AutoSaveLogs ?? false;
                MonitoringIntervalTextBox.Text = (_config.MonitoringInterval ?? 500).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Einstellungen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Speichert die aktuellen Einstellungen
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                _config.AutoStartSteam = AutoStartSteamCheckBox.IsChecked ?? true;
                _config.AutoStartSWTOR = AutoStartSWTORCheckBox.IsChecked ?? true;
                _config.AutoFillPassword = AutoFillPasswordCheckBox.IsChecked ?? true;
                
                _config.AutoStartTimerEnabled = AutoStartTimerCheckBox.IsChecked ?? false;
                if (int.TryParse(AutoStartTimerDelayTextBox.Text, out int timerDelay))
                    _config.AutoStartTimerDelay = timerDelay;

                if (int.TryParse(SteamStartDelayTextBox.Text, out int steamDelay))
                    _config.SteamStartDelay = steamDelay;
                if (int.TryParse(SWTORLauncherDelayTextBox.Text, out int swtorDelay))
                    _config.SWTORLauncherDelay = swtorDelay;
                if (int.TryParse(PasswordInputDelayTextBox.Text, out int passwordDelay))
                    _config.PasswordInputDelay = passwordDelay;
                
                if (int.TryParse(LauncherRamThresholdTextBox.Text, out int ramThreshold))
                    _config.LauncherRamThresholdMB = ramThreshold;

                if (SteamNormalRadioButton.IsChecked == true)
                    _config.SteamStartMode = "normal";
                else if (SteamMinimizedRadioButton.IsChecked == true)
                    _config.SteamStartMode = "minimized";
                else if (SteamSilentRadioButton.IsChecked == true)
                    _config.SteamStartMode = "silent";

                if (SWTORPathTextBox.Text != "Automatisch erkannt")
                    _config.SWTORPath = SWTORPathTextBox.Text;

                _config.UseWaitForInputIdle = UseWaitForInputIdleCheckBox.IsChecked ?? true;
                _config.UseCPUMonitoring = UseCPUMonitoringCheckBox.IsChecked ?? true;
                _config.UseMemoryMonitoring = UseMemoryMonitoringCheckBox.IsChecked ?? true;
                _config.UseWindowStability = UseWindowStabilityCheckBox.IsChecked ?? true;

                _config.DebugMode = DebugModeCheckBox.IsChecked ?? false;
                _config.ShowStatusBox = ShowStatusBoxCheckBox.IsChecked ?? true;
                _config.AutoSaveLogs = AutoSaveLogsCheckBox.IsChecked ?? false;
                if (int.TryParse(MonitoringIntervalTextBox.Text, out int monitoringInterval))
                    _config.MonitoringInterval = monitoringInterval;

                var configDir = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(configDir))
                    Directory.CreateDirectory(configDir!);

                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_configPath, json);

                MessageBox.Show("Einstellungen erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Einstellungen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event-Handler für den Steam-Pfad-Browse-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void BrowseSteamPath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Steam.exe auswählen",
                Filter = "Steam.exe|Steam.exe|Alle Dateien|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Steam"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SteamPathTextBox.Text = openFileDialog.FileName;
                _config.SteamPath = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Event-Handler für den SWTOR-Pfad-Browse-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void BrowseSWTORPath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "SWTOR.exe auswählen",
                Filter = "SWTOR.exe|swtor.exe|Alle Dateien|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Electronic Arts\BioWare\Star Wars - The Old Republic\swtor\retailclient"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SWTORPathTextBox.Text = openFileDialog.FileName;
                _config.SWTORPath = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Event-Handler für den Reset-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Möchten Sie alle Einstellungen auf die Standardwerte zurücksetzen?", 
                                       "Zurücksetzen bestätigen", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _config = new AppConfig();
                LoadSettings();
            }
        }

        /// <summary>
        /// Event-Handler für den Speichern-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// Event-Handler für das Ziehen der Titelleiste
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Event-Handler für den Minimieren-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Event-Handler für den Schließen-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
