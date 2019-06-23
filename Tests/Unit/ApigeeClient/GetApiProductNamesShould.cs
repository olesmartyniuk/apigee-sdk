using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    class GetApiProductNamesShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();

            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);            
        }

        [Test]
        public async Task ReturnListOfApiProductsNamesForValidJson()
        {
            string json = @"[
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

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);

            var apiProductNames = await apigeeService.GetApiProductNames();

            Assert.AreEqual(expectedList.Count, apiProductNames.Count);
            Assert.AreEqual(expectedList[0], apiProductNames[0]);
            Assert.AreEqual(expectedList[1], apiProductNames[1]);
            Assert.AreEqual(expectedList[2], apiProductNames[2]);
        }

        [Test]
        public async Task ReturnEmptyListOfApiProductsNamesForEmptyJson()
        {
            var json = @"[ ]";

            var url = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={entitiesLimit}";
            
            var apigeeService = GetInitializedApigeeService(url, json);

            List<string> apiProductNames = await apigeeService.GetApiProductNames();

            Assert.AreEqual(0, apiProductNames.Count);
        }

        [Test]
        public async Task ReturnListOfApiProductsNamesByPortions()
        {
            var jsonPortion1 = @"[ 'name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            var jsonPortion2 = @"[ 'name3' , 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>() { "name1", "name2", "name3", "name4" };

            var testEntitiesLimit = 3;
            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={testEntitiesLimit}&startKey=name3";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var apiProductNames = await apigeeService.GetApiProductNames();

            Assert.AreEqual(expectedList.Count, apiProductNames.Count);
            Assert.AreEqual(expectedList[0], apiProductNames[0]);
            Assert.AreEqual(expectedList[1], apiProductNames[1]);
            Assert.AreEqual(expectedList[2], apiProductNames[2]);
            Assert.AreEqual(expectedList[3], apiProductNames[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/apiproducts?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetApiProductNames());
        }
    }
}
