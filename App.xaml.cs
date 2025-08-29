using System.Windows;

namespace SWTORStarter
{
    /// <summary>
    /// Hauptanwendungsklasse für SWTOR Starter
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Wird beim Start der Anwendung aufgerufen
        /// </summary>
        /// <param name="e">Startup-Event-Argumente</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            this.DispatcherUnhandledException += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"Unbehandelter Fehler: {args.Exception}");
                args.Handled = true;
            };
        }
        
        /// <summary>
        /// Behandelt unbehandelte Exceptions in der WPF-Anwendung
        /// </summary>
        /// <param name="sender">Event-Sender</param>
        /// <param name="e">Exception-Event-Argumente</param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"App Fehler: {e.Exception}");
            
            e.Handled = true;
            
            try
            {
                if (this.MainWindow == null)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.MainWindow = mainWindow;
                }
            }
            catch
            {
                try
                {
                    var simpleWindow = new System.Windows.Window
                    {
                        Title = "SWTOR Starter - Fallback",
                        Width = 800,
                        Height = 600,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    
                    var textBlock = new System.Windows.Controls.TextBlock
                    {
                        Text = "SWTOR Starter läuft im Fallback-Modus.\nMaterialDesign konnte nicht geladen werden.",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16
                    };
                    
                    simpleWindow.Content = textBlock;
                    simpleWindow.Show();
                    this.MainWindow = simpleWindow;
                }
                catch
                {
                    System.Windows.MessageBox.Show(
                        "SWTOR Starter konnte nicht gestartet werden.\nFehler: " + e.Exception.Message,
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        
        /// <summary>
        /// Wird beim Beenden der Anwendung aufgerufen
        /// </summary>
        /// <param name="e">Exit-Event-Argumente</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
