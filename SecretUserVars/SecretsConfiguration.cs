using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SecretUserVars;

/// <summary>
/// The main class used for CRUD on the user variable file.
/// It provides a simple way to set up the variables, configure them
/// and load previous entries using a builder pattern. Chaining
/// method calls to control the save configuration is the ideal usage
/// of this class.
/// </summary>
public class SecretsConfiguration
{
    internal readonly Dictionary<string, string?> EnvVarsInternal;
    /// <summary>
    /// All existing variables. Null values are empty variables that will be configured.
    /// Modifications are prohibited to guarantee internal state of the dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, string?> EnvironmentVariables => EnvVarsInternal;
    /// <summary>
    /// Returns true if no values in the dictionary are null. Otherwise, returns false.
    /// </summary>
    public bool AllVariablesExist => EnvVarsInternal.All(kvp => kvp.Value != null);
    /// <summary>
    /// The (hopefully dynamic) file path to store the user variables in.
    /// </summary>
    public string SecretsFilePath { get; }
    
    /// <summary>
    /// Creates a new SecretsConfiguration object and loads variables from the file into this object.
    /// Because of the load, any previously existing variables that are no longer needed will
    /// still persist.
    /// </summary>
    /// <param name="secretsFilePath">The path to the file that holds the user variables.</param>
    /// <exception cref="ArgumentException">Thrown if the given path is a directory</exception>
    public SecretsConfiguration(string secretsFilePath)
    {
        SecretsFilePath = secretsFilePath;
        
        if (Directory.Exists(secretsFilePath))
        {
            throw new ArgumentException
                ("The given secrets path is a directory and not a file");
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

    
    /// <summary>
    /// Adds a variable to the dictionary of user variables for saving.
    /// Any existing keys will not be overwritten unless the argument says otherwise.
    /// </summary>
    /// <param name="key">The key/name of the variable. Cannot be empty</param>
    /// <param name="overwriteExistingKey">If true, any known values with this key are overwritten</param>
    /// <param name="newValue">The predetermined value to add to the variable to avoid being configured later</param>
    /// <exception cref="ArgumentException">Thrown if the key is empty</exception>
    /// <returns>This</returns>
    public SecretsConfiguration AddVariable(string key, bool overwriteExistingKey = false, string? newValue = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        
        EnvVarsInternal.TryAdd(key, newValue);
        
        if (overwriteExistingKey)
        {
            EnvVarsInternal[key] = newValue;
        }
        
        return this;
    }

    /// <summary>
    /// Reads new values for keys/variables with missing values using the given
    /// IVariableValueReader object. If any variables were set to be overwritten,
    /// this will read for those variables as well.
    /// </summary>
    /// <param name="envVarReader">The reader to get values from</param>
    /// <returns>This</returns>
    public SecretsConfiguration ConfigureMissing(IVariableValueReader envVarReader)
    {
        ArgumentNullException.ThrowIfNull(envVarReader);
        
        foreach (var envVar in EnvVarsInternal)
        {
            if (envVar.Value != null)
                continue;
            
            var v = envVarReader.ReadVariable(envVar.Key);
            EnvVarsInternal[envVar.Key] = v;
        }

        return this;
    }

    /// <summary>
    /// Simply deletes the file storing the user variables.
    /// </summary>
    public void PurgeSecretsFile()
    {
        File.Delete(SecretsFilePath);
    }

    /// <summary>
    /// Saves the variable entries to a file in JSON format.
    /// </summary>
    /// <returns>This</returns>
    public SecretsConfiguration SaveSecretsFile()
    {
        using var fs = File.Create(SecretsFilePath);
        JsonSerializer.Serialize(fs, EnvVarsInternal);
        return this;
    }

    /// <summary>
    /// Uses the key to retrieve the string value and uses the parseFunc to parse the string
    /// into a usable value. Then that value is pushed into the out argument.
    /// </summary>
    /// <param name="key">The key/name of the variable</param>
    /// <param name="dst">The destination for the parsed value to be put into</param>
    /// <param name="parseFunc">The function that handles the parsing</param>
    /// <typeparam name="T">Type of the final return value</typeparam>
    /// <returns>This</returns>
    /// <exception cref="NullReferenceException">Thrown if the key does not have a value</exception>
    public SecretsConfiguration PushVariableInto<T>(string key, out T dst, Func<string, T> parseFunc)
    {
        var value = EnvVarsInternal[key]
                    ?? throw new NullReferenceException("The key does not have a value");
        dst = parseFunc(value);
        return this;
    }

    /// <summary>
    /// Uses the key to retrieve the string value that is pushed into the out argument.
    /// </summary>
    /// <param name="key">The key/name of the variable</param>
    /// <param name="dst">The destination for the parsed value to be put into</param>
    /// <returns>This</returns>
    /// <exception cref="NullReferenceException">Thrown if the key does not have a value</exception>
    public SecretsConfiguration PushVariableInto(string key, out string dst)
    {
        dst = EnvVarsInternal[key] 
              ?? throw new NullReferenceException("The key does not have a value");
        return this;
    }

    /// <summary>
    /// Returns the current value of the key or adds the
    /// variable with the given value. If adding and the variable
    /// already exists, this will overwrite the existing value.
    /// </summary>
    /// <param name="key">The key/name of the variable to get/set</param>
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