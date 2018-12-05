using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Analysis
{
    public static class CompilationLoader
    {
        public interface IOptions
        {
            string Input { get; }
        }

        private const string ProjectName = "AnalyzingAssembly";

        public static Compilation Load(IOptions options)
        {
            var workspace = new AdhocWorkspace();
            var projectInfo = ProjectInfo
                .Create(ProjectId.CreateNewId(), VersionStamp.Create(), ProjectName, ProjectName, LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithMetadataReferences(GetReferences());

            var project = workspace.AddProject(projectInfo);

            var assembly = Assembly.GetExecutingAssembly();
            foreach (var embeddedSource in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".cs", System.StringComparison.Ordinal)))
            {
                using (var stream = assembly.GetManifestResourceStream(embeddedSource))
                {
                    workspace.AddDocument(project.Id, embeddedSource, SourceText.From(stream));
                }
            }

            var files = Directory.EnumerateFiles(options.Input, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                workspace.AddDocument(project.Id, file, SourceText.From(File.ReadAllText(file)));
            }

            return workspace
                .CurrentSolution
                .Projects
                .First()
                .GetCompilationAsync()
                .Result;
        }

        private static IEnumerable<MetadataReference> GetReferences()
        {
            return ((string)System.AppContext
                .GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
                .Split(Path.PathSeparator)
                .Select(a => MetadataReference.CreateFromFile(a));
        }
    }
}
