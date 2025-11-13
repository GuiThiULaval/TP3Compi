using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SyntaxAnalysis.Grammar;

namespace SemanticAnalysis
{
    public class SDT : ISDT
    {
        public Dictionary<Production, List<SemanticAction>> Rules { get; private set; }

        public SDT(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            Rules = [];
            OrderRules(definition);
        }

        private void OrderRules(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            foreach (KeyValuePair<Production, HashSet<SemanticAction>> pair in definition)
            {
                Rules[pair.Key] = [];
                Dictionary<Symbol, int> subscripts = [];
                subscripts[pair.Key.Head] = 0;

                // Inherited attributes.
                foreach (Symbol symbol in pair.Key.Body)
                {
                    if (subscripts.TryGetValue(symbol, out int subscript))
                    {
                        subscripts[symbol] = subscript + 1;
                    }
                    else
                    {
                        subscripts[symbol] = 0;
                    }

                    foreach (SemanticAction action in pair.Value)
                    {
                        if (action.Target.Symbol == symbol && action.Target.Subscript == subscripts[symbol])
                        {
                            pair.Value.Remove(action);
                            Rules[pair.Key].Add(action);
                        }
                    }
                }

                // Synthesized attributes.
                foreach (SemanticAction action in pair.Value)
                {
                    if (action.Target.Symbol == pair.Key.Head && action.Target.Subscript == 0)
                    {
                        pair.Value.Remove(action);
                        Rules[pair.Key].Add(action);
                    }
                }
            }
        }
    }
}
