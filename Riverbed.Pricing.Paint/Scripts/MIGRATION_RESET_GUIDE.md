# EF Core Migrations Reset Guide

## Overview
This guide helps you clean up all existing EF Core migrations and create a fresh starting point.

## ⚠️ IMPORTANT: Before You Begin

### 1. **Backup Your Database**
Since you're using a production hosted database (`s26.winhost.com`), you MUST create a backup first.

**Option A: Using SQL Server Management Studio (SSMS)**
1. Connect to: `s26.winhost.com`
2. Right-click database `DB_151509_riverbedutil`
3. Tasks → Back Up...
4. Choose backup location and click OK

**Option B: Using Entity Framework (Generate SQL Script)**
```powershell
# In Package Manager Console
Script-Migration -From 0 -To <latest-migration> -Output "database-backup.sql"
```

### 2. **Commit Current Code**
```bash
git add .
git commit -m "Pre-migration cleanup commit"
git push origin RoomImageDispl
```

---

## Migration Cleanup Process

### Method 1: Using PowerShell Script (Recommended)

1. **Run the cleanup script**
   ```powershell
   cd D:\Projects-Riverbed\Riverbed.Quoted.App\Riverbed.Pricing.Paint
   .\Scripts\CleanupMigrations.ps1
   ```

2. **Verify cleanup**
   - Check that `Data/Migrations` folder is empty or only contains directories

### Method 2: Manual Cleanup

1. **Delete migration files manually**
   - Navigate to: `Riverbed.Pricing.Paint\Data\Migrations\`
   - Delete ALL `.cs` files
   - Delete ALL `.Designer.cs` files
   - Keep the `Migrations` folder itself

2. **Or use this command**
   ```powershell
   Remove-Item "D:\Projects-Riverbed\Riverbed.Quoted.App\Riverbed.Pricing.Paint\Data\Migrations\*.cs" -Force
   ```

---

## Creating Fresh Migrations

### Option A: Keep Existing Database (Safer for Production)

This approach creates migrations that match your existing database schema:

1. **Remove migrations from EF Core history**
   ```powershell
   # In Package Manager Console
   # Set startup project: Riverbed.Pricing.Paint

   # Clear the __EFMigrationsHistory table (if you want to start fresh)
   # WARNING: Only do this if you're sure!
   ```

2. **Create new initial migration**
   ```powershell
   Add-Migration InitialCreate -Context PricingDbContext
   ```

3. **Review the generated migration**
   - Open the new migration file
   - It should match your current database schema

4. **Apply migration (if needed)**
   ```powershell
   Update-Database -Context PricingDbContext
   ```

### Option B: Fresh Database (Development Only)

⚠️ **WARNING: This DROPS the entire database!**

1. **Drop the database**
   ```powershell
   Drop-Database -Context PricingDbContext
   ```

2. **Create new initial migration**
   ```powershell
   Add-Migration InitialCreate -Context PricingDbContext
   ```

3. **Apply migration**
   ```powershell
   Update-Database -Context PricingDbContext
   ```

---

## Verification Steps

### 1. Check Migration Files
```powershell
Get-ChildItem "D:\Projects-Riverbed\Riverbed.Quoted.App\Riverbed.Pricing.Paint\Data\Migrations"
```
You should see:
- One `*_InitialCreate.cs` file
- One `*_InitialCreate.Designer.cs` file
- One `PricingDbContextModelSnapshot.cs` file
- Possibly `ApplicationDbContextModelSnapshot.cs` if you have multiple contexts

### 2. Verify Database
1. Check `__EFMigrationsHistory` table
   ```sql
   SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC
   ```
   Should show only the new `InitialCreate` migration

2. Verify all tables exist
3. Check that your application still runs correctly

### 3. Test the Application
1. Run the application: `dotnet run --project Riverbed.Pricing.Paint`
2. Test basic CRUD operations
3. Check that existing data is intact

---

## Multiple DbContext Scenario

If you have multiple DbContexts (e.g., `ApplicationDbContext` for Identity and `PricingDbContext` for business logic):

### Clean Both Contexts Separately

1. **For ApplicationDbContext (Identity)**
   ```powershell
   Add-Migration InitialIdentityCreate -Context ApplicationDbContext
   Update-Database -Context ApplicationDbContext
   ```

2. **For PricingDbContext (Business)**
   ```powershell
   Add-Migration InitialPricingCreate -Context PricingDbContext
   Update-Database -Context PricingDbContext
   ```

---

## Troubleshooting

### Issue: "The model backing the context has changed"
**Solution:** Delete the snapshot file and regenerate migrations
```powershell
Remove-Item "Data\Migrations\*ModelSnapshot.cs"
Add-Migration InitialCreate -Context PricingDbContext
```

### Issue: "There is already an object named 'TableName' in the database"
**Solution:** The database already has the tables. Either:
- Use Option A (keep existing database)
- Or drop the database and recreate (Option B)

### Issue: Migration generates too many changes
**Solution:** Your entities don't match the database schema. Review your entity models.

---

## Post-Cleanup Recommendations

### 1. Create a Seed Data Migration
After your initial migration, create seed data:
```powershell
Add-Migration SeedInitialData -Context PricingDbContext
```

### 2. Document Your Schema
Create a `DATABASE.md` file documenting:
- Entity relationships
- Important constraints
- Indexes

### 3. Set Up Migration Naming Convention
Use descriptive names:
- ✅ Good: `Add-Migration AddRoomImageSupport`
- ❌ Bad: `Add-Migration Update1`

### 4. Regular Migration Maintenance
- Combine related changes into single migrations
- Don't create migrations for every small change during development
- Squash migrations periodically (every major release)

---

## Quick Reference Commands

```powershell
# Create migration
Add-Migration <MigrationName> -Context <ContextName>

# Apply migration
Update-Database -Context <ContextName>

# Remove last migration (if not applied)
Remove-Migration -Context <ContextName>

# Rollback to specific migration
Update-Database -Migration <MigrationName> -Context <ContextName>

# Generate SQL script
Script-Migration -From <FromMigration> -To <ToMigration> -Context <ContextName>

# List migrations
Get-Migration -Context <ContextName>
```

---

## Your Specific Setup

Based on your project:
- **Project:** Riverbed.Pricing.Paint
- **Database:** DB_151509_riverbedutil on s26.winhost.com
- **Contexts:** Likely `PricingDbContext` and possibly `ApplicationDbContext`
- **Current Branch:** RoomImageDispl

### Recommended Approach
Use **Option A** (keep existing database) since you're on a hosted production database.

---

## Need Help?
If you encounter issues:
1. Check the error message carefully
2. Verify your DbContext configuration
3. Ensure connection string is correct
4. Check that startup project is set correctly in Package Manager Console
