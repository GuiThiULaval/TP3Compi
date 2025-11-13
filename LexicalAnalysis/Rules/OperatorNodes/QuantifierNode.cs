using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public abstract class QuantifierNode(string pattern, IOperatorNode operand) : OperatorNode(pattern)
    {
        public IOperatorNode Operand { get; init; } = operand;
    }
}
