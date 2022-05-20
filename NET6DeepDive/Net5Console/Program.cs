using System;
using System.Threading;

namespace Net5Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StringInterpolation();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
        //static void LambdaDemo()
        //{
        //    Action<int, int> message = (int x, int y) => Console.WriteLine(x + y);
        //    message(1, 2);
        //    //var message2 = s => Console.WriteLine("Hello!");
        //    var message3 = (int x, int y) => x * y;
        //    Console.WriteLine(message3(2, 3));
        //}

        static void StringInterpolation()
        {
            int x = 1, y = 2;
            Console.WriteLine($"X={x},Y={y}");
        }
    }
}
