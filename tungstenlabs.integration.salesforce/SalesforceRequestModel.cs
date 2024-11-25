using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace tungstenlabs.integration.salesforce
{
    public class SalesforceRequestModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("timestamp")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; } // Changed to DateTime for ISO8601 handling.

        [JsonProperty("sourceSystem")]
        public string SourceSystem { get; set; }

        [JsonProperty("targetSystem")]
        public string TargetSystem { get; set; }

        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public class Document
    {
        [JsonProperty("documentId")]
        public string DocumentId { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("documentMetadata")]
        public Dictionary<string, string> DocumentMetadata { get; set; }

        [JsonProperty("actions")]
        public List<Action> Actions { get; set; }
    }

    public class Action
    {
        [JsonProperty("actionType")]
        public string ActionType { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters { get; set; }
    }

    public class Parameters
    {
        [JsonProperty("parameter1")]
        public string Parameter1 { get; set; }

        [JsonProperty("parameter2")]
        public string Parameter2 { get; set; }

        [JsonProperty("nestedParameters")]
        public Dictionary<string, string> NestedParameters { get; set; }
    }

    public class Response
    {
        [JsonProperty("responseUrl")]
        public string ResponseUrl { get; set; }

        [JsonProperty("responseTemplate")]
        public ResponseTemplate ResponseTemplate { get; set; }
    }

    public class ResponseTemplate
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("responseData")]
        public List<ResponseData> ResponseData { get; set; }
    }

    public class ResponseData
    {
        [JsonProperty("documentId")]
        public string DocumentId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }
    }

}
