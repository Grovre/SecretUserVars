// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using SecretUserVars;

var secretsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\test.json";
var secretsConfig = new SecretsConfiguration(secretsPath)
    .AddVariable(key: "hello", overwriteExistingKey: false) // world
    .AddVariable("leggo") // my eggo
    .AddVariable("hugh") // mungus
    .AddVariable("special number") // some number
    .ConfigureMissing(new ConsoleVariableValueReader())
    .SaveSecretsFile()
    .PushVariableInto("hello", out var hello)
    .PushVariableInto("leggo", out var leggo)
    .PushVariableInto("hugh", out var hugh)
    .PushVariableInto("special number", out var specialNumber, int.Parse);

Console.WriteLine(string.Join(Environment.NewLine, secretsConfig.EnvironmentVariables));