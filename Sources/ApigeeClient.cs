using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ApigeeSDK.Models;
using ApigeeSDK.Services;
using ApigeeSDK.Exceptions;
using System.Net.Http;

namespace ApigeeSDK
{
    public class ApigeeClient 
    {
        private readonly string _baseUrl;        
        private readonly int _entitiesLimit;
        private readonly string _organizationName;
        private readonly string _environmentName;

        private readonly HttpClient _http;

        private readonly HttpServiceAuthenticated _httpService;        

        public ApigeeClient(ApigeeClientOptions options, HttpClient http = null)
        {
            ValidateOptions(options);

            if (http == null)
            {
                _http = new HttpClient
                {
                    Timeout = options.HttpTimeout
                };
            }
            else
            {
                _http = http;
            }

            _organizationName = options.OrgName;
            _environmentName = options.EnvName;
            _baseUrl = options.BaseUrl;            
            _entitiesLimit = options.EntitiesLimit;
            _httpService = new HttpServiceAuthenticated(options, _http);                                               
        }

        public async Task<List<string>> GetApplicationIds()
        {
            var url = GetAbsoluteUrl($"apps?rows={_entitiesLimit}");

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<Application>> GetApplications()
        {
            var url = GetAbsoluteUrl($"apps?expand=true&rows={_entitiesLimit}");

            return await GetEntitiesByPortions<Application>(url, x => x.ApplicationId.ToString(),
                x => JsonConvert.DeserializeObject<ApplicationsList>(x).Applications);
        }

        public async Task<Application> GetApplication(string applicationId)
        {
            var content = await _httpService.GetAsync(GetAbsoluteUrl($"apps/{applicationId}"));

            return JsonConvert.DeserializeObject<Application>(content);
        }

        public async Task<Application> GetApplication(string developerEmail, string applicationName)
        {
            var content =
                await _httpService.GetAsync(GetAbsoluteUrl($"developers/{developerEmail}/apps/{applicationName}"));

            return JsonConvert.DeserializeObject<Application>(content);
        }

        public async Task<List<string>> GetDeveloperApplicationNames(string developerEmail)
        {
            var url = GetAbsoluteUrl($"developers/{developerEmail}/apps?count={_entitiesLimit}");

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<string>> GetDevelopersEmails()
        {
            var url = GetAbsoluteUrl($"developers?count={_entitiesLimit}");

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<Developer>> GetDevelopers()
        {
            var url = GetAbsoluteUrl($"developers?expand=true&count={_entitiesLimit}");

            return await GetEntitiesByPortions<Developer>(url, x => x.Email, x => JsonConvert.DeserializeObject<DevelopersList>(x).Developers);
        }

        public async Task<List<Company>> GetCompanies()
        {
            var url = GetAbsoluteUrl($"companies?expand=true&count={_entitiesLimit}");

            return await GetEntitiesByPortions<Company>(url, x => x.Name, x => JsonConvert.DeserializeObject<CompanyList>(x).Companies);
        }

        public async Task<Developer> GetDeveloper(string developerEmail)
        {
            var content =
                await _httpService.GetAsync(GetAbsoluteUrl($"developers/{developerEmail}"));

            return JsonConvert.DeserializeObject<Developer>(content);
        }

        public async Task<List<string>> GetApiProductNames()
        {
            var url = GetAbsoluteUrl($"apiproducts?count={_entitiesLimit}");

            return await GetEntitiesByPortions<string>(url, x => x.ToString(), x => JsonConvert.DeserializeObject<List<string>>(x));
        }

        public async Task<List<ApiProduct>> GetApiProducts()
        {
            var url = GetAbsoluteUrl($"apiproducts?expand=true&count={_entitiesLimit}");

            return await GetEntitiesByPortions(
                url, 
                x => x.Name, 
                x => JsonConvert.DeserializeObject<ApiProductsList>(x).ApiProducts);
        }

        public async Task<ApiProduct> GetApiProduct(string productName)
        {
            var url = GetAbsoluteUrl($"apiproducts/{productName}");
            var content = await _httpService.GetAsync(url);

            return JsonConvert.DeserializeObject<ApiProduct>(content);
        }

        public async Task<ImportApiResponse> ImportApiProxy(string name, string pathToBundle)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "multipart/form-data")
            };
            var url = GetAbsoluteUrl($"apis?action=import&name={name}");

            var content = await _httpService.PostFileAsync(url, headerParams, pathToBundle);
            return JsonConvert.DeserializeObject<ImportApiResponse>(content);
        }

        public async Task CreateApiProduct(NewApiProduct apiProduct)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/json")
            };
            var url = GetAbsoluteUrl("apiproducts");

            await _httpService.PostJsonAsync(url, headerParams, JsonConvert.SerializeObject(apiProduct));
        }

        public async Task DeployApiProxy(string apiName, int revisionNumber)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            var url = GetAbsoluteUrl($"environments/{_environmentName}/apis/{apiName}/revisions/{revisionNumber}/deployments");

            await _httpService.PostAsync(url, headerParams);
        }

        public async Task UndeployApiProxy(string apiName, int revisionNumber)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            var url = GetAbsoluteUrl($"{_environmentName}/apis/{apiName}/revisions/{revisionNumber}/deployments");

            await _httpService.DeleteAsync(url, headerParams);
        }

        public async Task ForceDeployApiProxy(string apiName, int revisionNumber)
        {
            var deployedRevisions = await GetDeployedRevisionsForApiProxy(apiName);

            foreach (var revision in deployedRevisions)
            {
                await UndeployApiProxy(apiName, revision);
            }

            await DeployApiProxy(apiName, revisionNumber);
        }

        private void ValidateOptions(ApigeeClientOptions options)
        {
            if (options.EntitiesLimit <= 1)
            {
                throw new ApigeeSdkException("Incorrect entities limit: {options.EntitiesLimit}. Should be more than 1.");
            }
        }

        private async Task<List<int>> GetDeployedRevisionsForApiProxy(string apiName)
        {
            var headerParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
            var url = GetAbsoluteUrl($"environments/{_environmentName}/apis/{apiName}/deployments");

            var revisions = new List<int>();

            try
            {
                var content = await _httpService.GetAsync(url, headerParams);

                return JsonConvert.DeserializeObject<DeploymentDetails>(content)
                    .Revisions.Select(x => x.Name).ToList();
            }
            catch (ApigeeSdkHttpException ex)
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
            var all = new List<T>();
            List<T> portion = null;
            string startKey = null;
            var isFull = false;
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

        private string GetAbsoluteUrl(string relativeUrl)
        {
            return $"{_baseUrl}/v1/o/{_organizationName}/{relativeUrl}";
        }
    }
}
