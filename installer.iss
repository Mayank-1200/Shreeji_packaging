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

[Code]
function IsDotNet90Installed(): Boolean;
var
  Version: String;
  Release: Cardinal;
  SubKeys: TArrayOfString;
  I: Integer;
  KeyPath: String;
  DotNetPath: String;
  FindRec: TFindRec;
  FindResult: Boolean;
begin
  Result := False;

  // Method 1: Check if runtime files exist in Program Files (most reliable)
  // This works regardless of registry structure or Windows version
  // Check common version directories directly
  DotNetPath := ExpandConstant('{pf}\dotnet\shared\Microsoft.WindowsDesktop.App');
  if DirExists(DotNetPath + '\9.0.0') or DirExists(DotNetPath + '\9.0') or DirExists(DotNetPath + '\9.1') then
  begin
    Result := True;
    Exit;
  end;

  // Also check by enumerating if base directory exists
  if DirExists(DotNetPath) then
  begin
    FindResult := FindFirst(DotNetPath + '\*', FindRec);
    while FindResult do
    begin
      if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
      begin
        if (Copy(FindRec.Name, 1, 3) = '9.0') or (Copy(FindRec.Name, 1, 2) = '9.') then
        begin
          FindClose(FindRec);
          Result := True;
          Exit;
        end;
      end;
      FindResult := FindNext(FindRec);
    end;
    FindClose(FindRec);
  end;

  // Method 2: Check Program Files (x86) location
  DotNetPath := ExpandConstant('{pf32}\dotnet\shared\Microsoft.WindowsDesktop.App');
  if DirExists(DotNetPath + '\9.0.0') or DirExists(DotNetPath + '\9.0') or DirExists(DotNetPath + '\9.1') then
  begin
    Result := True;
    Exit;
  end;

  if DirExists(DotNetPath) then
  begin
    FindResult := FindFirst(DotNetPath + '\*', FindRec);
    while FindResult do
    begin
      if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
      begin
        if (Copy(FindRec.Name, 1, 3) = '9.0') or (Copy(FindRec.Name, 1, 2) = '9.') then
        begin
          FindClose(FindRec);
          Result := True;
          Exit;
        end;
      end;
      FindResult := FindNext(FindRec);
    end;
    FindClose(FindRec);
  end;

  // Method 3: Check registry paths - Standard x64 location (HKLM)
  KeyPath := 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';

  // Check for exact 9.0.0 version string value
  if RegQueryStringValue(HKLM, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  // Check Release DWORD for 9.0.0
  if RegQueryDWordValue(HKLM, KeyPath + '\9.0.0', 'Release', Release) then
  begin
    Result := True;
    Exit;
  end;

  // Enumerate subkeys to find any 9.x version
  if RegGetSubkeyNames(HKLM, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // Method 4: Check WOW6432Node location
  KeyPath := 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';

  if RegQueryStringValue(HKLM, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  if RegQueryDWordValue(HKLM, KeyPath + '\9.0.0', 'Release', Release) then
  begin
    Result := True;
    Exit;
  end;

  if RegGetSubkeyNames(HKLM, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // Method 5: Check alternative registry structure
  KeyPath := 'SOFTWARE\dotnet\Setup\InstalledVersions\sharedfx\Microsoft.WindowsDesktop.App';

  if RegQueryStringValue(HKLM, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  if RegGetSubkeyNames(HKLM, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // Method 6: Check WOW6432Node alternative structure
  KeyPath := 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\sharedfx\Microsoft.WindowsDesktop.App';

  if RegQueryStringValue(HKLM, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  if RegGetSubkeyNames(HKLM, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // Method 7: Check HKCU (Current User) registry
  KeyPath := 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';

  if RegQueryStringValue(HKCU, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  if RegGetSubkeyNames(HKCU, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // Method 8: Check HKCU alternative structure
  KeyPath := 'SOFTWARE\dotnet\Setup\InstalledVersions\sharedfx\Microsoft.WindowsDesktop.App';

  if RegQueryStringValue(HKCU, KeyPath, '9.0.0', Version) then
  begin
    Result := True;
    Exit;
  end;

  if RegGetSubkeyNames(HKCU, KeyPath, SubKeys) then
  begin
    for I := 0 to GetArrayLength(SubKeys) - 1 do
    begin
      if (Copy(SubKeys[I], 1, 3) = '9.0') or (Copy(SubKeys[I], 1, 2) = '9.') then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;
end;

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
  DotNetUrl: String;
  UserChoice: Integer;
begin
  Result := True;

  // Check if .NET 9.0 Desktop Runtime is installed
  if not IsDotNet90Installed() then
  begin
    // .NET 9.0 Runtime not found - show warning but allow installation to continue
    UserChoice := MsgBox('Warning: .NET 9.0 Desktop Runtime (x64) was not detected on this system.' + #13#10 + #13#10 +
              'The application may not run properly without it.' + #13#10 + #13#10 +
              'Would you like to:' + #13#10 +
              '• YES - Open download page and continue installation' + #13#10 +
              '• NO - Continue installation anyway' + #13#10 +
              '• CANCEL - Cancel installation',
              mbConfirmation, MB_YESNOCANCEL);

    if UserChoice = IDYES then
    begin
      // Open .NET 9.0 download page
      DotNetUrl := 'https://dotnet.microsoft.com/download/dotnet/9.0';
      ShellExec('open', DotNetUrl, '', '', SW_SHOW, ewNoWait, ErrorCode);
      // Continue with installation
      Result := True;
    end
    else if UserChoice = IDNO then
    begin
      // User wants to continue anyway
      Result := True;
    end
    else
    begin
      // User cancelled
      Result := False;
    end;
  end;
end;

