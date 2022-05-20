using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6Lib;

public class ApiResponse<T>
{
    public ApiResponse(T data)
    {
        Data = data;
    }
    public T Data { get; }
}

