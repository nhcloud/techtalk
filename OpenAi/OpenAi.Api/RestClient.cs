using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenAi.Api;

public class RestClient
{
    public RestClient()
    {
    }

    public async Task<string> GetAsync(string url)
    {
        var content = "Empty";
        try
        {
            var httpClient = GetHttpClient(url);
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            content = ex.ToString();
        }
        return content;
    }

    public async Task<string> PostAsync(string url, Dictionary<string, object> payload)
    {
        string responseMsg;
        try
        {
            var message = JsonSerializer.Serialize(payload);
            var httpClient = GetHttpClient(url);
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var stringContent = new StringContent(message, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        responseMsg = await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            responseMsg = ex.ToString();
        }

        return responseMsg;
    }
    public async Task<string> PostMultiPartAsync(string url, Dictionary<string, object> payLoad)
    {
        string responseMsg;
        try
        {
            var httpClient = GetHttpClient(url);
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("multipart/form-data"));
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var multipartContent = new MultipartFormDataContent())
                {
                    foreach (var item in payLoad)
                    {
                        var val = item.Value.ToString();
                        if (val.EndsWith(".png"))
                        {
                            var byteContent = GetFileContent(val);
                            var fileContent = new ByteArrayContent(byteContent, 0, byteContent.Length);
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            multipartContent.Add(fileContent, item.Key, val);
                        }
                        else
                        {
                            multipartContent.Add(new StringContent(val), item.Key);
                        }
                    }

                    request.Content = multipartContent;
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        responseMsg = await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            responseMsg = ex.ToString();
        }

        return responseMsg;
    }
    private HttpClient GetHttpClient(string url)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(url) };
        httpClient.Timeout = new TimeSpan(0, 5, 0);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppSettings.OpenAiKey);
        httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", AppSettings.OpenAiOrg);
        return httpClient;
    }
    private static byte[] GetFileContent(string fileName)
    {
        var fileContent = File.ReadAllBytes("c:/temp/" + fileName);
        return fileContent;
    }
}