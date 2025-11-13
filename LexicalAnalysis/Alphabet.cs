using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis
{
    public class Alphabet
    {
        public const char Epsilon = '\u03B5'; // 'ε'
        public const char EndOfFile = '\u2404'; // '␄'

        public static readonly List<char> Digits = [.. "0123456789"];
        public static readonly List<char> Lowercases = [.. "abcdefghijklmnopqrstuvwxyz"];
        public static readonly List<char> Uppercases = [.. "ABCDEFGHIJKLMNOPQRSTUVWXYZ"];

        public HashSet<char> Symbols { get; init; }

        public Alphabet(HashSet<char> symbols)
        {
            if (symbols.Contains(Epsilon))
            {
                throw new Exception($"Special symbol Epsilon={Epsilon} should not appear in alphabet.");
            }
            if (symbols.Contains(EndOfFile))
            {
                throw new Exception($"Special symbol EndOfFile={EndOfFile} should not appear in alphabet.");
            }

            Symbols = symbols;
            Symbols.UnionWith(Digits);
            Symbols.UnionWith(Lowercases);
            Symbols.UnionWith(Uppercases);
        }

        public static List<char> GetSymbolsFromRange(char begin, char end)
        {
            List<List<char>> ranges = [Digits, Lowercases, Uppercases];
            foreach (List<char> range in ranges)
            {
                int beginIndex = range.IndexOf(begin);
                if (beginIndex != -1)
                {
                    int endIndex = range.IndexOf(end);
                    if (endIndex == -1) throw new Exception($"End symbol={end} could not be found in range of begin symbol={begin}.");

                    // Ranges with the same begin and end symbols are permitted.
                    if (endIndex < beginIndex) throw new Exception($"Range from begin symbol={begin} to end symbol={end} is in reverse order.");

                    int count = endIndex - beginIndex + 1;
                    return range.GetRange(beginIndex, count);
                }
            }
            throw new Exception($"No range found for begin symbol={begin}.");
        }
    }
}
