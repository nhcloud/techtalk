using Net6Lib;

namespace Net6WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ApiResponse<List<User>>> Get([FromServices] IUserService userService)
    {
        return await _userService.Get();
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<User>> Get(int id)
    {
        return await _userService.Get(id);
    }

    [HttpPost]
    public async Task<ApiResponse<User>> Add(User user)
    {
        return await _userService.Add(user);
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse<User>> Save(int id, User user)
    {
        return await _userService.Save(id, user);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(int id)
    {
        return await _userService.Delete(id);
    }
}