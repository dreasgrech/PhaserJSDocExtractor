using System;
using System.Text;
using System.Text.RegularExpressions;

enum JSDocOperand
{
    Typedef,
    Callback
}

internal class Program
{
    // This regex captures JSDoc comments
    const string jsDocBlockRegex = @"/\*\*[^*]*\*+([^/*][^*]*\*+)*/"; 
    const string ValidFileExtensions = "*.js";

    static void Main()
    {
        string directoryPath = @"C:\Users\Administrator\Documents\Github\phaser\src";

        // WriteTypeDefsToFile(directoryPath, JSDocOperand.Typedef, Path.Combine(baseOutputDirectory, "typedefs.output"));
        // WriteTypeDefsToFile(directoryPath, JSDocOperand.Callback, Path.Combine(baseOutputDirectory, "callbacks.output"));

        var allJSDocBlocks = new List<string>();
        allJSDocBlocks.AddRange(ExtractTypeDefs(directoryPath, JSDocOperand.Typedef));
        allJSDocBlocks.AddRange(ExtractTypeDefs(directoryPath, JSDocOperand.Callback));
        File.WriteAllLines(@"C:\Users\Administrator\Documents\jsBotArena\scripts\AllPhaserTypedefs.js", allJSDocBlocks, Encoding.UTF8);
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
            if (match.Value.Contains(searchToken))
            {
                typedefs.Add(match.Value);
            }
        }

        return typedefs;
    }
}