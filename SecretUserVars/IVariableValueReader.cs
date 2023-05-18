namespace SecretUserVars;

/// <summary>
/// Interface allowing custom implementations for reading variables to be stored away from the project.
/// </summary>
public interface IVariableValueReader
{
    /// <summary>
    /// This method returns a parseable string of a variable given the key.
    /// For example, this can be hooked into a login interface to save a username and
    /// password to avoid repeat logins. However, that is only an example and passwords
    /// should not actually be stored in plain text.
    ///
    /// An implementation for console applications is already provided.  See
    /// ConsoleVariableValueReader for more.
    /// </summary>
    /// <param name="key">The key/name of the variable that will receive a value.</param>
    /// <returns>The value to be received and added to the key entry of the variables</returns>
    public string ReadVariable(string key);
}