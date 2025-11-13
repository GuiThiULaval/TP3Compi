using LexicalAnalysis;
using SyntaxAnalysis.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.TopDown
{
    public class NonrecursivePredictiveParser(ILLParsingTable parsingTable)
    {
        private readonly ILLParsingTable _parsingTable = parsingTable;

        public List<Production> Parse(List<Token> input)
        {
            List<Production> derivation = [];
            Stack<Symbol> stack = new([Symbol.End, _parsingTable.StartSymbol]);
            int pointer = 0;

            while (stack.Count > 0)
            {
                Symbol topSymbol = stack.Pop();
                Token topInput = input[pointer];

                if (topSymbol.Type == SymbolType.Terminal || topSymbol == Symbol.End)
                {
                    // Match.
                    if (topSymbol.Name != topInput.Name && topSymbol != Symbol.End && topInput != Token.End)
                    {
                        throw new Exception("Error while parsing, invalid input sequence.");
                    }

                    pointer++;
                }
                else
                {
                    string tokenName = topInput.Name;
                    if (tokenName == Token.End.Name)
                    {
                        tokenName = Symbol.End.Name;
                    }

                    Production production = _parsingTable.GetProduction(topSymbol, tokenName)
                        ?? throw new Exception("Error while parsing, blank cell.");

                    // Apply production.
                    derivation.Add(production);
                    if (production.Body[0] != Symbol.Epsilon)
                    {
                        for (int i = production.Body.Count - 1; i >= 0; i--)
                        {
                            stack.Push(production.Body[i]);
                        }
                    }
                }
            }

            return derivation;
        }
    }
}
