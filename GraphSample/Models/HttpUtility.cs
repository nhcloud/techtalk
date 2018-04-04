
using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GraphSample.Models
{
    public class HttpUtility 
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public HttpUtility(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }
        public NameValueCollection QueryString
        {
            get
            {
                var qs = new NameValueCollection();
                foreach (var item in _httpContextAccessor.HttpContext.Request.Query)
                {
                    qs.Add(item.Key, item.Value);
                }
                return qs;
            }
        }

        public Uri Url
        {
            get
            {
                var request = _httpContextAccessor.HttpContext.Request;
                var builder = new UriBuilder
                {
                    Scheme = request.Scheme,
                    Host = request.Host.Host,
                    Port = request.Host.Port ?? (request.IsHttps ? 443 : 80),
                    Path = request.Path,
                    Query = request.QueryString.ToUriComponent()
                };
                return builder.Uri;
            }
        }
        public bool IsLocal => true;

        public IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs()
        {
            var qs = new Dictionary<string, string>();
            foreach (var item in _httpContextAccessor.HttpContext.Request.Query)
            {
                qs.Add(item.Key, item.Value);
            }
            return qs;
        }
    }
}
