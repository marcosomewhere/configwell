# ConfigWell

Windows-Desktop-App zum sicheren Anzeigen und Bearbeiten von Konfigurationsdateien.

## Voraussetzungen

- Windows 10 / 11
- .NET Framework 4.8 (in Windows 10/11 bereits vorinstalliert)
- Visual Studio 2019 oder neuer (Community Edition reicht)

## Build & Start

```
1. ConfigWell.sln in Visual Studio öffnen
2. F5 drücken (Debug) oder Strg+F5 (ohne Debugger)
```

Oder per Kommandozeile:

```cmd
msbuild ConfigWell.WinForms\ConfigWell.WinForms.csproj /p:Configuration=Release
ConfigWell.WinForms\bin\Release\ConfigWell.exe
```

## Unterstützte Dateitypen

| Endung | Format | Kommentare |
|--------|--------|------------|
| `.properties` | key=value oder key: value | `#`, `;`, `!` |
| `.ini` | [Sektionen] mit key=value | `#`, `;` |
| `.xml`, `.config` | appSettings `<add key="..." value="..."/>` | XML-Kommentare |
| `.bat`, `.cmd` | `SET KEY=value` und `SET "KEY=value"` | `::`, `REM` |
| `.ps1` | `$Var = "Wert"`, `$Flag = $true`, `$Zahl = 42` | `#` |

## Bedienung

- **Öffnen**: Toolbar-Button oder Datei-Dialog
- **Speichern**: Strg+S oder Toolbar-Button
- **Neu parsen**: Liest die Datei neu ein (verwirft ungespeicherte Änderungen)
- **Linker Bereich**: Dateiinfos und erkannte Abschnitte/Gruppen
- **Mittlerer Bereich**: Settings-Formular (Checkboxen für bool, TextBox für Text/Zahlen)
- **Unterer Bereich**: Rohtext-Vorschau (live aktualisiert)
- **Statusleiste**: Pfad, Dateityp, Encoding, Änderungsstatus

## Backup-Verhalten

Vor jedem Speichern wird automatisch ein Backup erstellt:
- Speicherort: `.configwell_backups\` neben der Originaldatei
- Dateiname: `original.ext.YYYYMMDD_HHmmss.bak`
- Schlägt das Backup fehl, wird **nicht** gespeichert

## Sicherheit

- Keine externen Prozesse, keine Netzwerkzugriffe, keine Telemetrie
- Skriptdateien werden nur gelesen, nie ausgeführt
- Warnung bei Dateien in `C:\Windows` oder `C:\Program Files`
- Verständliche Fehlermeldung bei fehlenden Schreibrechten

## Bekannte Einschränkungen

- Kein Tabs / Mehrfachdokument-Ansicht
- XML: Nur `<appSettings>` wird vollständig unterstützt; andere XML-Strukturen werden heuristisch erkannt (markiert mit `*`)
- PS1: Nur einfache Variablenzuweisungen am Zeilenanfang (keine Splatting, Hashtables, etc.)
- Mehrzeilige Werte (z.B. in .properties) werden nicht unterstützt
- Keine Diff-Ansicht zwischen Backup und aktuellem Stand

## Testdateien

Im Ordner `SampleFiles\` befinden sich Beispieldateien für alle unterstützten Typen.
