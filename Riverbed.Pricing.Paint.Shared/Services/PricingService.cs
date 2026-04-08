using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using Riverbed.Pricing.Paint.Shared.Entities.StoredProc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Riverbed.Pricing.Paint.Shared.Services
{
    public class PricingService : IPricingService, IDisposable
    {
        #region Business Service Properties
        HttpClient _client = new HttpClient();
        HttpClient _stClient = new HttpClient();
        private bool disposedValue;
        private readonly ILogger<PricingService> _logger;
        private readonly Func<(IEnumerable<string>? cookies, IEnumerable<string>? authorization)>? _authProvider;

        // Optional client-side authorization evaluator for Users API
        private Func<Task<bool>>? _canAccessUsersApi;
        public void ConfigureUsersApiAuthorization(Func<Task<bool>> evaluator) => _canAccessUsersApi = evaluator;
        private async Task EnsureUsersApiAuthorizedAsync()
        {
            if (_canAccessUsersApi == null) return; // if not configured, skip client-side validation
            var ok = await _canAccessUsersApi.Invoke();
            if (!ok) throw new UnauthorizedAccessException("Not authorized to access users API.");
        }

        public PricingService(ILogger<PricingService> logger)
        {
            _logger = logger;

#if DEBUG
            // Use relative path that works from any sub-application
            _client.BaseAddress = new Uri("https://localhost:7027/");
            _stClient.BaseAddress = new Uri("https://localhost:7027/");
#else
            // Use relative path that works from any sub-application
            _client.BaseAddress = new Uri("https://bludog-software.com/rbp/");
            _stClient.BaseAddress = new Uri("https://bludog-software.com/rbp/");
#endif
            
            _logger.LogInformation($"Base address - {_client.BaseAddress}");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                               new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Bludog Software");
        }

        // Optional constructor to allow server-side injection of auth headers forwarding
        public PricingService(ILogger<PricingService> logger, Func<(IEnumerable<string>? cookies, IEnumerable<string>? authorization)> authProvider) : this(logger)
        {
            _authProvider = authProvider;
            TryForwardAuthenticationHeaders(_client);
            TryForwardAuthenticationHeaders(_stClient);
        }

        private void TryForwardAuthenticationHeaders(HttpClient httpClient)
        {
            try
            {
                if (_authProvider == null) return;
                var (cookies, authorization) = _authProvider.Invoke();
                if (cookies != null)
                {
                    if (httpClient.DefaultRequestHeaders.Contains("Cookie"))
                        httpClient.DefaultRequestHeaders.Remove("Cookie");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                }
                if (authorization != null)
                {
                    if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
                        httpClient.DefaultRequestHeaders.Remove("Authorization");
                    httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to forward auth headers to HttpClient");
            }
        }

        public void InitializeDbContext()
        {
            // _context = DbContextFactory.CreateDbContext();
        }
#endregion

        #region Merge HTML
        public async Task<string> MergeHtmlAsync(string companyGuid, string projectGuid, string html)
        {
            try
            {
                var payload = new { CompanyGuid = companyGuid, ProjectGuid = projectGuid, Html = html };
                var resp = await _client.PostAsJsonAsync("api/CompanyHTMLReports/merge-html", payload);
                var result = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("MergeHtmlAsync failed. Status: {Status} Body: {Body}", resp.StatusCode, result);
                    throw new Exception($"Merge HTML failed: {result}");
                }
                return result.Trim('"');
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during MergeHtmlAsync");
                throw;
            }
        }
        #endregion

        #region Email Service
        public class EmailRequestDto
        {
            public string ToAddress { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
            public string senderName { get; set; } = string.Empty;
            public string senderEmail { get; set; } = string.Empty;
            public string CcEmail { get; set; } = string.Empty;
            public string BccEmail { get; set; } = string.Empty;
            public string ProjectGuid { get; set; } = string.Empty;
            public string CompanyGuid { get; set; } = string.Empty;
            public bool IsHtml { get; set; } = true;
            public int? CompanyReportTypeId { get; set; }
        }

        public async Task<string> SendEmailAsync(string toAddress, string subject, string body, string senderName = "", string senderEmail = "", string ccEmail = "", bool isHtml = true, string projectGuid = "", string companyGuid = "")
        {
            var request = new EmailRequestDto
            {
                ToAddress = toAddress,
                Subject = subject,
                Body = body,
                senderName = senderName,
                CcEmail = ccEmail,
                IsHtml = isHtml,
                senderEmail = senderEmail,
                ProjectGuid = projectGuid ?? string.Empty,
                CompanyGuid = companyGuid ?? string.Empty
            };
            var response = await _client.PostAsJsonAsync("api/EmailService/Send", request);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email: {result}");
                throw new Exception($"Failed to send email: {result}");
            }
            return result;
        }

        public async Task<string> SendProjectEmailAsync(string projectGuid, string subject, string body, string toAddress = "", string senderName = "", string senderEmail = "", string ccEmail = "", string bccEmail = "", bool isHtml = true, int? companyReportTypeId = null, string companyGuid = "")
        {
            try
            {
                var payload = new EmailRequestDto
                {
                    ToAddress = toAddress ?? string.Empty,
                    Subject = subject ?? string.Empty,
                    Body = body ?? string.Empty,
                    senderName = senderName ?? string.Empty,
                    senderEmail = senderEmail ?? string.Empty,
                    CcEmail = ccEmail ?? string.Empty,
                    BccEmail = bccEmail ?? string.Empty,
                    IsHtml = isHtml,
                    CompanyReportTypeId = companyReportTypeId,
                    ProjectGuid = projectGuid ?? string.Empty,
                    CompanyGuid = companyGuid ?? string.Empty
                };
                var resp = await _client.PostAsJsonAsync($"api/EmailService/SendProject/{projectGuid}", payload);
                var result = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("SendProjectEmailAsync failed. Status: {Status} Body: {Body}", resp.StatusCode, result);
                    throw new Exception($"Failed to send project email: {result}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during SendProjectEmailAsync for {ProjectGuid}", projectGuid);
                throw;
            }
        }

        public async Task<List<EmailLogDto>> GetProjectEmailLogsAsync(string projectGuid)
        {
            try
            {
                var resp = await _client.GetAsync($"api/EmailService/Project/{projectGuid}/logs");
                var result = await resp.Content.ReadAsStringAsync();
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogError("GetProjectEmailLogsAsync failed. Status: {Status} Body: {Body}", resp.StatusCode, result);
                    throw new Exception($"Failed to get project email logs: {result}");
                }
                return System.Text.Json.JsonSerializer.Deserialize<List<EmailLogDto>>(result, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<EmailLogDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during GetProjectEmailLogsAsync for {ProjectGuid}", projectGuid);
                throw;
            }
        }
        #endregion

        #region Company CRUD

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies");
            return await _client.GetFromJsonAsync<List<Company>>("api/Companies");
        }

        public async Task CreateCompanyAsync(Company dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/Companies", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating Company.");
            }
        }

        /**
         * CreateCompanySettingsFromGlobalSettingsAsync
         * 
         * @param Company dataObj
         * Get the GlobalDefaults and create a companysettings object and use the HourlyRate with OverheadPercentage=50, LaborPercentage=36, MaterialPercentage=14, CompanyId=dataObj.Id
         * Create a CompanyDefaults object with CompanyId=dataObj.Id, Baseboards=1, Ceilings=1, Doors=1, Walls=1, Windows=1, PaintTypeBaseboardsId=4, PaintTypeCeilingsId=8, PaintTypeTrimDoorsId=4, PaintTypeWallsId=2
         * Create a PricingInteriorDefault object with CompanyId=dataObj.Id, BaseboardRatePerLinearFoot=50, CeilingRatePerSquareFoot=80, CrownMoldingRatePerLinearFoot=35, DoorRateEach=0.5, PaintCoats=2, PaintCoveragePerGallon=300, WallRatePerSquareFoot=100
         * 
         * @return Task
         */
        public async Task SetCompanyDefaultsAsync(Guid companyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/SetDefaults/{companyGuid.ToString()}");
                await _client.GetFromJsonAsync<GlobalDefaults>($"api/Companies/SetDefaults/{companyGuid.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Setting CompanyDefaults.");
                throw new Exception("Error Setting CompanyDefaults.", ex);
            }
        }

        public async Task UpdateCompanyAsync(Company dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/Companies/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating Company.");
            }
        }

        public async Task DeleteCompanyAsync(Guid id)
        {
            var idStr = id.ToString();
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{idStr}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/Companies/{idStr}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting Company.");
            }
        }

        public async Task<Company> GetCompanyAsync(Guid companyGuid)
        {
            var idStr = companyGuid.ToString();
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{idStr}");

            var comp = await _client.GetFromJsonAsync<Company>($"api/Companies/{idStr}");
            _logger.LogInformation($"GetCompany - {comp?.ToString() ?? "null"}");
            return comp;
        }

        #endregion

        #region AreaItem CRUD
        public async Task<List<AreaItem>> GetAllAreaItemsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AreaItems");
            return await _client.GetFromJsonAsync<List<AreaItem>>("api/AreaItems");
        }

        public async Task CreateAreaItemAsync(AreaItem dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AreaItems");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/AreaItems", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating AreaItem.");
            }
        }

        public async Task UpdateAreaItemAsync(AreaItem dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AreaItems/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/AreaItems/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating AreaItem.");
            }
        }

        public async Task DeleteAreaItemAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AreaItems/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/AreaItems/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting AreaItem.");
            }
        }

        public async Task<AreaItem> GetAreaItemAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AreaItems/{id}");

            var areaItem = await _client.GetFromJsonAsync<AreaItem>($"api/AreaItems/{id}");
            _logger.LogInformation($"GetAreaItem - {areaItem?.ToString() ?? "null"}");
            return areaItem;
        }
        #endregion

        #region CompanyCustomer CRUD
        public async Task<List<CompanyCustomer>> GetAllCompanyCustomersAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers");
            return await _client.GetFromJsonAsync<List<CompanyCustomer>>("api/CompanyCustomers");
        }
        public async Task<List<CompanyCustomer>> GetAllCompanyCustomersByGuidAsync(string guidId)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/CompanyGuid/{guidId}");
                using var response = await _client.GetAsync($"api/CompanyCustomers/CompanyGuid/{guidId}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("GetAllCompanyCustomersByGuidAsync failed. Status: {Status} Error: {Error}", response.StatusCode, error);
                    return new List<CompanyCustomer>();
                }

                return await response.Content.ReadFromJsonAsync<List<CompanyCustomer>>() ?? new List<CompanyCustomer>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception getting customers by guid {GuidId}", guidId);
                return new List<CompanyCustomer>();
            }
        }

        public async Task CreateCompanyCustomerAsync(CompanyCustomer dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CompanyCustomers", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating CompanyCustomer.");
            }
        }

        public async Task UpdateCompanyCustomerAsync(CompanyCustomer dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CompanyCustomers/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating CompanyCustomer.");
            }
        }

        public async Task DeleteCompanyCustomerAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CompanyCustomers/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CompanyCustomer.");
            }
        }

        public async Task<CompanyCustomer> GetCompanyCustomerAsync(string customerGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/CustomerGuid/{customerGuid}");

            var companyCustomer = await _client.GetFromJsonAsync<CompanyCustomer>($"api/CompanyCustomers/CustomerGuid/{customerGuid}");
            return companyCustomer;
        }

        public async Task<CompanyCustomer> GetCompanyCustomerByIdAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/{id}");

            var companyCustomer = await _client.GetFromJsonAsync<CompanyCustomer>($"api/CompanyCustomers/{id}");
            return companyCustomer;
        }
        #endregion

        #region CompanyDefaults CRUD
        public async Task<List<CompanyDefaults>> GetAllCompanyDefaultsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyDefaults");
            return await _client.GetFromJsonAsync<List<CompanyDefaults>>("api/CompanyDefaults");
        }

        public async Task CreateCompanyDefaultsAsync(CompanyDefaults dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyDefaults");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CompanyDefaults", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating CompanyDefaults.");
            }
        }

        public async Task UpdateCompanyDefaultsAsync(CompanyDefaults dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyDefaults/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CompanyDefaults/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating CompanyDefaults.");
            }
        }

        public async Task DeleteCompanyDefaultsAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyDefaults/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CompanyDefaults/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CompanyDefaults.");
            }
        }

        public async Task<CompanyDefaults?> GetCompanyDefaultsAsync(string guidId)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyDefaults/{guidId}");
                var result = await _client.GetFromJsonAsync<CompanyDefaults>($"api/CompanyDefaults/{guidId}");
                _logger.LogInformation($"Successfully retrieved CompanyDefaults for Company: {guidId}, Values: {result.PaintTypeTrimDoorsId}, " +
                    $"{result.PaintTypeTrimDoorsId}, {result.PaintTypeCeilingsId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching CompanyDefaults.");
                return null;
            }
        }
        #endregion

        #region CompanySettings CRUD

        public async Task<List<CompanySettings>> GetAllCompanySettingsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanySettings");
            return await _client.GetFromJsonAsync<List<CompanySettings>>("api/CompanySettings");
        }

        public async Task CreateCompanySettingsAsync(CompanySettings dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanySettings");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CompanySettings", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating CompanySettings.");
            }
        }

        public async Task UpdateCompanySettingsAsync(CompanySettings dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanySettings/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CompanySettings/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating CompanySettings.");
            }
        }

        public async Task DeleteCompanySettingsAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanySettings/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CompanySettings/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CompanySettings.");
            }
        }

        public async Task<CompanySettings> GetCompanySettingsAsync(string guidId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanySettings/{guidId}");
            return await _client.GetFromJsonAsync<CompanySettings>($"api/CompanySettings/{guidId}");
        }

        #endregion

        #region DifficultyLevel CRUD

        public async Task<List<DifficultyLevel>> GetAllDifficultyLevelsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/DifficultyLevels");
            return await _client.GetFromJsonAsync<List<DifficultyLevel>>("api/DifficultyLevels");
        }

        public async Task CreateDifficultyLevelAsync(DifficultyLevel dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/DifficultyLevels");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/DifficultyLevels", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating DifficultyLevel.");
            }
        }

        public async Task UpdateDifficultyLevelAsync(DifficultyLevel dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/DifficultyLevels/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/DifficultyLevels/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating DifficultyLevel.");
            }
        }

        public async Task DeleteDifficultyLevelAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/DifficultyLevels/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/DifficultyLevels/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting DifficultyLevel.");
            }
        }

        public async Task<DifficultyLevel> GetDifficultyLevelAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/DifficultyLevels/{id}");
            return await _client.GetFromJsonAsync<DifficultyLevel>($"api/DifficultyLevels/{id}");
        }

        #endregion

        #region ItemPaint CRUD

        public async Task<List<ItemPaint>> GetAllItemPaintsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemPaints");
            return await _client.GetFromJsonAsync<List<ItemPaint>>("api/ItemPaints");
        }

        public async Task CreateItemPaintAsync(ItemPaint dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemPaints");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/ItemPaints", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating ItemPaint.");
            }
        }

        public async Task UpdateItemPaintAsync(ItemPaint dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemPaints/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/ItemPaints/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating ItemPaint.");
            }
        }

        public async Task DeleteItemPaintAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemPaints/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/ItemPaints/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting ItemPaint.");
            }
        }

        public async Task<ItemPaint> GetItemPaintAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemPaints/{id}");
            return await _client.GetFromJsonAsync<ItemPaint>($"api/ItemPaints/{id}");
        }

        #endregion

        #region ItemType CRUD

        public async Task<List<ItemType>> GetAllItemTypesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemTypes");
            return await _client.GetFromJsonAsync<List<ItemType>>("api/ItemTypes");
        }

        public async Task CreateItemTypeAsync(ItemType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemTypes");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/ItemTypes", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating ItemType.");
            }
        }

        public async Task UpdateItemTypeAsync(ItemType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemTypes/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/ItemTypes/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating ItemType.");
            }
        }

        public async Task DeleteItemTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemTypes/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/ItemTypes/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting ItemType.");
            }
        }

        public async Task<ItemType> GetItemTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ItemTypes/{id}");
            return await _client.GetFromJsonAsync<ItemType>($"api/ItemTypes/{id}");
        }

        #endregion

        #region PaintBrand CRUD

        public async Task<List<PaintBrand>> GetAllPaintBrandsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintBrands");
            return await _client.GetFromJsonAsync<List<PaintBrand>>("api/PaintBrands");
        }

        public async Task CreatePaintBrandAsync(PaintBrand dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintBrands");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintBrands", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PaintBrand.");
            }
        }

        public async Task UpdatePaintBrandAsync(PaintBrand dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintBrands/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintBrands/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintBrand.");
            }
        }

        public async Task DeletePaintBrandAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintBrands/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintBrands/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintBrand.");
            }
        }

        public async Task<PaintBrand> GetPaintBrandAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintBrands/{id}");
            return await _client.GetFromJsonAsync<PaintBrand>($"api/PaintBrands/{id}");
        }

        #endregion

        #region PaintSheen CRUD

        public async Task<List<PaintSheen>> GetAllPaintSheensAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintSheens");
            return await _client.GetFromJsonAsync<List<PaintSheen>>("api/PaintSheens");
        }

        public async Task CreatePaintSheenAsync(PaintSheen dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintSheens");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintSheens", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PaintSheen.");
            }
        }

        public async Task UpdatePaintSheenAsync(PaintSheen dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintSheens/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintSheens/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintSheen.");
            }
        }

        public async Task DeletePaintSheenAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintSheens/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintSheens/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintSheen.");
            }
        }

        public async Task<PaintSheen> GetPaintSheenAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintSheens/{id}");
            return await _client.GetFromJsonAsync<PaintSheen>($"api/PaintSheens/{id}");
        }

        #endregion

        #region PaintType CRUD

        public async Task<List<PaintType>> GetAllPaintTypesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypes");
            return await _client.GetFromJsonAsync<List<PaintType>>("api/PaintTypes");
        }

        public async Task CreatePaintTypeAsync(PaintType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypes");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintTypes", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PaintType.");
            }
        }

        public async Task UpdatePaintTypeAsync(PaintType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypes/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintTypes/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintType.");
            }
        }

        public async Task DeletePaintTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypes/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintTypes/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintType.");
            }
        }

        public async Task<PaintType> GetPaintTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypes/{id}");
            return await _client.GetFromJsonAsync<PaintType>($"api/PaintTypes/{id}");
        }

        #endregion

        #region CompanyPaintType CRUD
        public async Task<List<CompanyPaintType>> GetAllCompanyPaintTypesAsync(string CompanyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintTypes/{CompanyGuid}");
            var compPaintTypes = await _client.GetFromJsonAsync<List<CompanyPaintType>>($"api/CompanyPaintTypes/{CompanyGuid}");
            return compPaintTypes;
        }

        public async Task CreateCompanyPaintTypeAsync(CompanyPaintType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintTypes");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CompanyPaintTypes", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating CompanyPaintType.");
            }
        }

        public async Task UpdateCompanyPaintTypeAsync(CompanyPaintType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintTypes/{dataObj.Id}/{dataObj.CompanyId.ToString()}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CompanyPaintTypes/{dataObj.Id}/{dataObj.CompanyId.ToString()}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating CompanyPaintType.");
            }
        }

        public async Task DeleteCompanyPaintTypeAsync(int id, string CompanyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintTypes/{id}/{CompanyGuid}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CompanyPaintTypes/{id}/{CompanyGuid}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CompanyPaintType.");
            }
        }

        public async Task<CompanyPaintType> GetCompanyPaintTypeAsync(int id, string CompanyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintTypes/{id}/{CompanyGuid}");
            return await _client.GetFromJsonAsync<CompanyPaintType>($"api/CompanyPaintTypes/{id}/{CompanyGuid}");
        }
        #endregion

        #region PricingType CRUD

        public async Task<List<PricingType>> GetAllPricingTypesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingTypes");
            return await _client.GetFromJsonAsync<List<PricingType>>("api/PricingTypes");
        }

        public async Task CreatePricingTypeAsync(PricingType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingTypes");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PricingTypes", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PricingType.");
            }
        }

        public async Task UpdatePricingTypeAsync(PricingType dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingTypes/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PricingTypes/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PricingType.");
            }
        }

        public async Task DeletePricingTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingTypes/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PricingTypes/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PricingType.");
            }
        }

        public async Task<PricingType> GetPricingTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingTypes/{id}");
            return await _client.GetFromJsonAsync<PricingType>($"api/PricingTypes/{id}");
        }

        #endregion

        #region PaintTypeName CRUD

        public async Task<List<PaintTypeName>> GetAllPaintTypeNamesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypeNames");
            return await _client.GetFromJsonAsync<List<PaintTypeName>>("api/PaintTypeNames");
        }

        public async Task CreatePaintTypeNameAsync(PaintTypeName dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypeNames");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintTypeNames", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PaintTypeName.");
            }
        }

        public async Task UpdatePaintTypeNameAsync(PaintTypeName dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypeNames/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintTypeNames/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintTypeName.");
            }
        }

        public async Task DeletePaintTypeNameAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypeNames/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintTypeNames/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintTypeName.");
            }
        }

        public async Task<PaintTypeName> GetPaintTypeNameAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintTypeNames/{id}");
            return await _client.GetFromJsonAsync<PaintTypeName>($"api/PaintTypeNames/{id}");
        }

        #endregion

        #region PaintQuality CRUD

        public async Task<List<PaintQuality>> GetAllPaintQualitiesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintQualities");
            return await _client.GetFromJsonAsync<List<PaintQuality>>("api/PaintQualities");
        }

        public async Task CreatePaintQualityAsync(PaintQuality dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintQualities");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintQualities", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PaintQuality.");
            }
        }

        public async Task UpdatePaintQualityAsync(PaintQuality dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintQualities/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintQualities/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintQuality.");
            }
        }

        public async Task DeletePaintQualityAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintQualities/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintQualities/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintQuality.");
            }
        }

        public async Task<PaintQuality> GetPaintQualityAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/AppPaintQualities/{id}");
            return await _client.GetFromJsonAsync<PaintQuality>($"api/AppPaintQualities/{id}");
        }

        #endregion

        #region ProjectArea CRUD

        public async Task<List<ProjectArea>> GetAllProjectAreasAsync(int projectId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ProjectAreas/ByProjectId/{projectId}");
            var roomList = await _client.GetFromJsonAsync<List<ProjectArea>>($"api/ProjectAreas/ByProjectId/{projectId}");
            return roomList;
        }

        public async Task<ProjectArea> CreateProjectAreaAsync(ProjectArea dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ProjectAreas");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/ProjectAreas", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating ProjectArea.");

                return httpResponseMsg.Content.ReadFromJsonAsync<ProjectArea>().Result;
            }
        }


        public async Task UpdateProjectAreaAsync(ProjectArea dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ProjectAreas/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/ProjectAreas/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating ProjectArea.");
            }
        }

        public async Task DeleteProjectAreaAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ProjectAreas/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/ProjectAreas/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting ProjectArea.");
            }
        }

        public async Task<ProjectArea> GetProjectAreaAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ProjectAreas/{id}");
            return await _client.GetFromJsonAsync<ProjectArea>($"api/ProjectAreas/{id}");
        }

        #endregion

        #region Project Adjustments
        public async Task<List<Adjustment>> GetAdjustmentsByProjectIdAsync(int projectId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Adjustments/ByProjectId/{projectId}");
            var adjustments = await _client.GetFromJsonAsync<List<Adjustment>>($"api/Adjustments/ByProjectId/{projectId}");
            return adjustments ?? new List<Adjustment>();
        }

        public async Task<Adjustment> CreateAdjustmentAsync(Adjustment dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Adjustments");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/Adjustments", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating ProjectArea.");

                var response = httpResponseMsg.Content.ReadFromJsonAsync<Adjustment>().Result;
                return response;
            }
        }

        public async Task UpdateAdjustmentAsync(int adjustmentId, Adjustment dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Adjustments/{adjustmentId}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/Adjustments/{adjustmentId}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating Adjustment.");
            }
        }

        public async Task DeleteAdjustmentAsync(string id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Adjustments/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/Adjustments/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting Adjustment.");
            }
        }
        #endregion

        #region StatusCode CRUD

        public async Task<List<StatusCode>> GetAllStatusCodesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/StatusCodes");
            return await _client.GetFromJsonAsync<List<StatusCode>>("api/StatusCodes");
        }

        public async Task CreateStatusCodeAsync(StatusCode dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/StatusCodes");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/StatusCodes", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating StatusCode.");
            }
        }

        public async Task UpdateStatusCodeAsync(StatusCode dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/StatusCodes/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/StatusCodes/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating StatusCode.");
            }
        }

        public async Task DeleteStatusCodeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/StatusCodes/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/StatusCodes/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting StatusCode.");
            }
        }

        public async Task<StatusCode> GetStatusCodeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/StatusCodes/{id}");
            return await _client.GetFromJsonAsync<StatusCode>($"api/StatusCodes/{id}");
        }

        #endregion

        #region PricingInteriorDefaults CRUD
        public async Task<List<PricingInteriorDefault>> GetAllPricingInteriorDefaultsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingInteriorDefaults");
            return await _client.GetFromJsonAsync<List<PricingInteriorDefault>>($"api/PricingInteriorDefaults");
        }
        public async Task<PricingInteriorDefault> GetPricingInteriorDefaultAsync(string guidId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingInteriorDefaults/{guidId}");
            return await _client.GetFromJsonAsync<PricingInteriorDefault>($"api/PricingInteriorDefaults/{guidId}");
        }

        public async Task CreatePricingInteriorDefaultAsync(PricingInteriorDefault dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingInteriorDefaults");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PricingInteriorDefaults", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating PricingInteriorDefault.");
            }
        }

        public async Task UpdatePricingInteriorDefaultAsync(PricingInteriorDefault dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingInteriorDefaults/{dataObj.CompanyId}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PricingInteriorDefaults/{dataObj.CompanyId}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PricingInteriorDefault.");
            }
        }

        public async Task DeletePricingInteriorDefaultAsync(string guidId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PricingInteriorDefaults/{guidId}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PricingInteriorDefaults/{guidId}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PricingInteriorDefault.");
            }
        }
        #endregion



        #region User Company Informaton
        public async Task<string> GetUserCompanyGuidAsync(string email)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/UserInfo/Company/{email}");
            var companyGuid = await _client.GetStringAsync($"api/UserInfo/Company/{email}");
            return companyGuid.Replace("\\", "").Replace("\"", "");
        }

        public async Task<List<GlobalDefaults>> GetAllGlobalDefaultsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GlobalDefaults");
            return await _client.GetFromJsonAsync<List<GlobalDefaults>>("api/GlobalDefaults");
        }

        public async Task CreateGlobalDefaultsAsync(GlobalDefaults dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GlobalDefaults");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/GlobalDefaults", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Creating GlobalDefaults.");
            }
        }

        public async Task UpdateGlobalDefaultsAsync(GlobalDefaults dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GlobalDefaults/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/GlobalDefaults/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating GlobalDefaults.");
            }
        }

        public async Task DeleteGlobalDefaultsAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GlobalDefaults/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/GlobalDefaults/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting GlobalDefaults.");
            }
        }

        public async Task<List<ProjectData>> GetAllCustomerProjectsAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/all/{id}");
            using (var response = await _client.GetAsync($"api/CustomerProjects/all/{id}"))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error Getting CustomerProjects.");

                return (await response.Content.ReadFromJsonAsync<List<ProjectData>>());
            }
        }

        public async Task<ProjectData> CreateCustomerProjectAsync(ProjectData dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CustomerProjects", dataObj))
            {
                if (!(httpResponseMsg.IsSuccessStatusCode))
                    throw new Exception("Error Creating CustomersProject.");
                else
                    return httpResponseMsg.Content.ReadFromJsonAsync<ProjectData>().Result;
            }
        }

        public async Task UpdateCustomerProjectAsync(ProjectData dataObj)
        {
            _logger.LogInformation("Updating Customer Project: {@DataObj}", dataObj);
            var json = System.Text.Json.JsonSerializer.Serialize(dataObj);
            _logger.LogInformation("Serialized JSON: {Json}", json);
            _logger.LogInformation("Base Address: {BaseAddress}", _client.BaseAddress);
            foreach (var header in _client.DefaultRequestHeaders)
            {
                _logger.LogInformation("Header: {Key} = {Value}", header.Key, string.Join(", ", header.Value));
            }
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/update/{dataObj.Id}");

            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CustomerProjects/update/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                    _logger.LogError("Error Updating Customer Project. Status: {StatusCode}, Error: {ErrorContent}", httpResponseMsg.StatusCode, errorContent);
                    throw new Exception($"Error Updating Customer Project: {errorContent}");
                }
            }
        }

        // Method to call the endpoint to update BaseAmount
        public async Task UpdateProjectBaseAmountAsync(int projectId, decimal baseAmount)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/{projectId}/base-amount");
                var response = await _client.PatchAsJsonAsync($"api/CustomerProjects/{projectId}/base-amount", baseAmount);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to update BaseAmount. Status Code: {response.StatusCode}, Error: {error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating BaseAmount: {ex.Message}", ex);
            }
        }
        public async Task DeleteCustomerProjectAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CustomerProjects/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CustomersProject.");
            }
        }

        public async Task<ProjectData> GetCustomerProjectAsync(string id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/{id}");
            using (var response = await _client.GetAsync($"api/CustomerProjects/{id}"))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error Getting CustomersProject.");

                return await response.Content.ReadFromJsonAsync<ProjectData>();
            }
        }

        public async Task<ProjectData> DuplicateCustomerProjectAsync(string sourceProjectGuid, ProjectData newProjectData)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CustomerProjects/duplicate/{sourceProjectGuid}");
            var response = await _client.PostAsJsonAsync($"api/CustomerProjects/duplicate/{sourceProjectGuid}", newProjectData);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error duplicating CustomerProject.");
            return await response.Content.ReadFromJsonAsync<ProjectData>();
        }
        #endregion

        #region Room CRUD
        public async Task<List<Room>> GetRoomsByProjectIdAsync(int projectId)
        {
            _logger.LogInformation("Fetching rooms for projectId: {ProjectId}", projectId);
            if (projectId <= 0)
            {
                throw new ArgumentException("Invalid projectId. Must be greater than 0.", nameof(projectId));
            }

            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Rooms/ByProjectId/{projectId}");
            var response = await _client.GetAsync($"api/Rooms/ByProjectId/{projectId}");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to fetch rooms for projectId: {ProjectId}. Status: {StatusCode}, Error: {ErrorContent}", projectId, response.StatusCode, errorContent);
                throw new Exception($"Error Getting Rooms: {response.StatusCode} - {errorContent}");
            }


            return await response.Content.ReadFromJsonAsync<List<Room>>();
        }

        public async Task<Room> CreateRoomAsync(Room dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Rooms");
            var httpResponseMsg = await _client.PostAsJsonAsync("api/Rooms", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
            {
                throw new Exception("Error Creating Room.");
            }
            else
            {
                return await httpResponseMsg.Content.ReadFromJsonAsync<Room>();
            }
        }

        public async Task<Room> UpdateRoomAsync(Room dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Rooms/{dataObj.Id}");
            var httpResponseMsg = await _client.PutAsJsonAsync($"api/Rooms/{dataObj.Id}", dataObj);

            _logger.LogInformation($"UpdateRoomAsync - {dataObj.Id} - {httpResponseMsg}");

            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception($"Error Updating Room. {httpResponseMsg}");
            else
                return await httpResponseMsg.Content.ReadFromJsonAsync<Room>();
        }

        public async Task DeleteRoomAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Rooms/{id}");
            using var httpResponseMsg = await _client.DeleteAsync($"api/Rooms/{id}");
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Deleting Room.");
        }

        public async Task<Room> GetRoomAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Rooms/{id}");
            var response = await _client.GetAsync($"api/Rooms/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Getting Room.");

            return await response.Content.ReadFromJsonAsync<Room>();
        }

        public async Task<RoomGlobalDefaults> GetRoomGlobalDefaultsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomGlobalDefaults");
            var response = await _client.GetAsync("api/RoomGlobalDefaults");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Getting RoomGlobalDefaults.");

            return await response.Content.ReadFromJsonAsync<RoomGlobalDefaults>();
        }

        public async Task<PricingResponseInterior> PriceHouseInteriorAsync(int projectId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintPricingEngine/Interior/{projectId}");
            using (var response = await _client.GetAsync($"api/PaintPricingEngine/Interior/{projectId}"))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error Getting PricingRequestInteriors.");

                var result = await response.Content.ReadFromJsonAsync<PricingResponseInterior>();
                return result;
            }
        }

        public async Task<PricingResponseInterior> PriceRoomInteriorAsync(int roomId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintPricingEngine/Interior/Room/{roomId}");
            var response = await _client.GetAsync($"api/PaintPricingEngine/Interior/Room/{roomId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Getting PricingRequestInterior.");

            return await response.Content.ReadFromJsonAsync<PricingResponseInterior>();
        }
        #endregion

        #region Reports
        /* Depreciated method, use GetInteriorQuoteAsPDFAsync instead */
        public async Task<byte[]> GetInteriorQuoteAsPDFAsync(string projectGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Report/Interior/QuoteAsPDF/{projectGuid}");
            var response = await _client.GetAsync($"api/Report/Interior/QuoteAsPDF/{projectGuid}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<List<CompanyProjectsDto>> GetAllCompanyProjectsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/ReportData/GetAllCompanyProjects");
            var response = await _client.GetAsync("api/ReportData/GetAllCompanyProjects");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<CompanyProjectsDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CompanyProjectsDto>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PricingService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region PaintableItem

        public async Task<List<PaintableItem>> GetAllPaintableItemsAsync(int roomId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItems/Room/{roomId}");
            var paintableItems = await _client.GetFromJsonAsync<List<PaintableItem>>($"api/PaintableItems/Room/{roomId}");
            if (paintableItems != null)
            {
                _logger.LogInformation("Successfully retrieved paintable items.");
            }
            return paintableItems;
        }

        public async Task<PaintableItem> CreatePaintableItemAsync(PaintableItem dataObj)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItems");
                var response = await _client.PostAsJsonAsync("api/PaintableItems", dataObj);
                if (response.IsSuccessStatusCode)
                {
                    var createdItem = await response.Content.ReadFromJsonAsync<PaintableItem>();
                    return createdItem ?? new PaintableItem();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create paintable item. Status code: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception("Error Creating Paintable Item.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating paintable item.");
                throw;
            }
        }

        public async Task UpdatePaintableItemAsync(PaintableItem dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItems/{dataObj.Id}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintableItems/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating PaintableItem.");
            }
        }

        public async Task DeletePaintableItemAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItems/{id}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintableItems/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting PaintableItem.");
            }
        }

        public async Task<PaintableItem> GetPaintableItemAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItems/{id}");
            return await _client.GetFromJsonAsync<PaintableItem>($"api/PaintableItems/{id}");
        }
        public async Task<List<CompanyPaintableItem>> GetAllCompanyPaintableItemsAsync(string CompanyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems/all/{CompanyGuid}");
                var response = await _client.GetAsync($"api/CompanyPaintableItems/all/{CompanyGuid}");
                if (response.IsSuccessStatusCode)
                {
                    var compPaintableItems = await response.Content.ReadFromJsonAsync<List<CompanyPaintableItem>>();
                    return compPaintableItems ?? new List<CompanyPaintableItem>();
                }
                else
                {
                    _logger.LogError($"Failed to get company paintable items. Status code: {response.StatusCode}");
                    throw new Exception("Error Getting Company Paintable Items.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting company paintable items.");
                throw;
            }
        }
        #endregion

        #region CompanyPaintableItem
        public async Task<List<CompanyPaintableItemSummary>> GetAllCompanyPaintableItemSummariesAsync(string CompanyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems/summary/{CompanyGuid}");
                var response = await _client.GetAsync($"api/CompanyPaintableItems/summary/{CompanyGuid}");
                if (response.IsSuccessStatusCode)
                {
                    var compPaintableItems = await response.Content.ReadFromJsonAsync<List<CompanyPaintableItemSummary>>();
                    return compPaintableItems ?? new List<CompanyPaintableItemSummary>();
                }
                else
                {
                    _logger.LogError($"Failed to get company paintable item summaries. Status code: {response.StatusCode}");
                    throw new Exception("Error Getting Company Paintable Item Summaries.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting company paintable item summaries.");
                throw;
            }
        }

        public async Task CreateCompanyPaintableItemAsync(CompanyPaintableItem dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems");
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/CompanyPaintableItems", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                    throw new Exception($"Internal server error: {errorContent}");
                }
                else
                {
                    _logger.LogInformation("Successfully created company paintable item.");
                }
            }
        }

        public async Task UpdateCompanyPaintableItemAsync(CompanyPaintableItem dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems/{dataObj.Id}/{dataObj.CompanyId.ToString()}");
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/CompanyPaintableItems/{dataObj.Id}/{dataObj.CompanyId.ToString()}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                    throw new Exception($"Error Updating CompanyPaintableItem. Server returned: {errorContent}");
                }
            }
        }

        public async Task DeleteCompanyPaintableItemAsync(int id, string CompanyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems/{id}/{CompanyGuid}");
            using (var httpResponseMsg = await _client.DeleteAsync($"api/CompanyPaintableItems/{id}/{CompanyGuid}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting CompanyPaintableItem.");
            }
        }

        public async Task<CompanyPaintableItem> GetCompanyPaintableItemAsync(int id, string CompanyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyPaintableItems/{id}/{CompanyGuid}");
                var response = await _client.GetAsync($"api/CompanyPaintableItems/{id}/{CompanyGuid}");
                if (response.IsSuccessStatusCode)
                {
                    var companyPaintableItem = await response.Content.ReadFromJsonAsync<CompanyPaintableItem>();
                    return companyPaintableItem ?? new CompanyPaintableItem();
                }
                else
                {
                    _logger.LogError($"Failed to get company paintable item. Status code: {response.StatusCode}");
                    throw new Exception("Error Getting Company Paintable Item.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting company paintable item.");
                throw;
            }
        }

        public async Task<List<PaintableItemCategory>> GetAllPaintableItemCategoriesAsync()
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItemCategories");
                var response = await _client.GetAsync($"api/PaintableItemCategories");
                if (response.IsSuccessStatusCode)
                {
                    var paintableItemCategories = await response.Content.ReadFromJsonAsync<List<PaintableItemCategory>>();
                    return paintableItemCategories ?? new List<PaintableItemCategory>();
                }
                else
                {
                    _logger.LogError($"Failed to get paintable item categories. Status code: {response.StatusCode}");
                    throw new Exception("Error Getting Paintable Item Categories.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting paintable item categories.");
                throw;
            }

        }

        public async Task<PaintableItemCategory> GetPaintableItemCategoryAsync(int id)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItemCategories/{id}");
                var response = await _client.GetAsync($"api/PaintableItemCategories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var paintableItemCategory = await response.Content.ReadFromJsonAsync<PaintableItemCategory>();
                    return paintableItemCategory;
                }
                else
                {
                    _logger.LogError($"Failed to get paintable item category. Status code: {response.StatusCode}");
                    throw new Exception("Error Getting Paintable Item Category.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting paintable item category.");
                throw;
            }
        }

        public async Task CreatePaintableItemCategoryAsync(PaintableItemCategory dataObj)
        {
            _logger.LogInformation($"Data Obj: Id={dataObj.Id}, Name={dataObj.Name}, Description={dataObj.Description}");

            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItemCategories");
                using (var httpResponseMsg = await _client.PostAsJsonAsync("api/PaintableItemCategories", dataObj))
                {
                    if (!httpResponseMsg.IsSuccessStatusCode)
                    {
                        var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                        throw new Exception($"Error Creating PaintableItemCategory. Server returned: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating paintable item category.");
                throw;
            }
        }

        public async Task UpdatePaintableItemCategoryAsync(PaintableItemCategory dataObj)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItemCategories/{dataObj.Id}");
                using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/PaintableItemCategories/{dataObj.Id}", dataObj))
                {
                    if (!httpResponseMsg.IsSuccessStatusCode)
                    {
                        var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                        throw new Exception($"Error Updating PaintableItemCategory. Server returned: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating paintable item category.");
                throw;
            }
        }

        public async Task DeletePaintableItemCategoryAsync(int id)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintableItemCategories/{id}");
                using (var httpResponseMsg = await _client.DeleteAsync($"api/PaintableItemCategories/{id}"))
                {
                    if (!httpResponseMsg.IsSuccessStatusCode)
                    {
                        var errorContent = await httpResponseMsg.Content.ReadAsStringAsync();
                        throw new Exception($"Error Deleting PaintableItemCategory. Server returned: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting paintable item category.");
                throw;
            }
        }
        #endregion

        #region User Password change
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            await EnsureUsersApiAuthorizedAsync();
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Users");
            using (var response = await _client.GetAsync("api/Users"))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error Getting Users.");
                var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                return users ?? new List<UserDto>();
            }
        }
        #endregion

        #region Google Calendar API
        public async Task<List<AddressSuggestionDto>> GetAddressSuggestionsAsync(string input)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GooglePlaces/autocomplete?input={input}");
            var response = await _client.GetAsync($"api/GooglePlaces/autocomplete?input={input}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Getting Address Suggestions.");

            return await response.Content.ReadFromJsonAsync<List<AddressSuggestionDto>>();
        }

        public async Task<AddressDetailsDto> GetAddressDetailsAsync(string placeId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GooglePlaces/details?placeId={placeId}");
            var response = await _client.GetAsync($"api/GooglePlaces/details?placeId={placeId}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Getting Address Details.");

            return await response.Content.ReadFromJsonAsync<AddressDetailsDto>();
        }
        #endregion

        #region Quote PDF Generation
        public async Task<byte[]> GetProjectQuoteAsPDFAsync(string projectId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/QuotePdf/Interior/{projectId}");
            var response = await _client.GetAsync($"api/QuotePdf/Interior/{projectId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<List<CompanyHTMLReportTemplate>> GetAllCompanyHTMLReportTemplatesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate");
            var response = await _client.GetAsync("api/CompanyHTMLReportTemplate");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CompanyHTMLReportTemplate>>();
        }

        public async Task<List<CompanyHTMLReportTemplate>> GetCompanyHTMLReportTemplatesByCompanyAsync(string companyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/company/{companyGuid}");
            var response = await _client.GetAsync($"api/CompanyHTMLReportTemplate/company/{companyGuid}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CompanyHTMLReportTemplate>>();
        }

        public async Task<CompanyHTMLReportTemplate> GetCompanyHTMLReportTemplateByTypeAndCompanyAsync(int reportTypeId, string companyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/type/{reportTypeId}/company/{companyGuid}");
            var response = await _client.GetAsync($"api/CompanyHTMLReportTemplate/type/{reportTypeId}/company/{companyGuid}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CompanyHTMLReportTemplate>();
        }

        public async Task<CompanyHTMLReportTemplate> GetCompanyHTMLReportTemplateAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/{id}");
            var response = await _client.GetAsync($"api/CompanyHTMLReportTemplate/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CompanyHTMLReportTemplate>();
        }

        public async Task<List<CompanyHTMLReportsTemplateMinimal>> GetMinimalTemplateInfoAsync(string companyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/templates/minimal/global/{companyGuid}");
                var response = await _client.GetAsync($"api/CompanyHTMLReportTemplate/templates/minimal/global/{companyGuid}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<CompanyHTMLReportsTemplateMinimal>>() ?? new List<CompanyHTMLReportsTemplateMinimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting minimal template info");
                throw;
            }
        }

        public async Task<List<CompanyHTMLReportsTemplateMinimal>> GetActiveMinimalTemplateInfoAsync(string companyGuid, bool GlobalOnly = true)
        {
            try
            {
                var url = GlobalOnly
                    ? $"api/CompanyHTMLReportTemplate/templates/minimal/global/{companyGuid}"
                    : $"api/CompanyHTMLReportTemplate/templates/minimal/{companyGuid}";
                _logger.LogInformation($"API Call - {_client.BaseAddress}{url}");
                var response = await _client.GetAsync(url);

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<CompanyHTMLReportsTemplateMinimal>>() ?? new List<CompanyHTMLReportsTemplateMinimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active minimal template info for {CompanyGuid}", companyGuid);
                throw;
            }
        }

        public async Task CreateCompanyHTMLReportTemplateAsync(CompanyHTMLReportTemplate report)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate");
            var response = await _client.PostAsJsonAsync("api/CompanyHTMLReportTemplate", report);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadFromJsonAsync<CompanyHTMLReportTemplate>();
        }

        public async Task UpdateCompanyHTMLReportTemplateAsync(CompanyHTMLReportTemplate report)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/{report.Id}");
            var response = await _client.PutAsJsonAsync($"api/CompanyHTMLReportTemplate/{report.Id}", report);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCompanyHTMLReportTemplateAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReportTemplate/{id}");
            var response = await _client.DeleteAsync($"api/CompanyHTMLReportTemplate/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Deleting CompanyHTMLReport.");
        }

        // CompanyHTMLReport CRUD operations
        public async Task<List<CompanyHTMLReport>> GetCompanyHTMLReportsByProjectAsync(string projectGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/project/{projectGuid}");
            var response = await _client.GetAsync($"api/CompanyHTMLReports/project/{projectGuid}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CompanyHTMLReport>>();
        }

        public async Task<CompanyHTMLReport> GetCompanyHTMLReportByTypeAndProjectAsync(int reportTypeId, string projectGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/type/{reportTypeId}/project/{projectGuid}");
            var response = await _client.GetAsync($"api/CompanyHTMLReports/type/{reportTypeId}/project/{projectGuid}");
            // Return null if not found
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            return await response.Content.ReadFromJsonAsync<CompanyHTMLReport>();
        }

        public async Task<CompanyHTMLReport> GetCompanyHTMLReportAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/{id}");
            var response = await _client.GetAsync($"api/CompanyHTMLReports/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CompanyHTMLReport>();
        }

        public async Task CreateCompanyHTMLReportAsync(CompanyHTMLReport report)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports");
            var response = await _client.PostAsJsonAsync("api/CompanyHTMLReports", report);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadFromJsonAsync<CompanyHTMLReport>();
        }

        public async Task UpdateCompanyHTMLReportAsync(CompanyHTMLReport report)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/{report.Id}");
            var response = await _client.PutAsJsonAsync($"api/CompanyHTMLReports/{report.Id}", report);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCompanyHTMLReportAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/{id}");
            var response = await _client.DeleteAsync($"api/CompanyHTMLReports/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Deleting CompanyHTMLReport.");
        }


        public async Task<List<CompanyReportType>> GetAllCompanyReportTypesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyReportTypes");
            var response = await _client.GetAsync("api/CompanyReportTypes");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CompanyReportType>>();
        }

        public async Task<CompanyReportType> GetCompanyReportTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyReportTypes/{id}");
            var response = await _client.GetAsync($"api/CompanyReportTypes/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CompanyReportType>();
        }

        public async Task CreateCompanyReportTypeAsync(CompanyReportType reportType)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyReportTypes");
            var response = await _client.PostAsJsonAsync("api/CompanyReportTypes", reportType);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Creating CompanyReportType.");
            await response.Content.ReadFromJsonAsync<CompanyReportType>();
        }

        public async Task UpdateCompanyReportTypeAsync(CompanyReportType reportType)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyReportTypes/{reportType.Id}");
            var response = await _client.PutAsJsonAsync($"api/CompanyReportTypes/{reportType.Id}", reportType);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Updating CompanyReportType.");
        }

        public async Task DeleteCompanyReportTypeAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyReportTypes/{id}");
            var response = await _client.DeleteAsync($"api/CompanyReportTypes/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error Deleting CompanyReportType.");
        }
        #endregion

        #region ServiceTitanConnectionData CRUD
        public async Task<List<ServiceTitanConnectionData>> GetAllServiceTitanConnectionDatasAsync()
        {
            _logger.LogInformation($"API Call - {_stClient.BaseAddress}api/ServiceTitanConnectionDatas");
            using (var httpResponseMsg = await _stClient.GetAsync("api/ServiceTitanConnectionDatas"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    return new List<ServiceTitanConnectionData>();
                var data = await httpResponseMsg.Content.ReadFromJsonAsync<List<ServiceTitanConnectionData>>();
                return data ?? new List<ServiceTitanConnectionData>();
            }
        }

        public async Task<ServiceTitanConnectionData?> CreateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj)
        {
            _logger.LogInformation($"API Call - {_stClient.BaseAddress}api/ServiceTitanConnectionDatas");
            using (var httpResponseMsg = await _stClient.PostAsJsonAsync("api/ServiceTitanConnectionDatas", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    return null;

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        public async Task<ServiceTitanConnectionData?> GetServiceTitanConnectionDataAsync(string companyGuid)
        {
            _logger.LogInformation($"Getting ServiceTitanConnectionData for companyGuid: {companyGuid}");
            _logger.LogInformation($"API Call - {_stClient.BaseAddress}api/ServiceTitanConnectionDatas/Company/{companyGuid}");
            var result = await _stClient.GetFromJsonAsync<ServiceTitanConnectionData>($"api/ServiceTitanConnectionDatas/Company/{companyGuid}");
            return result ?? null;
        }

        public async Task<ServiceTitanConnectionData> UpdateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj)
        {
            _logger.LogInformation($"API Call - {_stClient.BaseAddress}api/ServiceTitanConnectionDatas/{dataObj.Id}");
            using (var httpResponseMsg = await _stClient.PutAsJsonAsync($"api/ServiceTitanConnectionDatas/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating ServiceTitanConnectionData.");

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        public async Task<ServiceTitanConnectionData> DeleteServiceTitanConnectionDataAsync(int id)
        {
            _logger.LogInformation($"API Call - {_stClient.BaseAddress}api/ServiceTitanConnectionDatas/{id}");
            using (var httpResponseMsg = await _stClient.DeleteAsync($"api/ServiceTitanConnectionDatas/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting ServiceTitanConnectionData.");

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        #endregion

        #region User Management
        public async Task<List<string>> GetUserRolesAsync(string email)
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/roles/{email}");
                var response = await _client.GetAsync($"api/users/roles/{email}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
                }
                else
                {
                    _logger.LogError($"Failed to get roles for user {email}. Status code: {response.StatusCode}");
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user {Email}", email);
                throw;
            }
        }

        public async Task<List<string>> GetAvailableRolesAsync()
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/roles");
                return await _client.GetFromJsonAsync<List<string>>("api/users/roles") ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                throw;
            }
        }

        public async Task<List<CompanyDto>> GetCompaniesAsync()
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/companies");
                return await _client.GetFromJsonAsync<List<CompanyDto>>("api/users/companies") ?? new List<CompanyDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies");
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(UserDto user)
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/{user.Id}");
                var response = await _client.PutAsJsonAsync($"api/users/{user.Id}", user);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", user.Id);
                throw;
            }
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/{userId}/roles");
                var response = await _client.PutAsJsonAsync($"api/users/{userId}/roles", roles);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserCompanyAsync(string userId, string companyId)
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/{userId}/company");
                var response = await _client.PutAsJsonAsync($"api/users/{userId}/company", companyId);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string newPassword)
        {
            await EnsureUsersApiAuthorizedAsync();
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/users/{userId}/change-password");
                var dto = new ChangePasswordDto { NewPassword = newPassword };
                var response = await _client.PostAsJsonAsync($"api/users/{userId}/change-password", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                throw;
            }
        }
        #endregion

        public async Task DeleteAllCompanyHTMLReportsByProjectAsync(string projectGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/project/{projectGuid}");
            var response = await _client.DeleteAsync($"api/CompanyHTMLReports/project/{projectGuid}");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error deleting all CompanyHTMLReports for project {projectGuid}.");
        }

        public async Task<List<CompanyHTMLReport>> CopyCompanyHTMLReportTemplatesToProjectAsync(string companyGuid, string projectGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyHTMLReports/copy-templates/{companyGuid}/project/{projectGuid}");
            var response = await _client.PostAsync($"api/CompanyHTMLReports/copy-templates/{companyGuid}/project/{projectGuid}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to copy HTML report templates. Status: {StatusCode} Error: {Error}", response.StatusCode, error);
                throw new Exception($"Error copying report templates: {error}");
            }
            var result = await response.Content.ReadFromJsonAsync<List<CompanyHTMLReport>>();
            return result ?? new List<CompanyHTMLReport>();
        }

        /// <summary>
        /// Copies all global CompanyHTMLReportTemplates to the specified company.
        /// </summary>
        /// <param name="companyGuid">The company Guid as a string.</param>
        public async Task CopyGlobalCompanyHTMLReportTemplatesToCompanyAsync(string companyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{companyGuid}/CopyGlobalReportTemplates");
            var response = await _client.PostAsync($"api/Companies/{companyGuid}/CopyGlobalReportTemplates", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to copy global CompanyHTMLReportTemplates. Status: {response.StatusCode} Error: {error}");
                throw new Exception($"Error copying global CompanyHTMLReportTemplates: {error}");
            }
        }

        #region Global room settings (inclusions across project)
        public async Task<int> UpdateRoomInclusionsAsync(int projectId, RoomInclusionsRequest request)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/GlobalRoomSettings/project/{projectId}/inclusions");
            var response = await _client.PutAsJsonAsync($"api/GlobalRoomSettings/project/{projectId}/inclusions", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed updating room inclusions for project {ProjectId}. Status: {Status} Error: {Error}", projectId, response.StatusCode, error);
                throw new Exception($"Error updating room inclusions: {error}");
            }

            var payload = await response.Content.ReadFromJsonAsync<InclusionsUpdateResult>();
            return payload?.updatedRooms ?? 0;
        }

        public async Task<int> UpdateRoomPaintQualityAsync(int projectId, int paintQualityId)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/PaintQualities/project/{projectId}/global/paintquality/{paintQualityId}");
            var response = await _client.PutAsync($"api/PaintQualities/project/{projectId}/global/paintquality/{paintQualityId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed updating room paint quality for project {ProjectId}. Status: {Status} Error: {Error}", projectId, response.StatusCode, error);
                throw new Exception($"Error updating room paint quality: {error}");
            }

            // Controller returns a simple integer with the number of updated rooms
            var text = await response.Content.ReadAsStringAsync();
            if (int.TryParse(text, out var updated)) return updated;
            return 0;
        }
        #endregion

        #region Images CRUD
        public async Task<List<ImageAsset>> GetAllImagesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images");
            var list = await _client.GetFromJsonAsync<List<ImageAsset>>("api/Images");
            return list ?? new List<ImageAsset>();
        }

        public async Task<ImageAsset> GetImageAsync(Guid id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images/{id}");
            var resp = await _client.GetAsync($"api/Images/{id}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ImageAsset>() ?? new ImageAsset();
        }

        public async Task<byte[]> GetImageContentAsync(Guid id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images/{id}/content");
            var resp = await _client.GetAsync($"api/Images/{id}/content");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsByteArrayAsync();
        }

        public async Task<string> UploadImageAsync(byte[] data, string fileName, string contentType, string? imageName = null)
        {
            using var content = new MultipartFormDataContent();
            var byteContent = new ByteArrayContent(data);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(byteContent, "file", fileName);
            if (!string.IsNullOrWhiteSpace(imageName))
                content.Add(new StringContent(imageName), "imageName");

            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images");
            var resp = await _client.PostAsync("api/Images", content);
            resp.EnsureSuccessStatusCode();
            // Server returns plain text URL
            var url = await resp.Content.ReadAsStringAsync();
            return url.Trim('"');
        }

        public async Task UpdateImageAsync(ImageAsset image)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images/{image.Id}");
            var resp = await _client.PutAsJsonAsync($"api/Images/{image.Id}", image);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteImageAsync(Guid id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Images/{id}");
            var resp = await _client.DeleteAsync($"api/Images/{id}");
            resp.EnsureSuccessStatusCode();
        }
        #endregion

        public async Task<List<ProjectDataMinimal>> GetCompanyProjectsMinimalAsync(string companyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{companyGuid}/projects/minimal");
                var response = await _client.GetAsync($"api/Companies/{companyGuid}/projects/minimal");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get minimal projects for company {CompanyGuid}. Status: {Status} Error: {Error}", companyGuid, response.StatusCode, error);
                    return new List<ProjectDataMinimal>();
                }
                return await response.Content.ReadFromJsonAsync<List<ProjectDataMinimal>>() ?? new List<ProjectDataMinimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception getting minimal company projects for {CompanyGuid}", companyGuid);
                return new List<ProjectDataMinimal>();
            }
        }

        public async Task<List<ProjectDataMinimal>> GetAcceptedProjectsAsync(string companyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{companyGuid}/projects/accepted");
                var response = await _client.GetAsync($"api/Companies/{companyGuid}/projects/accepted");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get accepted projects for company {CompanyGuid}. Status: {Status} Error: {Error}", companyGuid, response.StatusCode, error);
                    return new List<ProjectDataMinimal>();
                }
                return await response.Content.ReadFromJsonAsync<List<ProjectDataMinimal>>() ?? new List<ProjectDataMinimal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception getting accepted projects for company {CompanyGuid}", companyGuid);
                return new List<ProjectDataMinimal>();
            }
        }

        public async Task DeleteCompanyCustomerAndRelatedDataAsync(Guid customerGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/CompanyCustomers/delete-with-related/{customerGuid}");
            var response = await _client.DeleteAsync($"api/CompanyCustomers/delete-with-related/{customerGuid}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to delete customer and related data. Status: {response.StatusCode} Error: {error}");
                throw new Exception($"Error deleting customer and related data: {error}");
            }
        }

        /// <summary>
        /// Sets the company interior pricing for the specified company.
        /// </summary>
        /// <param name="companyGuid">The company Guid as a string.</param>
        public async Task SetCompanyInteriorPricingAsync(string companyGuid)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/Companies/{companyGuid}/SetCompanyInteriorPricing");
            var response = await _client.PostAsync($"api/Companies/{companyGuid}/SetCompanyInteriorPricing", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to set company interior pricing. Status: {response.StatusCode} Error: {error}");
                throw new Exception($"Error setting company interior pricing: {error}");
            }
        }

        #region Dashboard
        public async Task<DashboardData> GetDashboardDataAsync(string companyGuid)
        {
            try
            {
                _logger.LogInformation($"API Call - {_client.BaseAddress}api/Dashboard/{companyGuid}");
                var response = await _client.GetAsync($"api/Dashboard/{companyGuid}");
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get dashboard data for company {CompanyGuid}. Status: {Status} Error: {Error}", companyGuid, response.StatusCode, error);
                    return new DashboardData();
                }
                return await response.Content.ReadFromJsonAsync<DashboardData>() ?? new DashboardData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception getting dashboard data for company {CompanyGuid}", companyGuid);
                return new DashboardData();
            }
        }
        #endregion

        #region SurfaceType 
        /// <summary>
        /// Loads surface type lookups for surface configuration screens.
        /// </summary>
        public async Task<List<SurfaceTypeLookup>> GetSurfaceTypeLookupsAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/SurfaceTypeLookups");
            return await _client.GetFromJsonAsync<List<SurfaceTypeLookup>>("api/SurfaceTypeLookups");
        }

        /// <summary>
        /// Loads a single surface type lookup for editing.
        /// </summary>
        public async Task<SurfaceTypeLookup> GetSurfaceTypeLookupAsync(byte id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/SurfaceTypeLookups/{id}");
            return await _client.GetFromJsonAsync<SurfaceTypeLookup>($"api/SurfaceTypeLookups/{id}");
        }

        /// <summary>
        /// Creates a surface type lookup record.
        /// </summary>
        public async Task CreateSurfaceTypeLookupAsync(SurfaceTypeLookup dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/SurfaceTypeLookups");
            using var httpResponseMsg = await _client.PostAsJsonAsync("api/SurfaceTypeLookups", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Creating SurfaceTypeLookup.");
        }

        /// <summary>
        /// Updates a surface type lookup record.
        /// </summary>
        public async Task UpdateSurfaceTypeLookupAsync(SurfaceTypeLookup dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/SurfaceTypeLookups/{dataObj.SurfaceTypeId}");
            using var httpResponseMsg = await _client.PutAsJsonAsync($"api/SurfaceTypeLookups/{dataObj.SurfaceTypeId}", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Updating SurfaceTypeLookup.");
        }

        /// <summary>
        /// Deletes a surface type lookup record.
        /// </summary>
        public async Task DeleteSurfaceTypeLookupAsync(byte id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/SurfaceTypeLookups/{id}");
            using var httpResponseMsg = await _client.DeleteAsync($"api/SurfaceTypeLookups/{id}");
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Deleting SurfaceTypeLookup.");
        }

        #endregion

        #region SurfaceTypeLookup

        #endregion

        #region RoomSurface
        /// <summary>
        /// Loads all room surface records for surface management.
        /// </summary>
        public async Task<List<RoomSurface>> GetAllRoomSurfacesAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfaces");
            return await _client.GetFromJsonAsync<List<RoomSurface>>("api/RoomSurfaces");
        }

        /// <summary>
        /// Loads a single room surface record.
        /// </summary>
        public async Task<RoomSurface> GetRoomSurfaceAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfaces/{id}");
            return await _client.GetFromJsonAsync<RoomSurface>($"api/RoomSurfaces/{id}");
        }

        /// <summary>
        /// Creates a room surface record.
        /// </summary>
        public async Task CreateRoomSurfaceAsync(RoomSurface dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfaces");
            using var httpResponseMsg = await _client.PostAsJsonAsync("api/RoomSurfaces", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Creating RoomSurface.");
        }

        /// <summary>
        /// Updates a room surface record.
        /// </summary>
        public async Task UpdateRoomSurfaceAsync(RoomSurface dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfaces/{dataObj.RoomSurfaceId}");
            using var httpResponseMsg = await _client.PutAsJsonAsync($"api/RoomSurfaces/{dataObj.RoomSurfaceId}", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Updating RoomSurface.");
        }

        /// <summary>
        /// Deletes a room surface record.
        /// </summary>
        public async Task DeleteRoomSurfaceAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfaces/{id}");
            using var httpResponseMsg = await _client.DeleteAsync($"api/RoomSurfaces/{id}");
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Deleting RoomSurface.");
        }
        #endregion

        #region RoomSurfacePaintLayer 
        /// <summary>
        /// Loads all room surface paint layers for management screens.
        /// </summary>
        public async Task<List<RoomSurfacePaintLayer>> GetAllRoomSurfacePaintLayersAsync()
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfacePaintLayers");
            return await _client.GetFromJsonAsync<List<RoomSurfacePaintLayer>>("api/RoomSurfacePaintLayers");
        }

        /// <summary>
        /// Loads a single room surface paint layer for editing.
        /// </summary>
        public async Task<RoomSurfacePaintLayer> GetRoomSurfacePaintLayerAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfacePaintLayers/{id}");
            return await _client.GetFromJsonAsync<RoomSurfacePaintLayer>($"api/RoomSurfacePaintLayers/{id}");
        }

        /// <summary>
        /// Creates a room surface paint layer record.
        /// </summary>
        public async Task CreateRoomSurfacePaintLayerAsync(RoomSurfacePaintLayer dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfacePaintLayers");
            using var httpResponseMsg = await _client.PostAsJsonAsync("api/RoomSurfacePaintLayers", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Creating RoomSurfacePaintLayer.");
        }

        /// <summary>
        /// Updates a room surface paint layer record.
        /// </summary>
        public async Task UpdateRoomSurfacePaintLayerAsync(RoomSurfacePaintLayer dataObj)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfacePaintLayers/{dataObj.RoomSurfacePaintLayerId}");
            using var httpResponseMsg = await _client.PutAsJsonAsync($"api/RoomSurfacePaintLayers/{dataObj.RoomSurfacePaintLayerId}", dataObj);
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Updating RoomSurfacePaintLayer.");
        }

        /// <summary>
        /// Deletes a room surface paint layer record.
        /// </summary>
        public async Task DeleteRoomSurfacePaintLayerAsync(int id)
        {
            _logger.LogInformation($"API Call - {_client.BaseAddress}api/RoomSurfacePaintLayers/{id}");
            using var httpResponseMsg = await _client.DeleteAsync($"api/RoomSurfacePaintLayers/{id}");
            if (!httpResponseMsg.IsSuccessStatusCode)
                throw new Exception("Error Deleting RoomSurfacePaintLayer.");
        }

        #endregion
    }
}

