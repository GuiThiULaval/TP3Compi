using SyntaxAnalysis.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.TopDown
{
    public class LLParsingTable : ILLParsingTable
    {
        public Symbol StartSymbol { get; }

        private readonly Dictionary<Symbol, Dictionary<string, Production?>> _table;

        public LLParsingTable(ContextFreeGrammar grammar)
        {
            StartSymbol = grammar.Start;
            _table = [];
            foreach (Symbol nonterminal in grammar.Nonterminals)
            {
                _table[nonterminal] = new Dictionary<string, Production?>() { { Symbol.End.Name, null } };
                foreach (Symbol terminal in grammar.Terminals)
                {
                    _table[nonterminal][terminal.Name] = null;
                }
            }

            // Fill table.
            foreach (Production production in grammar.Productions)
            {
                HashSet<Symbol> first = grammar.FirstOfBody(production.Body);
                foreach (Symbol symbol in first)
                {
                    if (symbol.Type == SymbolType.Terminal)
                    {
                        if (_table[production.Head][symbol.Name] == null)
                        {
                            _table[production.Head][symbol.Name] = production;
                        }
                        else
                        {
                            // First/First conflict.
                            throw new Exception($"Conflict for nonterminal={production.Head.Name} and terminal={symbol.Name}.");
                        }
                    }
                }
                if (first.Contains(Symbol.Epsilon))
                {
                    HashSet<Symbol> follow = grammar.Follow[production.Head];
                    foreach (Symbol symbol in follow)
                    {
                        if (_table[production.Head][symbol.Name] == null)
                        {
                            _table[production.Head][symbol.Name] = production;
                        }
                        else
                        {
                            // First/Follow conflict.
                            throw new Exception($"Conflict for nonterminal={production.Head.Name} and terminal={symbol.Name}.");
                        }
                    }
                }
            }
        }

        public Production? GetProduction(Symbol nonterminal, string tokenName)
        {
            return _table[nonterminal][tokenName];
        }
    }
}
