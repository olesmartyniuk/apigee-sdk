using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApigeeSDK.Models
{
    public class NewApiProduct
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("approvalType")]
        public ApprovalType ApprovalType { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("apiResources")]
        public List<string> ApiResources { get; set; } = new List<string>();

        [JsonProperty("environments")]
        public List<string> Environments { get; set; } = new List<string>();

        [JsonProperty("proxies")]
        public List<string> Proxies { get; set; } = new List<string>();

        [JsonProperty("quota")]
        public string Quota { get; set; }

        [JsonProperty("quotaInterval")]
        public string QuotaInterval { get; set; }

        [JsonProperty("quotaTimeUnit")]
        public string QuotaTimeUnit { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; } = new List<string>();
    }
}
