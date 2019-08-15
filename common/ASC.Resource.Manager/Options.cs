using CommandLine;

namespace ASC.Resource.Manager
{
    public class Options
    {
        [Option('p', "project", Required = false, HelpText = "Project")]
        public string Project { get; set; }

        [Option('m', "module", Required = false, HelpText = "Module")]
        public string Module { get; set; }

        [Option("fp", Required = false, HelpText = "File Path")]
        public string FilePath { get; set; }

        [Option('e', "exportpath", Required = false, HelpText = "Export Path", Default = "..\\..\\..\\..\\ASC.Common\\")]
        public string ExportPath { get; set; }

        [Option('c', "culture", Required = false, HelpText = "Culture")]
        public string Culture { get; set; }

        [Option('f', "format", Required = false, HelpText = "Format", Default = "xml")]
        public string Format { get; set; }

        [Option('k', "key", Required = false, HelpText = "Key", Default = "")]
        public string Key { get; set; }

        public void Deconstruct(out string project, out string module, out string filePath, out string exportPath, out string culture, out string format, out string key)
            => (project, module, filePath, exportPath, culture, format, key) = (Project, Module, FilePath, ExportPath, Culture, Format, Key);
    }
}
