// See https://aka.ms/new-console-template for more information
using Net6Console;
//while (true)
//{
//    Console .WriteLine("Hello!...");
//    Thread.Sleep(1000);
//}


FileStreamDemo().Wait();
ParallelSyncDemo();
ParallelAsyncDemo();
ArgumentNullExceptionDemo(null);
DateOnlyTimeOnly();
Defaults();
LambdaDemo();
StringInterpolation();
Console.WriteLine("Hello World!");
Thread.Sleep(1000);
Console.ReadLine();

//}

static void LambdaDemo()
{
    //Lambda have types
    var parse = (string s) => int.Parse(s);
    var result = parse("2");
    Action<int, int> message = (int x, int y) => Console.WriteLine(x + y);
    message(1, 2);
    //var message2 = s => Console.WriteLine("Hello!");
    var message3 = (int x, int y) => x * y;
    Console.WriteLine(message3(2, 3));
}

static void StringInterpolation()
{
    int x = 1, y = 2;
    Console.WriteLine($"X={x},Y={y}");
}
static void Defaults()
{
    //var arr = new[] {  1, 2, 3, 4, 5 };
    var arr = new List<int>();
    Console.WriteLine(arr.FirstOrDefault(defaultValue: 40));
    Console.WriteLine(arr.LastOrDefault(defaultValue: 50));
    Console.WriteLine(arr.SingleOrDefault(defaultValue: 60));
}

static void DateOnlyTimeOnly()
{
    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
    var currentDate = DateOnly.FromDateTime(DateTime.Now);
    Console.WriteLine($"Date:{currentDate}, Time:{currentTime}");
}

static void ArgumentNullExceptionDemo(string x)
{
    try
    {
        if (x is null)
        {
            throw new ArgumentNullException(paramName: x);
        }

        ArgumentNullException.ThrowIfNull(x);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}

static void ParallelSyncDemo()
{
    var numbers = Enumerable.Range(11, 20);
    ParallelOptions options = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };
    var result = Parallel.ForEach(numbers, options, (num) =>
    {
        var ran = Random.Shared;
        Task.Delay(ran.Next(200, 700)).Wait();
        Console.WriteLine($"ThreadId:{System.Environment.CurrentManagedThreadId:D2}==={num}");
    });
    Console.WriteLine("Done1");
}
static void ParallelAsyncDemo()
{
    var numbers = Enumerable.Range(11, 20);
    ParallelOptions options = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };
    var result = Parallel.ForEachAsync(numbers, options, async (num, token) =>
    {
        var ran = Random.Shared;
        Task.Delay(ran.Next(200, 700)).Wait();
        Console.WriteLine($"ThreadId:{System.Environment.CurrentManagedThreadId:D2}==={num}");
    });
    Console.WriteLine("Done2");
}

static async Task FileStreamDemo()
{

    var timer = new System.Diagnostics.Stopwatch();
    timer.Start();
    var openForReading = new FileStreamOptions { Mode = FileMode.Open };
    using FileStream source = new FileStream("c:/temp/6.0.2203.0101-AppManager.zip", openForReading);

    var createForWriting = new FileStreamOptions
    {
        Mode = FileMode.Create,
        Access = FileAccess.Write,
        Options = FileOptions.WriteThrough,
        BufferSize = 0, // disable FileStream buffering
        PreallocationSize = source.Length // specify size up-front
    };
    var length = source.Length;
    await using FileStream destination = new FileStream("c:/temp/dest-6.0.2203.0101-AppManager.zip", createForWriting);
    await source.CopyToAsync(destination);
    timer.Stop();
    Console.WriteLine($"Timer:{timer.Elapsed.TotalMilliseconds}-Length:{length == destination.Length}");

}

static void PreviewFeatureDemo()
{
    Console.WriteLine("First number: ");
    var left = PreviewF.ParseInvariant<float>(Console.ReadLine());

    Console.Write("Second number: ");
    var right = PreviewF.ParseInvariant<float>(Console.ReadLine());

    Console.WriteLine($"Result: {PreviewF.Add(left, right)}");
}