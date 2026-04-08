# Riverbed Quoted App - Architecture Diagram

## 🏗️ Solution Architecture Overview

This is a **Blazor Web App** (.NET 9) using the **Interactive WebAssembly** render mode with a multi-project architecture.

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         SOLUTION STRUCTURE                               │
│                                                                          │
│  ┌────────────────────┐  ┌─────────────────────┐  ┌──────────────────┐ │
│  │ Riverbed.Pricing.  │  │ Riverbed.Pricing.   │  │ Riverbed.Pricing.│ │
│  │ Paint              │  │ Paint.Client        │  │ Paint.Shared     │ │
│  │ (Server/Backend)   │  │ (WebAssembly/       │  │ (Shared Library) │ │
│  │                    │  │  Frontend)          │  │                  │ │
│  └────────────────────┘  └─────────────────────┘  └──────────────────┘ │
│                                                                          │
│  ┌────────────────────┐                                                 │
│  │ Riverbed.Pricing.  │                                                 │
│  │ Paint.Reports      │                                                 │
│  │ (DevExpress        │                                                 │
│  │  Reports)          │                                                 │
│  └────────────────────┘                                                 │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Full Architecture Flow

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER (WebAssembly)                       │
│  Riverbed.Pricing.Paint.Client                                               │
│                                                                               │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  📄 BLAZOR PAGES (@page routes)                                     │    │
│  │                                                                      │    │
│  │  • /dashboard → Dashboard.razor                                     │    │
│  │  • /project/quoted-projects → QuotedProjects.razor                  │    │
│  │  • /company-customer-list → CompanyCustomerList.razor               │    │
│  │  • /pricing/projectentrymain/{guid} → ProjectEntryMain.razor        │    │
│  │  • /company-defaults → CompanyDefaultsComponent.razor                │    │
│  │  • /company-settings → CompanySettingsPage.razor                    │    │
│  │  • /company-admin → CompanyAdmin.razor                              │    │
│  │  • /company-reports → CompanyHTMLReportPage.razor                   │    │
│  │  • /email-editor → EmailEditorPage.razor                            │    │
│  │                                                                      │    │
│  │  Component Hierarchy:                                                │    │
│  │  Pages/ ──┬── Project/ (QuotedProjects, ProjectEntryMain, etc.)     │    │
│  │           ├── Components/Company/ (CompanyAdmin, CompanyHome, etc.) │    │
│  │           ├── Components/EditForms/ (CompanyEditForm, etc.)         │    │
│  │           ├── Components/EmailEditor/                               │    │
│  │           ├── Components/Reports/                                   │    │
│  │           └── Utils/ (AllUserManagementComponent, etc.)             │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                          │                                    │
│                                          │ @inject IPricingService            │
│                                          ▼                                    │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  🔌 CLIENT SERVICE LAYER                                            │    │
│  │                                                                      │    │
│  │  IPricingService (Interface)                                        │    │
│  │  └─→ PricingService (Singleton, HttpClient wrapper)                 │    │
│  │                                                                      │    │
│  │  Helper Services:                                                   │    │
│  │  • FileSaveHelper                                                   │    │
│  │  • ThemeService                                                     │    │
│  │  • PersistentAuthenticationStateProvider                            │    │
│  │                                                                      │    │
│  │  UI Component Libraries:                                            │    │
│  │  • Blazorise (Bootstrap5)                                           │    │
│  │  • MudBlazor                                                        │    │
│  │  • Radzen (Dialog, Notification, Tooltip, ContextMenu)             │    │
│  │  • DevExpress Blazor Components                                    │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
└───────────────────────────────────────┬──────────────────────────────────────┘
                                        │
                                        │ HTTP/JSON Requests
                                        │ (https://localhost:7027/api/*)
                                        ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│                          SERVER LAYER (ASP.NET Core)                          │
│  Riverbed.Pricing.Paint                                                      │
│                                                                               │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  🌐 API CONTROLLERS (/api/[controller])                             │    │
│  │                                                                      │    │
│  │  CORE BUSINESS:                                                     │    │
│  │  • CompaniesController          → /api/Companies                   │    │
│  │  • CompanyCustomersController   → /api/CompanyCustomers            │    │
│  │  • CustomerProjectsController   → /api/Project/CustomerProjects    │    │
│  │  • CompanyDefaultsController    → /api/CompanyDefaults             │    │
│  │  • CompanySettingsController    → /api/CompanySettings             │    │
│  │                                                                      │    │
│  │  PROJECT & ROOMS:                                                   │    │
│  │  • ProjectAreasController       → /api/ProjectAreas                │    │
│  │  • RoomsController              → /api/Rooms                       │    │
│  │  • RoomSurfacesController       → /api/Surfaces/RoomSurfaces       │    │
│  │  • RoomSurfacePaintLayersController → /api/Surfaces/RoomSurface... │    │
│  │  • GlobalRoomSettingsController → /api/Project/GlobalRoomSettings  │    │
│  │  • RoomGlobalDefaultsController → /api/RoomGlobalDefaults          │    │
│  │                                                                      │    │
│  │  PAINT PRICING:                                                     │    │
│  │  • PaintPricingEngineController → Paint pricing calculations       │    │
│  │  • PricingRequestInteriorsController → /api/Paint/PricingRequest...│    │
│  │  • PricingResponseInteriorsController → /api/Paint/PricingResponse.│    │
│  │  • PricingInteriorDefaultsController → /api/Paint/PricingInterior..│    │
│  │  • PricingRequestExteriorsController                               │    │
│  │  • PricingResponseExteriorsController                              │    │
│  │                                                                      │    │
│  │  PAINTABLE ITEMS & INVENTORY:                                       │    │
│  │  • PaintableItemsController     → /api/PaintableItems              │    │
│  │  • CompanyPaintableItemsController → /api/CompanyPaintableItems    │    │
│  │  • PaintableItemCategoriesController → /api/PaintableItemCategories│    │
│  │  • AreaItemsController          → /api/AreaItems                   │    │
│  │  • ItemTypesController          → /api/ItemTypes                   │    │
│  │  • ItemPaintsController         → /api/ItemPaints                  │    │
│  │                                                                      │    │
│  │  PAINT CATALOG:                                                     │    │
│  │  • PaintTypesController         → /api/PaintTypes                  │    │
│  │  • CompanyPaintTypesController  → /api/CompanyPaintTypes           │    │
│  │  • PaintBrandsController        → /api/PaintBrands                 │    │
│  │  • PaintQualitiesController     → /api/PaintQualities              │    │
│  │  • PaintSheensController        → /api/PaintSheens                 │    │
│  │                                                                      │    │
│  │  REPORTING:                                                         │    │
│  │  • ReportController             → /api/Reporting/Report            │    │
│  │  • ReportDataController         → /api/Reporting/ReportData        │    │
│  │  • ReportDownloadController     → /api/Reporting/ReportDownload    │    │
│  │  • CompanyReportTypesController → /api/CompanyReportTypes          │    │
│  │  • CompanyHTMLReportController  → /api/Reporting/CompanyHTMLReport │    │
│  │  • CompanyHTMLReportTemplateController → Templates                │    │
│  │  • QuoteHtmlGenerator           → HTML generation utility         │    │
│  │  • QuotePdfController           → /api/QuotePdf                    │    │
│  │                                                                      │    │
│  │  UTILITIES:                                                         │    │
│  │  • DashboardController          → /api/Utils/Dashboard             │    │
│  │  • EmailServiceController       → /api/Utils/EmailService          │    │
│  │  • FileUploadController         → /api/Utils/FileUpload            │    │
│  │  • GooglePlacesController       → /api/Utils/GooglePlaces          │    │
│  │  • RoomImageController          → /api/Utils/RoomImage             │    │
│  │  • ImagesController             → /api/Images                      │    │
│  │  • ProxyController              → /api/Proxy                       │    │
│  │                                                                      │    │
│  │  CONFIGURATION:                                                     │    │
│  │  • GlobalDefaultsController     → /api/GlobalDefaults              │    │
│  │  • StatusCodesController        → /api/StatusCodes                 │    │
│  │  • DifficultyLevelsController   → /api/DifficultyLevels            │    │
│  │  • PricingTypesController       → /api/PricingTypes                │    │
│  │  • SurfaceTypeLookupsController → /api/Surfaces/SurfaceTypeLookups │    │
│  │  • AdjustmentsController        → /api/Adjustments                 │    │
│  │                                                                      │    │
│  │  INTEGRATIONS:                                                      │    │
│  │  • ServiceTitanConnectionDatasController                           │    │
│  │                                                                      │    │
│  │  ADMIN:                                                             │    │
│  │  • UsersController              → /api/Users (User management)     │    │
│  │  • UserInfoController           → /api/UserInfo                    │    │
│  │  • AspNetRolesController        → /api/AspNetRoles                 │    │
│  │  • CompanyProvisioningController → Company setup                   │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                          │                                    │
│                                          │ Entity Framework Core               │
│                                          ▼                                    │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  💾 DATA ACCESS LAYER                                               │    │
│  │                                                                      │    │
│  │  • PricingDbContext (EF Core DbContext)                             │    │
│  │  • ApplicationDbContext (Identity)                                  │    │
│  │                                                                      │    │
│  │  Business Logic:                                                    │    │
│  │  • PaintPricingEngine (Pricing calculations)                        │    │
│  │  • CompanyDefaultUtility (Company setup)                            │    │
│  │  • TemplateMergeUtility (Report templates)                          │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                          │                                    │
│                                          ▼                                    │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │  🔐 IDENTITY & AUTH                                                 │    │
│  │                                                                      │    │
│  │  • ASP.NET Core Identity (User/Role management)                     │    │
│  │  • IdentityUserAccessor                                             │    │
│  │  • PersistingRevalidatingAuthenticationStateProvider               │    │
│  │  • IdentityRedirectManager                                          │    │
│  │                                                                      │    │
│  │  Account Pages (Razor):                                             │    │
│  │  • /Account/Login, /Account/Register                                │    │
│  │  • /Account/Logout, /Account/Manage/*                               │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
└───────────────────────────────────────┬──────────────────────────────────────┘
                                        │
                                        │ SQL Queries
                                        ▼
                            ┌──────────────────────┐
                            │   SQL SERVER         │
                            │   DATABASE           │
                            │                      │
                            │  • Companies         │
                            │  • CompanyCustomers  │
                            │  • Projects          │
                            │  • Rooms             │
                            │  • RoomSurfaces      │
                            │  • PaintableItems    │
                            │  • etc...            │
                            └──────────────────────┘
```

---

## 📦 Shared Library Layer

```
┌────────────────────────────────────────────────────────────────────┐
│              RIVERBED.PRICING.PAINT.SHARED                          │
│                                                                     │
│  📁 Entities/ (Domain Models - shared across client & server)      │
│  ├── Company.cs                                                    │
│  ├── CompanyCustomer.cs                                            │
│  ├── ProjectData.cs                                                │
│  ├── Room.cs                                                       │
│  ├── RoomSurface.cs                                                │
│  ├── PaintableItem.cs                                              │
│  ├── CompanyPaintableItem.cs                                       │
│  ├── CompanyDefaults.cs                                            │
│  ├── CompanySettings.cs                                            │
│  ├── PaintType.cs, PaintBrand.cs, PaintQuality.cs, PaintSheen.cs  │
│  ├── StatusCode.cs, DifficultyLevel.cs                             │
│  ├── Paint/PricingRequestInterior.cs                               │
│  ├── Paint/PricingResponseInterior.cs                              │
│  ├── Reporting/CompanyHTMLReport.cs                                │
│  └── StoredProc/ (DB stored procedure DTOs)                        │
│                                                                     │
│  📁 Data/ (DTOs & View Models)                                     │
│  ├── ApplicationUser.cs                                            │
│  ├── ProjectDataMinimal.cs                                         │
│  ├── DashboardData.cs                                              │
│  ├── ProjectBoardItem.cs                                           │
│  ├── EmailLogDto.cs                                                │
│  ├── UserDto.cs                                                    │
│  └── PricingDbContext.cs (Shared DB Context definition)            │
│                                                                     │
│  📁 Services/ (Interface & Implementation)                         │
│  ├── IPricingService.cs (Main service interface)                  │
│  ├── PricingService.cs (HttpClient-based implementation)           │
│  ├── IServiceTitanDataService.cs                                  │
│  ├── ServiceTitanDataService.cs                                   │
│  └── ThemeService.cs                                               │
│                                                                     │
│  📁 PricingEngines/                                                │
│  ├── PaintPricingEngine.cs (Business logic for paint pricing)     │
│  └── PaintPricingEngineEx.cs                                       │
│                                                                     │
│  📁 EmailService/                                                  │
│  ├── IEmailService.cs                                              │
│  └── EmailService.cs                                               │
│                                                                     │
│  📁 Utils/                                                         │
│  └── FileSaveHelper.cs                                             │
│                                                                     │
│  📁 Reporting/                                                     │
│  └── ProjectTemplateTokens.cs                                      │
└────────────────────────────────────────────────────────────────────┘
```

---

## 🔌 Frontend-Backend Communication Pattern

### **Pattern 1: Blazor Page → Service → API → Database**

```
┌──────────────────┐
│ Dashboard.razor  │  @inject IPricingService AppService
│ (Client)         │  
└────────┬─────────┘
         │ await AppService.GetDashboardDataAsync(companyGuid)
         ▼
┌─────────────────────────┐
│ PricingService.cs       │  HttpClient.GetFromJsonAsync<DashboardData>
│ (Shared - Client side)  │  → "api/Utils/Dashboard/{companyGuid}"
└────────┬────────────────┘
         │ HTTP GET Request
         ▼
┌────────────────────────┐
│ DashboardController.cs │  [HttpGet("{companyGuid}")]
│ (Server)               │  public async Task<ActionResult<DashboardData>>
└────────┬───────────────┘
         │ EF Core Query
         ▼
┌────────────────┐
│ PricingDbContext│  _context.Projects.Where(...)
│ (Server)        │  .Include(...)
└────────┬────────┘
         │
         ▼
┌────────────────┐
│ SQL Server DB   │
└─────────────────┘
```

### **Example Flow: QuotedProjects Page**

```
User navigates to /project/quoted-projects
         │
         ▼
┌─────────────────────────┐
│ QuotedProjects.razor    │
│ @rendermode Interactive │
│ WebAssembly             │
└────────┬────────────────┘
         │ OnInitializedAsync()
         │ ├─ await PricingService.GetUserCompanyGuidAsync(email)
         │ └─ await PricingService.GetCompanyProjectsMinimalAsync(companyGuid)
         ▼
┌─────────────────────────┐
│ PricingService.cs       │
│ (HttpClient wrapper)    │
└────────┬────────────────┘
         │ HTTP GET /api/Companies/{companyGuid}/projects/minimal
         ▼
┌─────────────────────────┐
│ CompaniesController.cs  │
│ GetCompanyProjects      │
│ Minimal(companyGuid)    │
└────────┬────────────────┘
         │ EF Core Join: Projects + CompanyCustomers
         │ Filter: WHERE CompanyId = @companyGuid
         │         AND CreatedDate >= @fromDate
         ▼
┌─────────────────────────┐
│ Returns List<Project    │
│ DataMinimal>            │
│ - ProjectName           │
│ - CustomerName          │
│ - CreatedDate           │
│ - StatusCodeId          │
└────────┬────────────────┘
         │ JSON Response
         ▼
┌─────────────────────────┐
│ QuotedProjects.razor    │
│ Groups projects by week │
│ Displays in Cards       │
└─────────────────────────┘
```

---

## 🎨 UI Component Libraries Stack

```
┌─────────────────────────────────────────────────────┐
│              FRONTEND UI LIBRARIES                   │
│                                                      │
│  Primary Components:                                 │
│  • Blazorise (Bootstrap 5)  ← Main UI framework     │
│    └─ Cards, Buttons, Forms, Layout, Icons          │
│                                                      │
│  • MudBlazor                ← Material Design        │
│    └─ DataGrid, Dialogs, Advanced controls          │
│                                                      │
│  • Radzen                   ← Dialogs & Notifications│
│    └─ DialogService, NotificationService            │
│                                                      │
│  • DevExpress Blazor        ← Reports & Rich Editors│
│    └─ Reporting, RichEdit, Advanced grids           │
│                                                      │
│  Supporting Libraries:                               │
│  • Font Awesome Icons                                │
│  • Material Icons                                    │
│  • Blazorise RichTextEdit                            │
│  • Tewr.Blazor.FileReader (File uploads)            │
└─────────────────────────────────────────────────────┘
```

---

## 🗄️ Database Entity Relationships

```
┌────────────┐         ┌──────────────────┐         ┌──────────────┐
│  Company   │1      *│ CompanyCustomer  │1      *│ ProjectData  │
│            ├────────│                  ├────────│              │
│ CompanyGuid│         │ CompanyId (FK)   │         │ CompanyCust..│
└────────────┘         └──────────────────┘         └──────┬───────┘
      │1                                                    │1
      │                                                     │
      │*                                                    │*
┌─────────────────┐                                  ┌──────────────┐
│CompanyDefaults  │                                  │   Room       │
│CompanySettings  │                                  │              │
│CompanyPaintable │                                  │ ProjectGuid  │
│Item             │                                  └──────┬───────┘
└─────────────────┘                                         │1
                                                            │
                                                            │*
                                                     ┌──────────────┐
                                                     │ RoomSurface  │
                                                     │              │
                                                     └──────┬───────┘
                                                            │1
                                                            │
                                                            │*
                                                     ┌──────────────────────┐
                                                     │RoomSurfacePaintLayer │
                                                     └──────────────────────┘
```

---

## 🔐 Authentication & Authorization Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION FLOW                           │
│                                                                  │
│  1. User visits app → redirected to /Account/Login              │
│                                                                  │
│  2. ASP.NET Core Identity authenticates                          │
│     ├─ Email + Password validation                              │
│     └─ Sets authentication cookie                               │
│                                                                  │
│  3. AuthenticationStateProvider propagates to Blazor             │
│     ├─ Server: PersistingRevalidatingAuthenticationState...     │
│     └─ Client: PersistentAuthenticationStateProvider            │
│                                                                  │
│  4. Protected pages check [Authorize] attribute                  │
│                                                                  │
│  5. API calls include authentication headers                     │
│     └─ PricingService forwards cookies/auth headers              │
│                                                                  │
│  User Identity includes:                                         │
│  • Email (ClaimTypes.Email)                                      │
│  • CompanyGuid (custom claim)                                    │
│  • Roles (Admin, User, etc.)                                     │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📊 Key Application Workflows

### **1. Create New Project Workflow**

```
User → Dashboard → "Create Project" button
  ↓
ProjectEntryMain.razor (Client)
  ├─ Customer selection/creation
  ├─ Project details entry
  └─ await PricingService.CreateCustomerProjectAsync(projectData)
      ↓
  CustomerProjectsController.PostCustomerProject (Server)
      ├─ Validates data
      ├─ Saves to DB via EF Core
      └─ Returns created project
          ↓
  Room creation and pricing flows
```

### **2. Generate Quote Workflow**

```
User → Project Page → "Generate Quote"
  ↓
PricingRequestInteriorsController
  ├─ Collect room data
  ├─ Calculate pricing via PaintPricingEngine
  └─ Save PricingResponseInterior
      ↓
CompanyHTMLReportController
  ├─ Load template from CompanyHTMLReportTemplate
  ├─ Merge data using TemplateMergeUtility
  └─ Generate HTML report
      ↓
QuotePdfController (optional)
  └─ Convert HTML to PDF for download
```

### **3. Dashboard Data Loading**

```
Dashboard.razor loads:
  ├─ GetDashboardDataAsync(companyGuid)
  │   └─ Returns aggregated statistics:
  │       • Completed jobs (year/month)
  │       • Won quotes (year/month)
  │       • Active quotes (year/month)
  │       • Total amounts
  │
  └─ DashboardController executes:
      ├─ Complex EF queries with grouping
      ├─ Status-based filtering (Completed = 6, Accepted = 2)
      └─ Date range filtering
```

---

## 🛠️ Technology Stack Summary

| Layer | Technologies |
|-------|-------------|
| **Frontend** | Blazor WebAssembly (.NET 9), Blazorise, MudBlazor, Radzen, DevExpress |
| **Backend** | ASP.NET Core Web API (.NET 9), Entity Framework Core |
| **Authentication** | ASP.NET Core Identity |
| **Database** | SQL Server (via EF Core migrations) |
| **Reporting** | DevExpress Reporting, HTML templates |
| **Logging** | Serilog |
| **Email** | Custom EmailService |
| **External Integration** | ServiceTitan API |

---

## 📁 File Organization Patterns

### **Client Project Structure**
```
Riverbed.Pricing.Paint.Client/
├── Pages/
│   ├── Project/          (QuotedProjects, ProjectEntryMain)
│   ├── Components/
│   │   ├── Company/      (Admin, Settings, Defaults)
│   │   ├── EditForms/    (Reusable forms)
│   │   ├── EmailEditor/  (Email management)
│   │   └── Reports/      (Report viewers/editors)
│   └── Utils/            (Utilities, downloads)
├── Program.cs            (DI configuration)
└── wwwroot/              (Static assets)
```

### **Server Project Structure**
```
Riverbed.Pricing.Paint/
├── Controllers/
│   ├── Paint/            (Pricing controllers)
│   ├── Project/          (Project management)
│   ├── Reporting/        (Report generation)
│   ├── Surfaces/         (Surface/paint layers)
│   └── Utils/            (Helpers, dashboard)
├── Components/
│   ├── Account/          (Identity pages)
│   └── Pages/            (Server-rendered pages)
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── PricingDbContext.cs
│   └── Migrations/
├── Reports/              (DevExpress report definitions)
└── Program.cs            (Startup & DI)
```

---

## 🔄 Data Flow Example: Editing Company Defaults

```
┌──────────────────────────┐
│ User navigates to        │
│ /company-defaults        │
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ CompanyDefaultsComponent.razor   │
│ (Client - WebAssembly)           │
│                                  │
│ OnInitializedAsync:              │
│ • Get user company GUID          │
│ • await AppService.Get           │
│   CompanyDefaultsAsync(guid)     │
└────────┬─────────────────────────┘
         │ HTTP GET /api/CompanyDefaults/{guid}
         ▼
┌──────────────────────────────────┐
│ CompanyDefaultsController        │
│ (Server)                         │
│                                  │
│ [HttpGet("{guidId}")]            │
│ • Query _context.CompanyDefaults │
│ • .FirstOrDefaultAsync(...)      │
│ • Return JSON                    │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ CompanyDefaultsEditForm.razor    │
│ (Client)                         │
│                                  │
│ User edits:                      │
│ • Labor rate                     │
│ • Overhead percentage            │
│ • Profit margin                  │
│                                  │
│ OnSave:                          │
│ • await AppService.Update        │
│   CompanyDefaultsAsync(defaults) │
└────────┬─────────────────────────┘
         │ HTTP PUT /api/CompanyDefaults/{id}
         ▼
┌──────────────────────────────────┐
│ CompanyDefaultsController        │
│ (Server)                         │
│                                  │
│ [HttpPut("{id}")]                │
│ • Validate ModelState            │
│ • _context.Entry(defaults).State │
│   = EntityState.Modified         │
│ • await _context.SaveChangesAsync│
│ • Return NoContent()             │
└────────┬─────────────────────────┘
         │ Success response
         ▼
┌──────────────────────────────────┐
│ CompanyDefaultsComponent shows   │
│ success notification             │
│ (via Radzen NotificationService) │
└──────────────────────────────────┘
```

---

## 🎯 Key Integration Points

1. **PricingService** - Central hub for all client-server communication
   - Singleton in Client (WebAssembly)
   - Scoped in Server
   - Uses HttpClient with base address configuration

2. **Shared Entity Models** - Single source of truth
   - Defined in Riverbed.Pricing.Paint.Shared
   - Used by both client and server
   - Ensures type safety across boundaries

3. **Authentication State Synchronization**
   - Server sets auth cookie
   - Client reads via PersistentAuthenticationStateProvider
   - All API calls authenticated via cookies

4. **Render Modes**
   - Pages: `@rendermode InteractiveWebAssembly`
   - Full client-side execution after initial load
   - No server circuit dependency

---

## 📝 Notes

- **Blazor Render Mode**: Interactive WebAssembly (no server prerendering for data pages)
- **API Pattern**: RESTful conventions with `/api/[controller]` routing
- **Service Pattern**: Interface-based dependency injection for testability
- **Data Access**: Repository pattern via EF Core DbContext
- **Migrations**: EF Core Code-First with extensive migration history
- **Logging**: Structured logging with Serilog, suppresses verbose EF logs
- **File Structure**: Clean separation of concerns (Pages, Components, Controllers, Services)

---

**Generated:** 2025
**Architecture Type:** Blazor Web App with WebAssembly rendering + ASP.NET Core Web API backend
**Database:** SQL Server (via Entity Framework Core)
