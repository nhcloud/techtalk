using System;
using System.Threading.Tasks;
using GraphSample.Models;
using GraphSample.Models.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GraphSample.Controllers
{
    public class OAuth2Controller : Controller
    {
        private TenantAuthConfig _authConfig;
        public ActionResult SignIn()
        {
            _authConfig = TenantAuthConfig.GetDefaultConfig();
            var context = new OAuth2Client(_authConfig);
            var loginUri = context.GetLoginUri();
            return new RedirectResult(loginUri);
        }

        public async Task<ActionResult> Acs(string code, string status, string error)
        {
            _authConfig = TenantAuthConfig.GetDefaultConfig();
            var client = new OAuth2Client(_authConfig);
            var graphTokenResponse = await client.AcquireToken(ServiceHelper.HttpUtility.QueryString, _authConfig.GraphResourceId);
            ServiceHelper.Caching.Set("graphTokens", graphTokenResponse);
            ServiceHelper.Caching.Set("signedIn", true);
            return Redirect("http://localhost:52208");
        }
    }
}