using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public interface IOperatorNode
    {
        string Pattern { get; }

        NFA Compile(Alphabet alphabet, string tokenName);
        Fragment CompileFragment();
    }
}
