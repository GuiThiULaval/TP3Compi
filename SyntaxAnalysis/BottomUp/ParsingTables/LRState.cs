using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp.ParsingTables
{
    public class LRState
    {
        public Symbol Representative { get; }
        public HashSet<Item> Kernel { get; }
        public HashSet<Item> Items { get; }
        public Dictionary<Symbol, LRState> Transitions { get; }

        private readonly ContextFreeGrammar _grammar;

        public LRState(Symbol representative, HashSet<Item> kernel, ContextFreeGrammar grammar)
        {
            Validate(representative, kernel, grammar);

            Representative = representative;
            _grammar = grammar;

            Kernel = kernel;
            Items = KernelClosure();
            Transitions = [];
        }

        private static void Validate(Symbol representative, HashSet<Item> kernel, ContextFreeGrammar grammar)
        {
            if (representative != Symbol.AugmentedStart && !grammar.NonTerminals.Contains(representative) && !grammar.Terminals.Contains(representative))
            {
                throw new Exception($"Representative {representative.Name} is not part of grammar.");
            }

            if (kernel.Count == 0)
            {
                throw new Exception("Kernel is empty.");
            }

            foreach (Item item in kernel)
            {
                if (item.Production != grammar.AugmentedStartProduction && !grammar.Productions.Contains(item.Production))
                {
                    throw new Exception("Kernel item is from a production outside of grammar.");
                }
            }
        }

        public LRState? AddTransition(Symbol symbol, HashSet<LRState> collection)
        {
            LRState? newState = null;
            HashSet<Item> kernel = GotoKernel(symbol);

            // Only add transition if kernel is not empty.
            if (kernel.Count > 0)
            {
                // Try to retrieve state from collection.
                foreach (LRState state in collection)
                {
                    if (state.Kernel.SetEquals(kernel))
                    {
                        newState = state;
                        break;
                    }
                }

                // Add new state to transitions.
                newState ??= new(symbol, kernel, _grammar);
                Transitions.Add(symbol, newState);
            }

            return newState;
        }

        private HashSet<Item> KernelClosure()
        {
            HashSet<Item> closure = new(Kernel);
            bool added = true;
            while (added)
            {
                added = false;
                HashSet<Item> previousClosure = new(closure);
                foreach (Item item in previousClosure)
                {
                    if (item.Suffix.Count > 0)
                    {
                        Symbol head = item.Suffix[0];
                        foreach (Production production in _grammar.Productions)
                        {
                            if (production.Head == head)
                            {
                                Item newItem = Item.Fetch(production, 0);
                                if (closure.Add(newItem))
                                {
                                    added = true;
                                }
                            }
                        }
                    }
                }
            }
            return closure;
        }

        private HashSet<Item> GotoKernel(Symbol symbol)
        {
            HashSet<Item> prime = [];
            foreach (Item item in Items)
            {
                if (item.Suffix.Count > 0)
                {
                    Symbol first = item.Suffix[0];
                    if (first == symbol)
                    {
                        Item newItem = Item.Fetch(item.Production, item.Position + 1);
                        prime.Add(newItem);
                    }
                }
            }
            return prime;
        }
    }
}
