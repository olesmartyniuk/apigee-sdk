using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class Revision
    {
        [JsonProperty("name")]
        public int Name { get; set; }
    }
}
