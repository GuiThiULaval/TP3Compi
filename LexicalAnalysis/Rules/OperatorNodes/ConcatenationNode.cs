using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public class ConcatenationNode(string pattern, IOperatorNode leftFactor, IOperatorNode rightFactor) : OperatorNode(pattern)
    {
        public IOperatorNode LeftFactor { get; init; } = leftFactor;
        public IOperatorNode RightFactor { get; init; } = rightFactor;

        public override Fragment CompileFragment()
        {
            Fragment leftFragment = LeftFactor.CompileFragment();
            Fragment rightFragment = RightFactor.CompileFragment();

            leftFragment.Exit.AddTransition(Alphabet.Epsilon, rightFragment.Entry);

            return new(leftFragment.Entry, rightFragment.Exit);
        }
    }
}
