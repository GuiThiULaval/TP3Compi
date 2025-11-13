using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public class UnionNode(string pattern, IOperatorNode leftTerm, IOperatorNode rightTerm) : OperatorNode(pattern)
    {
        public IOperatorNode LeftTerm { get; init; } = leftTerm;
        public IOperatorNode RightTerm { get; init; } = rightTerm;

        public override Fragment CompileFragment()
        {
            Fragment leftFragment = LeftTerm.CompileFragment();
            Fragment rightFragment = RightTerm.CompileFragment();

            State entryState = new(Pattern);
            State exitState = new(Pattern);

            entryState.AddTransition(Alphabet.Epsilon, leftFragment.Entry);
            entryState.AddTransition(Alphabet.Epsilon, rightFragment.Entry);
            leftFragment.Exit.AddTransition(Alphabet.Epsilon, exitState);
            rightFragment.Exit.AddTransition(Alphabet.Epsilon, exitState);

            return new(entryState, exitState);
        }
    }
}
