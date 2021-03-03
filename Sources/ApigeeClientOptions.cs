using System;

namespace ApigeeSDK
{
    public class ApigeeClientOptions 
    {
        public string Email { get; set;  }
        public string Password { get; set;  }
        public string OrgName { get; set;  }
        public string EnvName { get; set; }
        public string BaseUrl { get; set; }
        public int EntitiesLimit { get; set; }
        public TimeSpan HttpTimeout { get; set; }
        public string AuthenticationUrl { get; set; }

        public ApigeeClientOptions(
            string email, 
            string password, 
            string orgName, 
            string envName) 
            : this(
                  email, 
                  password, 
                  orgName, 
                  envName, 
                  "https://api.enterprise.apigee.com", 
                  "https://login.apigee.com/oauth/token", 
                  TimeSpan.FromSeconds(30),
                  1000)
        {
        }

        public ApigeeClientOptions(
            string email, 
            string password, 
            string orgName, 
            string envName, 
            string baseUrl, 
            string authenticationUrl, 
            TimeSpan httpTimeout,
            int entitiesLimit)
        {
            Email = email;
            Password = password;
            OrgName = orgName;
            EnvName = envName;
            BaseUrl = baseUrl;
            AuthenticationUrl = authenticationUrl;
            HttpTimeout = httpTimeout;            
            EntitiesLimit = entitiesLimit;
        }
    }
}