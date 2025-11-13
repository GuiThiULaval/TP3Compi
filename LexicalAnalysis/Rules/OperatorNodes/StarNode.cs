using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public class StarNode(string pattern, IOperatorNode operand) : QuantifierNode(pattern, operand)
    {
        public override Fragment CompileFragment()
        {
            Fragment operandFragment = Operand.CompileFragment();

            State entryState = new(Pattern);
            State exitState = new(Pattern);

            entryState.AddTransition(Alphabet.Epsilon, operandFragment.Entry);
            entryState.AddTransition(Alphabet.Epsilon, exitState);
            operandFragment.Exit.AddTransition(Alphabet.Epsilon, operandFragment.Entry);
            operandFragment.Exit.AddTransition(Alphabet.Epsilon, exitState);

            return new(entryState, exitState);
        }
    }
}
