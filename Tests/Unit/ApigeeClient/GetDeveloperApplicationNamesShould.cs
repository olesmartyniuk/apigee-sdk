using NUnit.Framework;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDeveloperApplicationNamesShould : ApigeeClientTestsBase
    {
        private const int entitiesLimit = 1000;
        private const int statisticsLimit = 14000;

        [SetUp]
        protected override void Init()
        {
            base.Init();
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(entitiesLimit);
        }

        [Test]
        public void ReturnListOfDeveloperApplicationNamesForValidJson()
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


            string developerEmail = "developerEmail@email.com";
            string url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> applicationNames =
                apigeeService.GetDeveloperApplicationNames(developerEmail).Result;

            Assert.AreEqual(expectedList.Count, applicationNames.Count);
            Assert.AreEqual(expectedList[0], applicationNames[0]);
            Assert.AreEqual(expectedList[1], applicationNames[1]);
            Assert.AreEqual(expectedList[2], applicationNames[2]);
        }

        [Test]
        public void ReturnEmptyListOfDeveloperApplicationNamesForEmptyJson()
        {
            string json = @"[]";

            string developerEmail = "developerEmail@email.com";
            string url = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> applicationNames =
                apigeeService.GetDeveloperApplicationNames(developerEmail).Result;

            Assert.AreEqual(0, applicationNames.Count);
        }

        [Test]
        public void ReturnListOfDeveloperApplicationNamesByPortions()
        {
            string jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            string jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();


            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };


            string developerEmail = "developerEmail@email.com";
            int testEntitiesLimit = 3;
            string urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={testEntitiesLimit}";
            string urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={testEntitiesLimit}&startKey=name3";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<string> applicationNames =
                apigeeService.GetDeveloperApplicationNames(developerEmail).Result;

            Assert.AreEqual(expectedList.Count, applicationNames.Count);
            Assert.AreEqual(expectedList[0], applicationNames[0]);
            Assert.AreEqual(expectedList[1], applicationNames[1]);
            Assert.AreEqual(expectedList[2], applicationNames[2]);
            Assert.AreEqual(expectedList[3], applicationNames[3]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();

            string developerEmail = "developerEmail@email.com";

            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl +
                $"/v1/o/{OrgName}/developers/{developerEmail}/apps?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDeveloperApplicationNames(developerEmail));
        }
    }
}
