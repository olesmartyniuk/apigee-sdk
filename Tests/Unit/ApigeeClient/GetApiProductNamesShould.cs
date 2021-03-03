using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApiProductNamesShould : ApigeeClientTestsBase
    {
        [Fact]
        public async Task ReturnListOfApiProductsNamesForValidJson()
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

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={EntitiesLimit}";
            RegisterUrl(url, json);
            
            var apiProductNames = await GetApigeeClient().GetApiProductNames();

            Assert.Equal(expectedList.Count, apiProductNames.Count);
            Assert.Equal(expectedList[0], apiProductNames[0]);
            Assert.Equal(expectedList[1], apiProductNames[1]);
            Assert.Equal(expectedList[2], apiProductNames[2]);
        }

        [Fact]
        public async Task ReturnEmptyListOfApiProductsNamesForEmptyJson()
        {
            const string json = @"[ ]";

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={EntitiesLimit}";
            RegisterUrl(url, json);
            var client = GetApigeeClient();

            var apiProductNames = await client.GetApiProductNames();

            Assert.Equal(0, apiProductNames.Count);
        }

        [Fact]
        public async Task ReturnListOfApiProductsNamesByPortions()
        {
            var jsonPortion1 = @"[ 'name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"[ 'name3' , 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>() { "name1", "name2", "name3", "name4" };

            EntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={EntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={EntitiesLimit}&startKey=name3";

            RegisterUrl(urlForPortion2, jsonPortion2);
            RegisterUrl(urlForPortion1, jsonPortion1);

            var apigeeService = GetApigeeClient();
            var apiProductNames = await apigeeService.GetApiProductNames();

            Assert.Equal(expectedList.Count, apiProductNames.Count);
            Assert.Equal(expectedList[0], apiProductNames[0]);
            Assert.Equal(expectedList[1], apiProductNames[1]);
            Assert.Equal(expectedList[2], apiProductNames[2]);
            Assert.Equal(expectedList[3], apiProductNames[3]);
        }

        [Fact]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            RegisterUrl(BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={EntitiesLimit}", invalidJson);
            var apigeeService = GetApigeeClient();

            Assert.ThrowsAsync<Newtonsoft.Json.JsonException>(async () =>
                await apigeeService.GetApiProductNames());
        }
    }
}
