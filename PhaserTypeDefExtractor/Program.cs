#define MULTI_THREADED

using System.Text;
using System.Text.RegularExpressions;
#if MULTI_THREADED
using System.Collections.Concurrent;
#endif

namespace PhaserTypeDefExtractor
{
    enum JSDocOperand
    {
        Typedef,
        Callback
    }

    internal class Program
    {
        // This regex captures JSDoc comments
        private const string JSDOC_BLOCK_REGEX = @"/\*\*[^*]*\*+([^/*][^*]*\*+)*/";
        private const string VALID_FILE_EXTENSIONS = "*.js";
        private const string PHASER_DIRECTORY = @"C:\Users\Administrator\Documents\Github\phaser\src";
        private const string OUTPUT_FILE_PATH = @"C:\Users\Administrator\Documents\jsBotArena\scripts\AllPhaserTypedefs.js";
        private static readonly Encoding OutputFileEncoding = Encoding.UTF8;

        private static void Main()
        {
            var typedefs = ExtractTypeDefs(PHASER_DIRECTORY, JSDocOperand.Typedef);
            var callbacks = ExtractTypeDefs(PHASER_DIRECTORY, JSDocOperand.Callback);

            var allJSDocBlocks = new List<string>();
            allJSDocBlocks.AddRange(typedefs);
            allJSDocBlocks.AddRange(callbacks);
            File.WriteAllLines(OUTPUT_FILE_PATH, allJSDocBlocks, OutputFileEncoding);
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
            var matches = Regex.Matches(content, JSDOC_BLOCK_REGEX);
            
#if MULTI_THREADED
            var typedefs = new ConcurrentBag<string>();
            Parallel.ForEach(matches, match =>
            {
                if (match.Value.Contains(searchToken))
                {
                    typedefs.Add(match.Value);
                }
            });
#else
            var typedefs = new List<string>();
            foreach (Match match in matches)
            {
                if (match.Value.Contains(searchToken))
                {
                    typedefs.Add(match.Value);
                }
            }
#endif

            return typedefs;
        }
    }
}