using System.Text.Json.Serialization;

namespace Net7Console;

//[JsonDerivedType(typeof(Apple))]
[JsonDerivedType(typeof(Blueberry))]
[JsonDerivedType(typeof(Apple), "Medium")]//Type Descriminator attribute for deserialization

public class Fruit
{
   public string Name { get; set; } = null!;
}

public class Apple : Fruit
{
    public string? Color { get; set; }
}

public class Blueberry : Fruit
{

    public string? Color { get; set; }
    public string? Size { get; set; }
}