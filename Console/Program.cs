// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using SecretUserVars;

var secretsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\test.json";
var secretsConfig = new SecretsConfiguration(secretsPath)
    .AddVariable("hello")
    .AddVariable("leggo")
    .AddVariable("hugh")
    .ConfigureMissing(new ConsoleVariableValueReader())
    .SaveSecretsFile()
    ;

foreach (var kvp in secretsConfig.EnvironmentVariables)
    Console.WriteLine(kvp);