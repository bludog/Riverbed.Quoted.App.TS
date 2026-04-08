# ========================================
# Step-by-Step Migration Reset
# Execute these commands in order
# ========================================

# Navigate to project directory
cd "D:\Projects-Riverbed\Riverbed.Quoted.App"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Step 1: Backup Current State" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Create a backup branch
Write-Host "Creating backup branch..." -ForegroundColor Yellow
git checkout -b migration-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')
git add .
git commit -m "Backup before migration cleanup"
git checkout RoomImageDispl

Write-Host "✓ Backup branch created" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Step 2: Delete Migration Files" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$migrationsPath = "Riverbed.Pricing.Paint\Data\Migrations"

# Count files before deletion
$beforeCount = (Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File).Count
Write-Host "Found $beforeCount migration files" -ForegroundColor Yellow

# Confirmation
$confirm = Read-Host "Delete all migration files? (yes/no)"
if ($confirm -eq "yes") {

    # Keep a list of files being deleted
    $deletedFiles = Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File | Select-Object -ExpandProperty Name

    # Delete all .cs files in migrations folder
    Remove-Item "$migrationsPath\*.cs" -Force -Verbose

    # Verify deletion
    $afterCount = (Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File).Count

    if ($afterCount -eq 0) {
        Write-Host "✓ Successfully deleted $beforeCount migration files" -ForegroundColor Green
    } else {
        Write-Host "⚠ Warning: $afterCount files remain" -ForegroundColor Yellow
    }

    # Save list of deleted files
    $deletedFiles | Out-File "deleted-migrations.txt"
    Write-Host "✓ List of deleted files saved to: deleted-migrations.txt" -ForegroundColor Green

} else {
    Write-Host "Operation cancelled" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Step 3: Create New Migrations" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Open Visual Studio and Package Manager Console, then run:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Set Default Project to: Riverbed.Pricing.Paint" -ForegroundColor White
Write-Host ""
Write-Host "2. For Identity/Authentication context:" -ForegroundColor White
Write-Host "   Add-Migration InitialIdentity -Context ApplicationDbContext" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. For Business/Pricing context:" -ForegroundColor White
Write-Host "   Add-Migration InitialPricing -Context PricingDbContext" -ForegroundColor Cyan
Write-Host ""
Write-Host "4. Review the generated migrations carefully!" -ForegroundColor Yellow
Write-Host ""
Write-Host "5. Apply migrations:" -ForegroundColor White
Write-Host "   Update-Database -Context ApplicationDbContext" -ForegroundColor Cyan
Write-Host "   Update-Database -Context PricingDbContext" -ForegroundColor Cyan
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Important Notes:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "• Your database is HOSTED (s26.winhost.com)" -ForegroundColor Yellow
Write-Host "• Make sure you have a backup before Update-Database" -ForegroundColor Yellow
Write-Host "• The migrations might try to recreate existing tables" -ForegroundColor Yellow
Write-Host "• If tables exist, you may need to manually edit migrations" -ForegroundColor Yellow
Write-Host ""

# Pause for user to read
Read-Host "Press Enter to open Visual Studio..."

# Open Visual Studio solution
Start-Process "Riverbed.Quoted.App.sln"
