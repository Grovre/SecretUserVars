namespace SecretUserVars;

/// <summary>
/// A vey simple implementation of the IVariableValueReader interface that
/// asks for console input to read a value
/// </summary>
public class ConsoleVariableValueReader : IVariableValueReader
{
    public string ReadVariable(string key)
    {
        Console.WriteLine($"Enter value for secret variable '{key}': ");
        var value = Console.ReadLine() ?? string.Empty;
        return value;
    }
}