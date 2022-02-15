using System.Text.Json;

namespace Diorama.Internals.NamingStrategy;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}