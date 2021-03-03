using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApigeeSDK.Models
{
    public class DeploymentDetails
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("revision")]
        public List<Revision> Revisions { get; set; }
    }
}
