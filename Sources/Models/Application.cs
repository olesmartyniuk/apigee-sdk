using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class Application : BaseModel
    {
        [JsonProperty("appId")]
        public string ApplicationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }

        public string DisplayName
        {
            get
            {
                if (Attributes != null)
                {
                    foreach (var attribute in Attributes)
                    {
                        if (string.Equals(attribute.Name, "DisplayName", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return attribute.Value;
                        }
                    }
                }

                return string.Empty;
            }
        }

        [JsonProperty("status")]
        public string Status { get; set; }

        public bool IsApproved => string.Equals(Status, "approved", StringComparison.InvariantCultureIgnoreCase);

        [JsonProperty("developerId")]
        public string DeveloperId { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
    }
}
