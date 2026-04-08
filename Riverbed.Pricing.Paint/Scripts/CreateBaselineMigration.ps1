# ========================================
# Create Baseline Migration for Existing Database
# Use this when your database already has all tables
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Baseline Migration Creator" -ForegroundColor Cyan
Write-Host "For Existing Database with Tables" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will:" -ForegroundColor Yellow
Write-Host "1. Create initial migrations that match your existing database" -ForegroundColor White
Write-Host "2. Mark them as applied without actually running them" -ForegroundColor White
Write-Host "3. Set up a clean baseline for future migrations" -ForegroundColor White
Write-Host ""

Write-Host "Prerequisites:" -ForegroundColor Yellow
Write-Host "• Your database already has all tables" -ForegroundColor White
Write-Host "• You've deleted all old migration files" -ForegroundColor White
Write-Host "• Your entity models match the database schema" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Continue? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Cancelled" -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Visual Studio Commands" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Open Package Manager Console in Visual Studio and run these commands:" -ForegroundColor Yellow
Write-Host ""

Write-Host "# Set default project" -ForegroundColor Gray
Write-Host "Default project: Riverbed.Pricing.Paint" -ForegroundColor Cyan
Write-Host ""

Write-Host "# Create baseline migrations" -ForegroundColor Gray
Write-Host "Add-Migration InitialIdentity -Context ApplicationDbContext" -ForegroundColor Cyan
Write-Host "Add-Migration InitialPricing -Context PricingDbContext" -ForegroundColor Cyan
Write-Host ""

Write-Host "# Review the generated Up() method - it should match your database" -ForegroundColor Gray
Write-Host ""

Write-Host "# Now we need to mark these as applied WITHOUT running them" -ForegroundColor Yellow
Write-Host "# Since the database already has these tables" -ForegroundColor Yellow
Write-Host ""

Write-Host "# Open SQL Server Management Studio and connect to:" -ForegroundColor Gray
Write-Host "Server: s26.winhost.com" -ForegroundColor Cyan
Write-Host "Database: DB_151509_riverbedutil" -ForegroundColor Cyan
Write-Host ""

Write-Host "# Run this SQL to add migration records:" -ForegroundColor Gray
Write-Host @"
-- Check if __EFMigrationsHistory exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END

-- Clear old migration history (CAREFUL!)
-- DELETE FROM [__EFMigrationsHistory];

-- Insert the new baseline migrations
-- Replace the MigrationId with the actual timestamp from your generated migration file names

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
    (N'<YourInitialIdentityMigrationId>_InitialIdentity', N'9.0.0'),
    (N'<YourInitialPricingMigrationId>_InitialPricing', N'9.0.0');

-- Example:
-- INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
-- VALUES 
--     (N'20260320120000_InitialIdentity', N'9.0.0'),
--     (N'20260320120001_InitialPricing', N'9.0.0');
"@ -ForegroundColor Cyan

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Alternative: PowerShell Method" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If you prefer, you can use this PowerShell command instead:" -ForegroundColor Yellow
Write-Host ""

$sqlCommand = @"
# In Package Manager Console (after creating migrations):

# Get the migration IDs from the generated file names
`$identityMigrationId = '<timestamp>_InitialIdentity'
`$pricingMigrationId = '<timestamp>_InitialPricing'

# Execute SQL to mark as applied
`$sql = @"
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
    (N'`$identityMigrationId', N'9.0.0'),
    (N'`$pricingMigrationId', N'9.0.0');
"@

# This would need to be run against your database
"@

Write-Host $sqlCommand -ForegroundColor Cyan

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Verification" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "After completing the above:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Check migration history:" -ForegroundColor White
Write-Host "   SELECT * FROM __EFMigrationsHistory" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Try creating a new migration:" -ForegroundColor White
Write-Host "   Add-Migration TestMigration -Context PricingDbContext" -ForegroundColor Cyan
Write-Host "   (Should be empty or minimal changes)" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Remove the test migration:" -ForegroundColor White
Write-Host "   Remove-Migration -Context PricingDbContext" -ForegroundColor Cyan
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Done!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You now have a clean baseline for future migrations." -ForegroundColor Green
Write-Host ""

Read-Host "Press Enter to exit"
