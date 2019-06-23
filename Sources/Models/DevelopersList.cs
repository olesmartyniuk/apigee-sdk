using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class DevelopersList
    {
        [JsonProperty("developer")]
        public List<Developer> Developers { get; set; }
    }
}
