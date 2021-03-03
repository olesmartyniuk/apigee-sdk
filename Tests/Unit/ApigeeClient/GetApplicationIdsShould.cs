using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApplicationIdsShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfApplicationIdsForValidJson()
        {
            var json = @"[
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333'
                ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "11111111-1111-1111-1111-111111111111",
                "22222222-2222-2222-2222-222222222222",
                "33333333-3333-3333-3333-333333333333"
            };

            var url = BaseUrl + $"/v1/o/{OrgName}/apps?rows={EntitiesLimit}";
            RegisterUrl(url, json);
            var apigeeService = GetApigeeClient();
            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.Equal(expectedList.Count, applicationIds.Count);
            Assert.Equal(expectedList[0], applicationIds[0]);
            Assert.Equal(expectedList[1], applicationIds[1]);
            Assert.Equal(expectedList[2], applicationIds[2]);
        }

        [Fact]
        public async Task ReturnEmptyListForEmptyJson()
        {
            var emptyJson = @"[]";

            var url = BaseUrl + $"/v1/o/{OrgName}/apps?rows={EntitiesLimit}";
            RegisterUrl(url, emptyJson);
            var apigeeService = GetApigeeClient();

            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.Empty(applicationIds);
        }

        [Fact]
        public async Task ReturnListOfApplicationIdsByPortions()
        {
            var jsonPortion1 = @"[
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333'
                ]".QuotesToDoubleQuotes();

            var jsonPortion2 = @"[
                    '33333333-3333-3333-3333-333333333333',
                    '44444444-4444-4444-4444-444444444444'
                ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "11111111-1111-1111-1111-111111111111",
                "22222222-2222-2222-2222-222222222222",
                "33333333-3333-3333-3333-333333333333",
                "44444444-4444-4444-4444-444444444444"
            };

            EntitiesLimit = 3;

            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apps?rows={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apps?rows={EntitiesLimit}&startKey=33333333-3333-3333-3333-333333333333";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.Equal(expectedList.Count, applicationIds.Count);
            Assert.Equal(expectedList[0], applicationIds[0]);
            Assert.Equal(expectedList[1], applicationIds[1]);
            Assert.Equal(expectedList[2], applicationIds[2]);
            Assert.Equal(expectedList[3], applicationIds[3]);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/apps?rows={EntitiesLimit}", invalidJson);

            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () => await apigeeService.GetApplicationIds());
        }
    }
}
