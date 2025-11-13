using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public class GroupNode(string pattern, IOperatorNode group) : OperatorNode(pattern)
    {
        public IOperatorNode Group { get; init; } = group;

        public override Fragment CompileFragment()
        {
            return Group.CompileFragment();
        }
    }
}
