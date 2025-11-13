using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SyntaxAnalysis.Grammar;

namespace SemanticAnalysis
{
    public interface ISDT
    {
        Dictionary<Production, List<SemanticAction>> Rules { get; }
    }
}
