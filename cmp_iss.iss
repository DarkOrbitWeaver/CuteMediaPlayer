[Setup]
AppName=Cute Media Player
AppVersion=1.1
DefaultDirName={localappdata}\Cute Media Player
OutputBaseFilename=CuteMediaPlayer_Setup
Compression=lzma/ultra
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern
LicenseFile=LICENSE
SetupIconFile=Assets\appicon_c.ico
UninstallDisplayIcon={app}\Assets\appfac_x.ico
DisableProgramGroupPage=yes

[Files]
Source: "C:\DarkOrbitWeaver\CuteMediaPlayer\bin\Release\net8.0-windows\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "C:\DarkOrbitWeaver\CuteMediaPlayer\bin\Release\net8.0-windows\publish\Assets\appicon_c.ico"; DestDir: "{app}\Assets"
Source: "C:\DarkOrbitWeaver\CuteMediaPlayer\bin\Release\net8.0-windows\publish\Assets\appfac_x.ico"; DestDir: "{app}\Assets"

[Tasks]
Name: desktopicon; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "Additional icons:"
Name: startmenu; Description: "{cm:CreateStartMenuIcon}"; GroupDescription: "Additional icons:"; Flags: unchecked

[Icons]
; Desktop shortcut with main icon
Name: "{autodesktop}\Cute Media Player"; Filename: "{app}\CuteMediaPlayer.exe"; IconFilename: "{app}\Assets\appicon_c.ico"; Tasks: desktopicon
; Start Menu shortcut (directly in root) with alternate icon
Name: "{userprograms}\Cute Media Player"; Filename: "{app}\CuteMediaPlayer.exe"; IconFilename: "{app}\Assets\appfac_x.ico"; Tasks: startmenu

[Run]
Filename: "{app}\CuteMediaPlayer.exe"; Description: "{cm:LaunchProgram,Cute Media Player}"; Flags: postinstall nowait

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CuteMediaPlayer"; \
    ValueType: string; ValueName: "DisplayName"; ValueData: "Cute Media Player"; \
    Flags: uninsdeletekey

[CustomMessages]
CreateDesktopIcon=Create a &desktop icon
CreateStartMenuIcon=Create a &Start Menu entry
LaunchProgram=Launch Cute Media Player