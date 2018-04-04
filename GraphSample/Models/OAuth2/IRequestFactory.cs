using RestSharp;

namespace GraphSample.Models.OAuth2
{
    public interface IRequestFactory
    {
        IRestClient CreateClient();
        IRestRequest CreateRequest();
    }
}