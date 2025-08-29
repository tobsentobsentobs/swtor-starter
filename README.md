# 🚀 SWTOR Starter

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Release](https://img.shields.io/badge/Release-v1.0.0-orange.svg)](https://github.com/tobsentobsentobs/swtor-starter/releases/latest)

> **Ein intelligenter WPF-Desktop-Launcher für Star Wars: The Old Republic (SWTOR) mit automatischer Passwort-Eingabe und Steam-Integration**

## 🚀 Wie alles begann...

**Die Geschichte hinter SWTOR Starter ist simpel: Faulheit trifft auf KI-Hilfe! 😄**

> *"Ich hatte einfach keine Lust, jedes Mal mein SWTOR-Passwort einzutippen. 
> Also dachte ich mir: 'Warum nicht eine App bauen, die das für mich macht?' 
> Mit der Hilfe von Cursor (einem AI-powered Code Editor) haben wir dann diese Anwendung entwickelt."*

**Entstanden aus:**
- 🥱 **Faulheit** - Keine Lust auf wiederholte Passwort-Eingabe
- 🛠️ **AI-powered Development** - Entwickelt mit Hilfe von Cursor
- 🎮 **Gaming-Passion** - Für die SWTOR-Community

## ✨ Features

- 🔐 **Automatische Passwort-Eingabe** - Sichere AES-256 Verschlüsselung für SWTOR-Passwort
- 🚀 **Steam-Integration** - Startet SWTOR automatisch über Steam
- 🖥️ **Windows API Integration** - Native Windows-Funktionalität für Prozess-Management
- 🎨 **Moderne UI** - Material Design mit professionellem Look & Feel
- 🔒 **Sicherheit** - Lokale Verschlüsselung ohne Datenübertragung
- ⚡ **Performance** - Optimiert für schnellen Start und geringen Ressourcenverbrauch
- ⏰ **Auto-Start Timer** - Konfigurierbare Verzögerung für automatischen Start
- 📊 **Status-Überwachung** - Live-Status für Steam, SWTOR Launcher und Spiel
- 🧠 **Intelligente Launcher-Überwachung** - RAM-basierte Erkennung des Ladezustands

## 🚀 Quick Start

### 📥 Download

**SWTOR Starter v1.0.0 - Standalone EXE**

| Version | Größe | Beschreibung | Anforderungen |
|---------|-------|--------------|---------------|
| **Standalone EXE** | ~10.5 MB | Einzelne Datei, alle DLLs integriert | .NET 8.0 Runtime |

### 🔧 Installation

1. **Lade `SWTORStarter.exe` herunter** (10.5 MB)
2. **Installiere .NET 8.0 Runtime** (falls nicht vorhanden)
   - **Direkter Download:** [.NET 8.0 Runtime für Windows x64](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
   - **Wähle:** "Runtime" (nicht SDK)
   - **Version:** Windows x64 Installer
3. **Führe die EXE aus**

### 🎯 **WICHTIG: Korrekte Benutzung**

**Voraussetzungen:**
1. **SWTOR Launcher muss bereits einmal gestartet worden sein** - damit der Benutzername bereits eingetragen ist
2. **Steam muss installiert sein** - wird automatisch erkannt

**Schritt-für-Schritt Anleitung:**
1. **Passwort in SWTOR Starter speichern** - Klicke auf "Passwort speichern" und gib dein SWTOR-Passwort ein
2. **Start-Button drücken** - Das Programm startet Steam und SWTOR automatisch
3. **Finger vom Computer lassen!** - Das Programm macht alles automatisch:
   - Startet Steam
   - Startet SWTOR über Steam
   - Wartet bis der Launcher vollständig geladen ist
   - Füllt das Passwort automatisch ein
   - Behandelt alle Popup-Fenster

**⚠️ WICHTIG:** Nicht eingreifen während der automatischen Ausführung!

## 🛠️ Technische Details

### **Architektur**
- **Framework:** .NET 8.0 WPF
- **Pattern:** MVVM (Model-View-ViewModel)
- **UI:** Material Design mit XAML
- **Verschlüsselung:** AES-256 für Passwort-Speicherung

### **Komponenten**
- **MainWindow:** Hauptanwendung mit Status-Überwachung und Passwort-Management
- **SettingsWindow:** Konfigurations-Interface (über "Einstellungen"-Button)
- **WindowsApi:** Native Windows-API-Integration für Tastatureingaben
- **StatusItem:** WPF-Controls für Status-Anzeige
- **Converters:** WPF-Data-Binding-Konverter

### **Automatische Pfad-Erkennung**
- **Steam:** Wird automatisch gesucht in Startmenü, Standard-Pfaden und Windows Registry
- **SWTOR:** Wird automatisch gesucht in Standard-Installationspfaden und Windows Registry
- **Keine manuelle Konfiguration erforderlich!**

### **Sicherheitsfeatures**
- **AES-256 Verschlüsselung** für das gespeicherte SWTOR-Passwort
- **Lokale Konfiguration** - Keine Daten werden übertragen
- **Windows API Integration** - Sichere Prozess-Verwaltung und Tastatureingaben
- **Kein Master-Passwort erforderlich** - Einfache Passwort-Speicherung

## 📋 Systemanforderungen

### **Minimum**
- **Betriebssystem:** Windows 10 (Version 1809) oder höher
- **Architektur:** x64 (64-bit)
- **RAM:** 4 GB
- **Speicher:** 200 MB freier Speicherplatz

### **Empfohlen**
- **Betriebssystem:** Windows 11
- **Architektur:** x64 (64-bit)
- **RAM:** 8 GB oder höher
- **Speicher:** 500 MB freier Speicherplatz

### **Abhängigkeiten**
- **Standalone EXE:** .NET 8.0 Runtime (kostenlos von Microsoft)
- **Steam:** Muss installiert sein (wird automatisch erkannt)
- **SWTOR:** Muss bereits einmal gestartet worden sein

## 🔧 Konfiguration

### **Konfigurationsdatei**
Die Anwendung erstellt automatisch eine `config.json` im Anwendungsverzeichnis:

```json
{
  "EncryptedPassword": "verschlüsseltes_swtor_passwort",
  "AutoStartSteam": true,
  "AutoStartSWTOR": true,
  "AutoFillPassword": true,
  "SteamStartDelay": 30,
  "SWTORLauncherDelay": 45,
  "PasswordInputDelay": 5,
  "SteamStartMode": "minimized",
  "LauncherRamThresholdMB": 70,
  "AutoStartTimerEnabled": false,
  "AutoStartTimerDelay": 5
}
```

### **Einstellungen (über "Einstellungen"-Button)**
- **Passwort:** Dein SWTOR-Passwort (wird verschlüsselt gespeichert)
- **Auto-Start Timer:** Automatischer Start mit konfigurierbarer Verzögerung
- **Steam-Start-Modus:** Normal, minimiert oder silent
- **Verzögerungen:** Anpassbare Wartezeiten für Start-Sequenz
- **RAM-Schwellenwert:** Wann der Launcher als "vollständig geladen" gilt
- **Status-Box:** Schwebendes Status-Fenster anzeigen/verstecken

## 🚀 Entwicklung

### **Voraussetzungen**
- **Visual Studio 2022** oder **Visual Studio Code**
- **.NET 8.0 SDK**
- **Windows 10/11** (für WPF-Entwicklung)

### **Projekt klonen**
```bash
git clone https://github.com/tobsentobsentobs/swtor-starter.git
cd swtor-starter
```

### **Abhängigkeiten wiederherstellen**
```bash
dotnet restore
```

### **Projekt starten**
```bash
dotnet run --project SWTORStarter.csproj
```

### **Release-Build erstellen**
```bash
# Standalone EXE (Framework-Dependent)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish
```

## 📚 Dokumentation

- **[CHANGELOG.md](CHANGELOG.md)** - Vollständige Versionshistorie
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Entwickler-Richtlinien
- **[SECURITY.md](SECURITY.md)** - Sicherheitsrichtlinien
- **[LICENSE](LICENSE)** - MIT-Lizenz

## 🐛 Bekannte Probleme

### **Aktuelle Version (v1.0.0)**
- **Keine bekannten kritischen Probleme**
- Alle Features funktionieren wie erwartet
- Stabile Performance auf Windows 10/11

### **Häufige Fragen**
- **Frage:** Funktioniert die Anwendung ohne .NET 8.0 Runtime?
  - **Antwort:** Nein, die Standalone EXE benötigt .NET 8.0 Runtime. Dies ist kostenlos von Microsoft verfügbar.

- **Frage:** Wo werden meine Passwörter gespeichert?
  - **Antwort:** Das SWTOR-Passwort wird lokal in `config.json` gespeichert und mit AES-256 verschlüsselt.

- **Frage:** Muss ich Steam und SWTOR manuell konfigurieren?
  - **Antwort:** Nein, beide werden automatisch erkannt. Nur der SWTOR Launcher muss bereits einmal gestartet worden sein.

- **Frage:** Wo kann ich .NET 8.0 Runtime herunterladen?
  - **Antwort:** Direkt von Microsoft: [.NET 8.0 Runtime Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## 🤝 Beitragen

Wir freuen uns über Beiträge! Bitte lies zuerst unsere [Beitragsrichtlinien](CONTRIBUTING.md).

### **Wie du helfen kannst**
- 🐛 **Bug-Reports** - Erstelle ein Issue für gefundene Probleme
- 💡 **Feature-Requests** - Schlage neue Features vor
- 📝 **Dokumentation** - Verbessere die Dokumentation
- 🔧 **Code** - Reiche Pull Requests ein

### **Issue-Templates**
- [🐛 Bug Report](.github/ISSUE_TEMPLATE/bug_report.md)
- [💡 Feature Request](.github/ISSUE_TEMPLATE/feature_request.md)

## 📄 Lizenz

Dieses Projekt steht unter der [MIT-Lizenz](LICENSE). Siehe die [LICENSE](LICENSE)-Datei für Details.

## 🙏 Danksagung

- **SWTOR-Community** - Für Feedback und Unterstützung
- **Material Design** - Für das wunderschöne UI-Design
- **.NET Community** - Für das großartige Framework
- **Cursor** - Für den AI-powered Code Editor, der diese Entwicklung ermöglicht hat
- **Alle Entwickler** - Die zu diesem Projekt beigetragen haben

## 🔗 Links

- **Repository:** https://github.com/tobsentobsentobs/swtor-starter
- **Releases:** https://github.com/tobsentobsentobs/swtor-starter/releases
- **Issues:** https://github.com/tobsentobsentobs/swtor-starter/issues
- **Discussions:** https://github.com/tobsentobsentobs/swtor-starter/discussions
- **Wiki:** https://github.com/tobsentobsentobs/swtor-starter/wiki
- **.NET 8.0 Runtime:** https://dotnet.microsoft.com/en-us/download/dotnet/8.0

---

## ⭐ **Star das Repository!**

Wenn dir SWTOR Starter gefällt, gib uns einen Stern! ⭐

**May the Force be with you!** ⚡

---

<div align="center">

**Entwickelt mit ❤️ für die SWTOR-Community**

[![GitHub stars](https://img.shields.io/github/stars/tobsentobsentobs/swtor-starter?style=social)](https://github.com/tobsentobsentobs/swtor-starter)
[![GitHub forks](https://img.shields.io/badge/GitHub-Issues-blue.svg)](https://github.com/tobsentobsentobs/swtor-starter/issues)
[![GitHub pull requests](https://img.shields.io/badge/GitHub-PRs-green.svg)](https://github.com/tobsentobsentobs/swtor-starter/pulls)

</div>
