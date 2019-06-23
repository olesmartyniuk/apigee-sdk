using System;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class Company : BaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public bool IsActive => string.Equals(Status, "active", StringComparison.InvariantCultureIgnoreCase);
    }
}