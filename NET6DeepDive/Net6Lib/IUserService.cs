using System.Text.Json;

namespace Net6Lib;

public interface IUserService
{
    Task<ApiResponse<List<User>>> Get();
    Task<ApiResponse<User>> Get(int id);
    Task<ApiResponse<User>> Add(User user);
    Task<ApiResponse<User>> Save(int id, User user);
    Task<ApiResponse<bool>> Delete(int it);
}
