using System;
using System.Net;

namespace GraphSample.Models.OAuth2
{
    public class UnexpectedResponseException : Exception
    {
        public UnexpectedResponseException(dynamic response, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Response = response;
            Data.Add("httpstatuscode", statusCode);
        }

        public UnexpectedResponseException(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; set; }
        public dynamic Response { get; }
    }
}