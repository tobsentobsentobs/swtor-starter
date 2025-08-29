using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SWTORStarter.Converters
{
    /// <summary>
    /// Konverter für Status-Farben basierend auf Status-Text
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert Status-Text in entsprechende Farben
        /// </summary>
        /// <param name="value">Status-Text als String</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter (nicht verwendet)</param>
        /// <param name="culture">Kultur (nicht verwendet)</param>
        /// <returns>SolidColorBrush für den Status</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string statusText)
            {
                if (statusText.Contains("✓") || 
                    statusText.Contains("Verbunden") || 
                    statusText.Contains("Läuft") || 
                    statusText.Contains("Gespeichert"))
                {
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));
                }
                else if (statusText.Contains("✗") || 
                         statusText.Contains("Nicht verbunden") || 
                         statusText.Contains("Gestoppt") || 
                         statusText.Contains("Nicht gespeichert"))
                {
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));
                }
                else if (statusText.Contains("Warnung") || 
                         statusText.Contains("Überprüfe") || 
                         statusText.Contains("Lade"))
                {
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0));
                }
            }
            
            return new SolidColorBrush(Color.FromRgb(176, 176, 176));
        }

        /// <summary>
        /// Rückkonvertierung wird nicht unterstützt
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Kultur</param>
        /// <returns>NotImplementedException</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Konverter für Status-Icons basierend auf Status-Text
    /// </summary>
    public class StatusIconConverter : IValueConverter
    {
        /// <summary>
        /// Konvertiert Status-Text in entsprechende Icon-Namen
        /// </summary>
        /// <param name="value">Status-Text als String</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter (nicht verwendet)</param>
        /// <param name="culture">Kultur (nicht verwendet)</param>
        /// <returns>Icon-Name als String</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string statusText)
            {
                if (statusText.Contains("✓") || 
                    statusText.Contains("Verbunden") || 
                    statusText.Contains("Läuft") || 
                    statusText.Contains("Gespeichert"))
                {
                    return "CheckCircle";
                }
                else if (statusText.Contains("✗") || 
                         statusText.Contains("Nicht verbunden") || 
                         statusText.Contains("Gestoppt") || 
                         statusText.Contains("Nicht gespeichert"))
                {
                    return "CloseCircle";
                }
                else if (statusText.Contains("Warnung") || 
                         statusText.Contains("Überprüfe"))
                {
                    return "AlertCircle";
                }
                else if (statusText.Contains("Lade"))
                {
                    return "Loading";
                }
            }
            return "Information";
        }

        /// <summary>
        /// Rückkonvertierung wird nicht unterstützt
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Kultur</param>
        /// <returns>NotImplementedException</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Konverter für Status-Text ohne Icons
    /// </summary>
    public class StatusTextConverter : IValueConverter
    {
        /// <summary>
        /// Entfernt Icons aus Status-Text
        /// </summary>
        /// <param name="value">Status-Text als String</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter (nicht verwendet)</param>
        /// <param name="culture">Kultur (nicht verwendet)</param>
        /// <returns>Bereinigter Status-Text</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string statusText)
            {
                return statusText.Replace("✓ ", "").Replace("✗ ", "").Replace("• ", "");
            }
            return value;
        }

        /// <summary>
        /// Rückkonvertierung wird nicht unterstützt
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="targetType">Zieltyp</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Kultur</param>
        /// <returns>NotImplementedException</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}