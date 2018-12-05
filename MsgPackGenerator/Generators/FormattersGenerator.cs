using System.IO;
using System.Text;
using Analysis;
using Templates;

namespace Generators
{
    public static class FormattersGenerator
    {
        public interface IOptions : FormattersTemplate.IOptions
        {
            string Output { get; }
        }

        public static void Generate(Collector collector, IOptions options)
        {
            var template = new FormattersTemplate(collector, options);
            var generatedText = template.TransformText();

            var output = Path.Join(options.Output, $"{options.ClassName}.cs");
            var fileInfo = new FileInfo(output);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(output, generatedText, Encoding.UTF8);
            System.Console.WriteLine($"Output written to {fileInfo.FullName}");
        }
    }
}
