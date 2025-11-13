using SyntaxAnalysis.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.TopDown
{
    public interface ILLParsingTable
    {
        Symbol StartSymbol { get; }
        Production? GetProduction(Symbol nonTerminal, string tokenName);
    }
}
