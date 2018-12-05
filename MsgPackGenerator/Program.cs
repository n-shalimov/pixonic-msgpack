using Analysis;
using CommandLine;
using Generators;

class Program
{
    private static int Main(string[] args) =>
        Parser.Default
            .ParseArguments<FormattersOptions, JavaDtoOptions>(args)
            .MapResult(
                (FormattersOptions options) => GenerateFormatters(options),
                (JavaDtoOptions options) => GenerateJavaDto(options),
                errors => 1
            );

    private static int GenerateFormatters(FormattersOptions options)
    {
        var compilation = CompilationLoader.Load(options);
        var collector = new Collector(compilation);
        FormattersGenerator.Generate(collector, options);
        return 0;
    }

    private static int GenerateJavaDto(JavaDtoOptions options)
    {
        var compilation = CompilationLoader.Load(options);
        var collector = new Collector(compilation);
        JavaDtoGenerator.Generate(collector, options);
        return 0;
    }
}
