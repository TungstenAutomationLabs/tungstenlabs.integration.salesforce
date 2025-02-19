using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Runtime.InteropServices;
//using Newtonsoft.Json;
using tungstenlabs.integration.salesforce;
//using Newtonsoft.Json.Linq;
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
            string base64string = @"{""FIELD1"":[{""Key"": ""Model"", ""Value"": ""US receipt"", ""Confidence"": ""1""},{""Key"": ""Account number"", ""Value"": ""0123456789-1"", ""Confidence"": ""0.99""},{""Key"": ""Date"", ""Value"": ""2020-02-24,ocrText:02/24/2020"", ""Confidence"": ""0.99""},{""Key"": ""Name"", ""Value"": ""PGGE"", ""Confidence"": ""0.95""},{""Key"": ""Company name"", ""Value"": ""PGGE"", ""Confidence"": ""0.95""},{""Key"": ""Website"", ""Value"": ""www.pge.com/MyEnergy"", ""Confidence"": ""0.99""},{""Key"": ""Due date"", ""Value"": ""03/12/2020"", ""Confidence"": ""0.99""},{""Key"": ""Meter"", ""Value"": ""0000011111"", ""Confidence"": ""0.99""},{""Key"": ""Total Usage"", ""Value"": ""17,384.640000 kWh"", ""Confidence"": ""0.99""},{""Key"": ""Serial"", ""Value"": ""R"", ""Confidence"": ""0.99""},{""Key"": ""Service For"", ""Value"": ""1234 Main St"", ""Confidence"": ""0.99""},{""Key"": ""Service Agreement ID"", ""Value"": ""9876543210"", ""Confidence"": ""0.99""},{""Key"": ""Rotating Outage Block"", ""Value"": ""50"", ""Confidence"": ""0.67""},{""Key"": ""Rate Schedule"", ""Value"": ""A10S Medium General Demand-Metered Service"", ""Confidence"": ""0.99""},{""Key"": ""Additional Messages"", ""Value"": ""Requested rate change complete Per your\\nrequest, we have changed your rate schedule. If\\nyou have questions, or would like more\\ninformation, call 1-800-468-4743."", ""Confidence"": ""0.98""},{""Key"": ""Phone number"", ""Value"": ""(800) 468-4743"", ""Confidence"": ""0.99""},{""Key"": ""Generation Credit"", ""Value"": ""-1,657.44"", ""Confidence"": ""0.99""},{""Key"": ""Purchase order"", ""Value"": ""488.51"", ""Confidence"": ""0.99""},{""Key"": ""Franchise Fee Surcharge"", ""Value"": ""12.00"", ""Confidence"": ""0.99""},{""Key"": ""City of Monterey Utility Users' Tax (5.000"", ""Value"": ""97.54"", ""Confidence"": ""0.99""},{""Key"": ""Total"", ""Value"": ""2,060.41"", ""Confidence"": ""1""},{""Key"": ""Subtotal"", ""Value"": ""2,060.41"", ""Confidence"": ""1""},{""Key"": ""Electric Usage This Period"", ""Value"": ""17,384.640000 kWh, 30 billing days"", ""Confidence"": ""0.99""},{""Key"": ""Page"", ""Value"": ""3 of 5"", ""Confidence"": ""1""},{""Key"": ""Company address"", ""Value"": """", ""Confidence"": ""1""},{""Key"": ""Service Information"", ""Value"": ""Meter # 0000011111\\nTotal Usage 17,384.640000 kWh\\nSerial R\\nRotating Outage Block 50"", ""Confidence"": ""0.97""},{""Key"": ""Time"", ""Value"": """", ""Confidence"": ""1""}]}";

            //var jsonData = JObject.Parse(authentication);
            //var accessToken = jsonData["access_token"]?.ToString();
            //var instanceUrl = jsonData["instance_url"]?.ToString();
            //bool result = aPIHelper.Initialize(sfurl, clientid, clientsecret, sessionID, sdk);

            //ng eventname = "tungstenconnect__JobResponse__e";
            //string payload = "{\r\n  \"tungstenconnect__Process_Name__c\": \"CreateAccountProcess\",\r\n  \"tungstenconnect__Related_Object__c\": \"Account\",\r\n  \"tungstenconnect__Response_Message__c\": \"{\\\"AccountName\\\":\\\"ABC Corp\\\",\\\"AccountType\\\":\\\"Customer\\\",\\\"Industry\\\":\\\"Technology\\\",\\\"AnnualRevenue\\\":5000000}\",\r\n  \"tungstenconnect__Response_Timestamp__c\": \"2024-11-10T10:00:00Z\"\r\n}";
            //var response = aPIHelper.BuildEventDataJson(data);

            //var json = aPIHelper.BuildDocumentJsonFromTAFolder("4b697a3e-68dc-4372-9a41-b265016f2e41", sdk, sessionID, "abc123");
            //var response = aPIHelper.BuildDocumentJsonFromBase64AIResult(base64string, "abc123", "");
        }
    }
}
