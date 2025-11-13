using LexicalAnalysis.Rules.OperatorNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules
{
    public class RegularExpression
    {
        public string TokenName { get; init; }
        public string Pattern { get; init; }

        public RegularExpression(string tokenName, string pattern)
        {
            TokenName = tokenName;
            Pattern = pattern;
        }

        public IOperatorNode Parse()
        {
            RegularExpressionParser parser = new();
            return parser.GetOperatorTree(Pattern);
        }
    }
}
