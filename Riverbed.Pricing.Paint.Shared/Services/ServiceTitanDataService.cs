using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Riverbed.Pricing.Paint.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace Riverbed.Pricing.Paint.Shared.Services
{
    public class ServiceTitanDataService : IServiceTitanDataService
    {
        private readonly ILogger<ServiceTitanDataService> _logger;
        private readonly HttpClient _client;

        public ServiceTitanDataService(ILogger<ServiceTitanDataService> logger)
        {
            _logger = logger;
            _client = new HttpClient();

#if DEBUG
            _client.BaseAddress = new Uri("https://bludog-software.com/rbst/");
#else
            _client.BaseAddress = new Uri("https://bludog-software.com/rbst/");
#endif
            _logger.LogInformation($"Base address - {_client.BaseAddress}");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Bludog Software");
        }


        #region ServiceTitanConnectionData CRUD
        public async Task<List<ServiceTitanConnectionData>> GetAllServiceTitanConnectionDatasAsync()
        {
            // Call the API to get all ServiceTitanConnectionData
            using (var httpResponseMsg = await _client.GetAsync("api/ServiceTitanConnectionDatas"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    return new List<ServiceTitanConnectionData>();
                var data = await httpResponseMsg.Content.ReadFromJsonAsync<List<ServiceTitanConnectionData>>();
                return data ?? new List<ServiceTitanConnectionData>();
            }
        }

        public async Task<ServiceTitanConnectionData?> CreateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj)
        {
            // Since we don't have direct DB write access in our accessor interface,
            // we'll still use the API for write operations
            using (var httpResponseMsg = await _client.PostAsJsonAsync("api/ServiceTitanConnectionDatas", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    return null;

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        public Task<ServiceTitanConnectionData?> GetServiceTitanConnectionDataAsync(string companyGuid)
        {
            // Call the API to get ServiceTitanConnectionData by companyGuid
            return _client.GetFromJsonAsync<ServiceTitanConnectionData>($"api/ServiceTitanConnectionDatas/{companyGuid}");
        }

        public async Task<ServiceTitanConnectionData> UpdateServiceTitanConnectionDataAsync(ServiceTitanConnectionData dataObj)
        {
            using (var httpResponseMsg = await _client.PutAsJsonAsync($"api/ServiceTitanConnectionDatas/{dataObj.Id}", dataObj))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Updating ServiceTitanConnectionData.");

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        public async Task<ServiceTitanConnectionData> DeleteServiceTitanConnectionDataAsync(int id)
        {
            using (var httpResponseMsg = await _client.DeleteAsync($"api/ServiceTitanConnectionDatas/{id}"))
            {
                if (!httpResponseMsg.IsSuccessStatusCode)
                    throw new Exception("Error Deleting ServiceTitanConnectionData.");

                return await httpResponseMsg.Content.ReadFromJsonAsync<ServiceTitanConnectionData>();
            }
        }

        public async Task<string> GetUserCompanyGuidAsync(string email)
        {
            var companyGuid = await _client.GetStringAsync($"api/UserInfo/Company/{email}");
            return companyGuid.Replace("\\", "").Replace("\"", "");
        }
        #endregion
    }
}