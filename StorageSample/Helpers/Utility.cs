using System.Threading;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Services.AppAuthentication;

namespace StorageSample.Helpers
{
    public class Utility
    {
        public static IKey GetKey(string keyIdentifier)
        {
            var cloudResolver = new KeyVaultKeyResolver(GetKeyVaultClient());
            var rsa = cloudResolver.ResolveKeyAsync(keyIdentifier, CancellationToken.None).GetAwaiter().GetResult();
            return rsa;
        }
        public static string GetSecret(string secretIdentifier)
        {
            if (string.IsNullOrEmpty(secretIdentifier)) { return ""; }
            var kv = GetKeyVaultClient();
            return kv.GetSecretAsync(secretIdentifier).Result.Value;
        }
        private static KeyVaultClient GetKeyVaultClient()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        }

    }
}
