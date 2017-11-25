using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OICNet.ResourceTypesGenerator.Internal
{
    public class CliOptions
    {


        [Option('i', "input", Required = true,
          HelpText = "Input files to be processed. Glob patterns are also supported")]
        public IEnumerable<string> InputFilePatterns { get; set; }

        [Option('n', "namespace", Required = true,
                  HelpText = "The namespace to place genreated classes in.")]
        public string Namespace { get; set; }

        [Option('o', "output", Default = "Output",
          HelpText = "Output directory")]
        public string OutputDirectory { get; set; }

        [Option('s', "schema",
          HelpText = "Input schemas to reference")]
        public IEnumerable<string> InputSchemas { get; set; }

        [Option('S', "schema-dir",
          HelpText = "Input schema search directory")]
        public IEnumerable<string> InputSchemaPaths { get; set; } = Enumerable.Empty<string>();
    }
}
