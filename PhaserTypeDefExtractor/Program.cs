#define MULTI_THREADED

using System;
using System.Text;
using System.Text.RegularExpressions;
#if MULTI_THREADED
using System.Collections.Concurrent;
#endif

enum JSDocOperand
{
    Typedef,
    Callback
}

internal class Program
{
    // This regex captures JSDoc comments
    const string JSDOC_BLOCK_REGEX = @"/\*\*[^*]*\*+([^/*][^*]*\*+)*/"; 
    const string VALID_FILE_EXTENSIONS = "*.js";

    private static void Main()
    {
        var directoryPath = @"C:\Users\Administrator\Documents\Github\phaser\src";

        // WriteTypeDefsToFile(directoryPath, JSDocOperand.Typedef, Path.Combine(baseOutputDirectory, "typedefs.output"));
        // WriteTypeDefsToFile(directoryPath, JSDocOperand.Callback, Path.Combine(baseOutputDirectory, "callbacks.output"));

        var typedefs = ExtractTypeDefs(directoryPath, JSDocOperand.Typedef);
        var callbacks = ExtractTypeDefs(directoryPath, JSDocOperand.Callback);
        
        var allJSDocBlocks = new List<string>();
        allJSDocBlocks.AddRange(typedefs);
        allJSDocBlocks.AddRange(callbacks);
        File.WriteAllLines(@"C:\Users\Administrator\Documents\jsBotArena\scripts\AllPhaserTypedefs.js", allJSDocBlocks, Encoding.UTF8);
    }

#if MULTI_THREADED
    private static ConcurrentBag<string> ExtractTypeDefs(string directoryPath, JSDocOperand jsDocOperand)
#else
    private static List<string> ExtractTypeDefs(string directoryPath, JSDocOperand jsDocOperand)
#endif
    {
        var searchToken = jsDocOperand switch
        {
            JSDocOperand.Typedef => "@typedef",
            JSDocOperand.Callback => "@callback",
            _ => throw new ArgumentOutOfRangeException(nameof(jsDocOperand), jsDocOperand, null)
        };
        
        var jsFiles = Directory.GetFiles(directoryPath, VALID_FILE_EXTENSIONS, SearchOption.AllDirectories);

#if MULTI_THREADED
        var allTypeDefs = new ConcurrentBag<string>();
        Parallel.ForEach(jsFiles, jsFile =>
        {
            var content = File.ReadAllText(jsFile);
            var jsDocBlocks = FindTypeDefsInContent(content, searchToken);
            Parallel.ForEach(jsDocBlocks, jsDocBlock => { allTypeDefs.Add(jsDocBlock); });
        });
#else
        var allTypeDefs = new List<string>();
        foreach (var jsFile in jsFiles)
        {
            var content = File.ReadAllText(jsFile);
            var jsDocBlocks = FindTypeDefsInContent(content, searchToken);
            allTypeDefs.AddRange(jsDocBlocks);
        }
#endif

        return allTypeDefs;
    }

    private static IEnumerable<string> FindTypeDefsInContent(string content, string searchToken)
    {
        var typedefs = new List<string>();

        var matches = Regex.Matches(content, JSDOC_BLOCK_REGEX);
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