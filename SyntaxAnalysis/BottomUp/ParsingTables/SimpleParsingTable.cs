using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.BottomUp.Actions;
using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp.ParsingTables
{
    public class SimpleParsingTable : ILRParsingTable
    {
        public LRState StartState { get; }
        public HashSet<LRState> Collection { get; }

        public Dictionary<LRState, Dictionary<string, IParsingAction>> _table;

        public SimpleParsingTable(ContextFreeGrammar grammar)
        {
            Item startItem = Item.Fetch(grammar.AugmentedStartProduction, 0);
            StartState = new(grammar.AugmentedStartProduction.Head, [startItem], grammar);
            Collection = [StartState];
            ConstructCollection(grammar);

            _table = [];
            FillTable(grammar);
        }

        private void ConstructCollection(ContextFreeGrammar grammar)
        {
            IEnumerable<Symbol> allSymbols = grammar.Nonterminals.Union(grammar.Terminals);

            Queue<LRState> unvisited = new(Collection);
            while (unvisited.Count > 0)
            {
                LRState state = unvisited.Dequeue();
                foreach (Symbol symbol in allSymbols)
                {
                    // Try to add transition.
                    LRState? newState = state.AddTransition(symbol, Collection);
                    if (newState is not null && Collection.Add(newState))
                    {
                        // New state must be visited.
                        unvisited.Enqueue(newState);
                    }
                }
            }
        }

        private void FillTable(ContextFreeGrammar grammar)
        {
            foreach (LRState state in Collection)
            {
                Dictionary<string, IParsingAction> actions = [];
                foreach (Item item in state.Items)
                {
                    foreach (Symbol terminal in grammar.Terminals)
                    {
                        if (item.Suffix.Count > 0 && item.Suffix[0] == terminal && state.Transitions.TryGetValue(terminal, out LRState? destination))
                        {
                            ShiftAction shiftAction = new(destination);
                            if (!actions.TryAdd(terminal.Name, shiftAction))
                            {
                                throw new Exception($"Conflict for state={state.Representative.Name} and terminal={terminal.Name}.");
                            }
                        }
                    }
                    if (item.IsComplete)
                    {
                        if (item.Production.Head == Symbol.AugmentedStart)
                        {
                            AcceptAction acceptAction = new();
                            if (!actions.TryAdd(Symbol.End.Name, acceptAction))
                            {
                                throw new Exception($"Conflict for state={state.Representative.Name} and terminal={Symbol.End.Name}.");
                            }
                        }
                        else
                        {
                            ReduceAction reduceAction = new(item.Production);
                            foreach (Symbol followSymbol in grammar.Follow[item.Production.Head])
                            {
                                if (!actions.TryAdd(followSymbol.Name, reduceAction))
                                {
                                    throw new Exception($"Conflict for state={state.Representative.Name} and terminal={followSymbol.Name}.");
                                }
                            }
                        }
                    }
                }
                foreach (Symbol nonterminal in grammar.Nonterminals)
                {
                    if (state.Transitions.TryGetValue(nonterminal, out LRState? destination))
                    {
                        GotoAction gotoAction = new(destination);
                        if (!actions.TryAdd(nonterminal.Name, gotoAction))
                        {
                            throw new Exception($"Conflict for state={state.Representative.Name} and terminal={nonterminal.Name}.");
                        }
                    }
                }
                _table.Add(state, actions);
            }
        }

        public void ExecuteAction(LRState state, string symbolName, ParsingContext context)
        {
            if (!_table.TryGetValue(state, out Dictionary<string, IParsingAction>? actions))
            {
                throw new Exception($"State {state.Representative.Name} is not part of table.");
            }
            if (!actions.TryGetValue(symbolName, out IParsingAction? action))
            {
                throw new Exception($"State {state.Representative.Name} has no action for symbol {symbolName}.");
            }

            action.Execute(context);
        }
    }
}
