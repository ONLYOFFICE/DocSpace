using CommandLine;

namespace ASC.Resource.Manager
{
    public class Options
    {
        [Option('p', "project", Required = false, HelpText = "Project")]
        public string Project { get; set; }

        [Option('m', "module", Required = false, HelpText = "Module")]
        public string Module { get; set; }

        [Option('e', "exportpath", Required = false, HelpText = "Export Path")]
        public string ExportPath { get; set; }

        [Option('c', "culture", Required = false, HelpText = "Culture")]
        public string Culture { get; set; }
    }
}
