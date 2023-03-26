using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Net7Console;
using BenchmarkDotNet.Attributes;

// See https://aka.ms/new-console-template for more information
Console.WriteLine(".NET 7 Sample...");
JsonPolymorphism();
JsonCustomization();
new RegexDemo().IsGreeting("hello nhcloud!");
Console.ReadLine();
[Benchmark]
static void JsonPolymorphism()
{

    var fruit = new Fruit { Name = "Base-Fruit" };
    var apple = new Apple { Name = """Derived-"Fruit"-Apple""", Color = "Green" };//Raw string literal 
    Fruit appleFruit = new Apple { Name = "X", Color = "Red" };

    var jsonFruit = JsonSerializer.Serialize(fruit);
    var jsonApple = JsonSerializer.Serialize(apple);
    var jsonAppleFruit1 = JsonSerializer.Serialize<Fruit>(apple);
    var jsonAppleFruit2 = JsonSerializer.Serialize(appleFruit);


    var toFruit = JsonSerializer.Deserialize<Fruit>(jsonFruit);
    var toApple = JsonSerializer.Deserialize<Apple>(jsonApple);
    Fruit? toFruit1 = JsonSerializer.Deserialize<Apple>(jsonAppleFruit1);
    var toFruit2 = JsonSerializer.Deserialize<Fruit>(jsonAppleFruit2);

}
[Benchmark]
static void JsonCustomization()
{
    var bbFruit = new Blueberry { Name = "X", Color = "Blue" ,Size="Large"};
    var blueberry = JsonSerializer.Serialize(bbFruit);
    JsonSerializerOptions options = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                RemoveIgnoredProperties
            }
        }
    };

    var jsonFruit2 = JsonSerializer.Serialize(bbFruit, options);
}
[Benchmark]
static void RemoveIgnoredProperties(JsonTypeInfo obj)
{
    //List<JsonPropertyInfo> properties = new();

    foreach (var prop in obj.Properties)
    {
        if (prop.Name != "Size") continue;
        obj.Properties.Remove(prop);
        //properties.Add(prop);
        break;


    }
    //obj.Properties.Clear();
    //foreach (var property in properties)
    //{
    //    obj.Properties.Add(property);
    //}
}