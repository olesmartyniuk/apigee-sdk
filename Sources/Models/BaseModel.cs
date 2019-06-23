using System;
using ApigeeSDK.Utils;
using Newtonsoft.Json;

namespace ApigeeSDK.Models
{
    public class BaseModel
    {
        [JsonProperty("createdAt")]
        public long CreatedAt { get; set; }

        public DateTime CreatedAtDateTime => DateTimeUtils.UnixTimeStampToDateTime(CreatedAt);

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("lastModifiedAt")]
        public long LastModifiedAt { get; set; }

        public DateTime LastModifiedAtDateTime => DateTimeUtils.UnixTimeStampToDateTime(LastModifiedAt);

        [JsonProperty("lastModifiedBy")]
        public string LastModifiedBy { get; set; }
    }
}
