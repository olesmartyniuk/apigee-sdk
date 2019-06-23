using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
