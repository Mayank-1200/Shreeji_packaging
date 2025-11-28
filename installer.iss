[Setup]
AppName=Shreeji Packaging
AppVersion=1.0
DefaultDirName={pf}\ShreejiPackaging
DefaultGroupName=Shreeji Packaging
UninstallDisplayIcon={app}\ShreejiPackaging.exe
OutputDir=C:\Users\mayan\OneDrive\Desktop\Installer
OutputBaseFilename=ShreejiPackagingSetup
Compression=lzma
SolidCompression=yes
DisableProgramGroupPage=yes
WizardStyle=modern

[Files]
Source: "C:\Users\mayan\shreeji_packaging\bin\Release\net9.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Shreeji Packaging"; Filename: "{app}\ShreejiPackaging.exe"
Name: "{group}\Uninstall Shreeji Packaging"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\ShreejiPackaging.exe"; Description: "Launch Shreeji Packaging"; Flags: nowait postinstall skipifsilent

