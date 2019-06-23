using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class CompanyList
    {
        [JsonProperty("company")]
        public List<Company> Companies { get; set; }
    }
}
