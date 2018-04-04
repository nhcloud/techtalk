using System.Collections.Specialized;

namespace GraphSample.Models.OAuth2
{
    public static class NameValueCollectionExtensions
    {
        public static string GetOrThrowUnexpectedResponse(this NameValueCollection collection, string key)
        {
            var value = collection[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new UnexpectedResponseException(key);
            return value;
        }
    }
}