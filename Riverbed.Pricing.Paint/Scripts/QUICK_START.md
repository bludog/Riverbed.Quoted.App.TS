# Migration Cleanup - Quick Start

## 🚀 Quick Commands (Choose Your Scenario)

### Scenario 1: Database Already Has All Tables (RECOMMENDED for Production)

```powershell
# 1. Delete old migration files
cd "D:\Projects-Riverbed\Riverbed.Quoted.App"
Remove-Item "Riverbed.Pricing.Paint\Data\Migrations\*.cs" -Force

# 2. Create baseline migrations (in Package Manager Console)
Add-Migration InitialIdentity -Context ApplicationDbContext
Add-Migration InitialPricing -Context PricingDbContext

# 3. Mark as applied WITHOUT running (SQL Server Management Studio)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
    (N'<timestamp>_InitialIdentity', N'9.0.0'),
    (N'<timestamp>_InitialPricing', N'9.0.0');
```

### Scenario 2: Fresh Start (Development Only - DROPS DATABASE!)

```powershell
# In Package Manager Console

# 1. Drop existing database
Drop-Database -Context PricingDbContext

# 2. Delete migration files
cd "D:\Projects-Riverbed\Riverbed.Quoted.App"
Remove-Item "Riverbed.Pricing.Paint\Data\Migrations\*.cs" -Force

# 3. Create fresh migrations
Add-Migration InitialIdentity -Context ApplicationDbContext
Add-Migration InitialPricing -Context PricingDbContext

# 4. Apply migrations
Update-Database -Context ApplicationDbContext
Update-Database -Context PricingDbContext
```

---

## 📋 Step-by-Step (Scenario 1 - Recommended)

### Step 1: Backup
```bash
git checkout -b migration-backup-before-cleanup
git add .
git commit -m "Backup before migration cleanup"
git push origin migration-backup-before-cleanup
git checkout RoomImageDispl
```

### Step 2: Delete Old Migrations
```powershell
cd "D:\Projects-Riverbed\Riverbed.Quoted.App\Riverbed.Pricing.Paint"
.\Scripts\ExecuteCleanup.ps1
```

### Step 3: Create New Migrations

**In Visual Studio → Tools → NuGet Package Manager → Package Manager Console:**

```powershell
# Ensure Default Project is: Riverbed.Pricing.Paint

Add-Migration InitialIdentity -Context ApplicationDbContext
Add-Migration InitialPricing -Context PricingDbContext
```

### Step 4: Get Migration IDs

Check the newly created files in `Data/Migrations/`:
- Example: `20260320153045_InitialIdentity.cs`
- The ID is: `20260320153045_InitialIdentity`

### Step 5: Mark as Applied

**Connect to SQL Server:**
- Server: `s26.winhost.com`
- Database: `DB_151509_riverbedutil`
- User: `DB_151509_riverbedutil_user`

**Run SQL:**
```sql
-- View current migrations
SELECT * FROM __EFMigrationsHistory;

-- Clear old migrations (OPTIONAL - be careful!)
-- DELETE FROM __EFMigrationsHistory;

-- Insert new baseline migrations
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES 
    (N'20260320153045_InitialIdentity', N'9.0.0'),
    (N'20260320153046_InitialPricing', N'9.0.0');
-- ↑ Replace with YOUR actual migration IDs!
```

### Step 6: Verify

```sql
-- Should show only your 2 new migrations
SELECT * FROM __EFMigrationsHistory;
```

**In Package Manager Console:**
```powershell
# This should create an EMPTY migration (no changes)
Add-Migration TestVerification -Context PricingDbContext

# If it's empty, delete it
Remove-Migration -Context PricingDbContext
```

### Step 7: Commit
```bash
git add .
git commit -m "Clean up migrations - created new baseline"
git push origin RoomImageDispl
```

---

## 🛠️ Your Database Info

- **Server:** s26.winhost.com
- **Database:** DB_151509_riverbedutil
- **User:** DB_151509_riverbedutil_user
- **Two Contexts:**
  - `ApplicationDbContext` (Identity/Auth)
  - `PricingDbContext` (Business logic)

---

## ⚠️ Important Notes

1. **Your database is PRODUCTION** - be careful!
2. **Always backup first**
3. **Test in development first** if possible
4. The baseline approach doesn't actually modify the database
5. It just tells EF Core "these migrations are already applied"

---

## 🆘 Troubleshooting

### "There is already an object named 'X' in the database"
→ You ran `Update-Database` on an existing DB. Use the baseline approach instead.

### "Migration already exists"
→ You didn't delete all old migration files. Check for `.Designer.cs` files too.

### "Unable to generate migration"
→ Check that your entity models match the database schema.

---

## 📞 Next Steps After Cleanup

1. **Test the application** - make sure everything works
2. **Create your new migration** (e.g., for room images):
   ```powershell
   Add-Migration AddRoomImageSupport -Context PricingDbContext
   Update-Database -Context PricingDbContext
   ```

3. **Set up naming convention:**
   - ✅ Good: `AddRoomImageSupport`, `AddSecondaryContactFields`
   - ❌ Bad: `Update1`, `Changes`, `Fix`

---

## 🎯 Success Criteria

✅ Only 2-3 migration files in `Data/Migrations/`:
   - `*_InitialIdentity.cs` + Designer
   - `*_InitialPricing.cs` + Designer
   - Model snapshots

✅ `__EFMigrationsHistory` shows only 2 entries

✅ Application runs without errors

✅ Can create new migrations successfully
