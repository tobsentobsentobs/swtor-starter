# Contributing to SWTOR Starter

Vielen Dank für Ihr Interesse, zu SWTOR Starter beizutragen! Dieses Dokument enthält Richtlinien und Informationen für Entwickler, die zum Projekt beitragen möchten.

## 🚀 Wie Sie beitragen können

### 🐛 Bug Reports
- Verwenden Sie die [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md)
- Beschreiben Sie den Fehler so detailliert wie möglich
- Fügen Sie Schritte zur Reproduktion hinzu
- Geben Sie Informationen über Ihr System an (Windows-Version, .NET-Version)

### 💡 Feature Requests
- Verwenden Sie die [Feature Request Template](.github/ISSUE_TEMPLATE/feature_request.md)
- Beschreiben Sie das gewünschte Feature klar und präzise
- Erklären Sie, warum dieses Feature nützlich wäre
- Überlegen Sie sich, wie es implementiert werden könnte

### 🔧 Code Contributions
- Forken Sie das Repository
- Erstellen Sie einen Feature-Branch
- Schreiben Sie sauberen, gut dokumentierten Code
- Testen Sie Ihre Änderungen gründlich
- Erstellen Sie einen Pull Request

## 🛠️ Entwicklungsumgebung

### Voraussetzungen
- **Visual Studio 2022** oder **Visual Studio Code**
- **.NET 8.0 SDK**
- **Windows 10/11** (für WPF-Entwicklung)
- **Git** für Versionskontrolle

### Projekt einrichten
```bash
# Repository klonen
git clone https://github.com/tobsentobsentobs/swtor-starter.git
cd swtor-starter

# Abhängigkeiten wiederherstellen
dotnet restore

# Projekt starten
dotnet run --project SWTORStarter.csproj
```

### Build erstellen
```bash
# Debug-Build
dotnet build

# Release-Build
dotnet build -c Release

# Publish für Release
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish
```

## 📝 Code-Standards

### C# Coding Standards
- Verwenden Sie **C# 12.0** Features
- Folgen Sie den **Microsoft C# Coding Conventions**
- Verwenden Sie **XML-Dokumentation** für öffentliche APIs
- Schreiben Sie **aussagekräftige Variablen- und Methodennamen**

### WPF Standards
- Verwenden Sie **MVVM Pattern**
- Trennen Sie **UI-Logik** von **Business-Logik**
- Verwenden Sie **Data Binding** wo möglich
- Halten Sie **XAML** sauber und lesbar

### Beispiel für XML-Dokumentation
```csharp
/// <summary>
/// Verschlüsselt ein Passwort mit AES-256
/// </summary>
/// <param name="password">Das zu verschlüsselnde Passwort</param>
/// <returns>Das verschlüsselte Passwort als Base64-String</returns>
private string EncryptPassword(string password)
{
    // Implementation
}
```

## 🔒 Sicherheitsrichtlinien

### Passwort-Behandlung
- **Niemals** Passwörter im Klartext loggen
- Verwenden Sie **AES-256 Verschlüsselung** für gespeicherte Passwörter
- **Keine** Passwörter an externe Dienste senden
- Alle Passwort-Operationen **lokal** durchführen

### Windows API Verwendung
- Verwenden Sie **sichere Windows API Aufrufe**
- Behandeln Sie **Fehler** ordnungsgemäß
- Verwenden Sie **try-catch Blöcke** für kritische Operationen
- **Validieren** alle Eingaben vor der Verarbeitung

## 🧪 Testing

### Unit Tests
- Schreiben Sie **Unit Tests** für neue Features
- Verwenden Sie **xUnit** oder **NUnit** als Test-Framework
- Testen Sie **Edge Cases** und **Fehlerszenarien**
- Zielen Sie auf **hohe Testabdeckung**

### Integration Tests
- Testen Sie **End-to-End Szenarien**
- Überprüfen Sie **Steam-Integration**
- Testen Sie **Passwort-Verschlüsselung**
- Validieren Sie **Konfigurationsdateien**

### Manuelle Tests
- Testen Sie auf **Windows 10** und **Windows 11**
- Überprüfen Sie **verschiedene .NET Runtime Versionen**
- Testen Sie **verschiedene Steam-Installationen**
- Validieren Sie **SWTOR Launcher Integration**

## 📚 Dokumentation

### Code-Dokumentation
- **XML-Dokumentation** für alle öffentlichen APIs
- **Inline-Kommentare** für komplexe Logik
- **README-Updates** bei neuen Features
- **CHANGELOG-Updates** bei Änderungen

### Benutzer-Dokumentation
- **README.md** aktuell halten
- **Installationsanweisungen** überprüfen
- **Konfigurationsbeispiele** bereitstellen
- **Troubleshooting** dokumentieren

## 🔄 Pull Request Prozess

### Vor dem PR
- [ ] Code kompiliert ohne Fehler
- [ ] Alle Tests bestehen
- [ ] Code-Standards eingehalten
- [ ] Dokumentation aktualisiert
- [ ] Änderungen getestet

### PR erstellen
1. **Fork** das Repository
2. **Branch** für Ihr Feature erstellen
3. **Änderungen** committen
4. **Push** zu Ihrem Fork
5. **Pull Request** erstellen

### PR Template verwenden
- Beschreiben Sie **was** geändert wurde
- Erklären Sie **warum** die Änderung nötig ist
- Fügen Sie **Screenshots** hinzu (falls relevant)
- Verlinken Sie **Issues** (falls vorhanden)

## 🚫 Was nicht erlaubt ist

- **Breaking Changes** ohne vorherige Diskussion
- **Sicherheitslücken** in Code oder Dokumentation
- **Ungetestete Features** in den main Branch
- **Unvollständige Dokumentation**
- **Code ohne XML-Dokumentation**

## 🤝 Community

### Kommunikation
- **Respektvoll** und **konstruktiv** sein
- **Fragen stellen** wenn etwas unklar ist
- **Feedback geben** und **Feedback annehmen**
- **Anderen Entwicklern helfen**

### Code Reviews
- **Konstruktive Kritik** üben
- **Positive Aspekte** hervorheben
- **Verbesserungsvorschläge** machen
- **Lernen** von anderen Entwicklern

## 📞 Hilfe bekommen

### Fragen stellen
- **GitHub Issues** für Bug Reports
- **GitHub Discussions** für allgemeine Fragen
- **Pull Request Comments** für Code-spezifische Fragen

### Ressourcen
- [.NET 8.0 Dokumentation](https://docs.microsoft.com/en-us/dotnet/)
- [WPF Dokumentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Material Design für WPF](https://materialdesigninxaml.net/)
- [GitHub Guides](https://guides.github.com/)

---

**Vielen Dank für Ihre Beiträge!** 🎉

*May the Force be with you!* ⚡
