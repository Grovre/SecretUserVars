# SecretUserVars
This is a very small library that provides a quick and simple way to add and maintain secrets that should be kept separate from a project.
A good example of these secrets are API keys, usernames and passwords.
The entire process is embedded into an application and runs only at the beginning.
No third-party library is used. Only namespaces part of .NET 7 are used.

# How to use the library
1. Instantiate the `SecretsConfiguration` class with a dynamic file path (such as within local appdata) to work on platforms supported by .NET.
2. Add variables with the `AddVariable` method. Use the overwrite bool to force a read even if the variable already exists.
3. Configure the missing variables with the `ConfigureMissing` method. An `IVariableValueReader` must be provided for the source of variable values. A console implementation of the interface is provided for a quick start.
4. Save the secrets file so users don't have to input them again.
5. Push the secrets into variables for use within the program.

Here is an example of reading existing variables from a file or, if they don't exist, reading them with the given `IVariableValueReader`.

<img width="684" alt="image" src="https://github.com/Grovre/SecretUserVars/assets/50428844/1e9cba08-080c-41d3-b0c6-4d58c18b3a2d">

# How it works
Internally, a dictionary backs all variables and is stored in JSON. Upon instantiation of the configuration class, the class will try to read existing variables from the given path.
If the file does not exist, the dictionary will be empty. If the file is only partially filled with the required variables, the missing ones added will be configured.
