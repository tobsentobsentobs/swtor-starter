using System.Windows;
using System.Windows.Controls;

namespace SWTORStarter.Controls
{
    /// <summary>
    /// Benutzerdefiniertes Steuerelement für Status-Anzeigen
    /// </summary>
    public partial class StatusItem : UserControl
    {
        /// <summary>
        /// Dependency Property für den Status-Text
        /// </summary>
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(StatusItem), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Ruft den Status-Text ab oder legt ihn fest
        /// </summary>
        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der StatusItem-Klasse
        /// </summary>
        public StatusItem()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}