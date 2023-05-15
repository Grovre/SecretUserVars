namespace SecretUserVars;

public interface IVariableValueReader
{
    public string ReadVariable(string key);
}