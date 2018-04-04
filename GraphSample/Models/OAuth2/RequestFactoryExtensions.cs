using System;
using RestSharp;

namespace GraphSample.Models.OAuth2
{
    public static class RequestFactoryExtensions
    {
        public static IRestClient CreateClient(this IRequestFactory factory, Endpoint endpoint)
        {
            var client = factory.CreateClient();
            client.BaseUrl = new Uri(endpoint.BaseUri);
            return client;
        }

        public static IRestRequest CreateRequest(this IRequestFactory factory, Endpoint endpoint)
        {
            return CreateRequest(factory, endpoint, Method.GET);
        }

        public static IRestRequest CreateRequest(this IRequestFactory factory, Endpoint endpoint, Method method)
        {
            var request = factory.CreateRequest();
            request.Resource = endpoint.Resource;
            request.Method = method;
            return request;
        }
    }
}