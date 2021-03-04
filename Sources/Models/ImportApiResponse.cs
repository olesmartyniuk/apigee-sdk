using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class ImportApiResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }
    }
}
