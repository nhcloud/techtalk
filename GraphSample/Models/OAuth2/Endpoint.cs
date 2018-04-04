using System;

namespace GraphSample.Models.OAuth2
{
    public class Endpoint
    {
        public Endpoint(string endpointUrl)
        {
            Uri = endpointUrl;
            if (string.IsNullOrEmpty(Uri)) return;
            var uri = new Uri(endpointUrl);
            BaseUri = new Uri(endpointUrl).AbsoluteUri.Replace(uri.PathAndQuery, "");//$filter=userType eq 'Guest' query was not working
            Resource = uri.PathAndQuery;
        }

        public string BaseUri { get; set; }
        public string Resource { get; set; }
        public string Uri { get; set; }
    }
}