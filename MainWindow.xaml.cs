using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json;
using SWTORStarter.Controls;

namespace SWTORStarter
{
    public partial class MainWindow : Window
    {
        private readonly string _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SWTORStarter", "config.json");
        
        private readonly string _configDir;
        private AppConfig _config = new AppConfig();
        private bool _isSteamRunning = false;
        private bool _isSWTORLauncherRunning = false;
        private bool _isSWTORGameRunning = false;
        private Window? _statusBoxWindow = null;
        private DateTime _startTime = DateTime.Now;
        private DispatcherTimer _steamCheckTimer;
        private DispatcherTimer _swtorCheckTimer;
        private DispatcherTimer _statusUpdateTimer;
        private DispatcherTimer _autoStartTimer;
        private DispatcherTimer _countdownTimer;
        private int _countdownSeconds = 5;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                this.WindowState = WindowState.Normal;
                this.Visibility = Visibility.Visible;
                this.ShowInTaskbar = true;
                this.Show();
                this.Activate();
                this.Focus();
                
                if (this.Width <= 0 || this.Height <= 0)
                {
                    this.Width = 1100;
                    this.Height = 700;
                }
                
                if (this.Left <= 0 || this.Top <= 0)
                {
                    this.Left = (System.Windows.SystemParameters.WorkArea.Width - this.Width) / 2;
                    this.Top = (System.Windows.SystemParameters.WorkArea.Height - this.Height) / 2;
                }
                
                _configDir = Path.GetDirectoryName(_configPath)!;
                Directory.CreateDirectory(_configDir);
                
                LoadConfig();
                CheckPasswordOnStartup();
                
                _steamCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                _steamCheckTimer.Tick += SteamCheckTimer_Tick;
                _steamCheckTimer.Start();
                
                _swtorCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _swtorCheckTimer.Tick += SWTORCheckTimer_Tick;
                _swtorCheckTimer.Start();
                
                _statusUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
                _statusUpdateTimer.Start();
                
                _autoStartTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_config.AutoStartTimerDelay ?? 5) };
                _autoStartTimer.Tick += AutoStartTimer_Tick;
                
                _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _countdownTimer.Tick += CountdownTimer_Tick;
                
                if (_config.AutoStartTimerEnabled == true)
                {
                    StartAutoStartTimer();
                }
                
                _ = CheckSteamStatusAsync();
                UpdateStatusDisplay();
                CreateStatusBox();
                
                LogMessage("Anwendung gestartet");
                
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    try
                    {
                        this.Show();
                        this.Activate();
                        this.Focus();
                        this.WindowState = WindowState.Normal;
                        
                        if (this.Width <= 0 || this.Height <= 0)
                        {
                            this.Width = 1100;
                            this.Height = 700;
                        }
                        
                        if (this.Left <= 0 || this.Top <= 0)
                        {
                            this.Left = (System.Windows.SystemParameters.WorkArea.Width - this.Width) / 2;
                            this.Top = (System.Windows.SystemParameters.WorkArea.Height - this.Height) / 2;
                        }
                        
                        LogMessage("Fenster-Initialisierung abgeschlossen");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Fehler beim Anzeigen des Fensters: {ex.Message}");
                    }
                }));
                
                this.IsVisibleChanged += (sender, e) =>
                {
                    if (!this.IsVisible)
                    {
                        this.Show();
                        this.Activate();
                    }
                };
            }
            catch (Exception ex)
            {
                try
                {
                    this.Title = "SWTOR Starter - Fallback Mode";
                    this.Width = 800;
                    this.Height = 600;
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    this.WindowState = WindowState.Normal;
                    this.Visibility = Visibility.Visible;
                    this.ShowInTaskbar = true;
                    
                    var textBlock = new System.Windows.Controls.TextBlock
                    {
                        Text = $"SWTOR Starter läuft im Fallback-Modus.\nFehler beim Laden der UI: {ex.Message}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        TextWrapping = TextWrapping.Wrap
                    };
                    
                    this.Content = textBlock;
                    this.Show();
                    this.Activate();
                    this.Focus();
                    
                    System.Diagnostics.Debug.WriteLine($"MainWindow Fallback gestartet: {ex.Message}");
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Fallback fehlgeschlagen: {fallbackEx.Message}");
                    MessageBox.Show($"SWTOR Starter konnte nicht gestartet werden.\nFehler: {ex.Message}\nFallback fehlgeschlagen: {fallbackEx.Message}", 
                        "Kritischer Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Lädt die Konfiguration aus einer JSON-Datei
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    _config = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                }
                else
                {
                    _config = new AppConfig();
                }
            }
            catch (Exception ex)
            {
                _config = new AppConfig();
                LogMessage($"Fehler beim Laden der Konfiguration: {ex.Message}");
            }
        }

        /// <summary>
        /// Speichert die aktuelle Konfiguration in eine JSON-Datei
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Speichern der Konfiguration: {ex.Message}");
            }
        }

        /// <summary>
        /// Überprüft den Passwort-Status beim Start der Anwendung
        /// </summary>
        private void CheckPasswordOnStartup()
        {
            if (string.IsNullOrEmpty(_config.EncryptedPassword))
            {
                ShowPasswordInputState();
                PasswordStatusText.Text = "Kein Passwort gespeichert. Bitte geben Sie Ihr SWTOR-Passwort ein.";
                PasswordStatusText.Foreground = Brushes.Orange;
            }
            else
            {
                ShowPasswordSavedState();
            }
        }

        /// <summary>
        /// Event-Handler für den Passwort-Speichern-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void SavePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Bitte geben Sie ein Passwort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _config.EncryptedPassword = EncryptPassword(PasswordBox.Password);
                SaveConfig();
                
                ShowPasswordSavedState();
                
                PasswordBox.Password = "";
                LogMessage("Passwort gespeichert");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Passworts: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                LogMessage($"Fehler beim Speichern des Passworts: {ex.Message}");
            }
        }

        /// <summary>
        /// Zeigt den gespeicherten Passwort-Status an
        /// </summary>
        private void ShowPasswordSavedState()
        {
            PasswordStatusText.Text = "Passwort erfolgreich gespeichert ✓";
            PasswordStatusText.Foreground = Brushes.Green;
            
            PasswordBox.Visibility = Visibility.Collapsed;
            SavePasswordButton.Visibility = Visibility.Collapsed;
            
            PasswordButtonsPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Zeigt den Passwort-Eingabe-Status an
        /// </summary>
        private void ShowPasswordInputState()
        {
            PasswordBox.Visibility = Visibility.Visible;
            SavePasswordButton.Visibility = Visibility.Visible;
            
            PasswordButtonsPanel.Visibility = Visibility.Collapsed;
            
            PasswordStatusText.Text = "";
        }

        /// <summary>
        /// Event-Handler für den Passwort-Bearbeiten-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void EditPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPasswordInputState();
            PasswordBox.Focus();
        }

        /// <summary>
        /// Event-Handler für den Passwort-Löschen-Button
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Event-Argumente</param>
        private void DeletePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMessage("Lösche gespeichertes Passwort...");
                
                _config.EncryptedPassword = null;
                
                SaveConfig();
                
                ShowPasswordInputState();
                
                PasswordStatusText.Text = "Passwort wurde gelöscht";
                PasswordStatusText.Foreground = Brushes.Orange;
                PasswordStatusText.Visibility = Visibility.Visible;
                
                LogMessage("Gespeichertes Passwort wurde erfolgreich gelöscht");
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Löschen des Passworts: {ex.Message}");
                MessageBox.Show($"Fehler beim Löschen des Passworts: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMessage("Öffne Einstellungen...");
                
                var settingsWindow = new SettingsWindow(_config);
                settingsWindow.Owner = this;
                settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                
                var result = settingsWindow.ShowDialog();
                
                                if (result == true)
                {
                    LogMessage("Einstellungen wurden aktualisiert");
                    
                    LoadConfig();
                    
                    UpdateStatusDisplay();
                
                UpdateStatusBox();
                }
                else
                {
                    LogMessage("Einstellungen wurden nicht gespeichert");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Öffnen der Einstellungen: {ex.Message}");
                MessageBox.Show($"Fehler beim Öffnen der Einstellungen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Verschlüsselt ein Passwort mit AES-256
        /// </summary>
        /// <param name="password">Das zu verschlüsselnde Passwort</param>
        /// <returns>Das verschlüsselte Passwort als Base64-String</returns>
        private string EncryptPassword(string password)
        {
            var keyBytes = new byte[32];
            var keyString = "SWTORStarter2024!SecureKey32BytesLong!";
            var keyBytesFromString = Encoding.UTF8.GetBytes(keyString);
            Array.Copy(keyBytesFromString, keyBytes, Math.Min(keyBytesFromString.Length, keyBytes.Length));
            
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            
            using var encryptor = aes.CreateEncryptor();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var encryptedBytes = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);
            
            var result = new byte[aes.IV.Length + encryptedBytes.Length];
            Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
            Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);
            
            return Convert.ToBase64String(result);
        }

        private string DecryptPassword(string encryptedPassword)
        {
            try
            {

                var keyBytes = new byte[32];
                var keyString = "SWTORStarter2024!SecureKey32BytesLong!";
                var keyBytesFromString = Encoding.UTF8.GetBytes(keyString);
                Array.Copy(keyBytesFromString, keyBytes, Math.Min(keyBytesFromString.Length, keyBytes.Length));
                
                var encryptedBytes = Convert.FromBase64String(encryptedPassword);
                
                using var aes = Aes.Create();
                aes.Key = keyBytes;
                
                var iv = new byte[16];
                var encryptedData = new byte[encryptedBytes.Length - 16];
                
                Array.Copy(encryptedBytes, 0, iv, 0, 16);
                Array.Copy(encryptedBytes, 16, encryptedData, 0, encryptedBytes.Length - 16);
                
                aes.IV = iv;
                
                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private async void SteamCheckTimer_Tick(object? sender, EventArgs e)
        {
            await CheckSteamStatusAsync();
        }

        private async void SWTORCheckTimer_Tick(object? sender, EventArgs e)
        {
            await CheckSWTORStatusAsync();
        }

        private void StatusUpdateTimer_Tick(object? sender, EventArgs e)
        {
            UpdateStatusDisplay();
            UpdateStatusBox();
        }
        
        private void AutoStartTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                LogMessage("Auto-Start Timer abgelaufen - Starte SWTOR automatisch...");

                _autoStartTimer.Stop();

                if (LaunchButton.IsEnabled)
                {
                    LaunchButton_Click(this, new RoutedEventArgs());
                }
                else
                {
                    LogMessage("Start-Button ist deaktiviert - Auto-Start übersprungen");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Auto-Start Timer: {ex.Message}");
            }
        }
        
        private void StartAutoStartTimer()
        {
            try
            {
                if (_autoStartTimer != null)
                {
                    _autoStartTimer.Stop();
                    _autoStartTimer.Interval = TimeSpan.FromSeconds(_config.AutoStartTimerDelay ?? 5);
                    _autoStartTimer.Start();
                    
                    if (_countdownTimer != null)
                    {
                        _countdownTimer.Stop();
                        _countdownSeconds = _config.AutoStartTimerDelay ?? 5;
                        _countdownTimer.Start();
                        UpdateCountdownDisplay(_countdownSeconds);
                    }
                    
                    LogMessage($"Auto-Start Timer gestartet - Verzögerung: {_config.AutoStartTimerDelay ?? 5} Sekunden");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Starten des Auto-Start Timers: {ex.Message}");
            }
        }
        
        private void StartAutoStartTimerWithDelay(int delaySeconds)
        {
            try
            {
                if (_autoStartTimer != null)
                {
                    _autoStartTimer.Stop();
                    _autoStartTimer.Interval = TimeSpan.FromSeconds(delaySeconds);
                    _autoStartTimer.Start();
                    
                    if (_countdownTimer != null)
                    {
                        _countdownTimer.Stop();
                        _countdownSeconds = delaySeconds;
                        _countdownTimer.Start();
                        UpdateCountdownDisplay(_countdownSeconds);
                    }
                    
                    LogMessage($"Auto-Start Timer mit Verzögerung gestartet: {delaySeconds} Sekunden");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Starten des Auto-Start Timers mit Verzögerung: {ex.Message}");
            }
        }
        
        private void StopAutoStartTimer()
        {
            try
            {
                if (_autoStartTimer != null)
                {
                    _autoStartTimer.Stop();
                    LogMessage("Auto-Start Timer gestoppt");
                }
                
                if (_countdownTimer != null)
                {
                    _countdownTimer.Stop();
                    ResetCountdownDisplay();
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Stoppen des Auto-Start Timers: {ex.Message}");
            }
        }
        
        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                _countdownSeconds--;
                
                if (_countdownSeconds > 0)
                {
                    UpdateCountdownDisplay(_countdownSeconds);
                }
                else
                {
                    _countdownTimer.Stop();
                    UpdateCountdownDisplay(0);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Countdown Timer: {ex.Message}");
            }
        }
        
        private void UpdateCountdownDisplay(int seconds)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (CountdownTextBlock != null)
                        {
                            if (seconds > 0)
                            {
                                CountdownTextBlock.Text = $"{seconds}";
                                CountdownTextBlock.Foreground = new SolidColorBrush(Colors.LightGreen);
                                CountdownTextBlock.Visibility = Visibility.Visible;
                                
                                if (AutoStartTimerCheckBox != null && AutoStartTimerCheckBox.IsChecked != true)
                                {
                                    AutoStartTimerCheckBox.IsChecked = true;
                                    LogMessage("Checkbox-Status mit Timer synchronisiert");
                                }
                            }
                            else
                            {
                                CountdownTextBlock.Visibility = Visibility.Collapsed;
                            }
                        }
                        LogMessage($"Countdown: {seconds} Sekunden verbleibend");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Fehler beim UI-Update der Countdown-Anzeige: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Aktualisieren der Countdown-Anzeige: {ex.Message}");
            }
        }
        
        private void ResetCountdownDisplay()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (CountdownTextBlock != null)
                        {
                            CountdownTextBlock.Visibility = Visibility.Collapsed;
                        }
                        LogMessage("Countdown zurückgesetzt");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Fehler beim UI-Reset der Countdown-Anzeige: {ex.Message}");
                    }
                });
                _countdownSeconds = _config.AutoStartTimerDelay ?? 5;
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Zurücksetzen der Countdown-Anzeige: {ex.Message}");
            }
        }

        private void UpdateStatusDisplay()
        {
            try
            {

                var systemInfo = new StringBuilder();
                systemInfo.AppendLine($"• .NET: {Environment.Version}");
                systemInfo.AppendLine($"• OS: {Environment.OSVersion.VersionString}");
                systemInfo.AppendLine($"• Speicher: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
                systemInfo.AppendLine($"• Laufzeit: {DateTime.Now - _startTime:mm\\:ss}");
                
                SystemInfoText.Text = systemInfo.ToString();

                ConnectionStatusPanel.Children.Clear();

                var steamStatus = new StatusItem
                {
                    StatusText = $"Steam: {(_isSteamRunning ? "✓ Verbunden" : "✗ Nicht verbunden")}"
                };
                ConnectionStatusPanel.Children.Add(steamStatus);

                var swtorLauncherStatus = new StatusItem
                {
                    StatusText = $"SWTOR Launcher: {(_isSWTORLauncherRunning ? "✓ Läuft" : "✗ Gestoppt")}"
                };
                ConnectionStatusPanel.Children.Add(swtorLauncherStatus);

                var swtorGameStatus = new StatusItem
                {
                    StatusText = $"SWTOR: {(_isSWTORGameRunning ? "✓ Läuft" : "✗ Gestoppt")}"
                };
                ConnectionStatusPanel.Children.Add(swtorGameStatus);

                var passwordStatus = new StatusItem
                {
                    StatusText = $"Passwort: {(!string.IsNullOrEmpty(_config.EncryptedPassword) ? "✓ Gespeichert" : "✗ Nicht gespeichert")}"
                };
                ConnectionStatusPanel.Children.Add(passwordStatus);

                var performanceInfo = new StringBuilder();
                if (_isSteamRunning)
                {
                    var steamProcesses = Process.GetProcessesByName("steam");
                    if (steamProcesses.Length > 0)
                    {
                        var process = steamProcesses[0];
                        performanceInfo.AppendLine($"• Steam PID: {process.Id}");
                        performanceInfo.AppendLine($"• Steam RAM: {process.WorkingSet64 / 1024 / 1024} MB");
                    }
                }
                if (_isSWTORLauncherRunning)
                {
                    var launcherProcesses = Process.GetProcessesByName("launcher");
                    if (launcherProcesses.Length > 0)
                    {
                        var process = launcherProcesses[0];
                        performanceInfo.AppendLine($"• SWTOR Launcher PID: {process.Id}");
                        performanceInfo.AppendLine($"• SWTOR Launcher RAM: {process.WorkingSet64 / 1024 / 1024} MB");
                    }
                }
                if (_isSWTORGameRunning)
                {
                    var swtorProcesses = Process.GetProcessesByName("swtor");
                    if (swtorProcesses.Length > 0)
                    {
                        var process = swtorProcesses[0];
                        performanceInfo.AppendLine($"• SWTOR Game PID: {process.Id}");
                        performanceInfo.AppendLine($"• SWTOR Game RAM: {process.WorkingSet64 / 1024 / 1024} MB");
                    }
                }
                if (performanceInfo.Length == 0)
                {
                    performanceInfo.AppendLine("• Keine aktiven Prozesse");
                }
                
                PerformanceText.Text = performanceInfo.ToString();
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Aktualisieren der Status-Anzeige: {ex.Message}");
            }
        }

        private async Task CheckSteamStatusAsync()
        {
            try
            {

                var steamProcesses = Process.GetProcessesByName("steam");
                if (steamProcesses.Length == 0)
                {
                    steamProcesses = Process.GetProcessesByName("Steam");
                }
                if (steamProcesses.Length == 0)
                {
                    steamProcesses = Process.GetProcessesByName("Steam.exe");
                }
                if (steamProcesses.Length == 0)
                {
                    steamProcesses = Process.GetProcessesByName("steamwebhelper");
                }
                if (steamProcesses.Length == 0)
                {
                    steamProcesses = Process.GetProcessesByName("SteamWebHelper");
                }

                var wasSteamRunning = _isSteamRunning;
                _isSteamRunning = steamProcesses.Length > 0;

                if (_isSteamRunning != wasSteamRunning)
                {
                    LogMessage($"Steam Status geändert: {(_isSteamRunning ? "Läuft" : "Gestoppt")}");
                }

                if (_isSteamRunning)
                {
                    LogMessage($"Steam läuft (PID: {steamProcesses[0].Id}) - {steamProcesses[0].ProcessName}");
                }
                else
                {
                    LogMessage("Steam läuft nicht");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler bei der Steam-Überprüfung: {ex.Message}");
            }
        }

        private async Task CheckSWTORStatusAsync()
        {
            try
            {

                var launcherProcesses = Process.GetProcessesByName("launcher");
                if (launcherProcesses.Length == 0)
                {
                    launcherProcesses = Process.GetProcessesByName("launcher.exe");
                }

                var wasLauncherRunning = _isSWTORLauncherRunning;
                _isSWTORLauncherRunning = launcherProcesses.Length > 0;

                var gameProcesses = Process.GetProcessesByName("swtor");
                if (gameProcesses.Length == 0)
                {
                    gameProcesses = Process.GetProcessesByName("swtor.exe");
                }

                var wasGameRunning = _isSWTORGameRunning;
                _isSWTORGameRunning = gameProcesses.Length > 0;

                if (_isSWTORLauncherRunning != wasLauncherRunning)
                {
                    if (_isSWTORLauncherRunning)
                    {
                        LogMessage("SWTOR Launcher gestartet - Start-Button deaktiviert");
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchButton.IsEnabled = false;
                            LaunchStatusText.Text = "SWTOR Launcher läuft - Start-Button deaktiviert";
                        });
                        
                        await Task.Delay(3000);
                        await AutoFillPasswordAsync();
                    }
                    else if (!_isSWTORGameRunning)
                    {
                        LogMessage("SWTOR Launcher beendet - Start-Button wieder aktiviert");
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchButton.IsEnabled = true;
                            LaunchStatusText.Text = "Bereit zum Starten";
                        });
                    }
                    else
                    {
                        LogMessage("SWTOR Launcher beendet - Start-Button bleibt deaktiviert");
                    }
                }

                if (_isSWTORGameRunning != wasGameRunning)
                {
                    if (_isSWTORGameRunning)
                    {
                        LogMessage("SWTOR Game gestartet - Start-Button bleibt deaktiviert");
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchButton.IsEnabled = false;
                            LaunchStatusText.Text = "SWTOR läuft - Spiel aktiv!";
                        });
                    }
                    else if (!_isSWTORLauncherRunning)
                    {
                        LogMessage("SWTOR Game beendet - Start-Button wieder aktiviert");
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchButton.IsEnabled = true;
                            LaunchStatusText.Text = "Bereit zum Starten";
                        });
                    }
                    else
                    {
                        LogMessage("SWTOR Game beendet - Start-Button bleibt deaktiviert");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler bei der SWTOR Status-Überprüfung: {ex.Message}");
            }
        }
        
        private async Task AutoFillPasswordAsync()
        {
            try
        {
            if (string.IsNullOrEmpty(_config.EncryptedPassword))
            {
                    LogMessage("Kein Passwort gespeichert - Auto-Fill übersprungen");
                return;
            }

                LogMessage("Versuche Passwort automatisch in SWTOR Launcher einzufügen...");

                await Task.Delay(3000);

                var decryptedPassword = DecryptPassword(_config.EncryptedPassword);
                
                if (!string.IsNullOrEmpty(decryptedPassword))
                {
                    LogMessage($"Passwort entschlüsselt - Länge: {decryptedPassword.Length} Zeichen");

                    await SendPasswordSimple(decryptedPassword);
                    
                    LogMessage("Passwort wurde automatisch in den SWTOR Launcher eingefügt");
                    await Dispatcher.InvokeAsync(() =>
                    {
                        LaunchStatusText.Text = "Passwort automatisch eingefügt - SWTOR kann gestartet werden";
                    });
                }
                else
                {
                    LogMessage("FEHLER: Passwort konnte nicht entschlüsselt werden");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim automatischen Passwort-Einfügen: {ex.Message}");
            }
        }
        
        private async Task SendPasswordSimple(string password)
        {
            try
            {
                LogMessage("Starte einfache Strg+V Passwort-Eingabe...");

                await Task.Delay(1000);

                System.Windows.Clipboard.SetText(password);
                LogMessage("Passwort in Zwischenablage kopiert");

                await Task.Delay(200);

                var inputs = new WindowsApi.INPUT[4];

                inputs[0] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = 0x11, // VK_CONTROL
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                inputs[1] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = 0x56, // V
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                inputs[2] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = 0x56, // V
                            wScan = 0,
                            dwFlags = WindowsApi.KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                inputs[3] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = 0x11, // VK_CONTROL
                            wScan = 0,
                            dwFlags = WindowsApi.KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                uint result = WindowsApi.SendInput(4, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(WindowsApi.INPUT)));
                
                if (result == 0)
                {
                    LogMessage($"WARNUNG: Strg+V fehlgeschlagen - Error: {System.Runtime.InteropServices.Marshal.GetLastWin32Error()}");
                }
                else
                {
                    LogMessage("Strg+V erfolgreich gesendet - Passwort eingefügt");
                }

                await Task.Delay(500);
                await SendEnterKey();
                
                        }
                        catch (Exception ex)
                        {
                LogMessage($"Fehler beim einfachen Passwort-Einfügen: {ex.Message}");
            }
                        }

        private async void RefreshSteamButton_Click(object sender, RoutedEventArgs e)
                        {

                            await CheckSteamStatusAsync();
        }

        private void AutoStartTimerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.AutoStartTimerEnabled = true;
                SaveConfig();
                StartAutoStartTimer();
                LogMessage("Auto-Start Timer aktiviert");
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Aktivieren des Auto-Start Timers: {ex.Message}");
            }
        }
        
        private void AutoStartTimerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.AutoStartTimerEnabled = false;
                SaveConfig();
                StopAutoStartTimer();
                LogMessage("Auto-Start Timer deaktiviert");
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Deaktivieren des Auto-Start Timers: {ex.Message}");
            }
        }
        
        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogMessage("Starte SWTOR...");
                LaunchButton.IsEnabled = false;
                LaunchStatusText.Text = "SWTOR wird gestartet...";

                LogMessage("Starte SWTOR über Steam (minimiert)...");

                string steamPath = await FindSteamPathAsync();
                if (string.IsNullOrEmpty(steamPath))
                {
                    throw new InvalidOperationException("Steam konnte nicht automatisch gefunden werden. Stelle sicher, dass Steam installiert ist.");
                }
                
                LogMessage($"Steam gefunden: {steamPath}");

                var startInfo = new ProcessStartInfo
                {
                    FileName = steamPath,
                    Arguments = "-silent -applaunch 1286830",
                    UseShellExecute = true
                };
                
                var process = Process.Start(startInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("SWTOR-Prozess konnte nicht gestartet werden");
                }
                
                LogMessage("SWTOR über Steam gestartet - Warte auf Launcher...");

                await WaitForLauncherReadyAsync();

                await ActivateSWTORLauncherAsync();

                await InsertPasswordSimple();
                
                LaunchStatusText.Text = "SWTOR erfolgreich über Steam gestartet ✓";
                LogMessage("SWTOR erfolgreich über Steam gestartet mit automatischer Passwort-Eingabe");
                }
                catch (Exception ex)
                {
                LogMessage($"Fehler beim Starten von SWTOR: {ex.Message}");
                LaunchStatusText.Text = $"Fehler: {ex.Message}";
                LaunchButton.IsEnabled = true;
            }
        }
        
        private async Task ActivateSWTORLauncherAsync()
        {
            try
            {
                LogMessage("Aktiviere SWTOR Launcher im Vordergrund...");

                await Task.Delay(1000);

                IntPtr launcherWindow = IntPtr.Zero;
                int attempts = 0;
                const int maxAttempts = 10;
                
                while (launcherWindow == IntPtr.Zero && attempts < maxAttempts)
                {
                    launcherWindow = FindSWTORLauncherWindow();
                    if (launcherWindow == IntPtr.Zero)
                    {
                        LogMessage($"Launcher-Fenster nicht gefunden, Versuch {attempts + 1}/{maxAttempts}");
                        await Task.Delay(1000);
                        attempts++;
                    }
                }
                
                if (launcherWindow != IntPtr.Zero)
                {
                    LogMessage($"SWTOR Launcher-Fenster gefunden: {launcherWindow}");

                    WindowsApi.SetForegroundWindow(launcherWindow);
                    WindowsApi.BringWindowToTop(launcherWindow);
                    WindowsApi.ShowWindow(launcherWindow, 9); // SW_RESTORE = 9
                    
                    LogMessage("SWTOR Launcher erfolgreich im Vordergrund aktiviert");

                    await Task.Delay(500);
                }
                else
                {
                    LogMessage("WARNUNG: SWTOR Launcher-Fenster konnte nicht gefunden werden");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Aktivieren des Launchers: {ex.Message}");
            }
        }
        
        private IntPtr FindSWTORLauncherWindow()
        {
            try
            {

                var windowTitles = new[]
                {
                    "Star Wars: The Old Republic",
                    "SWTOR",
                    "launcher",
                    "Launcher"
                };
                
                foreach (var title in windowTitles)
                {
                    var window = WindowsApi.FindWindow(null, title);
                    if (window != IntPtr.Zero)
                    {
                        LogMessage($"SWTOR Launcher-Fenster gefunden mit Titel: {title}");
                        return window;
                    }
                }

                var processes = Process.GetProcessesByName("launcher");
                if (processes.Length > 0)
                {
                    var process = processes[0];
                    LogMessage($"SWTOR Launcher-Prozess gefunden: {process.ProcessName} (PID: {process.Id})");
                    return process.MainWindowHandle;
                }
                
                return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Suchen des Launcher-Fensters: {ex.Message}");
                return IntPtr.Zero;
            }
        }
        
        private async Task InsertPasswordSimple()
        {
            try
            {
                if (string.IsNullOrEmpty(_config.EncryptedPassword))
                {
                    LogMessage("Kein Passwort gespeichert - Auto-Fill übersprungen");
                    return;
                }
                
                LogMessage("Versuche Passwort automatisch einzufügen...");

                var decryptedPassword = DecryptPassword(_config.EncryptedPassword);
                
                if (!string.IsNullOrEmpty(decryptedPassword))
                {
                    LogMessage($"Passwort entschlüsselt - Länge: {decryptedPassword.Length} Zeichen");

                    System.Windows.Clipboard.SetText(decryptedPassword);
                    LogMessage("Passwort in Zwischenablage kopiert");

                    await Task.Delay(1000);

                    WinForms.SendKeys.SendWait("^v");
                    LogMessage("Strg+V gesendet - Passwort eingefügt");

                    await Task.Delay(500);
                    WinForms.SendKeys.SendWait("{ENTER}");
                    LogMessage("ENTER gesendet");

                    await Task.Delay(2000);
                    LogMessage("Warte auf Popup-Fenster...");

                    WinForms.SendKeys.SendWait("{ENTER}");
                    LogMessage("Zweiter ENTER für Popup gesendet");
                    
                    await Dispatcher.InvokeAsync(() =>
                    {
                        LaunchStatusText.Text = "Passwort automatisch eingefügt - Popup behandelt ✓";
                    });
                }
                else
                {
                    LogMessage("FEHLER: Passwort konnte nicht entschlüsselt werden");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim automatischen Passwort-Einfügen: {ex.Message}");
            }
        }

        private async Task<string> FindSteamPathAsync()
        {

            var startMenuPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Steam", "Steam.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Steam", "Steam.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Steam", "Steam.lnk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Steam", "Steam.lnk")
            };

            foreach (var startMenuPath in startMenuPaths)
            {
                if (File.Exists(startMenuPath))
                {
                    LogMessage($"Steam Startmenü-Verknüpfung gefunden: {startMenuPath}");
                    return startMenuPath;
                }
            }

            var possiblePaths = new[]
            {
                @"C:\Program Files (x86)\Steam\Steam.exe",
                @"C:\Program Files\Steam\Steam.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam", "Steam.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "Steam.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Steam", "Steam.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steam", "Steam.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "Steam.exe")
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    LogMessage($"Steam direkt gefunden: {path}");
                    return path;
                }
            }

            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam");
                if (key != null)
                {
                    var steamPath = key.GetValue("SteamPath") as string;
                    if (!string.IsNullOrEmpty(steamPath))
                    {
                        var fullPath = Path.Combine(steamPath, "Steam.exe");
                        if (File.Exists(fullPath))
                        {
                            LogMessage($"Steam in Registry gefunden: {fullPath}");
                            return fullPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Lesen der Registry: {ex.Message}");
            }

            LogMessage("Steam konnte nicht gefunden werden");
            return string.Empty;
        }

        private string GetSWTORPath()
        {
            var possiblePaths = new[]
            {
                @"C:\Program Files (x86)\Electronic Arts\BioWare\Star Wars - The Old Republic\swtor\retailclient\swtor.exe",
                @"C:\Program Files\Electronic Arts\BioWare\Star Wars - The Old Republic\swtor\retailclient\swtor.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Electronic Arts", "BioWare", "Star Wars - The Old Republic", "swtor", "retailclient", "swtor.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Electronic Arts", "BioWare", "Star Wars - The Old Republic", "swtor", "retailclient", "swtor.exe")
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    LogMessage($"SWTOR gefunden: {path}");
                    return path;
                }
            }

            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BioWare\Star Wars - The Old Republic");
                if (key != null)
                {
                    var swtorPath = key.GetValue("InstallLocation") as string;
                    if (!string.IsNullOrEmpty(swtorPath))
                    {
                        var fullPath = Path.Combine(swtorPath, "swtor", "retailclient", "swtor.exe");
                        if (File.Exists(fullPath))
                        {
                            LogMessage($"SWTOR in Registry gefunden: {fullPath}");
                            return fullPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Lesen der SWTOR Registry: {ex.Message}");
            }

            LogMessage("SWTOR konnte nicht gefunden werden");
            return string.Empty;
        }

        private async Task SendEnterKey()
        {
            try
            {
                LogMessage("Sende ENTER-Taste...");
                
                var inputs = new WindowsApi.INPUT[2];

                inputs[0] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = WindowsApi.VK_ENTER,
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };

                inputs[1] = new WindowsApi.INPUT
                {
                    type = WindowsApi.INPUT_KEYBOARD,
                    inputUnion = new WindowsApi.INPUTUNION
                    {
                        ki = new WindowsApi.KEYBDINPUT
                        {
                            wVk = WindowsApi.VK_ENTER,
                            wScan = 0,
                            dwFlags = WindowsApi.KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero
                        }
                    }
                };
                
                uint result = WindowsApi.SendInput(2, inputs, System.Runtime.InteropServices.Marshal.SizeOf(typeof(WindowsApi.INPUT)));
                
                if (result > 0)
                {
                    LogMessage("ENTER-Taste erfolgreich gesendet");
                }
                else
                {
                    LogMessage($"WARNUNG: ENTER-Taste fehlgeschlagen - Error: {System.Runtime.InteropServices.Marshal.GetLastWin32Error()}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Senden der ENTER-Taste: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}\n";
            
            Dispatcher.Invoke(() =>
            {

                UpdateStatusDisplay();
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _steamCheckTimer?.Stop();
            _swtorCheckTimer?.Stop();
            _statusUpdateTimer?.Stop();
            _autoStartTimer?.Stop();
            _countdownTimer?.Stop();

            _statusBoxWindow?.Close();
            
            base.OnClosed(e);
        }

        /// <summary>
        /// Erstellt und zeigt die Status-Box als separates schwebendes Fenster oben rechts
        /// </summary>
        private void CreateStatusBox()
        {
            try
            {
                if (_statusBoxWindow != null)
                {
                    _statusBoxWindow.Close();
                    _statusBoxWindow = null;
                }

                _statusBoxWindow = new Window
                {
                    Title = "SWTOR Status",
                    Width = 350,
                    Height = 320,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true,
                    Background = new SolidColorBrush(Color.FromArgb(200, 33, 33, 33)),
                    Topmost = true,
                    ResizeMode = ResizeMode.NoResize,
                    ShowInTaskbar = false
                };

                var screenWidth = System.Windows.SystemParameters.WorkArea.Width;
                _statusBoxWindow.Left = screenWidth - _statusBoxWindow.Width - 20;
                _statusBoxWindow.Top = 20;

                var statusPanel = new StackPanel
                {
                    Margin = new Thickness(10),
                    Orientation = Orientation.Vertical
                };

                var titleText = new TextBlock
                {
                    Text = "SWTOR STATUS",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                statusPanel.Children.Add(titleText);

                var steamStatus = new TextBlock
                {
                    Text = "Steam: Gestoppt",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                statusPanel.Children.Add(steamStatus);

                var launcherStatus = new TextBlock
                {
                    Text = "SWTOR Launcher: Gestoppt",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                statusPanel.Children.Add(launcherStatus);

                var gameStatus = new TextBlock
                {
                    Text = "SWTOR: Gestoppt",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                statusPanel.Children.Add(gameStatus);

                var separator = new Separator
                {
                    Margin = new Thickness(0, 10, 0, 10),
                    Background = new SolidColorBrush(Colors.Gray)
                };
                statusPanel.Children.Add(separator);

                var programStatusTitle = new TextBlock
                {
                    Text = "PROGRAMM STATUS:",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.LightBlue),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                statusPanel.Children.Add(programStatusTitle);

                var programStatus = new TextBlock
                {
                    Text = "Bereit zum Starten",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 3, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                };
                statusPanel.Children.Add(programStatus);

                var launchButtonStatus = new TextBlock
                {
                    Text = "Start-Button: Aktiviert",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                statusPanel.Children.Add(launchButtonStatus);

                var passwordStatus = new TextBlock
                {
                    Text = "Passwort: Nicht gespeichert",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                statusPanel.Children.Add(passwordStatus);

                var configStatus = new TextBlock
                {
                    Text = "Konfiguration: Geladen",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 3, 0, 0)
                };
                statusPanel.Children.Add(configStatus);

                var closeButton = new Button
                {
                    Content = "×",
                    Width = 24,
                    Height = 24,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, -12, 5, 0),
                    Background = new SolidColorBrush(Colors.Red),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                closeButton.Click += (s, e) => _statusBoxWindow?.Close();

                var buttonPanel = new Grid();
                buttonPanel.Children.Add(closeButton);

                var mainPanel = new StackPanel();
                mainPanel.Children.Add(buttonPanel);
                mainPanel.Children.Add(statusPanel);

                _statusBoxWindow.Content = mainPanel;

                if (_config.ShowStatusBox == true)
                {
                    _statusBoxWindow.Show();
                    LogMessage("Status-Box wurde erstellt und angezeigt");
                }
                else
                {
                    LogMessage("Status-Box wurde erstellt aber nicht angezeigt (ShowStatusBox = false)");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Erstellen der Status-Box: {ex.Message}");
            }
        }

        /// <summary>
        /// Aktualisiert die Status-Box mit aktuellen Werten
        /// </summary>
        private void UpdateStatusBox()
        {
            try
            {
                if (_statusBoxWindow?.IsVisible == true)
                {
                    var mainPanel = _statusBoxWindow.Content as StackPanel;
                    if (mainPanel?.Children.Count > 1)
                    {
                        var statusPanel = mainPanel.Children[1] as StackPanel;
                        if (statusPanel?.Children.Count > 8) // Jetzt haben wir mehr Children
                        {

                            if (statusPanel.Children[1] is TextBlock steamStatus)
                            {
                                steamStatus.Text = $"Steam: {(_isSteamRunning ? "✓ Läuft" : "✗ Gestoppt")}";
                                steamStatus.Foreground = new SolidColorBrush(_isSteamRunning ? Colors.LightGreen : Colors.Red);
                            }

                            if (statusPanel.Children[2] is TextBlock launcherStatus)
                            {
                                launcherStatus.Text = $"SWTOR Launcher: {(_isSWTORLauncherRunning ? "✓ Läuft" : "✗ Gestoppt")}";
                                launcherStatus.Foreground = new SolidColorBrush(_isSWTORLauncherRunning ? Colors.LightGreen : Colors.Red);
                            }

                            if (statusPanel.Children[3] is TextBlock gameStatus)
                            {
                                gameStatus.Text = $"SWTOR: {(_isSWTORGameRunning ? "✓ Läuft" : "✗ Gestoppt")}";
                                gameStatus.Foreground = new SolidColorBrush(_isSWTORGameRunning ? Colors.LightGreen : Colors.Red);

                                if (_isSWTORGameRunning && _statusBoxWindow?.IsVisible == true)
                                {
                                    _statusBoxWindow.Close();
                                    LogMessage("Status-Box automatisch geschlossen (SWTOR läuft)");
                                }
                            }

                            if (statusPanel.Children[7] is TextBlock programStatus)
                            {
                                var statusText = GetProgramStatusText();
                                programStatus.Text = statusText;
                                programStatus.Foreground = new SolidColorBrush(GetProgramStatusColor(statusText));
                            }

                            if (statusPanel.Children[8] is TextBlock launchButtonStatus)
                            {
                                var buttonText = LaunchButton.IsEnabled ? "Aktiviert" : "Deaktiviert";
                                launchButtonStatus.Text = $"Start-Button: {buttonText}";
                                launchButtonStatus.Foreground = new SolidColorBrush(LaunchButton.IsEnabled ? Colors.LightGreen : Colors.Orange);
                            }

                            if (statusPanel.Children[9] is TextBlock passwordStatus)
                            {
                                var hasPassword = !string.IsNullOrEmpty(_config.EncryptedPassword);
                                var passwordText = hasPassword ? "Gespeichert" : "Nicht gespeichert";
                                passwordStatus.Text = $"Passwort: {passwordText}";
                                passwordStatus.Foreground = new SolidColorBrush(hasPassword ? Colors.LightGreen : Colors.Red);
                            }

                            if (statusPanel.Children[10] is TextBlock configStatus)
                            {
                                var configText = _config != null ? "Geladen" : "Fehler";
                                configStatus.Text = $"Konfiguration: {configText}";
                                configStatus.Foreground = new SolidColorBrush(_config != null ? Colors.LightGreen : Colors.Red);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Aktualisieren der Status-Box: {ex.Message}");
            }
        }

        /// <summary>
        /// Ermittelt den aktuellen Programm-Status als Text
        /// </summary>
        private string GetProgramStatusText()
        {
            if (_isSWTORGameRunning)
                return "SWTOR läuft - Spiel aktiv";
            else if (_isSWTORLauncherRunning)
                return "Launcher läuft - Warte auf Spiel";
            else if (_isSteamRunning)
                return "Steam läuft - Bereit für SWTOR";
            else
                return "Bereit zum Starten";
        }

        /// <summary>
        /// Ermittelt die Farbe für den Programm-Status
        /// </summary>
        private Color GetProgramStatusColor(string statusText)
        {
            if (statusText.Contains("Spiel aktiv"))
                return Colors.LightGreen;
            else if (statusText.Contains("Launcher läuft"))
                return Colors.Orange;
            else if (statusText.Contains("Steam läuft"))
                return Colors.LightBlue;
            else
                return Colors.White;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowAndActivateWindow()
        {
            try
            {

                this.Show();
                this.Activate();
                this.Focus();
                this.WindowState = WindowState.Normal;
                this.Visibility = Visibility.Visible;

                this.Topmost = true;
                this.Topmost = false;

                if (this.Left <= 0 || this.Top <= 0)
                {
                    this.Left = (System.Windows.SystemParameters.WorkArea.Width - this.Width) / 2;
                    this.Top = (System.Windows.SystemParameters.WorkArea.Height - this.Height) / 2;
                }
                
                LogMessage("Fenster wurde explizit angezeigt und aktiviert");
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Anzeigen des Fensters: {ex.Message}");
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                ShowAndActivateWindow();
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ShowAndActivateWindow();

            RestoreTimerState();
        }
        
        private void RestoreTimerState()
        {
            try
            {

                if (AutoStartTimerCheckBox != null)
                {

                    AutoStartTimerCheckBox.IsChecked = _config.AutoStartTimerEnabled ?? false;

                    AutoStartTimerCheckBox.UpdateLayout();
                    
                    LogMessage($"Checkbox-Status wiederhergestellt: {(_config.AutoStartTimerEnabled ?? false)}");

                    if (_config.AutoStartTimerEnabled == true)
                    {
                        var delay = _config.AutoStartTimerDelay ?? 5;
                        StartAutoStartTimerWithDelay(delay);
                        LogMessage($"Timer-Status wiederhergestellt - Verzögerung: {delay} Sekunden");
                    }
                }
                else
                {
                    LogMessage("WARNUNG: AutoStartTimerCheckBox ist noch nicht verfügbar - versuche verzögerte Wiederherstellung");

                    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                    {
                        try
                        {
                            if (AutoStartTimerCheckBox != null)
                            {

                                AutoStartTimerCheckBox.IsChecked = _config.AutoStartTimerEnabled ?? false;

                                AutoStartTimerCheckBox.UpdateLayout();
                                
                                LogMessage($"Checkbox-Status verzögert wiederhergestellt: {(_config.AutoStartTimerEnabled ?? false)}");

                                if (_config.AutoStartTimerEnabled == true)
                                {
                                    var delay = _config.AutoStartTimerDelay ?? 5;
                                    StartAutoStartTimerWithDelay(delay);
                                    LogMessage($"Timer-Status verzögert wiederhergestellt - Verzögerung: {delay} Sekunden");
                                }
                            }
                            else
                            {
                                LogMessage("FEHLER: AutoStartTimerCheckBox ist auch nach Verzögerung nicht verfügbar");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"Fehler bei verzögerter Timer-Status-Wiederherstellung: {ex.Message}");
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler beim Wiederherstellen des Timer-Status: {ex.Message}");
            }
        }

        /// <summary>
        /// Wartet bis der SWTOR Launcher vollständig geladen ist, basierend auf RAM-Verbrauch
        /// </summary>
        private async Task WaitForLauncherReadyAsync()
        {
            try
            {
                LogMessage("Starte RAM-basierte Launcher-Überwachung...");
                
                const int maxWaitTimeSeconds = 120; // Maximale Wartezeit: 2 Minuten
                var ramThresholdMB = _config.LauncherRamThresholdMB ?? 70; // Konfigurierbarer Schwellenwert
                var ramThresholdBytes = ramThresholdMB * 1024 * 1024; // Konvertiere zu Bytes
                
                var startTime = DateTime.Now;
                var attempts = 0;
                const int maxAttempts = 60; // Maximal 60 Versuche (alle 2 Sekunden)
                
                LogMessage($"RAM-Schwellenwert: {ramThresholdMB}MB - Maximale Wartezeit: {maxWaitTimeSeconds}s");
                
                while (attempts < maxAttempts)
                {
                    attempts++;

                    if ((DateTime.Now - startTime).TotalSeconds > maxWaitTimeSeconds)
                    {
                        LogMessage($"WARNUNG: Maximale Wartezeit ({maxWaitTimeSeconds}s) überschritten - fahre trotzdem fort");
                        break;
                    }

                    var launcherProcesses = Process.GetProcessesByName("launcher");
                    if (launcherProcesses.Length == 0)
                    {
                        launcherProcesses = Process.GetProcessesByName("launcher.exe");
                    }
                    
                    if (launcherProcesses.Length > 0)
                    {
                        var launcherProcess = launcherProcesses[0];
                        var ramUsageBytes = launcherProcess.WorkingSet64;
                        var ramUsageMB = ramUsageBytes / 1024 / 1024;
                        
                        LogMessage($"Launcher gefunden - RAM: {ramUsageMB}MB (Schwellenwert: {ramThresholdMB}MB) - Versuch {attempts}/{maxAttempts}");

                        if (ramUsageBytes > ramThresholdBytes)
                        {
                            LogMessage($"Launcher vollständig geladen! RAM: {ramUsageMB}MB - Wartezeit: {(DateTime.Now - startTime).TotalSeconds:F1}s");
                            return;
                        }

                        var progressPercent = Math.Min((int)((ramUsageBytes * 100) / ramThresholdBytes), 99);
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchStatusText.Text = $"Launcher lädt... {progressPercent}% ({ramUsageMB}MB / {ramThresholdMB}MB)";
                        });
                    }
                    else
                    {
                        LogMessage($"Launcher-Prozess noch nicht gefunden - Versuch {attempts}/{maxAttempts}");
                        await Dispatcher.InvokeAsync(() =>
                        {
                            LaunchStatusText.Text = $"Suche Launcher-Prozess... Versuch {attempts}/{maxAttempts}";
                        });
                    }

                    await Task.Delay(2000);
                }

                LogMessage($"WARNUNG: Launcher war nach {maxAttempts} Versuchen nicht vollständig geladen - fahre trotzdem fort");
            }
            catch (Exception ex)
            {
                LogMessage($"Fehler bei der Launcher-Überwachung: {ex.Message} - fahre trotzdem fort");
            }
        }
    }

    public class AppConfig
    {
        public string EncryptedPassword { get; set; } = string.Empty;

        public bool? AutoStartSteam { get; set; } = true;
        public bool? AutoStartSWTOR { get; set; } = true;
        public bool? AutoFillPassword { get; set; } = true;

        public int? SteamStartDelay { get; set; } = 30;
        public int? SWTORLauncherDelay { get; set; } = 45;
        public int? PasswordInputDelay { get; set; } = 5;

        public string? SteamStartMode { get; set; } = "minimized";
        public string? SteamPath { get; set; }

        public string? SWTORPath { get; set; }

        public bool? UseWaitForInputIdle { get; set; } = true;
        public bool? UseCPUMonitoring { get; set; } = true;
        public bool? UseMemoryMonitoring { get; set; } = true;
        public bool? UseWindowStability { get; set; } = true;

        public int? LauncherRamThresholdMB { get; set; } = 70; // Schwellenwert in MB für "vollständig geladen"

        public bool? DebugMode { get; set; } = false;
        public bool? ShowStatusBox { get; set; } = true;
        public bool? AutoSaveLogs { get; set; } = false;
        public int? MonitoringInterval { get; set; } = 500;

        public bool? AutoStartTimerEnabled { get; set; } = false;
        public int? AutoStartTimerDelay { get; set; } = 5;
    }
}
