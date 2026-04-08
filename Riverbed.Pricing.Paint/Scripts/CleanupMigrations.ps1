# ========================================
# Migration Cleanup Script
# ========================================
# This script removes all EF Core migrations and prepares for a fresh start
# 
# IMPORTANT: Run this ONLY if you have a database backup!
# ========================================

$projectPath = "D:\Projects-Riverbed\Riverbed.Quoted.App\Riverbed.Pricing.Paint"
$migrationsPath = Join-Path $projectPath "Data\Migrations"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "EF Core Migrations Cleanup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verify migrations folder exists
if (-not (Test-Path $migrationsPath)) {
    Write-Host "ERROR: Migrations folder not found at: $migrationsPath" -ForegroundColor Red
    exit 1
}

# Count current migrations
$migrationFiles = Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File
$totalFiles = $migrationFiles.Count

Write-Host "Found $totalFiles migration files" -ForegroundColor Yellow
Write-Host ""

# Warning prompt
Write-Host "WARNING: This will delete ALL migration files!" -ForegroundColor Red
Write-Host "Make sure you have:" -ForegroundColor Yellow
Write-Host "  1. A database backup" -ForegroundColor Yellow
Write-Host "  2. Committed any important code changes" -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "Type 'DELETE' to continue or anything else to cancel"

if ($confirmation -ne "DELETE") {
    Write-Host "Operation cancelled." -ForegroundColor Green
    exit 0
}

Write-Host ""
Write-Host "Deleting migration files..." -ForegroundColor Yellow

try {
    # Delete all migration files
    Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File | Remove-Item -Force
    Get-ChildItem -Path $migrationsPath -Filter "*.Designer.cs" -File | Remove-Item -Force

    Write-Host "✓ Successfully deleted $totalFiles migration files" -ForegroundColor Green

    # List remaining files (should only be directory if any)
    $remainingFiles = Get-ChildItem -Path $migrationsPath -File
    if ($remainingFiles.Count -eq 0) {
        Write-Host "✓ Migrations folder is now clean" -ForegroundColor Green
    } else {
        Write-Host "! Some files remain:" -ForegroundColor Yellow
        $remainingFiles | ForEach-Object { Write-Host "  - $($_.Name)" }
    }

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "1. Open Package Manager Console in Visual Studio" -ForegroundColor White
    Write-Host "2. Run: Add-Migration InitialCreate" -ForegroundColor White
    Write-Host "3. Review the generated migration" -ForegroundColor White
    Write-Host "4. Run: Update-Database" -ForegroundColor White
    Write-Host ""

} catch {
    Write-Host "ERROR: Failed to delete migration files" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
