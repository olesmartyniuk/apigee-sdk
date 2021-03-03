using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApplicationIdsShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);            
        }

        [Test]
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

            var url = BaseUrl + $"/v1/o/{OrgName}/apps?rows={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, json);
            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.AreEqual(expectedList.Count, applicationIds.Count);
            Assert.AreEqual(expectedList[0], applicationIds[0]);
            Assert.AreEqual(expectedList[1], applicationIds[1]);
            Assert.AreEqual(expectedList[2], applicationIds[2]);
        }

        [Test]
        public async Task ReturnEmptyListForEmptyJson()
        {
            var emptyJson = @"[]";

            var url = BaseUrl + $"/v1/o/{OrgName}/apps?rows={entitiesLimit}";

            var apigeeService = GetInitializedApigeeService(url, emptyJson);

            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.AreEqual(0, applicationIds.Count);
        }

        [Test]
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

            var testEntitiesLimit = 3;

            var urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/apps?rows={testEntitiesLimit}";
            var urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/apps?rows={testEntitiesLimit}&startKey=33333333-3333-3333-3333-333333333333";

            RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            RegisterUrlAndJson(urlForPortion2, jsonPortion2);

            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);

            var apigeeService = Container.Resolve<ApigeeClient>();
            var applicationIds = await apigeeService.GetApplicationIds();

            Assert.AreEqual(expectedList.Count, applicationIds.Count);
            Assert.AreEqual(expectedList[0], applicationIds[0]);
            Assert.AreEqual(expectedList[1], applicationIds[1]);
            Assert.AreEqual(expectedList[2], applicationIds[2]);
            Assert.AreEqual(expectedList[3], applicationIds[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            var invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService  = this.GetInitializedApigeeService(BaseUrl + $"/v1/o/{OrgName}/apps?rows={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () => await apigeeService.GetApplicationIds());
        }
    }
}
