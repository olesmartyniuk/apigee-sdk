using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class ApiProductsList
    {
        [JsonProperty("apiProduct")]
        public List<ApiProduct> ApiProducts { get; set; }
    }
}
