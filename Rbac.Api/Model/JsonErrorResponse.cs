namespace Rbac.Api.Model;

public class JsonErrorResponse
{
    public string[] Messages { get; set; }

    public object DeveloperMessage { get; set; }
}