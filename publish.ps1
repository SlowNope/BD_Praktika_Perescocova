$ErrorActionPreference = 'Stop'
$output = Join-Path $PSScriptRoot 'publish'
if (Test-Path -LiteralPath $output) { Remove-Item -LiteralPath $output -Recurse -Force }
dotnet publish (Join-Path $PSScriptRoot 'src/ProgrammingTasks.Wpf/ProgrammingTasks.Wpf.csproj') -c Release -r win-x64 --self-contained true -o $output
Write-Host "Готовая автономная версия находится в: $output"
