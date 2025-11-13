using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp.ParsingTables
{
    public interface ILRParsingTable
    {
        LRState StartState { get; }
        void ExecuteAction(LRState state, string symbolName, ParsingContext context);
    }
}
