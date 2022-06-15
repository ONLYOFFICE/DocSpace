[Setup]
AppName=MySQL Installer Runner
AppVersion=0.1.0
AppCopyright=© Ascensio System SIA 2019. All rights reserved
AppPublisher=Ascensio System SIA
AppPublisherURL=https://www.onlyoffice.com/
VersionInfoVersion=0.1.0
DefaultDirName={pf}\MySQL Installer Runner
DefaultGroupName=MySQL Installer Runner
CreateUninstallRegKey=no
Uninstallable=no
OutputBaseFilename="MySQL Installer Runner"
OutputDir=/

[Run]
Filename: "{pf}\MySQL\MySQL Installer for Windows\MySQLInstallerConsole.exe"; Parameters: "community install server;{param:MYSQL_VERSION|5.7.25};X64:*:servertype=Server;passwd={param:PASSWORD_PROP} -silent"; Flags: runhidden