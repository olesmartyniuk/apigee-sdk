using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class ApplicationsList
    {
        [JsonProperty("app")]
        public List<Application> Applications { get; set; }
    }
}
