# Changelog

Alle wichtigen Änderungen an diesem Projekt werden in dieser Datei dokumentiert.

Das Format basiert auf [Keep a Changelog](https://keepachangelog.com/de/1.0.0/),
und dieses Projekt folgt dem [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Geplant
- Verbesserte Steam-Integration
- Zusätzliche Sicherheitsfeatures
- Performance-Optimierungen
- Erweiterte Konfigurationsoptionen

## [1.0.0] - 2025-08-29

### Hinzugefügt
- **WPF Desktop Application** - Moderne Windows-Desktop-Anwendung mit Material Design
- **Automatische Passwort-Eingabe** - Sichere AES-256 Verschlüsselung für SWTOR-Passwort
- **Steam-Integration** - Nahtlose Integration mit Steam für automatischen Start
- **Windows API Integration** - Native Windows-Funktionalität für Prozess-Management und Tastatureingaben
- **Professionelle UI** - Material Design mit modernem Look & Feel
- **Status-Überwachung** - Live-Status für Steam, SWTOR Launcher und Spiel
- **Konfigurations-Interface** - Umfangreiche Einstellungsmöglichkeiten über SettingsWindow
- **Auto-Start Timer** - Automatischer Start mit konfigurierbarer Verzögerung
- **Intelligente Launcher-Überwachung** - RAM-basierte Erkennung des Ladezustands
- **Status-Box** - Schwebendes Status-Fenster mit Live-Updates

### Technische Details
- **Framework:** .NET 8.0 WPF
- **Architektur:** MVVM Pattern
- **UI:** Material Design mit XAML
- **Verschlüsselung:** AES-256 für Passwort-Speicherung
- **Konfiguration:** JSON-basierte Einstellungen
- **Prozess-Management:** Windows API Integration
- **Automatische Pfad-Erkennung:** Steam und SWTOR werden automatisch gefunden

### Konfiguration
- **SWTOR-Passwort** - Einzige Anmeldeinformation (kein Benutzername erforderlich)
- **Steam-Pfad** - Automatisch erkannt (keine manuelle Konfiguration)
- **SWTOR-Pfad** - Automatisch erkannt (keine manuelle Konfiguration)
- **Timer-Einstellungen** - Anpassbare Verzögerungen für Start-Sequenz
- **Auto-Start** - Windows-Startup-Integration
- **RAM-Schwellenwert** - Konfigurierbarer Schwellenwert für Launcher-Ladezustand

### Bekannte Probleme
- **Keine kritischen Probleme** in der aktuellen Version
- Alle Features funktionieren wie erwartet
- Stabile Performance auf Windows 10/11

### Systemanforderungen
- **Betriebssystem:** Windows 10 (Version 1809) oder höher
- **Architektur:** x64 (64-bit)
- **RAM:** 4 GB (Minimum), 8 GB (Empfohlen)
- **Speicher:** 200 MB freier Speicherplatz
- **Abhängigkeiten:** .NET 8.0 Runtime, Steam installiert, SWTOR bereits einmal gestartet

### Installation
1. **Release herunterladen** - SWTORStarter.exe (10.5 MB)
2. **.NET 8.0 Runtime installieren** - Falls nicht vorhanden
3. **Anwendung ausführen** - Einfacher Start ohne Installation
4. **Passwort konfigurieren** - SWTOR-Passwort eintragen

### Sicherheit
- **AES-256 Verschlüsselung** für das gespeicherte SWTOR-Passwort
- **Lokale Speicherung** - Keine Daten werden übertragen
- **Kein Master-Passwort erforderlich** - Einfache Passwort-Speicherung
- **Windows API Integration** - Sichere Prozess-Verwaltung und Tastatureingaben

### Dokumentation
- **Vollständige README.md** - Umfassende Anleitung mit korrekter Benutzung
- **CHANGELOG.md** - Diese Versionshistorie
- **CONTRIBUTING.md** - Entwickler-Richtlinien
- **SECURITY.md** - Sicherheitsrichtlinien
- **LICENSE** - MIT-Lizenz

---

## Änderungsprotokoll

### Versionierung
- **MAJOR.MINOR.PATCH** Format
- **MAJOR:** Inkompatible API-Änderungen
- **MINOR:** Neue Features (rückwärtskompatibel)
- **PATCH:** Bug-Fixes (rückwärtskompatibel)

### Kategorien
- **Hinzugefügt** - Neue Features
- **Geändert** - Änderungen an bestehenden Features
- **Veraltet** - Bald entfernte Features
- **Entfernt** - Entfernte Features
- **Behoben** - Bug-Fixes
- **Sicherheit** - Sicherheitsverbesserungen

---

**Entwickelt mit ❤️ für die SWTOR-Community**

*May the Force be with you!* ⚡
