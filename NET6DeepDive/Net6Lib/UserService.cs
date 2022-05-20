using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Net6Lib;

public class UserService : IUserService
{
    private static readonly List<User> Users = new()
    {
        new User(1, "First User", "FirstUser@domain.com"),
        new User(2, "Second User", "SecondUser@domain.com"),
        new User(3, "Third User", "ThirdUser@domain.com")
    };
    public async Task<ApiResponse<List<User>>> Get()
    {
        return await Task.FromResult(new ApiResponse<List<User>>(Users));
    }

    public async Task<ApiResponse<User>> Get(int id)
    {
        var data = Users.FirstOrDefault(p => p.Id == id);
        return await Task.FromResult(new ApiResponse<User>(data));
    }

    public async Task<ApiResponse<User>> Add(User user)
    {
        Users.Add(user);
        return await Task.FromResult(new ApiResponse<User>(user));
    }

    public async Task<ApiResponse<User>> Save(int id, User user)
    {
        Users.RemoveAll(p => p.Id == id);
        Users.Add(user);
        return await Task.FromResult(new ApiResponse<User>(user));
    }

    public async Task<ApiResponse<bool>> Delete(int id)
    {
        Users.RemoveAll(p => p.Id == id);
        return await Task.FromResult(new ApiResponse<bool>(true));
    }
}