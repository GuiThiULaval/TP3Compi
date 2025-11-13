using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;
using SyntaxAnalysis.BottomUp.ParsingTables;
using SyntaxAnalysis.Grammar;

namespace SyntaxAnalysis.BottomUp
{
    public class ParsingContext(ILRParsingTable table)
    {
        public ILRParsingTable Table { get; } = table;
        public int CurrentInputPosition { get; set; } = 0;
        public Stack<LRState> StateStack { get; } = new([table.StartState]);
        public Stack<Production> Derivation { get; } = [];
        public bool Accepted { get; set; } = false;
    }

    public class ShiftReduceParser(ILRParsingTable table)
    {
        private readonly ILRParsingTable _table = table;

        public List<Production> Parse(List<Token> tokens)
        {
            ValidateInput(tokens);

            ParsingContext context = new(_table);
            while (!context.Accepted)
            {
                if (context.CurrentInputPosition >= tokens.Count)
                {
                    throw new Exception("Could not parse input sequence.");
                }

                // Execute next action.
                LRState topState = context.StateStack.Peek();
                string currentTokenName = tokens[context.CurrentInputPosition].Name;
                if (currentTokenName == Token.End.Name)
                {
                    currentTokenName = Symbol.End.Name;
                }
                _table.ExecuteAction(topState, currentTokenName, context);
            }

            return [.. context.Derivation];
        }

        private static void ValidateInput(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("Input should contain at least one token.");
            }

            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i] == Token.End)
                {
                    throw new Exception("End token should only appear at end of input.");
                }
            }

            if (tokens.Last() != Token.End)
            {
                throw new Exception("Input should end with end token.");
            }
        }
    }
}
