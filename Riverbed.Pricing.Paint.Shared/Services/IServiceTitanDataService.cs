// Riverbed.ServiceTitanService\Services\IServiceTitanDataService.cs
using Riverbed.Pricing.Paint.Entities;

namespace Riverbed.Pricing.Paint.Shared.Services
{
    public interface IServiceTitanDataService
    {
        // Methods for ServiceTitanConnectionData
        Task<List<ServiceTitanConnectionData>> GetAllServiceTitanConnectionDatasAsync();
        Task<ServiceTitanConnectionData?> CreateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj);
        Task<ServiceTitanConnectionData> UpdateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj);
        Task<ServiceTitanConnectionData> DeleteServiceTitanConnectionDataAsync(int id);
        Task<string> GetUserCompanyGuidAsync(string email);
        Task<ServiceTitanConnectionData?> GetServiceTitanConnectionDataAsync(string companyGuid);
    }
}