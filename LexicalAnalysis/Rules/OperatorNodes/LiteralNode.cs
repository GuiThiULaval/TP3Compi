using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public class LiteralNode(string pattern, char literal) : OperatorNode(pattern)
    {
        public char Literal { get; init; } = literal;

        public override Fragment CompileFragment()
        {
            State entryState = new(Pattern);
            State exitState = new(Pattern);

            entryState.AddTransition(Literal, exitState);

            return new(entryState, exitState);
        }
    }
}
