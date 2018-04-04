using System;

namespace GraphSample.Models.OAuth2
{
    [Serializable]
    public class TokenResponse
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Bearer { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}