using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.BottomUp.ParsingTables;
using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp.Actions
{
    public class ReduceAction(Production production) : IParsingAction
    {
        public string Name { get; } = $"r{production.Head.Name}";
        public Production Production { get; } = production;

        public void Execute(ParsingContext context)
        {
            // Pop body states.
            if (Production.Body[0] != Symbol.Epsilon)
            {
                for (int i = Production.Body.Count; i > 0; i--)
                {
                    LRState state = context.StateStack.Pop();
                    Symbol symbol = Production.Body[i - 1];
                    if (state.Representative != symbol)
                    {
                        throw new Exception($"State representative {state.Representative.Name} should be the same as body symbol {symbol.Name}.");
                    }
                }
            }

            // Execute goto action.
            context.Table.ExecuteAction(context.StateStack.Peek(), Production.Head.Name, context);
            LRState topState = context.StateStack.Peek();
            if (topState.Representative != Production.Head)
            {
                throw new Exception($"State representative {topState.Representative.Name} should be the same as head symbol {Production.Head.Name}.");
            }

            // Extend derivation.
            context.Derivation.Push(Production);
        }
    }
}
