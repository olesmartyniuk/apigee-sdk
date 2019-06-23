using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity;

namespace ApigeeSDK.Unit.Tests
{
    public class GetDevelopersEmailsShould : ApigeeClientTestsBase
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
        public void ReturnListOfDeveloperNamesForValidJson()
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

            string url = BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> developerNames = apigeeService.GetDevelopersEmails().Result;

            Assert.AreEqual(expectedList.Count, developerNames.Count);
            Assert.AreEqual(expectedList[0], developerNames[0]);
            Assert.AreEqual(expectedList[1], developerNames[1]);
            Assert.AreEqual(expectedList[2], developerNames[2]);
        }

        [Test]
        public void ReturnEmptyListOfDeveloperNamesForEmptyJson()
        {
            string json = @"[]";


            string url = BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}";

            var apigeeService = this.GetInitializedApigeeService(url, json);

            List<string> developerNames = apigeeService.GetDevelopersEmails().Result;

            Assert.AreEqual(0, developerNames.Count);
        }

        [Test]
        public void ReturnListOfDeveloperNamesByPortions()
        {
            string jsonPortion1 = @"['name1', 'name2', 'name3' ]".QuotesToDoubleQuotes();
            string jsonPortion2 = @"['name3', 'name4' ]".QuotesToDoubleQuotes();

            var expectedList = new List<string>()
            {
                "name1", "name2", "name3", "name4"
            };
            int testEntitiesLimit = 3;
            string urlForPortion1 = BaseUrl + $"/v1/o/{OrgName}/developers?count={testEntitiesLimit}";
            string urlForPortion2 = BaseUrl + $"/v1/o/{OrgName}/developers?count={testEntitiesLimit}&startKey=name3";

            this.RegisterUrlAndJson(urlForPortion1, jsonPortion1);
            this.RegisterUrlAndJson(urlForPortion2, jsonPortion2);
            _apigeeClientOptionsMock.Setup(x => x.EntitiesLimit).Returns(testEntitiesLimit);
            var apigeeService = Container.Resolve<ApigeeClient>();

            List<string> developerNames = apigeeService.GetDevelopersEmails().Result;

            Assert.AreEqual(expectedList.Count, developerNames.Count);
            Assert.AreEqual(expectedList[0], developerNames[0]);
            Assert.AreEqual(expectedList[1], developerNames[1]);
            Assert.AreEqual(expectedList[2], developerNames[2]);
        }

        [Test]
        public void ThrowJsonSerializationExceptionIfJsonIsInvalid()
        {
            string invalidJson = @"[
                    '11111111-1111-1111-1111-111111111
                    '33333333-3333-3333-3333-333333333333'
                ".QuotesToDoubleQuotes();
            
            var apigeeService = this.GetInitializedApigeeService(
                BaseUrl + $"/v1/o/{OrgName}/developers?count={entitiesLimit}",
                invalidJson);

            Assert.ThrowsAsync(Is.InstanceOf<Newtonsoft.Json.JsonException>(), async () =>
                await apigeeService.GetDevelopersEmails());
        }
    }
}
