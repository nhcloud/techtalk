using System;
using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace GraphSample.Models.OAuth2
{
    public static class RestClientExtensions
    {
        public static async Task<IRestResponse> ExecuteAndVerify(this IRestClient client, IRestRequest request)
        {
            return await Task.Run(() =>
            {
                var response = client.Execute(request);
                if (request.Method == Method.GET)
                {
                    if (string.IsNullOrWhiteSpace(response.Content) || response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                                var ex = new Exception("Resource Not Found");
                                ex.Data.Add("httpstatuscode", response.StatusCode);
                                throw ex;
                            case HttpStatusCode.Unauthorized:
                                var exu = new Exception("You donot have access to this resource");
                                exu.Data.Add("httpstatuscode", response.StatusCode);
                                throw exu;
                            default:
                                throw new UnexpectedResponseException(response, response.StatusCode);
                        }
                    }


                }
                return response;
            });
        }
    }
}