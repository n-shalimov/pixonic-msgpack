using System.Linq;
using Analysis;
using CommandLine;
using Generators;

[Verb("formatters", HelpText = "Generate C# serializers")]
public class FormattersOptions : CompilationLoader.IOptions, FormattersGenerator.IOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to source code to analyze.")]
    public string Input { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output directory.")]
    public string Output { get; set; }

    [Option('n', "name", Required = false, Default = "MagPack.GeneratedFormatters", HelpText = "Generated class full name.")]
    public string Name { get; set; }

    public string Namespace => string.Join('.', Name.Split('.').SkipLast(1).ToArray());
    public string ClassName => Name.Split('.').TakeLast(1).First();
}


[Verb("java-dto", HelpText = "Generate Java DTO classes")]
public class JavaDtoOptions : CompilationLoader.IOptions, JavaDtoGenerator.IOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to source code to analyze.")]
    public string Input { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output directory.")]
    public string Output { get; set; }

    [Option('n', "namespace", Required = false, Default = "", HelpText = "C# root namespace.")]
    public string Namespace { get; set; }

    [Option('p', "package", Required = false, Default = "", HelpText = "Java root package.")]
    public string Package { get; set; }
}
