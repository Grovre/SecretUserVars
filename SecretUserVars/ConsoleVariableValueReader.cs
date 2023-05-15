namespace SecretUserVars;

public class ConsoleVariableValueReader : IVariableValueReader
{
    public string ReadVariable(string key)
    {
        Console.WriteLine($"Enter value for secret variable '{key}': ");
        var value = Console.ReadLine() ?? string.Empty;
        return value;
    }
}