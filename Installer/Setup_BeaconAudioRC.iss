#define MyAppName "Beacon Audio RC"
#define MyAppVersion "2.0"
#define MyAppPublisher "Beacon Audio"
#define MyAppExeName "BeaconAudioRC.exe"

[Setup]
AppId={{A3C9E281-2292-4211-9A23-1F6B120938AA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=Setup_Beacon_Audio_RC
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Files]
Source: "..\BeaconAudioRC\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\BeaconAudioRC\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"