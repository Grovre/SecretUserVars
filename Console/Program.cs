// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using SecretUserVars;

var secretsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\test.json";
var secretsConfig = new SecretsConfiguration(secretsPath)
    .AddVariable("hello")
    .AddVariable("leggo")
    .AddVariable("hugh")
    .ConfigureMissing(new ConsoleVariableValueReader())
    .PushVariableInto("hello", out var hello)
    .PushVariableInto("leggo", out var leggo)
    .PushVariableInto("hugh", out var hugh)
    ;

secretsConfig.PurgeSecretsFile();

Console.WriteLine(hello);
Console.WriteLine(leggo);
Console.WriteLine(hugh);