using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApigeeSDK.Models
{
    public class Revision
    {
        [JsonProperty("name")]
        public int Name { get; set; }
    }
}
