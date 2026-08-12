#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "Kör tester med kodtäckning (Coverlet)..." -ForegroundColor Cyan
dotnet test Dropzone.sln `
    --collect:"XPlat Code Coverage" `
    --settings Dropzone.Tests/.runsettings `
    --results-directory TestResults

Write-Host "Genererar HTML-rapport (ReportGenerator)..." -ForegroundColor Cyan
dotnet reportgenerator `
    -reports:"TestResults/**/coverage.cobertura.xml" `
    -targetdir:"TestResults/CoverageReport" `
    -reporttypes:"Html;HtmlSummary;Cobertura" `
    -sourcedirs:"Dropzone"

$cobertura = Join-Path $root "TestResults\CoverageReport\Cobertura.xml"
Write-Host ""
Write-Host "Klart!" -ForegroundColor Green
Write-Host "  HTML-rapport:  TestResults/CoverageReport/index.html"
Write-Host "  Cobertura:     TestResults/CoverageReport/Cobertura.xml"
Write-Host ""
Write-Host "Tips: Öppna index.html i webbläsaren, eller använd Coverage Gutters i VS Code/Cursor" -ForegroundColor DarkGray
Write-Host "      med Cobertura-filen ovan för täckning direkt i kodvyn." -ForegroundColor DarkGray
