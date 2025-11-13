using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public record Range(char Start, char End);

    public class CharacterClassNode(string pattern, HashSet<Range> ranges, HashSet<char> literals) : OperatorNode(pattern)
    {
        public HashSet<Range> Ranges { get; init; } = ranges;
        public HashSet<char> Literals { get; init; } = literals;

        public override Fragment CompileFragment()
        {
            State entryState = new(Pattern);
            State exitState = new(Pattern);

            foreach (Range range in Ranges)
            {
                foreach (char symbol in Alphabet.GetSymbolsFromRange(range.Start, range.End))
                {
                    entryState.AddTransition(symbol, exitState);
                }
            }
            foreach (char literal in Literals)
            {
                entryState.AddTransition(literal, exitState);
            }

            return new(entryState, exitState);
        }
    }
}
