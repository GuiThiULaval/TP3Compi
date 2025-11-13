using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.BottomUp.ParsingTables;

namespace SyntaxAnalysis.BottomUp.Actions
{
    public class GotoAction(LRState state) : IParsingAction
    {
        public string Name { get; } = state.Representative.Name;
        public LRState State { get; } = state;

        public void Execute(ParsingContext context)
        {
            context.StateStack.Push(State);
        }
    }
}
