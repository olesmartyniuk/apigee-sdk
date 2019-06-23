using System;

namespace ApigeeSDK
{
    public class ApigeeClientOptions 
    {
        public virtual string Email { get; set;  }
        public virtual string Password { get; set;  }
        public virtual string OrgName { get; set;  }
        public virtual string EnvName { get; set; }
        public virtual string BaseUrl { get; set; }
        public virtual int EntitiesLimit { get; set; }
        public virtual TimeSpan HttpTimeout { get; set; }
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
                  TimeSpan.FromSeconds(30))
        {
        }

        public ApigeeClientOptions(
            string email, 
            string password, 
            string orgName, 
            string envName, 
            string baseUrl, 
            string authenticationUrl, 
            TimeSpan httpTimeout)
        {
            Email = email;
            Password = password;
            OrgName = orgName;
            EnvName = envName;
            BaseUrl = baseUrl;
            AuthenticationUrl = authenticationUrl;
            HttpTimeout = httpTimeout;            
            EntitiesLimit = 1000;
        }
    }
}