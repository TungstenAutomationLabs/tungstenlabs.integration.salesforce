using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using tungstenlabs.integration.salesforce;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace TestApiHelper
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string sdk = @"https://ktacloudeco-dev.ktaprt.dev.kofaxcloud.com/Services/Sdk";
            string sessionID = @"D2A967C768C7854B91C210DF77F118A4";
            List<string> list = new List<string>() { "SFDC-ACCESS-TOKEN", "SFDC-INSTANCE-URL"};

            APIHelper aPIHelper = new APIHelper();
            ServerVariableHelper serverVariableHelper = new ServerVariableHelper();

            string clientid = "3MVG9JJwBBbcN47IjlhkurcqzwmOP61SwVsSZqc7Uznk08UusZKIL0SoyXSTegkaQfkYwED6BCCxQzWntupow";
            string clientsecret = "10D5D69BA3BD101C8F7869E7986C201CF94FB1D32A70C5611D3A733029657737";
            string sfurl = "https://connect-efficiency-4062-dev-ed.scratch.my.salesforce.com";

            //var jsonData = JObject.Parse(authentication);
            //var accessToken = jsonData["access_token"]?.ToString();
            //var instanceUrl = jsonData["instance_url"]?.ToString();
            //bool result = aPIHelper.Initialize(sfurl, clientid, clientsecret, sessionID, sdk);

            string eventname = "tungstenconnect__JobResponse__e";
            string payload = "{\r\n  \"tungstenconnect__Process_Name__c\": \"CreateAccountProcess\",\r\n  \"tungstenconnect__Related_Object__c\": \"Account\",\r\n  \"tungstenconnect__Response_Message__c\": \"{\\\"AccountName\\\":\\\"ABC Corp\\\",\\\"AccountType\\\":\\\"Customer\\\",\\\"Industry\\\":\\\"Technology\\\",\\\"AnnualRevenue\\\":5000000}\",\r\n  \"tungstenconnect__Response_Timestamp__c\": \"2024-11-10T10:00:00Z\"\r\n}";
            var response = aPIHelper.CreatePlatformEvent(eventname, payload, sessionID, sdk);
        }
    }
}
