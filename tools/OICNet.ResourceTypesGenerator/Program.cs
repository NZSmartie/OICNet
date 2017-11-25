using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Diagnostics;
using System.IO;

using OICNet.ResourceTypesGenerator.Internal;

namespace OICNet.ResourceTypesGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cliArgsResults = new CommandLine.Parser(p => p.HelpWriter = Console.Out).ParseArguments<CliOptions>(args);

            if (cliArgsResults.Tag == CommandLine.ParserResultType.NotParsed)
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine(args);
                    Console.ReadLine();
                }
                return;
            }

            var options = (cliArgsResults as CommandLine.Parsed<CliOptions>)?.Value
                ?? throw new InvalidCastException();

            var schemaResolver = new OicSchemaResolver();

            // Preload our resolver with desired schemas
            foreach (var schemaPath in options.InputSchemas)
            {
                if (!File.Exists(schemaPath))
                {
                    Console.WriteLine($"Schema not found: {schemaPath}");
                    return;

                }
                schemaResolver.Add(schemaPath);
            }

            foreach(var searchPath in options.InputSchemaPaths)
                schemaResolver.SearchIn(searchPath);

            var generator = new ResourceTypeGenerator(schemaResolver);
            generator.Namespace = options.Namespace;

            foreach (var pathPattern in options.InputFilePatterns)
            {
                foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, pathPattern))
                {
                    var outputFile = Path.GetFileNameWithoutExtension(file) + ".cs";
                    //Console.WriteLine($"Going to read {file}");
                    generator.With(file)
                             .SubType<OicCoreResource>()
                             .Generate(Path.Combine(options.OutputDirectory, outputFile));
                }
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine($"Press <enter> to exit");
                Console.ReadLine();
            }
        }
    }
}
