using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MyApp // Note: actual namespace depends on the project name.
{
    enum JSDocOperand
    {
        Typedef,
        Callback
    }
    
    internal class Program
    {
        // static void Main(string[] args)
        // {
        //     Console.WriteLine("is it working?");
        //     
        //     var files = System.IO.Directory.GetFiles(@"", "*.js", SearchOption.AllDirectories);
        //     
        // }
        
        const string jsDocBlockRegex = @"/\*\*[^*]*\*+([^/*][^*]*\*+)*/"; // This regex captures JSDoc comments
        const string ValidFileExtensions = "*.js";
        
        static void Main()
        {
            string directoryPath = @"C:\Users\Administrator\Documents\Github\phaser\src"; // replace with your directory path
            
            // var outputFile = @$"C:\Users\Administrator\Documents\Github\phaser\src\dreasoutput-{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.output";
            const string baseOutputDirectory = @"D:\phasertypedexextractor_output\";
            
            WriteTypeDefsToFile(directoryPath, JSDocOperand.Typedef, Path.Combine(baseOutputDirectory, "typedefs.output"));
            WriteTypeDefsToFile(directoryPath, JSDocOperand.Callback, Path.Combine(baseOutputDirectory, "callbacks.output"));
        }

        private static void WriteTypeDefsToFile(string directoryPath, JSDocOperand jsDocOperand, string outputFilepath)
        {
            var typedefs = ExtractTypeDefs(directoryPath, jsDocOperand);
            
            Console.WriteLine("Total: " + typedefs.Count);

            // Print the extracted typedefs for demonstration
            foreach (var typedef in typedefs)
            {
                Console.WriteLine(typedef);
            }

            // Write the typedefs to the file
            File.WriteAllLines(outputFilepath, typedefs, Encoding.UTF8);
        }
        
        private static List<string> ExtractTypeDefs(string directoryPath, JSDocOperand jsDocOperand)
        {
            var allTypeDefs = new List<string>();
            var jsFiles = Directory.GetFiles(directoryPath, ValidFileExtensions, SearchOption.AllDirectories);
            
            var searchToken = jsDocOperand switch
            {
                JSDocOperand.Typedef => "@typedef",
                JSDocOperand.Callback => "@callback",
                _ => throw new ArgumentOutOfRangeException(nameof(jsDocOperand), jsDocOperand, null)
            };

            foreach (var jsFile in jsFiles)
            {
                var content = File.ReadAllText(jsFile);
                allTypeDefs.AddRange(FindTypeDefsInContent(content, searchToken));
            }

            return allTypeDefs;
        }

        private static List<string> FindTypeDefsInContent(string content, string searchToken)
        {
            var typedefs = new List<string>();

            var matches = Regex.Matches(content, jsDocBlockRegex);
            foreach (Match match in matches)
            {
                 //if (match.Value.Contains("@typedef"))
                // if (match.Value.Contains("@callback"))
                if (match.Value.Contains(searchToken))
                {
                    typedefs.Add(match.Value);
                }
            }

            return typedefs;
        }
    }
}
    
