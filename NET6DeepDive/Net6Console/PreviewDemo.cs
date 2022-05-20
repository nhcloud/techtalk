using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Net6Console;

public class PreviewF
{
    public static T Add<T>(T left, T right)
        where T : INumber<T>
    {
        return left + right;
    }

    public static T ParseInvariant<T>(string s)
        where T : IParseable<T>
    {
        return T.Parse(s, CultureInfo.InvariantCulture);
    }


}