using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;
using GraphSample.Models.OAuth2;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace GraphSample.Models.OAuth2
{
    public class OAuth2Client 
    {
        private readonly IRequestFactory _factory;

        public OAuth2Client(TenantAuthConfig customAuthConfig)
        {
            _factory = new RequestFactory();
            AuthConfiguration = customAuthConfig;
            AccessCodeServiceEndpoint = new Endpoint(AuthConfiguration.AuthorizeUri);
            AccessTokenServiceEndpoint = new Endpoint(AuthConfiguration.TokenUri);
        }
        public string AccessToken { get; private set; }
        public string IdToken { get; private set; }
        public string RefreshToken { get; private set; }
        public string TokenType { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public string TokenResponse { get; private set; }
        public string State { get; private set; }
        public string GetLoginUri(string state = null)
        {
            var client = _factory.CreateClient();
            client.BaseUrl = new Uri(AccessCodeServiceEndpoint.BaseUri);
            var request = _factory.CreateRequest();
            request.Resource = AccessCodeServiceEndpoint.Resource;
            request.Method = Method.GET;
            request.AddObject(new
            {
                response_type = OAuth2Constants.Code,
                client_id = AuthConfiguration.ClientId,
                redirect_uri = AuthConfiguration.SpRedirectUri,
            });
            if (!string.IsNullOrEmpty(AuthConfiguration.Scope))
            {
                request.AddObject(new
                {
                    scope = AuthConfiguration.Scope
                });
            }
            return client.BuildUri(request).ToString();
        }
        public async Task<TokenResponse> AcquireToken(NameValueCollection parameters, string resourceId)
        {
            GrantType = OAuth2Constants.GrantTypeConstants.AuthorizationCode;
            CheckErrorAndSetState(parameters);
            await QueryAccessToken(parameters, "", resourceId);
            return new TokenResponse { Token = TokenResponse, Bearer = AccessToken, ExpireAt = ExpiresAt };
        }
        public async Task<TokenResponse> QueryAccessToken(NameValueCollection parameters, string refreshToken, string resourceId = "")
        {
            var client = _factory.CreateClient();
            client.BaseUrl = new Uri(AccessTokenServiceEndpoint.BaseUri);
            var request = _factory.CreateRequest();
            request.Resource = AccessTokenServiceEndpoint.Resource;
            request.Method = Method.POST;
            if (string.IsNullOrEmpty(resourceId))
            {
                resourceId = AuthConfiguration.GraphResourceId;
            }
            if (GrantType == null && !string.IsNullOrEmpty(refreshToken))
            {
                GrantType = "refresh_token";
            }
            request.AddObject(new
            {
                client_id = AuthConfiguration.ClientId,
                client_secret = AuthConfiguration.ClientSecret,
                grant_type = GrantType
            });
            if (GrantType == "refresh_token")
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = parameters.GetOrThrowUnexpectedResponse("refresh_token");
                }
                request.AddObject(new
                {
                    refresh_token = refreshToken
                });
            }
            else
            {
                request.AddObject(new
                {
                    code = parameters.GetOrThrowUnexpectedResponse("code"),
                    redirect_uri = AuthConfiguration.SpRedirectUri
                });
                if (!string.IsNullOrEmpty(resourceId))
                {
                    request.AddObject(new
                    {
                        resource = resourceId
                    });
                }
            }

            var response = await client.ExecuteAndVerify(request);
            TokenResponse = response.Content;
            AccessToken = ParseTokenResponse(response.Content, OAuth2Constants.AccessToken);
            IdToken = ParseTokenResponse(response.Content, OAuth2Constants.IdToken);
            if (string.IsNullOrEmpty(AccessToken))
            {
                
                System.Diagnostics.Debug.WriteLine("QueryAccessToken ERROR");
                if (!string.IsNullOrEmpty(response.Content))
                {
                    System.Diagnostics.Debug.WriteLine(response.Content);
                    throw new Exception(response.Content);
                }
                else
                {
                    throw new UnexpectedResponseException(OAuth2Constants.AccessToken);
                }
            }

            if (GrantType != "refresh_token")
                RefreshToken = ParseTokenResponse(response.Content, OAuth2Constants.GrantTypeConstants.RefreshToken);

            TokenType = ParseTokenResponse(response.Content, OAuth2Constants.TokenType);

            if (int.TryParse(ParseTokenResponse(response.Content, OAuth2Constants.ExpiresIn), out var expiresIn))
                ExpiresAt = DateTime.Now.AddSeconds(expiresIn);
            return new TokenResponse { Token = TokenResponse, Bearer = AccessToken, ExpireAt = ExpiresAt };
        }
        public async Task<dynamic> GetData(string uri, Dictionary<string, string> headers, string token)
        {
            var endpoint = new Endpoint(uri);
            var client = _factory.CreateClient();
            client.BaseUrl = new Uri(endpoint.BaseUri);
            var accessToken = ParseTokenResponse(token, "access_token");
            client.AddDefaultHeader("Authorization", $"bearer {accessToken}");
            var request = _factory.CreateRequest();

            if (headers != null && headers.Count > 0)
            {
                foreach (var headerName in headers.Keys)
                {
                    request.AddHeader(headerName, headers[headerName]);
                }
            }

            request.Resource = endpoint.Resource;
            request.Method = Method.GET;
            IRestResponse response;
            try
            {
                response = await client.ExecuteAndVerify(request);
            }
            catch (UnexpectedResponseException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            return response.Content;
        }
        public async Task<byte[]> GetRawBytesData(string uri, NameValueCollection param, string token)
        {
            var endpoint = new Endpoint(uri);
            var client = _factory.CreateClient();
            client.BaseUrl = new Uri(endpoint.BaseUri);
            var accessToken = ParseTokenResponse(token, "access_token");
            client.AddDefaultHeader("Authorization", $"bearer {accessToken}");
            var request = _factory.CreateRequest();
            request.Resource = endpoint.Resource;
            request.Method = Method.GET;
            IRestResponse response;
            try
            {
                response = await client.ExecuteAndVerify(request);
            }
            catch (UnexpectedResponseException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            return response.RawBytes;
        }
        public async Task<dynamic> UpdateData(string uri, string method, NameValueCollection param, Dictionary<string, string> headers, string jsonData, string token, bool useAppToken = false)
        {

            var client = _factory.CreateClient();

            var request = GetRequestForUpdate(client, uri, method, headers, token, useAppToken);

            request.AddHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(jsonData))
            {
                request.AddParameter("application/json", JsonConvert.DeserializeObject(jsonData), ParameterType.RequestBody);
            }

            IRestResponse response;

            try
            {
                response = await client.ExecuteAndVerify(request);
            }
            catch (UnexpectedResponseException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            return response;
        }
        public async Task<dynamic> UpdateData(string uri, string method, NameValueCollection param, Dictionary<string, string> headers, byte[] binaryData, string token)
        {
            var client = _factory.CreateClient();
            var request = GetRequestForUpdate(client, uri, method, headers, token);

            request.AddParameter("image/jpeg", binaryData, ParameterType.RequestBody);

            IRestResponse response;

            try
            {
                response = await client.ExecuteAndVerify(request);
            }
            catch (UnexpectedResponseException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
            return response;
        }
        public string SetPrincipal(dynamic user, string token)
        {
            GrantType = OAuth2Constants.GrantTypeConstants.AuthorizationCode;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                IList<Claim> claims = new List<Claim>();
                if (user != null)
                {
                    var response = JObject.Parse(user);
                    claims = new List<Claim>
                    {
                        new Claim("email",  response["email"].Value<string>()),
                        new Claim("given_name",  response["given_name"].Value<string>()),
                        new Claim("family_name",  response["family_name"].Value<string>())
                    };
                }
                else
                {
                    var tokens = handler.ReadToken(AccessToken) as JwtSecurityToken;
                    if (tokens != null) claims = (IList<Claim>)tokens.Claims;
                }

                var claimsIdentity = new ClaimsIdentity(claims, "OAuth2");
                var principal = new ClaimsPrincipal(claimsIdentity);
                Thread.CurrentPrincipal = principal;
                //if (HttpContext.Current.User == null || HttpContext.Current.User.Identity.IsAuthenticated) return HttpContext.Current.User.Identity.Name;

                //HttpContext.Current.User = Thread.CurrentPrincipal;
                var userData = string.Join("|", "User");
                var userName = claims.FirstOrDefault(p => p.Type == AuthConfiguration.ClaimsMapping["email"])?.Value;
                //SetFormsAuthenticationTicket(userName, userData);
                return userName;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return "";
            }
        }
        #region helpers
        private string GrantType { get; set; }
        private Endpoint AccessCodeServiceEndpoint { get; }
        private Endpoint AccessTokenServiceEndpoint { get; }
        private TenantAuthConfig AuthConfiguration { get; }
        private static string ParseTokenResponse(string content, string key)
        {
            if (String.IsNullOrEmpty(content) || String.IsNullOrEmpty(key))
                return null;

            try
            {
                var token = JObject.Parse(content).SelectToken(key);
                return token?.ToString();
            }
            catch (JsonReaderException)
            {
                return "";
            }
        }
        private void CheckErrorAndSetState(NameValueCollection parameters)
        {
            const string errorFieldName = "error";
            var error = parameters[errorFieldName];
            if (!string.IsNullOrWhiteSpace(error))
                throw new UnexpectedResponseException(errorFieldName);

            State = parameters["state"];
        }
        //private static void SetFormsAuthenticationTicket(string userName, string userData)
        //{
        //    var ticket = new FormsAuthenticationTicket(
        //        1,
        //        userName,
        //        DateTime.Now,
        //        DateTime.Now.AddMinutes(AppSettings.DwpCookieDuration),
        //        true,
        //        userData,
        //        FormsAuthentication.FormsCookiePath);

        //    var encryptedTicket = FormsAuthentication.Encrypt(ticket);
        //    var cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
        //    {
        //        HttpOnly = true,
        //        Expires = DateTime.Now.AddMinutes(AppSettings.DwpCookieDuration)
        //    };
        //    HttpContext.Current.Response.Cookies.Add(cookie);
        //}

        private IRestRequest GetRequestForUpdate(IRestClient client, string uri, string method, Dictionary<string, string> headers, string token, bool useAppToken = false)
        {
            var endpoint = new Endpoint(uri);
            client.BaseUrl = new Uri(endpoint.BaseUri);
            var accessToken = useAppToken ? token : ParseTokenResponse(token, "access_token");
            client.AddDefaultHeader("Authorization", $"bearer {accessToken}");
            var request = _factory.CreateRequest();
            request.Resource = endpoint.Resource;

            switch (method.ToLower())
            {
                case "post":
                    request.Method = Method.POST;
                    break;
                case "delete":
                    request.Method = Method.DELETE;
                    break;
                case "patch":
                    request.Method = Method.PATCH;
                    break;
            }

            if (headers != null && headers.Count > 0)
            {
                foreach (var headerName in headers.Keys)
                {
                    request.AddHeader(headerName, headers[headerName]);
                }
            }
            return request;
        }
        #endregion
    }
}