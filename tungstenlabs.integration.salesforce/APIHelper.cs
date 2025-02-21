/*
 * tungstenlabs.integration.salesforce
 *
 * End User License Agreement (EULA)
 *
 * IMPORTANT: PLEASE READ THIS AGREEMENT CAREFULLY BEFORE USING THIS SOFTWARE.
 *
 * 1. GRANT OF LICENSE: Tungsten Automation grants you a limited, non-exclusive,
 * non-transferable, and revocable license to use this software solely for the
 * purposes described in the documentation accompanying the software.
 *
 * 2. RESTRICTIONS: You may not sublicense, rent, lease, sell, distribute,
 * redistribute, assign, or otherwise transfer your rights to this software.
 * You may not reverse engineer, decompile, or disassemble this software,
 * except and only to the extent that such activity is expressly permitted by
 * applicable law notwithstanding this limitation.
 *
 * 3. COPYRIGHT: This software is protected by copyright laws and international
 * copyright treaties, as well as other intellectual property laws and treaties.
 *
 * 4. DISCLAIMER OF WARRANTY: THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESS
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN
 * NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * 5. TERMINATION: Without prejudice to any other rights, Tungsten Automation may
 * terminate this EULA if you fail to comply with the terms and conditions of this
 * EULA. In such event, you must destroy all copies of the software and all of its
 * component parts.
 *
 * 6. GOVERNING LAW: This agreement shall be governed by the laws of USA,
 * without regard to conflicts of laws principles. Any disputes arising hereunder shall
 * be subject to the exclusive jurisdiction of the courts of USA.
 *
 * 7. ENTIRE AGREEMENT: This EULA constitutes the entire agreement between you and
 * Tungsten Automation relating to the software and supersedes all prior or contemporaneous
 * understandings regarding such subject matter. No amendment to or modification of this
 * EULA will be binding unless made in writing and signed by Tungsten Automation.
 *
 * Tungsten Automation
 * www.tungstenautomation.com
 * 11/12/2024
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tungstenlabs.integration.salesforce
{
    public class SFPlatformEventData
    {
        public string ObjectName { get; set; }
        public object ObjectValue { get; set; }
    }

    public class APIHelper
    {

        #region "Public"
        public const string SV_ACCESS_TOKEN = "SFDC-ACCESS-TOKEN";
        public const string SV_INSTANCE_URL = "SFDC-INSTANCE-URL";
        public const string SV_CLIENT_ID = "SFDC-CLIENT-ID";
        public const string SV_CLIENT_SECRET = "SFDC-CLIENT-SECRET";

        /// <summary>
        /// Authenticates with Salesforce using OAuth2 and retrieves an access token, then saves all values in TA's server variables.
        /// </summary>
        public bool Initialize(string baseUrl, string clientId, string clientSecret, string taSessionId, string taSdkUrl)
        {
            string tokenUrl = $"{baseUrl}/services/oauth2/token";

            string postData = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }

            string responseContent;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            var jsonData = JObject.Parse(responseContent);
            var accessToken = jsonData["access_token"]?.ToString();
            var instanceUrl = jsonData["instance_url"]?.ToString();
            List<string> vars = new List<string>() { SV_ACCESS_TOKEN, SV_INSTANCE_URL, SV_CLIENT_ID, SV_CLIENT_SECRET };

            ServerVariableHelper serverVariableHelper = new ServerVariableHelper();
            var dict = serverVariableHelper.GetServerVariables(taSessionId, taSdkUrl, vars);
            dict[SV_ACCESS_TOKEN] = new KeyValuePair<string, string>(dict[SV_ACCESS_TOKEN].Key, accessToken);
            dict[SV_INSTANCE_URL] = new KeyValuePair<string, string>(dict[SV_INSTANCE_URL].Key, instanceUrl);
            dict[SV_CLIENT_ID] = new KeyValuePair<string, string>(dict[SV_CLIENT_ID].Key, clientId);
            dict[SV_CLIENT_SECRET] = new KeyValuePair<string, string>(dict[SV_CLIENT_SECRET].Key, clientSecret);

            Dictionary<string, string> newDict = dict.ToDictionary(kvp => kvp.Value.Key, kvp => kvp.Value.Value);
            serverVariableHelper.UpdateServerVariables(newDict, taSessionId, taSdkUrl);

            return true;

        }

        /// <summary>
        /// Authenticates with Salesforce using OAuth2 and retrieves an access token.
        /// </summary>
        public bool Authenticate(string taSessionId, string taSdkUrl)
        {
            List<string> vars = new List<string>() { SV_INSTANCE_URL, SV_CLIENT_ID, SV_CLIENT_SECRET, SV_ACCESS_TOKEN };

            ServerVariableHelper serverVariableHelper = new ServerVariableHelper();
            var sv = serverVariableHelper.GetServerVariables(taSessionId, taSdkUrl, vars);

            string tokenUrl = $"{sv[SV_INSTANCE_URL].Value}/services/oauth2/token";

            string postData = $"grant_type=client_credentials&client_id={sv[SV_CLIENT_ID].Value}&client_secret={sv[SV_CLIENT_SECRET].Value}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }

            string responseContent;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            var jsonData = JObject.Parse(responseContent);
            var accessToken = jsonData["access_token"]?.ToString();
            var instanceUrl = jsonData["instance_url"]?.ToString();

            Dictionary<string,string> dict = new Dictionary<string, string>() { { sv[SV_ACCESS_TOKEN].Key, accessToken }, { sv[SV_INSTANCE_URL].Key, instanceUrl } };
            serverVariableHelper.UpdateServerVariables(dict, taSessionId, taSdkUrl);

            return true;

        }

        /// <summary>
        /// Creates a platform event in Salesforce.
        /// </summary>
        /// <param name="eventName">API name of the platform event.</param>
        /// <param name="eventData">Key-value pairs representing the event fields and their values.</param>
        public string CreatePlatformEvent(string eventName, string eventData, string taSessionId, string taSdkUrl)
        {
            int maxRetries = 3;
            int count = 0;
            bool shouldRetry;
            string responseContent="";

            List<string> vars = new List<string>() { SV_ACCESS_TOKEN, SV_INSTANCE_URL };

            do 
            {
                shouldRetry = false;
                ServerVariableHelper serverVariableHelper = new ServerVariableHelper();
                var sv = serverVariableHelper.GetServerVariables(taSessionId, taSdkUrl, vars);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(CombineUrls(sv[SV_INSTANCE_URL].Value, eventName));
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = $"Bearer {sv[SV_ACCESS_TOKEN].Value}";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(eventData);
                }

                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex) when (ex.Response is HttpWebResponse httpResponse && httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (count < maxRetries)
                    {
                        count++;
                        shouldRetry = true;
                        Authenticate(taSessionId, taSdkUrl); // Call the method to authenticate and refresh tokens.
                    }
                    else
                    {
                        throw new InvalidOperationException("Maximum retry attempts reached. Unable to authenticate.", ex);
                    }
                }
                catch (WebException ex)
                {
                    using (HttpWebResponse response = (HttpWebResponse)ex.Response)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }

            } while (shouldRetry);

            return responseContent;
        }

        /// <summary>
        /// Creates Event data JSON to be sent to Salesforce
        /// </summary>
        /// <param name="processName">API name of the platform event.</param>
        /// <param name="relatedObject">Name of the related Salesforce object</param>
        /// <param name="responseMessage">JSON to be sent to Salesforce.</param>
        public string BuildEventDataJson(string processName, string relatedObject, string responseMessage)
        {
            // Define the object structure
            var jsonObject = new
            {
                tungstenconnect__Process_Name__c = processName,
                tungstenconnect__Response_Message__c = IsValidJson(responseMessage) ? responseMessage : JsonConvert.SerializeObject(responseMessage),
                tungstenconnect__Response_Timestamp__c = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            // Serialize the object to JSON
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }

        /// <summary>
        /// Creates Event data JSON to be sent to Salesforce
        /// </summary>
        /// <param name="data">Dictionary holding the key/value Json structure.</param>
        public string BuildEventDataJson(List<SFPlatformEventData> data)
        {
            // Process the dictionary so that string values are converted as needed
            var result = new Dictionary<string, object>();

            foreach (var ed in data)
            {
                result[ed.ObjectName] = ProcessValue(ed.ObjectValue);
            }

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                // Use Formatting.Indented if you want pretty printing
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(result, settings);
        }

        /// <summary>
        /// Creates JSON with the Folder's document & document fields after extraction.
        /// </summary>
        /// <param name="taFolderID">TotalAgility Folder ID (Instance ID).</param>
        /// <param name="taSdkUrl">URL for TotalAgility SDK</param>
        /// <param name="taSessionID">TotalAgility Session ID.</param>
        public string BuildFolderJsonFromTAExtraction(string taFolderID, string taSdkUrl, string taSessionID)
        {
            var folder = GetTAFolder(taFolderID, taSdkUrl, taSessionID);

            return SimplifyFolderJson(folder);
        }

        /// <summary>
        /// Creates JSON with the Folder's single document & document fields after extraction.
        /// </summary>
        /// <param name="taFolderID">TotalAgility Folder ID (Instance ID).</param>
        /// <param name="taSdkUrl">URL for TotalAgility SDK</param>
        /// <param name="taSessionID">TotalAgility Session ID.</param>
        public string BuildDocumentJsonFromTAFolder(string taFolderID, string taSdkUrl, string taSessionID, string sfDocumentId)
        {
            var folder = GetTAFolder(taFolderID, taSdkUrl, taSessionID);

            return SimplifyFolderDocumentJson(folder, sfDocumentId, taSdkUrl, taSessionID);
        }

        /// <summary>
        /// Creates JSON with the single document & document fields after extraction.
        /// </summary>
        /// <param name="taDocumentID">TotalAgility Document ID (Instance ID).</param>
        /// <param name="taSdkUrl">URL for TotalAgility SDK</param>
        /// <param name="taSessionID">TotalAgility Session ID.</param>
        public string BuildDocumentJsonFromTADocument(string taDocumentID, string taSdkUrl, string taSessionID, string sfDocumentId)
        {
            string document = GetTADocument(taDocumentID, taSdkUrl, taSessionID);

            return SimplifyDocumentJson(document, sfDocumentId, taSdkUrl, taSessionID);
        }

        /// <summary>
        /// Creates Salesforce compatible JSON from the Base64.AI extraction result.
        /// </summary>
        /// <param name="base64Result">JSON String from the Base64.AI Connector</param>
        /// <param name="taDocumentID">TotalAgility Document ID</param>
        /// <param name="sfDocumentId">Salesforce Document ID.</param>
        public string BuildDocumentJsonFromBase64AIResult(string base64Result, string taDocumentID, string sfDocumentId)
        {
            // Parse the input JSON string
            JObject inputObj = JObject.Parse(base64Result);

            // Extract FIELD1 array
            JArray field1Array = (JArray)inputObj["FIELD1"];

            // Prepare the Name_Value_Pairs dictionary
            Dictionary<string, string> nameValuePairs = new Dictionary<string, string>();

            // Populate the Name_Value_Pairs dictionary
            foreach (var item in field1Array)
            {
                string key = item["Key"].ToString();
                string value = item["Value"].ToString();
                nameValuePairs[key] = value;
            }

            // Construct the output JSON object
            var outputObj = new
            {
                OtherDocument = new
                {
                    SalesforceId = sfDocumentId,
                    TungstenId = taDocumentID,
                    DocumentName = nameValuePairs.ContainsKey("Model") ? nameValuePairs["Model"] : string.Empty,
                    Name_Value_Pairs = JsonConvert.SerializeObject(nameValuePairs, Formatting.None)
                }
            };

            // Serialize the output object to JSON string
            string outputJson = JsonConvert.SerializeObject(outputObj, Formatting.Indented);
            return outputJson;
        }

        #endregion "Public"

        #region "Private Methods"

        private string CombineUrls(string rootUrl, string eventUrl) 
        {
            // Ensure rootUrl has a scheme
            if (!rootUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !rootUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                rootUrl = "https://" + rootUrl.Trim();
            }

            // Normalize slashes
            rootUrl = rootUrl.TrimEnd('/');
            eventUrl = eventUrl.TrimStart('/');

            return $"{rootUrl}/{eventUrl}";
        }

        private string SimplifyFolderJson(string inputJson)
        {
            // Parse the input JSON
            JObject jsonObject = JObject.Parse(inputJson);

            // Navigate to the documents array
            var documents = jsonObject["d"]?["Documents"] as JArray;
            if (documents == null) return "{}";

            var simplifiedDocuments = new List<object>();

            // Iterate through documents
            foreach (var document in documents)
            {
                // Get basic document information
                var documentId = document["Id"]?.ToString();
                var documentName = document["Name"]?.ToString();

                // Extract fields into a dictionary
                var fields = new Dictionary<string, object>();
                var fieldArray = document["Fields"] as JArray;

                if (fieldArray != null)
                {
                    // Iterate over the fields of the first document
                    foreach (var field in fieldArray)
                    {
                        // Extract field name and value
                        string fieldName = field["Name"].ToString();
                        object fieldValue;

                        // Convert value based on its type
                        if (int.TryParse(field["Value"].ToString(), out int intValue))
                        {
                            fieldValue = intValue;
                        }
                        else if (decimal.TryParse(field["Value"].ToString(), out decimal decimalValue))
                        {
                            fieldValue = decimalValue;
                        }
                        else
                        {
                            fieldValue = field["Value"].ToString();
                        }

                        // Add to the simplified fields dictionary
                        fields[fieldName] = fieldValue;
                    }
                }
                // Build simplified document object
                var simplifiedDocument = new
                {
                    Document = documentId,
                    Name = documentName,
                    Fields = fields
                };

                simplifiedDocuments.Add(simplifiedDocument);
            }

            // Build the final output JSON
            var simplifiedJson = new
            {
                Documents = simplifiedDocuments
            };

            return JsonConvert.SerializeObject(simplifiedJson, Formatting.Indented);
        }

        private string SimplifyFolderDocumentJson(string inputJson, string sfDocumentId, string taSdkUrl, string taSessionId)
        {
            string taDocumentId = "";

            // Parse the input JSON into a JObject
            JObject parsedJson = JObject.Parse(inputJson);

            // Navigate to the Documents array within the JSON
            JArray documents = (JArray)parsedJson["d"]["Documents"];

            taDocumentId = documents[0]["Id"]?.ToString();

            // Prepare a dictionary to hold the simplified fields
            Dictionary<string,object> simplifiedFields = new Dictionary<string,object>();
            simplifiedFields["SalesforceId"] = sfDocumentId;
            simplifiedFields["TungstenId"] = taDocumentId;

            //Get document type/name
            var documentName = documents[0]["Name"]?.ToString();

            // Iterate over the fields of the first document
            foreach (var field in documents[0]["Fields"])
            {
                // Extract field name and value
                string fieldName = field["Name"]?.ToString();
                object fieldValue;

                if (field["Table"]["Rows"].HasValues)
                //if (fieldName == "Accounts")
                //    fieldValue = JArray.Parse("[ {  \"AccountName\": \"MyAccess Checking\", \"AccountNumber\": \"0004 6280\", \"Balance\": 0.00  }, { \"AccountName\": \"Regular Sayings\", \"AccountNumber\": \"0004 8167\", \"Balance\": 56441.08 } ]");
                    fieldValue = JArray.Parse(ConvertDatasetJson(GetTADocumentTableFieldValue(taDocumentId, fieldName, taSdkUrl, taSessionId)));
                else 
                {
                    // Convert value based on its type
                    if (int.TryParse(field["Value"]?.ToString(), out int intValue))
                    {
                        fieldValue = intValue;
                    }
                    else if (decimal.TryParse(field["Value"]?.ToString(), out decimal decimalValue))
                    {
                        fieldValue = decimalValue;
                    }
                    else
                    {
                        fieldValue = field["Value"]?.ToString();
                    }
                }

                // Add to the simplified fields dictionary; do not add temporary (tmp) fields
                if (!fieldName.StartsWith("tmp"))
                    simplifiedFields[fieldName] = fieldValue;
            }
            // Create the outer object with the dynamic name
            var simplifiedDocument = new Dictionary<string, object>
            {
                { documentName, simplifiedFields }
            };

            // Serialize the simplified fields dictionary into a JSON string
            string simplifiedJson = JsonConvert.SerializeObject(simplifiedDocument, Formatting.Indented);

            return simplifiedJson;
        }

        private string SimplifyDocumentJson(string document, string sfDocumentId, string taSdkUrl, string taSessionId)
        {
            string taDocumentId = "";

            // Parse the input JSON into a JObject
            JObject parsedJson = JObject.Parse(document);

            // Navigate to the Documents array within the JSON
            JArray documents = (JArray)parsedJson["d"];

            // Prepare a dictionary to hold the simplified fields
            Dictionary<string, object> simplifiedFields = new Dictionary<string, object>();
            simplifiedFields["SalesforceId"] = sfDocumentId;
            simplifiedFields["TungstenId"] = taDocumentId;

            //Get document type/name
            var documentName = documents[0]["Name"]?.ToString();

            // Iterate over the fields of the first document
            foreach (var field in documents[0]["Fields"])
            {
                // Extract field name and value
                string fieldName = field["Name"]?.ToString();
                object fieldValue;

                if (field["Table"]["Rows"].HasValues)
                    //if (fieldName == "Accounts")
                    //    fieldValue = JArray.Parse("[ {  \"AccountName\": \"MyAccess Checking\", \"AccountNumber\": \"0004 6280\", \"Balance\": 0.00  }, { \"AccountName\": \"Regular Sayings\", \"AccountNumber\": \"0004 8167\", \"Balance\": 56441.08 } ]");
                    fieldValue = JArray.Parse(ConvertDatasetJson(GetTADocumentTableFieldValue(taDocumentId, fieldName, taSdkUrl, taSessionId)));
                else
                {
                    // Convert value based on its type
                    if (int.TryParse(field["Value"]?.ToString(), out int intValue))
                    {
                        fieldValue = intValue;
                    }
                    else if (decimal.TryParse(field["Value"]?.ToString(), out decimal decimalValue))
                    {
                        fieldValue = decimalValue;
                    }
                    else
                    {
                        fieldValue = field["Value"]?.ToString();
                    }
                }

                // Add to the simplified fields dictionary; do not add temporary (tmp) fields
                if (!fieldName.StartsWith("tmp"))
                    simplifiedFields[fieldName] = fieldValue;
            }
            // Create the outer object with the dynamic name
            var simplifiedDocument = new Dictionary<string, object>
            {
                { documentName, simplifiedFields }
            };

            // Serialize the simplified fields dictionary into a JSON string
            string simplifiedJson = JsonConvert.SerializeObject(simplifiedDocument, Formatting.Indented);

            return simplifiedJson;
        }

        private string GetTAFolder(string folderID, string ktaSDKUrl, string sessionID)
        {
            string result = "";

            try
            {
                //Setting the URi and calling the get document API
                var KTAGetFolder = ktaSDKUrl + "/CaptureDocumentService.svc/json/GetFolder";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(KTAGetFolder);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                // CONSTRUCT JSON Payload
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"sessionId\":\"" + sessionID + "\",\"folderId\":\"" + folderID + "\" }";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private string GetTADocumentTableFieldValue(string documentId, string tableName, string taSDKUrl, string sessionID)
        {
            string result = "";

            try
            {
                //Setting the URi and calling the get document API
                var KTAGetFolder = taSDKUrl + "/CaptureDocumentService.svc/json/GetDocumentTableFieldValue";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(KTAGetFolder);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                // CONSTRUCT JSON Payload
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"sessionId\":\"" + sessionID + "\",\"documentId\":\"" + documentId + "\", \"tableFieldIdentity\": { \"Name\": \"" + tableName + "\" }}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        private object ProcessValue(object value)
        {
            if (value is string s)
            {
                //if (IsValidJson(s))
                //    return JsonConvert.SerializeObject(s);

                // Try converting to an integer
                if (int.TryParse(s, out int intValue))
                    return intValue;

                // Try converting to a decimal
                if (decimal.TryParse(s, out decimal decimalValue))
                    return decimalValue;

                // Try converting to a DateTime
                if (DateTime.TryParse(s, out DateTime dateValue))
                    return dateValue;

                // If no conversion applies, return the original string
                return s;
            }
            // If the value is a list of objects, process each element in the list
            else if (value is IEnumerable<object> list)
            {
                var newList = new List<object>();
                foreach (var item in list)
                {
                    newList.Add(ProcessValue(item));
                }
                return newList;
            }

            // For any other type, return the value as-is
            return value;

        }

        private bool IsValidJson(string input)
        {
            try
            {
                JsonConvert.DeserializeObject(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string ConvertDatasetJson(string input)
        {
            // Parse the input JSON to get the embedded XML string
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(input);
            string xmlData = jsonObject.d;

            // Load the XML string into a DataSet
            DataSet dataSet = new DataSet();
            using (var reader = new StringReader(xmlData))
            {
                dataSet.ReadXml(reader);
            }

            // Extract the DataTable (assuming it's the first table in the dataset)
            DataTable table = dataSet.Tables[0];

            // Convert the DataTable to a JSON array
            var records = new List<object>();
            foreach (DataRow row in table.Rows)
            {
                var record = new
                {
                    AccountName = row["AccountName"].ToString(),
                    AccountNumber = row["AccountNumber"].ToString(),
                    Balance = ConvertBalance(row["Balance"].ToString())
                };
                records.Add(record);
            }

            // Return the JSON array as a string
            return JsonConvert.SerializeObject(records, Formatting.Indented);
        }

        private double ConvertBalance(string balance)
        {
            // Remove commas and handle incorrect OCR reads
            balance = balance.Replace(",", "").Replace("L", "1").Replace("@", "8");
            if (double.TryParse(balance, out double result))
            {
                return result;
            }
            return 0.0; // Default value if parsing fails
        }

        private string GetTADocument(string docID, string ktaSDKUrl, string sessionID)
        {

            try
            {
                //Setting the URi and calling the get document API
                string KTAGetDocument = ktaSDKUrl + "/CaptureDocumentService.svc/json/GetDocument";
                HttpClient httpClient = new HttpClient();
                var getRequestPayload = new
                {
                    sessionId = sessionID,
                    documentId = docID
                };

                var getRequestContent = new StringContent(JsonConvert.SerializeObject(getRequestPayload), Encoding.UTF8, "application/json");
                var getResponse = httpClient.PostAsync(KTAGetDocument, getRequestContent).GetAwaiter().GetResult();

                if (!getResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching TA document: {getResponse.ReasonPhrase}");
                }

                var getResponseContent = getResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return getResponseContent.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception GetKTADocumentFile: " + ex.ToString(), ex);
            }
        }

        #endregion "Private Methods"
    }
}
