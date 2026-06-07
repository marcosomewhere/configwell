# Konfiguration für PowerShell-Deployment-Skript

# Zielserver-Hostname
$ServerHost = "deploy.internal.local"

# Zielport
$ServerPort = 443

# Anwendungsname
$AppName = "MyWebApp"

# Versionsnummer
$AppVersion = "3.2.1"

# Deployment-Verzeichnis
$DeployPath = "C:\inetpub\wwwroot\myapp"

# Debug-Ausgaben aktivieren
$DebugMode = $false

# Backup vor Deployment erstellen
$CreateBackup = $true

# Maximale Anzahl Backup-Versionen
$MaxBackups = 5

# Timeout in Sekunden
$TimeoutSeconds = 120

# E-Mail-Benachrichtigung aktivieren
$SendNotification = $false

# Empfänger-Adresse für Benachrichtigungen
$NotificationEmail = "admin@example.com"
