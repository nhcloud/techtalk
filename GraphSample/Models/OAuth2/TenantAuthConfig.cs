using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GraphSample.Models.OAuth2
{
    [Serializable]
    public class TenantAuthConfig : ICloneable
    {
        public static TenantAuthConfig GetDefaultConfig()
        {
            return new TenantAuthConfig
            {
                SpRedirectUri = "http://localhost:52208/oauth2/acs",
                ClientId = "52c057f6-428c-4816-8b5f-e699f18c55ca",
                ClientSecret = "Miuh1Hkzn1U6MMcs263aTPdHit1KGrW0tuAyQL+T0tM=",
                AuthorizeUri = @"https://login.microsoftonline.com/common/oauth2/authorize",
                TokenUri = @"https://login.microsoftonline.com/common/oauth2/token",
                GraphUri = @"https://graph.microsoft.com/v1.0",
                ProfileUri = @"https://graph.microsoft.com/v1.0/me/",
                GraphResourceId = @"https://graph.microsoft.com/",
                ClaimsMapping = new Dictionary<string, string> { { "email", "upn" } }
            };
        }

        public string SpRedirectUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorizeUri { get; set; }
        public string TokenUri { get; set; }
        public string ProfileUri { get; set; }
        public string GraphUri { get; set; }
        public string Scope { get; set; }
        public string GraphResourceId { get; set; }
        public string GrantType { get; set; }
        public Dictionary<string, string> ClaimsMapping { get; set; }
        public object Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;
                return formatter.Deserialize(ms);
            }
        }
    }
}