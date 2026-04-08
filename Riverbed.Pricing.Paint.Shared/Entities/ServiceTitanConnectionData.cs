using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Entities
{
    public class ServiceTitanConnectionData 
    {
        public int Id { get; set; }
        public string ServiceTitanApiUrl { get; set; }
        public string ServiceTitanApiVersion { get; set; }
        public string ServiceTitanApiClientId { get; set; }
        public long ServiceTitanTenantId { get; set; }
        public string ServiceTitanSecretKey { get; set; }
        public string ServiceTitanAppKey { get; set; }
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
        public bool IsActive { get; set; }
        public int TimeIntervalInSeconds { get; set; }
        public DateTime LastSyncDate { get; set; }
    }
}