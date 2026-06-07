# ConfigWell — Claude-Kontext

## Projekt

WinForms-Desktop-App (.NET Framework 4.8) zum sicheren Öffnen und Bearbeiten von Konfigurationsdateien.
Keine Cloud, kein Netzwerk, keine externen Prozesse, keine Telemetrie.

## Architektur

```
ConfigWell.WinForms/
  Models/          — Datenmodelle: ParsedConfig, ConfigSection, ConfigEntry, FileType
  Parsers/         — IConfigParser + konkrete Parser pro Dateityp + ParserFactory
  Services/        — FileService (Öffnen), BackupService, ConfigWriterService (Speichern)
  Controls/        — SettingsFormPanel: dynamisch generiertes Einstellungsformular
  MainForm.cs      — Hauptfenster, Toolbar, StatusBar, Event-Verdrahtung
SampleFiles/       — Testdateien für alle unterstützten Formate
```

## Kernregeln

- Parser lesen nur Text, starten niemals externe Prozesse
- Backup wird vor jedem Speichern erstellt; schlägt Backup fehl → kein Speichern
- Änderungen werden nur auf expliziten Klick auf "Speichern" geschrieben
- Kommentare, Leerzeilen und Formatierung bleiben beim Speichern erhalten (ConfigWriterService)
- Heuristisch erkannte Einstellungen sind im Formular mit `*` und orangefarbener Schrift markiert

## Dateitypen und zugehörige Parser

| Typ | Parser | Kommentarsyntax |
|-----|--------|-----------------|
| .properties | PropertiesParser | #, ;, ! |
| .ini | IniParser | #, ; (auch inline) |
| .xml/.config | XmlConfigParser | XML-Kommentare vor <add> |
| .bat/.cmd | BatParser | ::, REM |
| .ps1 | Ps1Parser | # |

## Codex / Entwicklungshinweise

- .NET Framework 4.8, kein NuGet außer BCL
- WinForms, keine WPF, kein XAML
- Keine async/await (WinForms 4.8 UI bleibt synchron; Dateien sind klein)
- ConfigWriterService.ApplyChanges() schreibt Entry.Value-Änderungen zurück in RawText
- SettingsFormPanel generiert Controls dynamisch; CheckBox für Boolean, TextBox sonst
- MainForm.OnConfigChanged() aktualisiert die Rohtext-Vorschau live
