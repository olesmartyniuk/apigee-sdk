using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class ApiProduct : BaseModel
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; set; }

        public bool IsPublic
        {
            get
            {
                if (Attributes != null)
                {
                    foreach (var attribute in Attributes)
                    {
                        if (string.Equals(attribute.Name, "access", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return string.Equals(attribute.Value, "public", StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                }

                return false;
            }
        }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
