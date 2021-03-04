using System;

namespace ApigeeSDK.Models
{
    public class Statistics
    {
        public string ProductId { get; set; }

        public string ApplicationId { get; set; }

        public string CountryCode { get; set; }
        /// <summary>
        /// Numbers of calls.
        /// </summary>
        public double CallsAll { get; set; }
        /// <summary>
        /// Failed calls can be provoked by either API Proxy or underlying service.
        /// </summary>
        public double CallsErrors { get; set; }
    
        /// <summary>
        /// Minimum response time.
        /// </summary>
        public double MinResponseTime { get; set; }

        /// <summary>
        /// Maximum response time.
        /// </summary>
        public double MaxResponseTime { get; set; }

        public DateTime PeriodBegin { get; set; }

        public DateTime PeriodEnd { get; set; }

        public bool IsEmpty()
        {
            return (CallsAll + CallsErrors + MinResponseTime + MaxResponseTime) == 0;
        }
    }
}
