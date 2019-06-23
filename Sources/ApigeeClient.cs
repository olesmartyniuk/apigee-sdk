using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ApigeeSDK.Models;
using ApigeeSDK.Services;
using ApigeeSDK.Exceptions;

namespace ApigeeSDK
{
    public class ApigeeClient 
    {
        private string _baseUrl;
        private string _authenticationUrl;
        private int _entitiesLimit;
        private string _organizationName;
        private string _environmentName;

        private HttpServiceAuthenticated _httpService;

        private string BaseUrlWithOrg => _baseUrl + $"/v1/o/{_organizationName}";        

        public ApigeeClient(ApigeeClientOptions options)
        {
            _organizationName = options.OrgName;
            _environmentName = options.EnvName;
            _baseUrl = options.BaseUrl;
            _authenticationUrl = options.AuthenticationUrl;
            _entitiesLimit = options.EntitiesLimit;
            
            var tokenProvider = new TokenProvider(_authenticationUrl, options.HttpTimeout, options.Email, options.Password);                
            _httpService = new HttpServiceAuthenticated(new HttpService(options.HttpTimeout), tokenProvider);                                                
        }

        public async Task<List<string>> GetApplicationIds()
        {
            var url = BaseUrlWithOrg + $"/apps?rows={_entitiesLimit}";

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<Application>> GetApplications()
        {
            var url = BaseUrlWithOrg + $"/apps?expand=true&rows={_entitiesLimit}";

            return await GetEntitiesByPortions<Application>(url, x => x.ApplicationId.ToString(),
                x => JsonConvert.DeserializeObject<ApplicationsList>(x).Applications);
        }

        public async Task<Application> GetApplication(string applicationId)
        {
            var content = await _httpService.GetAsync(BaseUrlWithOrg + $"/apps/{applicationId}");

            return JsonConvert.DeserializeObject<Application>(content);
        }

        public async Task<Application> GetApplication(string developerEmail, string applicationName)
        {
            var content =
                await _httpService.GetAsync(BaseUrlWithOrg + $"/developers/{developerEmail}/apps/{applicationName}");

            return JsonConvert.DeserializeObject<Application>(content);
        }

        public async Task<List<string>> GetDeveloperApplicationNames(string developerEmail)
        {
            var url = BaseUrlWithOrg + $"/developers/{developerEmail}/apps?count={_entitiesLimit}";

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<string>> GetDevelopersEmails()
        {
            var url = BaseUrlWithOrg + $"/developers?count={_entitiesLimit}";

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<Developer>> GetDevelopers()
        {
            var url = BaseUrlWithOrg + $"/developers?expand=true&count={_entitiesLimit}";

            return await GetEntitiesByPortions<Developer>(url, x => x.Email, x => JsonConvert.DeserializeObject<DevelopersList>(x).Developers);
        }

        public async Task<List<Company>> GetCompanies()
        {
            var url = BaseUrlWithOrg + $"/companies?expand=true&count={_entitiesLimit}";

            return await GetEntitiesByPortions<Company>(url, x => x.Name, x => JsonConvert.DeserializeObject<CompanyList>(x).Companies);
        }

        public async Task<Developer> GetDeveloper(string developerEmail)
        {
            var content =
                await _httpService.GetAsync(BaseUrlWithOrg + $"/developers/{developerEmail}");

            return JsonConvert.DeserializeObject<Developer>(content);
        }

        public async Task<List<string>> GetApiProductNames()
        {
            var url = BaseUrlWithOrg + $"/apiproducts?count={_entitiesLimit}";

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<ApiProduct>> GetApiProducts()
        {
            var url = BaseUrlWithOrg + $"/apiproducts?expand=true&count={_entitiesLimit}";

            return await GetEntitiesByPortions<ApiProduct>(url, x => x.Name, x => JsonConvert.DeserializeObject<ApiProductsList>(x).ApiProducts);
        }

        public async Task<ApiProduct> GetApiProduct(string productName)
        {
            var url = BaseUrlWithOrg + $"/apiproducts/{productName}";
            var content = await _httpService.GetAsync(url);

            return JsonConvert.DeserializeObject<ApiProduct>(content);
        }

        public async Task<ImportApiResponse> ImportApiProxy(string name, string pathToBundle)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "multipart/form-data")
            };
            string url = BaseUrlWithOrg + $"/apis?action=import&name={name}";

            var content = await _httpService.PostFileAsync(url, headerParams, pathToBundle);
            return JsonConvert.DeserializeObject<ImportApiResponse>(content);
        }

        public async Task CreateApiProduct(NewApiProduct apiProduct)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/json")
            };
            var url = BaseUrlWithOrg + $"/apiproducts";

            await _httpService.PostJsonAsync(url, headerParams, JsonConvert.SerializeObject(apiProduct));
        }

        public async Task DeployApiProxy(string apiName, int revisionNumber)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            var url = BaseUrlWithOrg + $"/environments/{_environmentName}/apis/{apiName}/revisions/{revisionNumber}/deployments";

            await _httpService.PostAsync(url, headerParams);
        }

        public async Task UndeployApiProxy(string apiName, int revisionNumber)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            string url = BaseUrlWithOrg + $"/{_environmentName}/apis/{apiName}/revisions/{revisionNumber}/deployments";

            await _httpService.DeleteAsync(url, headerParams);
        }

        public async Task ForceDeployApiProxy(string apiName, int revisionNumber)
        {
            var deployedRevisions = await GetDeployedRevisionsForApiProxy(apiName);

            foreach (int revision in deployedRevisions)
            {
                await UndeployApiProxy(apiName, revision);
            }

            await DeployApiProxy(apiName, revisionNumber);
        }

        private async Task<List<int>> GetDeployedRevisionsForApiProxy(string apiName)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            string url = BaseUrlWithOrg + $"/environments/{_environmentName}/apis/{apiName}/deployments";

            List<int> revisions = new List<int>();

            try
            {
                var content = await _httpService.GetAsync(url, headerParams);

                return JsonConvert.DeserializeObject<DeploymentDetails>(content)
                    .Revisions.Select(x => x.Name).ToList();
            }
            catch (ApigeeSDKHttpException ex)
                when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = JsonConvert.DeserializeObject<Error>(ex.Message);
                if (error.Code == ErrorCodes.ApplicationNotDeployed)
                {
                    return new List<int>();
                }

                throw;
            }
        }

        private async Task<List<T>> GetEntitiesByPortions<T>(string url, Func<T, string> getKeyFunc, Func<string, List<T>> parserFunc)
        {
            List<T> all = new List<T>();
            List<T> portion = null;
            string startKey = "elexander@ukr.net";
            bool isFull = false;
            do
            {
                if (portion != null)
                {
                    startKey = getKeyFunc(portion.Last());
                }

                portion = await GetPortionOfEntities<T>(url, startKey, parserFunc);

                all.AddRange(portion);

                isFull = portion.Count == _entitiesLimit;

                if (isFull)
                {
                    all.RemoveAt(all.Count - 1);
                }
            } while (isFull);

            return all;
        }

        private async Task<List<T>> GetPortionOfEntities<T>(string url, string startKey, Func<string, List<T>> parserFunc)
        {
            if (!string.IsNullOrWhiteSpace(startKey))
            {
                url = $"{url}&startKey={startKey}";
            }

            var content = await _httpService.GetAsync(url);

            return parserFunc(content);
        }
    }
}
