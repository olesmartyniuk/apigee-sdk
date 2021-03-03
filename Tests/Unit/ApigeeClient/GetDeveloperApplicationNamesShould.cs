using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDeveloperApplicationNamesShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfDeveloperApplicationNamesForValidJson()
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


            var developerEmail = "developerEmail@email.com";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.Equal(expectedList.Count, applicationNames.Count);
            Assert.Equal(expectedList[0], applicationNames[0]);
            Assert.Equal(expectedList[1], applicationNames[1]);
            Assert.Equal(expectedList[2], applicationNames[2]);
        }

        [Fact]
        public async Task ReturnEmptyListOfDeveloperApplicationNamesForEmptyJson()
        {
            var json = @"[]";

            var developerEmail = "developerEmail@email.com";
            var url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.Empty(applicationNames);
        }

        [Fact]
        public async Task ReturnListOfDeveloperApplicationNamesByPortions()
        {
            var jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };

            var developerEmail = "developerEmail@email.com";
            EntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={EntitiesLimit}&startKey=name3";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var applicationNames = await apigeeService.GetDeveloperApplicationNames(developerEmail);

            Assert.Equal(expectedList.Count, applicationNames.Count);
            Assert.Equal(expectedList[0], applicationNames[0]);
            Assert.Equal(expectedList[1], applicationNames[1]);
            Assert.Equal(expectedList[2], applicationNames[2]);
            Assert.Equal(expectedList[3], applicationNames[3]);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            var developerEmail = "developerEmail@email.com";
            RegisterUrl(BaseUrl +
                        $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={EntitiesLimit}", invalidJson);

            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetDeveloperApplicationNames(developerEmail));
        }
    }
}
