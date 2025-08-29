using System;
using System.Runtime.InteropServices;

namespace SWTORStarter
{
    /// <summary>
    /// Statische Klasse für Windows API Aufrufe
    /// </summary>
    public static class WindowsApi
    {
        /// <summary>
        /// Virtual Key Code für ENTER-Taste
        /// </summary>
        public const int VK_ENTER = 0x0D;
        
        /// <summary>
        /// ShowWindow Konstante für Fenster wiederherstellen
        /// </summary>
        public const int SW_RESTORE = 9;
        
        /// <summary>
        /// SendInput Flag für Tastendruck
        /// </summary>
        public const int KEYEVENTF_KEYDOWN = 0x0000;
        
        /// <summary>
        /// SendInput Flag für Tastenfreigabe
        /// </summary>
        public const int KEYEVENTF_KEYUP = 0x0002;
        
        /// <summary>
        /// INPUT Typ für Tastatureingabe
        /// </summary>
        public const int INPUT_KEYBOARD = 1;

        /// <summary>
        /// Findet ein Fenster anhand der Klasse und des Fensternamens
        /// </summary>
        /// <param name="lpClassName">Fensterklassenname</param>
        /// <param name="lpWindowName">Fenstername</param>
        /// <returns>Handle zum gefundenen Fenster oder IntPtr.Zero</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Setzt ein Fenster in den Vordergrund
        /// </summary>
        /// <param name="hWnd">Handle zum Fenster</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Zeigt ein Fenster mit dem angegebenen Befehl an
        /// </summary>
        /// <param name="hWnd">Handle zum Fenster</param>
        /// <param name="nCmdShow">Anzeigebefehl</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Bringt ein Fenster in den Vordergrund
        /// </summary>
        /// <param name="hWnd">Handle zum Fenster</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        
        /// <summary>
        /// Sendet Tastatureingaben an das aktive Fenster
        /// </summary>
        /// <param name="nInputs">Anzahl der Inputs</param>
        /// <param name="pInputs">Array von INPUT-Strukturen</param>
        /// <param name="cbSize">Größe der INPUT-Struktur</param>
        /// <returns>Anzahl der erfolgreich verarbeiteten Inputs</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        
        /// <summary>
        /// INPUT-Struktur für Tastatureingaben
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            /// <summary>
            /// Typ der Eingabe (INPUT_KEYBOARD für Tastatur)
            /// </summary>
            public uint type;
            
            /// <summary>
            /// Union für verschiedene Eingabetypen
            /// </summary>
            public INPUTUNION inputUnion;
        }
        
        /// <summary>
        /// Union-Struktur für verschiedene Eingabetypen
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            /// <summary>
            /// Tastatureingabe
            /// </summary>
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }
        
        /// <summary>
        /// Tastatureingabe-Struktur
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            /// <summary>
            /// Virtual Key Code
            /// </summary>
            public ushort wVk;
            
            /// <summary>
            /// Hardware Scan Code
            /// </summary>
            public ushort wScan;
            
            /// <summary>
            /// Flags für die Eingabe
            /// </summary>
            public uint dwFlags;
            
            /// <summary>
            /// Zeitstempel der Eingabe
            /// </summary>
            public uint time;
            
            /// <summary>
            /// Zusätzliche Informationen
            /// </summary>
            public IntPtr dwExtraInfo;
        }
    }
}
