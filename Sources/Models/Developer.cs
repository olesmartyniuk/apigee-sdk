using System;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class Developer : BaseModel
    {
        [JsonProperty("developerId")]
        public string DeveloperId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public bool IsActive => string.Equals(Status, "active", StringComparison.InvariantCultureIgnoreCase);

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
