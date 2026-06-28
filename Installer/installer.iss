[Setup]
AppName=AlkaConvertio
AppVersion=1.0.0
AppPublisher=sanyAlka
AppCopyright=Copyright (C) 2026 sanyAlka
AppPublisherURL=https://github.com/sanyAlka
AppSupportURL=https://github.com/sanyAlka
AppUpdatesURL=https://github.com/sanyAlka

DefaultDirName={pf}\AlkaConv
DefaultGroupName=PDF to Image AlkaConv
UninstallDisplayIcon={app}\PdfToImage.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.

WizardImageFile=C:\Users\Ains\Downloads\PdfToImage\Installer\WizModernImage-IS.bmp
WizardSmallImageFile=C:\Users\Ains\Downloads\PdfToImage\Installer\WizModernSmallImage-IS.bmp

OutputBaseFilename=AlkaConv_Setup
PrivilegesRequired=admin
CreateUninstallRegKey=yes
Uninstallable=yes

[InstallDelete]
; Удаляем всё содержимое папки и саму папку перед установкой
Type: filesandordirs; Name: "{app}"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs
 
[Registry] 
; Convert to JPG
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG"; ValueType: string; ValueName: ""; ValueData: "В JPG"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\PdfToImage.exe"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociatioё1ns\.pdf\Shell\ConvertToJPG\command"; ValueType: string; ValueName: ""; ValueData: """{app}\PdfToImage.exe"" ""%1"""; Flags: uninsdeletekey

; High Quality
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG-High"; ValueType: string; ValueName: ""; ValueData: "В JPG в подпапке"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG-High\command"; ValueType: string; ValueName: ""; ValueData: """{app}\PdfToImage.exe"" -subfolder ""%1"""; Flags: uninsdeletekey

; Delete PDF
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG-Delete"; ValueType: string; ValueName: ""; ValueData: "В JPG с удалением PDF"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.pdf\Shell\ConvertToJPG-Delete\command"; ValueType: string; ValueName: ""; ValueData: """{app}\PdfToImage.exe"" -subfolder -delete ""%1"""; Flags: uninsdeletekey


[Icons]
Name: "{group}\PDF to Image AlkaConv"; Filename: "{app}\PdfToImage.exe"
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"
Name: "{userdesktop}\PDF to Image AlkaConv"; Filename: "{app}\PdfToImage.exe"

[Run]
Filename: "{app}\PdfToImage.exe"; Description: "Launch AlkaConv"; Flags: nowait postinstall skipifsilent