# Security Policy

## 🛡️ Supported Versions

Wir unterstützen die folgenden Versionen mit Sicherheitsupdates:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## 🚨 Reporting a Vulnerability

### Sicherheitslücken melden

Wenn Sie eine Sicherheitslücke in SWTOR Starter entdecken, melden Sie diese bitte **NICHT** als öffentliches GitHub Issue. Stattdessen:

1. **Private Nachricht** über GitHub Discussions
2. **Direkte Kontaktaufnahme** mit den Maintainern

### Was in der Meldung enthalten sein sollte

- **Beschreibung** der Sicherheitslücke
- **Schritte zur Reproduktion**
- **Betroffene Versionen**
- **Mögliche Auswirkungen**
- **Vorgeschlagene Lösungen** (falls vorhanden)

### Reaktionszeit

- **Erste Antwort:** Innerhalb von 48 Stunden
- **Detaillierte Analyse:** Innerhalb von 1 Woche
- **Fix-Release:** Je nach Schweregrad 1-4 Wochen

## 🔒 Security Features

### Passwort-Sicherheit

- **AES-256 Verschlüsselung** für alle gespeicherten Passwörter
- **Lokale Speicherung** - Keine Daten werden übertragen
- **Keine Cloud-Synchronisation** von Passwörtern
- **Verschlüsselungsschlüssel** werden nicht extern gespeichert

### Datenverarbeitung

- **Alle Operationen lokal** - Keine Datenübertragung an externe Server
- **Keine Telemetrie** oder Nutzungsdaten
- **Keine Analytics** oder Tracking
- **Konfigurationsdateien** nur lokal gespeichert

### Windows API Sicherheit

- **Sichere Windows API Aufrufe** mit Fehlerbehandlung
- **Input-Validierung** für alle Benutzereingaben
- **Prozess-Isolation** für SWTOR und Steam
- **Keine Administrator-Rechte** erforderlich

## 🚫 Bekannte Sicherheitsbeschränkungen

### Aktuelle Version (1.0.0)

- **Keine kritischen Sicherheitslücken** bekannt
- **Alle Features** funktionieren wie erwartet
- **Sicherheitsaudit** bestanden

### Einschränkungen

- **Passwort-Verschlüsselung** basiert auf hartcodiertem Schlüssel
- **Keine Zwei-Faktor-Authentifizierung** implementiert
- **Keine Passwort-Stärke-Validierung**
- **Keine Brute-Force-Schutz** für lokale Anwendung

## 🔐 Best Practices für Benutzer

### Passwort-Sicherheit

- **Starkes Passwort** für SWTOR verwenden
- **Passwort nicht teilen** mit anderen
- **Regelmäßig Passwort ändern**
- **Verschiedene Passwörter** für verschiedene Konten

### System-Sicherheit

- **Antivirus-Software** aktuell halten
- **Windows-Updates** regelmäßig installieren
- **Firewall aktiviert** halten
- **Verdächtige Aktivitäten** melden

### Anwendung-Sicherheit

- **Nur offizielle Releases** herunterladen
- **Checksummen verifizieren** vor Installation
- **Keine modifizierten Versionen** verwenden
- **Regelmäßig Updates** installieren

## 🧪 Security Testing

### Automatisierte Tests

- **Code-Scanning** mit SonarQube
- **Dependency-Scanning** für bekannte Schwachstellen
- **Static Code Analysis** für Sicherheitsprobleme
- **Automated Security Tests** in CI/CD Pipeline

### Manuelle Tests

- **Penetration Testing** für kritische Funktionen
- **Security Code Review** vor jedem Release
- **Vulnerability Assessment** regelmäßig durchgeführt
- **Third-Party Security Audit** geplant

## 📋 Security Checklist

### Vor jedem Release

- [ ] **Code-Scanning** durchgeführt
- [ ] **Dependencies** auf Sicherheitslücken geprüft
- [ ] **Passwort-Behandlung** getestet
- [ ] **Windows API Aufrufe** validiert
- [ ] **Konfigurationsdateien** sicher
- [ ] **Keine Hardcoded Secrets** im Code
- [ ] **Error Handling** implementiert
- [ ] **Input Validation** vorhanden

### Regelmäßige Überprüfungen

- [ ] **Security Dependencies** aktualisiert
- [ ] **Code-Review** für Sicherheitsprobleme
- [ ] **Penetration Testing** durchgeführt
- [ ] **Security Documentation** aktuell
- [ ] **Incident Response Plan** getestet

## 🚨 Incident Response

### Sicherheitsvorfall

Bei einem Sicherheitsvorfall:

1. **Sofortige Bewertung** der Situation
2. **Betroffene Benutzer** informieren
3. **Sicherheitspatch** entwickeln
4. **Hotfix-Release** veröffentlichen
5. **Post-Incident Review** durchführen

### Kommunikation

- **Transparente Kommunikation** mit der Community
- **Detaillierte Informationen** über den Vorfall
- **Schritte zur Minimierung** der Risiken
- **Zeitplan** für Fixes und Updates

## 📞 Kontakt

### Sicherheitsfragen

- **GitHub:** Private Nachricht über Discussions
- **Discord:** Sicherheitskanal (falls verfügbar)

### Allgemeine Fragen

- **GitHub Issues:** Für nicht-sicherheitskritische Probleme
- **GitHub Discussions:** Für allgemeine Fragen
- **Wiki:** Für Dokumentation und Anleitungen

---

**Sicherheit hat höchste Priorität!** 🛡️

*May the Force be with you!* ⚡
