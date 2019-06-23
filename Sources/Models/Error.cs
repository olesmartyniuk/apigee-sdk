using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("contexts")]
        public List<object> Contexts { get; set; }
    }
}
