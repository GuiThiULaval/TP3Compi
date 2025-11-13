using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;
using SemanticAnalysis.Attributes;
using SyntaxAnalysis.Grammar;

namespace SemanticAnalysis
{
    public class SemanticAnalyzer(ISDT scheme)
    {
        private readonly ISDT _scheme = scheme;

        public void Annotate(ParseNode node, Queue<Token> tokens)
        {
            if (node.Symbol.Type == SymbolType.Terminal)
            {
                Token token = tokens.Dequeue();
                node.SetLexicalValue(token.Value);
            }
            else if (node.Symbol != Symbol.Epsilon)
            {
                if (node.Production is null)
                {
                    throw new Exception($"Node for nonterminal symbol={node.Symbol.Name} should have a production.");
                }

                Dictionary<Symbol, int> subscripts = [];
                subscripts[node.Symbol] = 0;

                int index = 0;
                List<SemanticAction> actions = _scheme.Rules[node.Production];

                // Inherited attributes.
                foreach (ParseNode child in node.Children)
                {
                    if (subscripts.TryGetValue(child.Symbol, out int subscript))
                    {
                        subscripts[child.Symbol] = subscript + 1;
                    }
                    else
                    {
                        subscripts[child.Symbol] = 0;
                    }

                    while (index < actions.Count && actions[index].Target.Symbol == child.Symbol && actions[index].Target.Subscript == subscripts[child.Symbol])
                    {
                        actions[index].Execute(node);
                        index++;
                    }

                    Annotate(child, tokens);
                }

                // Synthesized attributes.
                while (index < actions.Count)
                {
                    if (actions[index].Target.Symbol != node.Symbol || actions[index].Target.Subscript != 0)
                    {
                        throw new Exception("Unexpected action.");
                    }

                    actions[index].Execute(node);
                    index++;
                }
            }
        }

        public ParseNode BuildLeftmostTree(List<Production> derivations)
        {
            return BuildTree(derivations, false);
        }

        public ParseNode BuildRightmostTree(List<Production> derivations)
        {
            return BuildTree(derivations, true);
        }

        private ParseNode BuildTree(List<Production> derivations, bool reverse)
        {
            if (derivations.Count == 0)
            {
                throw new Exception("No derivation to build parse tree.");
            }

            Stack<ParseNode> availableChildren = new();

            for (int i = derivations.Count - 1; i >= 0; i--)
            {
                Production production = derivations[i];
                ParseNode headNode = new(production.Head, production);

                // Order for leftmost or rightmost.
                List<Symbol> orderedBody = production.Body;
                if (reverse)
                {
                    orderedBody = [.. Enumerable.Reverse(production.Body)];
                }

                // Add child nodes.
                foreach (Symbol symbol in orderedBody)
                {
                    if (symbol.Type == SymbolType.Terminal || symbol == Symbol.Epsilon)
                    {
                        ParseNode childNode = new(symbol, null);
                        headNode.Children.Add(childNode);
                    }
                    else
                    {
                        ParseNode childNode = availableChildren.Pop();
                        if (childNode.Symbol != symbol)
                        {
                            throw new Exception($"Current symbol={symbol.Name} does not match with next available child symbol={childNode.Symbol.Name}.");
                        }
                        headNode.Children.Add(childNode);
                    }
                }

                // Store head node for future child.
                availableChildren.Push(headNode);
            }

            return availableChildren.Pop();
        }
    }
}
