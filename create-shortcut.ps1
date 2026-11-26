$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$exe = Join-Path $root 'publish-current\ProgrammingTasks.Wpf.exe'
if (-not (Test-Path -LiteralPath $exe)) { & (Join-Path $root 'publish.ps1') }
$shortcutPath = Join-Path $root 'ProgrammingTasks.lnk'
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $exe
$shortcut.WorkingDirectory = Join-Path $root 'publish'
$shortcut.Description = 'ProgrammingTasks application'
$shortcut.IconLocation = "$exe,0"
$shortcut.Save()
Write-Host "Ярлык создан: $shortcutPath"
