using NUnit.Framework;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetApplicationIdsShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;
        private const int statisticsLimit = 14000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            apigeeServiceOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);            
        }

        [Test]
        public void ReturnListOfApplicationIdsForValidJson()
        {
            string json = @"[
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

            string url = baseUri + $"/v1/o/{orgName}/apps?rows={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> applicationIds = apigeeService.GetApplicationIds().Result;

            Assert.AreEqual(expectedList.Count, applicationIds.Count);
            Assert.AreEqual(expectedList[0], applicationIds[0]);
            Assert.AreEqual(expectedList[1], applicationIds[1]);
            Assert.AreEqual(expectedList[2], applicationIds[2]);
        }

        [Test]
        public void ReturnEmptyListForEmptyJson()
        {
            string emptyJson = @"[]";

            string url = baseUri + $"/v1/o/{orgName}/apps?rows={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, emptyJson);

            List<string> applicationIds = apigeeService.GetApplicationIds().Result;

            Assert.AreEqual(0, applicationIds.Count);
        }

        [Test]
        public void ReturnListOfApplicationIdsByPortions()
        {
            string jsonPortion1 = @"[
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333'
                ]".QuotesToDoubleQuotes();

            string jsonPortion2 = @"[
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

            int testEntitiesLimit = 3;

            string urlForPortion1 = baseUri + $"/v1/o/{orgName}/apps?rows={testEntitiesLimit}";
            string urlForPortion2 = baseUri + $"/v1/o/{orgName}/apps?rows={testEntitiesLimit}&startKey=33333333-3333-3333-3333-333333333333";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            apigeeServiceOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<string> applicationIds = apigeeService.GetApplicationIds().Result;

            Assert.AreEqual(expectedList.Count, applicationIds.Count);
            Assert.AreEqual(expectedList[0], applicationIds[0]);
            Assert.AreEqual(expectedList[1], applicationIds[1]);
            Assert.AreEqual(expectedList[2], applicationIds[2]);
            Assert.AreEqual(expectedList[3], applicationIds[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            var apigeeService  = this.GetInitializedApigeeService(baseUri + $"/v1/o/{orgName}/apps?rows={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () => await apigeeService.GetApplicationIds());
        }
    }
}
