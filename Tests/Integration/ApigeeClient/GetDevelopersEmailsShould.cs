using System.Collections.Generic;
using System.Threading.Tasks;
using ApigeeSDK.Integration.Tests.Utils;
using Xunit;

namespace ApigeeSDK.Integration.Tests.ApigeeClient
{
    public class GetDevelopersEmailsShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfDeveloperNamesForValidJson()
        {
            var json = @"[
                    'name1',
                    'name2',
                    'name3'
                ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1",
                "name2",
                "name3"
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/developers?count={EntitiesLimit}";

            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.Equal(expectedList.Count, developerNames.Count);
            Assert.Equal(expectedList[0], developerNames[0]);
            Assert.Equal(expectedList[1], developerNames[1]);
            Assert.Equal(expectedList[2], developerNames[2]);
        }

        [Fact]
        public async Task ReturnEmptyListOfDeveloperNamesForEmptyJson()
        {
            var json = @"[]";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers?count={EntitiesLimit}";

            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.Empty(developerNames);
        }

        [Fact]
        public async Task ReturnListOfDeveloperNamesByPortions()
        {
            var jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };

            EntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers?count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers?count={EntitiesLimit}&startKey=name3";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var developerNames = await apigeeService.GetDevelopersEmails();

            Assert.Equal(expectedList.Count, developerNames.Count);
            Assert.Equal(expectedList[0], developerNames[0]);
            Assert.Equal(expectedList[1], developerNames[1]);
            Assert.Equal(expectedList[2], developerNames[2]);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/developers?count={EntitiesLimit}", invalidJson);
            
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetDevelopersEmails());
        }
    }
}
