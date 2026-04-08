using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Services
{
    public interface IPricingService
    {
        // Methods for Company
        Task<List<Company>> GetAllCompaniesAsync();
        Task CreateCompanyAsync(Company dataObj);
        Task UpdateCompanyAsync(Company dataObj);
        Task DeleteCompanyAsync(Guid id);
        Task<Company> GetCompanyAsync(Guid id);
        Task SetCompanyDefaultsAsync(Guid companyGuid);

        // Methods for CompanyCustomer
        Task<List<CompanyCustomer>> GetAllCompanyCustomersAsync();
        Task<List<CompanyCustomer>> GetAllCompanyCustomersByGuidAsync(string guidId);
        Task CreateCompanyCustomerAsync(CompanyCustomer dataObj);
        Task UpdateCompanyCustomerAsync(CompanyCustomer dataObj);
        Task DeleteCompanyCustomerAsync(int id);
        Task<CompanyCustomer> GetCompanyCustomerAsync(string customerId);
        Task<CompanyCustomer> GetCompanyCustomerByIdAsync(int id);

        // Utility: Delete a company customer and all related data (projects, rooms, etc)
        Task DeleteCompanyCustomerAndRelatedDataAsync(Guid customerGuid);

        // Methods for CustomersProject
        Task<List<ProjectData>> GetAllCustomerProjectsAsync(int id);
        Task<ProjectData> CreateCustomerProjectAsync(ProjectData dataObj);
        Task UpdateCustomerProjectAsync(ProjectData dataObj);
        Task DeleteCustomerProjectAsync(int id);
        Task<ProjectData> GetCustomerProjectAsync(string id);
        Task<ProjectData> DuplicateCustomerProjectAsync(string sourceProjectGuid, ProjectData newProjectData);
        Task UpdateProjectBaseAmountAsync(int projectId, decimal baseAmount);

        // Company projects (minimal)
        Task<List<ProjectDataMinimal>> GetCompanyProjectsMinimalAsync(string companyGuid);
        Task<List<ProjectDataMinimal>> GetAcceptedProjectsAsync(string companyGuid);

        // Methods for CompanyDefaults
        Task<List<CompanyDefaults>> GetAllCompanyDefaultsAsync();
        Task CreateCompanyDefaultsAsync(CompanyDefaults dataObj);
        Task UpdateCompanyDefaultsAsync(CompanyDefaults dataObj);
        Task DeleteCompanyDefaultsAsync(int id);
        Task<CompanyDefaults?> GetCompanyDefaultsAsync(string guidId);

        // Methods for CompanySettings
        Task<List<CompanySettings>> GetAllCompanySettingsAsync();
        Task CreateCompanySettingsAsync(CompanySettings dataObj);
        Task UpdateCompanySettingsAsync(CompanySettings dataObj);
        Task DeleteCompanySettingsAsync(int id);
        Task<CompanySettings> GetCompanySettingsAsync(string guidId);

        // Methods for GlobalDefaults
        Task<List<GlobalDefaults>> GetAllGlobalDefaultsAsync();
        Task CreateGlobalDefaultsAsync(GlobalDefaults dataObj);
        Task UpdateGlobalDefaultsAsync(GlobalDefaults dataObj);
        Task DeleteGlobalDefaultsAsync(int id);

        // Methods for DifficultyLevel
        Task<List<DifficultyLevel>> GetAllDifficultyLevelsAsync();
        Task CreateDifficultyLevelAsync(DifficultyLevel dataObj);
        Task UpdateDifficultyLevelAsync(DifficultyLevel dataObj);
        Task DeleteDifficultyLevelAsync(int id);
        Task<DifficultyLevel> GetDifficultyLevelAsync(int id);

        // Methods for ItemPaint
        Task<List<ItemPaint>> GetAllItemPaintsAsync();
        Task CreateItemPaintAsync(ItemPaint dataObj);
        Task UpdateItemPaintAsync(ItemPaint dataObj);
        Task DeleteItemPaintAsync(int id);
        Task<ItemPaint> GetItemPaintAsync(int id);

        // Methods for ItemType
        Task<List<ItemType>> GetAllItemTypesAsync();
        Task CreateItemTypeAsync(ItemType dataObj);
        Task UpdateItemTypeAsync(ItemType dataObj);
        Task DeleteItemTypeAsync(int id);
        Task<ItemType> GetItemTypeAsync(int id);

        // Methods for PaintBrand
        Task<List<PaintBrand>> GetAllPaintBrandsAsync();
        Task CreatePaintBrandAsync(PaintBrand dataObj);
        Task UpdatePaintBrandAsync(PaintBrand dataObj);
        Task DeletePaintBrandAsync(int id);
        Task<PaintBrand> GetPaintBrandAsync(int id);

        // Methods for PaintQuality
        Task<List<PaintQuality>> GetAllPaintQualitiesAsync();
        Task CreatePaintQualityAsync(PaintQuality dataObj);
        Task UpdatePaintQualityAsync(PaintQuality dataObj);
        Task DeletePaintQualityAsync(int id);
        Task<PaintQuality> GetPaintQualityAsync(int id);

        // Methods for PaintSheen
        Task<List<PaintSheen>> GetAllPaintSheensAsync();
        Task CreatePaintSheenAsync(PaintSheen dataObj);
        Task UpdatePaintSheenAsync(PaintSheen dataObj);
        Task DeletePaintSheenAsync(int id);
        Task<PaintSheen> GetPaintSheenAsync(int id);

        // Methods for PaintType
        Task<List<PaintType>> GetAllPaintTypesAsync();
        Task CreatePaintTypeAsync(PaintType dataObj);
        Task UpdatePaintTypeAsync(PaintType dataObj);
        Task DeletePaintTypeAsync(int id);
        Task<PaintType> GetPaintTypeAsync(int id);

        // Methods for CompanyPaintType
        Task<List<CompanyPaintType>> GetAllCompanyPaintTypesAsync(string CompanyGuid);
        Task CreateCompanyPaintTypeAsync(CompanyPaintType dataObj);
        Task UpdateCompanyPaintTypeAsync(CompanyPaintType dataObj);
        Task DeleteCompanyPaintTypeAsync(int id, string CompanyGuid);
        Task<CompanyPaintType> GetCompanyPaintTypeAsync(int id, string CompanyGuid);

        // Methods for PaintTypeName
        Task<List<PaintTypeName>> GetAllPaintTypeNamesAsync();
        Task CreatePaintTypeNameAsync(PaintTypeName dataObj);
        Task UpdatePaintTypeNameAsync(PaintTypeName dataObj);
        Task DeletePaintTypeNameAsync(int id);
        Task<PaintTypeName> GetPaintTypeNameAsync(int id);

        // Methods for PricingType
        Task<List<PricingType>> GetAllPricingTypesAsync();
        Task CreatePricingTypeAsync(PricingType dataObj);
        Task UpdatePricingTypeAsync(PricingType dataObj);
        Task DeletePricingTypeAsync(int id);
        Task<PricingType> GetPricingTypeAsync(int id);

        // Methods for ProjectArea
        Task<List<ProjectArea>> GetAllProjectAreasAsync(int ProjectId);
        Task<ProjectArea> CreateProjectAreaAsync(ProjectArea dataObj);
        Task UpdateProjectAreaAsync(ProjectArea dataObj);
        Task DeleteProjectAreaAsync(int id);
        Task<ProjectArea> GetProjectAreaAsync(int id);

        // Methods for StatusCode
        Task<List<StatusCode>> GetAllStatusCodesAsync();
        Task CreateStatusCodeAsync(StatusCode dataObj);
        Task UpdateStatusCodeAsync(StatusCode dataObj);
        Task DeleteStatusCodeAsync(int id);
        Task<StatusCode> GetStatusCodeAsync(int id);

        // Methods for Room
        Task<List<Room>> GetRoomsByProjectIdAsync(int projectId);
        Task<Room> CreateRoomAsync(Room dataObj);
        Task<Room> UpdateRoomAsync(Room dataObj);
        Task DeleteRoomAsync(int id);
        Task<Room> GetRoomAsync(int id);
        Task<RoomGlobalDefaults> GetRoomGlobalDefaultsAsync();

        // Methods for SurfaceTypeLookup
        Task<List<SurfaceTypeLookup>> GetSurfaceTypeLookupsAsync();
        Task<SurfaceTypeLookup> GetSurfaceTypeLookupAsync(byte id);
        Task CreateSurfaceTypeLookupAsync(SurfaceTypeLookup dataObj);
        Task UpdateSurfaceTypeLookupAsync(SurfaceTypeLookup dataObj);
        Task DeleteSurfaceTypeLookupAsync(byte id);

        // Methods for RoomSurface
        Task<List<RoomSurface>> GetAllRoomSurfacesAsync();
        Task<RoomSurface> GetRoomSurfaceAsync(int id);
        Task CreateRoomSurfaceAsync(RoomSurface dataObj);
        Task UpdateRoomSurfaceAsync(RoomSurface dataObj);
        Task DeleteRoomSurfaceAsync(int id);

        // Methods for RoomSurfacePaintLayer
        Task<List<RoomSurfacePaintLayer>> GetAllRoomSurfacePaintLayersAsync();
        Task<RoomSurfacePaintLayer> GetRoomSurfacePaintLayerAsync(int id);
        Task CreateRoomSurfacePaintLayerAsync(RoomSurfacePaintLayer dataObj);
        Task UpdateRoomSurfacePaintLayerAsync(RoomSurfacePaintLayer dataObj);
        Task DeleteRoomSurfacePaintLayerAsync(int id);

        // Methods for Global room settings (inclusions across project)
        Task<int> UpdateRoomInclusionsAsync(int projectId, RoomInclusionsRequest request);
        Task<int> UpdateRoomPaintQualityAsync(int projectId, int paintQualityId);
        // Methods for PricingRequestInterior
        Task<PricingResponseInterior> PriceHouseInteriorAsync(int projectId);
        Task<PricingResponseInterior> PriceRoomInteriorAsync(int roomId);

        // Reporting Services
        Task<byte[]> GetInteriorQuoteAsPDFAsync(string projectGuid);

        // Reporting: Get all company projects
        Task<List<CompanyProjectsDto>> GetAllCompanyProjectsAsync();

        // Company PricingInteriorDefaults
        Task<List<PricingInteriorDefault>> GetAllPricingInteriorDefaultsAsync();
        Task CreatePricingInteriorDefaultAsync(PricingInteriorDefault dataObj);
        Task<PricingInteriorDefault> GetPricingInteriorDefaultAsync(string guidId);
        Task UpdatePricingInteriorDefaultAsync(PricingInteriorDefault dataObj);
        Task DeletePricingInteriorDefaultAsync(string guidId);

        // Project Adjustments
        // Task<Adjustment> GetAdjustmentByIdAsync(string id);
        Task<List<Adjustment>> GetAdjustmentsByProjectIdAsync(int projectId);
        Task<Adjustment> CreateAdjustmentAsync(Adjustment dataObj);
        Task UpdateAdjustmentAsync(int adjustmentId, Adjustment dataObj);
        Task DeleteAdjustmentAsync(string id);

        // Methods for PaintableItem
        Task<List<PaintableItem>> GetAllPaintableItemsAsync(int roomId);
        Task<PaintableItem> GetPaintableItemAsync(int id);
        Task<PaintableItem> CreatePaintableItemAsync(PaintableItem dataObj);
        Task UpdatePaintableItemAsync(PaintableItem dataObj);
        Task DeletePaintableItemAsync(int id);

        // Methods for CompanyPaintableItem
        Task<List<CompanyPaintableItem>> GetAllCompanyPaintableItemsAsync(string CompanyGuid);
        Task<List<CompanyPaintableItemSummary>> GetAllCompanyPaintableItemSummariesAsync(string CompanyGuid);
        Task<CompanyPaintableItem> GetCompanyPaintableItemAsync(int id, string CompanyGuid);
        Task CreateCompanyPaintableItemAsync(CompanyPaintableItem dataObj);
        Task UpdateCompanyPaintableItemAsync(CompanyPaintableItem dataObj);
        Task DeleteCompanyPaintableItemAsync(int id, string CompanyGuid);

        // Methods for PaintableItemCategories
        Task<List<PaintableItemCategory>> GetAllPaintableItemCategoriesAsync();
        Task<PaintableItemCategory> GetPaintableItemCategoryAsync(int id);
        Task CreatePaintableItemCategoryAsync(PaintableItemCategory dataObj);
        Task UpdatePaintableItemCategoryAsync(PaintableItemCategory dataObj);
        Task DeletePaintableItemCategoryAsync(int id);

        // Methods for UserDtos
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ChangePasswordAsync(string userId, string newPassword);
        Task<List<string>> GetAvailableRolesAsync();
        Task<List<CompanyDto>> GetCompaniesAsync();
        Task<bool> UpdateUserAsync(UserDto user);
        Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<bool> UpdateUserCompanyAsync(string userId, string companyId);

        // Methods for Address Suggestions
        Task<List<AddressSuggestionDto>> GetAddressSuggestionsAsync(string input);
        Task<AddressDetailsDto> GetAddressDetailsAsync(string placeId);

        // User Company Information
        Task<string> GetUserCompanyGuidAsync(string email);
        Task<List<string>> GetUserRolesAsync(string email); // Added method for user roles

        // PDF Quote Generation
        Task<byte[]> GetProjectQuoteAsPDFAsync(string projectId);

        // Methods for CompanyHTMLReport
        Task<List<CompanyHTMLReportTemplate>> GetAllCompanyHTMLReportTemplatesAsync();
        Task<List<CompanyHTMLReportTemplate>> GetCompanyHTMLReportTemplatesByCompanyAsync(string companyGuid);
        Task<CompanyHTMLReportTemplate> GetCompanyHTMLReportTemplateByTypeAndCompanyAsync(int reportTypeId, string companyGuid);
        Task<CompanyHTMLReportTemplate> GetCompanyHTMLReportTemplateAsync(int id);
        Task CreateCompanyHTMLReportTemplateAsync(CompanyHTMLReportTemplate report);
        Task UpdateCompanyHTMLReportTemplateAsync(CompanyHTMLReportTemplate report);
        Task DeleteCompanyHTMLReportTemplateAsync(int id);
        Task<List<CompanyHTMLReportsTemplateMinimal>> GetMinimalTemplateInfoAsync(string companyGuid);
        Task<List<CompanyHTMLReportsTemplateMinimal>> GetActiveMinimalTemplateInfoAsync(string companyGuid, bool GlobalOnly = true);

        // Methods for CompanyHTMLReport
        Task<List<CompanyHTMLReport>> GetCompanyHTMLReportsByProjectAsync(string projectGuid);
        Task<CompanyHTMLReport> GetCompanyHTMLReportByTypeAndProjectAsync(int reportTypeId, string projectGuid);
        Task<CompanyHTMLReport> GetCompanyHTMLReportAsync(int id);
        Task CreateCompanyHTMLReportAsync(CompanyHTMLReport report);
        Task UpdateCompanyHTMLReportAsync(CompanyHTMLReport report);
        Task DeleteCompanyHTMLReportAsync(int id);
        Task DeleteAllCompanyHTMLReportsByProjectAsync(string projectGuid);

        // Methods for CompanyReportType
        Task<List<CompanyReportType>> GetAllCompanyReportTypesAsync();
        Task<CompanyReportType> GetCompanyReportTypeAsync(int id);
        Task CreateCompanyReportTypeAsync(CompanyReportType reportType);
        Task UpdateCompanyReportTypeAsync(CompanyReportType reportType);
        Task DeleteCompanyReportTypeAsync(int id);

        // ServiceTitan Connection Data
        // Methods for ServiceTitanConnectionData
        Task<List<ServiceTitanConnectionData>> GetAllServiceTitanConnectionDatasAsync();
        Task<ServiceTitanConnectionData?> CreateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj);
        Task<ServiceTitanConnectionData> UpdateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj);
        Task<ServiceTitanConnectionData> DeleteServiceTitanConnectionDataAsync(int id);
        Task<ServiceTitanConnectionData?> GetServiceTitanConnectionDataAsync(string companyGuid);

        // Email Service
        Task<string> SendEmailAsync(string toAddress, string subject, string body, string senderName = "", string senderEmail = "", string ccEmail = "", bool isHtml = true, string projectGuid = "", string companyGuid = "");
        Task<string> SendProjectEmailAsync(string projectGuid, string subject, string body, string toAddress = "", string senderName = "", string senderEmail = "", string ccEmail = "", string bccEmail = "", bool isHtml = true, int? companyReportTypeId = null, string companyGuid = "");
        Task<List<EmailLogDto>> GetProjectEmailLogsAsync(string projectGuid);

        // Copy CompanyHTMLReportTemplates to project (ReportTypeId <= 6)
        Task<List<CompanyHTMLReport>> CopyCompanyHTMLReportTemplatesToProjectAsync(string companyGuid, string projectGuid);

        // Images
        Task<List<ImageAsset>> GetAllImagesAsync();
        Task<ImageAsset> GetImageAsync(Guid id);
        Task<byte[]> GetImageContentAsync(Guid id);
        Task<string> UploadImageAsync(byte[] data, string fileName, string contentType, string? imageName = null);
        Task UpdateImageAsync(ImageAsset image);
        Task DeleteImageAsync(Guid id);

        // Template merge
        Task<string> MergeHtmlAsync(string companyGuid, string projectGuid, string html);

        /// <summary>
        /// Copies all global CompanyHTMLReportTemplates to the specified company.
        /// </summary>
        /// <param name="companyGuid">The company Guid as a string.</param>
        /// <returns>Task</returns>
        Task CopyGlobalCompanyHTMLReportTemplatesToCompanyAsync(string companyGuid);

        /// <summary>
        /// Sets the company interior pricing for the specified company.
        /// </summary>
        /// <param name="companyGuid">The company Guid as a string.</param>
        /// <returns>Task</returns>
        Task SetCompanyInteriorPricingAsync(string companyGuid);

        // Dashboard
        /// <summary>
        /// Gets dashboard statistics and recent projects for a company
        /// </summary>
        /// <param name="companyGuid">The company Guid as a string.</param>
        /// <returns>Dashboard data including statistics and project lists</returns>
        Task<DashboardData> GetDashboardDataAsync(string companyGuid);
    }
}
