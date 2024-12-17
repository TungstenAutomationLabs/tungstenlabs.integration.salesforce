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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tungstenlabs.integration.salesforce
{
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

                string eventUrl = $"{sv[SV_INSTANCE_URL].Value}/services/data/v57.0/sobjects/{eventName}/";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(eventUrl);
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
                tungstenconnect__Related_Object__c = relatedObject,
                tungstenconnect__Response_Message__c = IsValidJson(responseMessage) ? responseMessage : JsonConvert.SerializeObject(responseMessage),
                tungstenconnect__Response_Timestamp__c = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            // Serialize the object to JSON
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }

        /// <summary>
        /// Creates JSON with the Folder's document & document fields after extraction.
        /// </summary>
        /// <param name="taFolderID">TotalAgility Folder ID (Instance ID).</param>
        /// <param name="taSdkUrl">URL for TotalAgility SDK</param>
        /// <param name="taSessionID">TotalAgility Session ID.</param>
        public string BuildJsonFromTAExtraction(string taFolderID, string taSdkUrl, string taSessionID)
        {
            var folder = GetKTAFolder(taFolderID, taSdkUrl, taSessionID);

            return SimplifyJson(folder);
        }

        #endregion "Public"

        #region "Private Methods"

        private string SimplifyJson(string inputJson)
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

        private string GetKTAFolder(string folderID, string ktaSDKUrl, string sessionID)
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

        #endregion "Private Methods"
    }
}
