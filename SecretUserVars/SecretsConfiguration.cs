using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SecretUserVars;

public class SecretsConfiguration
{
    internal readonly Dictionary<string, string?> EnvVarsInternal;
    public IReadOnlyDictionary<string, string?> EnvironmentVariables => EnvVarsInternal;
    public bool AllVariablesExist => EnvVarsInternal.All(kvp => kvp.Value != null);
    public string SecretsFilePath { get; }
    
    public SecretsConfiguration(string secretsFilePath)
    {
        SecretsFilePath = secretsFilePath;
        
        if (Directory.Exists(secretsFilePath))
        {
            throw new ArgumentException("The given secrets path is a directory");
        }
        
        if (!File.Exists(secretsFilePath))
        {
            EnvVarsInternal = new();
            return;
        }

        using var fs = File.OpenRead(secretsFilePath);
        EnvVarsInternal = JsonSerializer.Deserialize<Dictionary<string, string?>>(fs)!;
        EnvVarsInternal ??= new();
    }

    public SecretsConfiguration AddVariable(string key, bool overwriteExistingKey = false)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("The key cannot be null or empty");
        
        EnvVarsInternal.TryAdd(key, null);
        
        if (overwriteExistingKey)
        {
            EnvVarsInternal[key] = null;
        }
        
        return this;
    }

    public SecretsConfiguration ConfigureMissing(IVariableValueReader envVarReader)
    {
        foreach (var envVar in EnvVarsInternal)
        {
            if (envVar.Value != null)
                continue;
            
            var v = envVarReader.ReadVariable(envVar.Key);
            EnvVarsInternal[envVar.Key] = v;
        }

        return this;
    }

    public void PurgeSecretsFile()
    {
        File.Delete(SecretsFilePath);
    }

    public SecretsConfiguration SaveSecretsFile()
    {
        using var fs = File.Create(SecretsFilePath);
        JsonSerializer.Serialize(fs, EnvVarsInternal);
        return this;
    }

    public SecretsConfiguration PushVariableInto<T>(string key, out T dst, Func<string, T> parseFunc)
    {
        dst = parseFunc(EnvVarsInternal[key]!);
        if (dst == null)
            throw new NullReferenceException("The key does not have a value");
        return this;
    }

    public SecretsConfiguration PushVariableInto(string key, out string dst)
    {
        dst = EnvVarsInternal[key];
        if (dst == null)
            throw new NullReferenceException("The key does not have a value");
        return this;
    }

    public string? this[string key]
    {
        get
        {
            EnvVarsInternal.TryGetValue(key, out var v);
            return v;
        }
        
        set
        {
            EnvVarsInternal.TryAdd(key, value);
            EnvVarsInternal[key] = value;
        }
    }
}