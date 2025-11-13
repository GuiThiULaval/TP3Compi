namespace SyntaxAnalysis.Grammar
{
    public class ContextFreeGrammar
    {
        public Symbol Start { get; }
        public Production AugmentedStartProduction { get; }
        public HashSet<Production> Productions { get; }

        public HashSet<Symbol> Terminals { get; }
        public HashSet<Symbol> NonTerminals { get; }

        public Dictionary<Symbol, HashSet<Symbol>> First { get; }
        public Dictionary<Symbol, HashSet<Symbol>> Follow { get; }

        public ContextFreeGrammar(Symbol start, HashSet<Production> productions, HashSet<Symbol> terminals, HashSet<Symbol> nonTerminals)
        {
            Start = start;
            AugmentedStartProduction = new(Symbol.AugmentedStart, [start]);
            Productions = productions;

            Terminals = terminals;
            NonTerminals = nonTerminals;
            ComputeSymbols();

            ValidateProductions(start);

            First = [];
            Follow = [];
            ComputeFirstSets();
            ComputeFollowSets(start);
        }

        public HashSet<Symbol> FirstOfBody(List<Symbol> symbols)
        {
            HashSet<Symbol> firstOfBody = [];
            foreach (var symbol in symbols)
            {
                if (symbol.Type == SymbolType.NonTerminal)
                {
                    firstOfBody.UnionWith(First[symbol]);
                    if (!firstOfBody.Remove(Symbol.Epsilon))
                    {
                        return firstOfBody;
                    }
                }
                else
                {
                    firstOfBody.Add(symbol);
                    return firstOfBody;
                }
            }
            firstOfBody.Add(Symbol.Epsilon);
            return firstOfBody;
        }

        private void ComputeSymbols()
        {
            foreach (var production in Productions)
            {
                NonTerminals.Add(production.Head);
                foreach (var symbol in production.Body)
                {
                    switch (symbol.Type)
                    {
                        case SymbolType.Terminal:
                            Terminals.Add(symbol);
                            break;
                        case SymbolType.NonTerminal:
                            NonTerminals.Add(symbol);
                            break;
                        case SymbolType.Special:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void ValidateProductions(Symbol start)
        {
            foreach (var nonTerminal in NonTerminals)
            {
                if (Productions.All(production => production.Head != nonTerminal))
                {
                    throw new Exception($"NonTerminal {nonTerminal.Name} is not defined.");
                }

                if (nonTerminal != start && !Productions.Any(production => production.Body.Contains(nonTerminal)))
                {
                    throw new Exception($"NonTerminal {nonTerminal.Name} is not reachable.");
                }
            }
        }

        private void ComputeFirstSets()
        {
            foreach (var nonTerminal in NonTerminals)
            {
                First[nonTerminal] = [];
            }

            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var production in Productions)
                {
                    var firstOfBody = FirstOfBody(production.Body);
                    foreach (var firstSymbol in firstOfBody)
                    {
                        if (First[production.Head].Add(firstSymbol))
                        {
                            changed = true;
                        }
                    }
                }
            }
        }

        private void ComputeFollowSets(Symbol start)
        {
            foreach (var nonTerminal in NonTerminals)
            {
                Follow[nonTerminal] = [];
            }
            Follow[start].Add(Symbol.End);

            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var production in Productions)
                {
                    for (var i = 0; i < production.Body.Count; i++)
                    {
                        var symbol = production.Body[i];
                        if (symbol.Type != SymbolType.NonTerminal) continue;
                        var first = FirstOfBody(production.Body[(i + 1)..]);
                        if (first.Remove(Symbol.Epsilon))
                        {
                            foreach (var followSymbol in Follow[production.Head])
                            {
                                if (Follow[symbol].Add(followSymbol))
                                {
                                    changed = true;
                                }
                            }
                        }
                        foreach (var firstSymbol in first.Where(firstSymbol => Follow[symbol].Add(firstSymbol)))
                        {
                            changed = true;
                        }
                    }
                }
            }
        }
    }
}
