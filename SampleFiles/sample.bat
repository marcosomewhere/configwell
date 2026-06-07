@ECHO OFF
:: Konfiguration für Deployment-Skript
:: Umgebungsvariablen für die Anwendung

:: Zielserver
SET SERVER_HOST=192.168.1.100

:: Zielport
SET SERVER_PORT=8080

:: Anwendungspfad auf dem Server
SET "APP_PATH=C:\Apps\MyApplication"

:: Backup-Verzeichnis
SET "BACKUP_DIR=D:\Backups\MyApp"

:: Debug-Modus aktivieren (true/false)
SET DEBUG_MODE=false

:: Anzahl paralleler Threads
SET THREAD_COUNT=4

:: Maximale Wartezeit in Sekunden
SET TIMEOUT_SECONDS=30

:: Datenbankserver
SET DB_SERVER=db.internal.local

:: Datenbankname
SET "DB_NAME=ProductionDatabase"

ECHO Konfiguration geladen.
