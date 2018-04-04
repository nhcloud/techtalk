using RestSharp;

namespace GraphSample.Models.OAuth2
{
    public class RequestFactory : IRequestFactory
    {
        public IRestClient CreateClient()
        {
            return new RestClient();
        }

        public IRestRequest CreateRequest()
        {
            return new RestRequest();
        }
    }
}