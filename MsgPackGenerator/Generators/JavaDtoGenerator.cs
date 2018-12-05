using System.IO;
using System.Linq;
using System.Text;
using Analysis;
using Templates;

namespace Generators
{
    public static class JavaDtoGenerator
    {
        public interface IOptions : JavaDtoTemplate.IOptions
        {
            string Output { get; }
        }

        public static void Generate(Collector collector, IOptions options)
        {
            var objects = collector.ObjectDefinitions.Select(d => new JavaDtoTemplate(d, options));

            foreach (var template in objects)
            {
                var output = Path.Join(options.Output, template.Output);
                var fileInfo = new FileInfo(output);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                File.WriteAllText(output, template.TransformText(), Encoding.UTF8);
                System.Console.WriteLine($"DTO written to {fileInfo.FullName}");
            }
        }
    }
}
