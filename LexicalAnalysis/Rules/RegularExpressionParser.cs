using LexicalAnalysis.Rules.OperatorNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = LexicalAnalysis.Rules.OperatorNodes.Range;

namespace LexicalAnalysis.Rules
{
    internal class ParsingContext
    {
        public string Pattern { get; init; }
        public int Position { get => _position; }
        public RegularExpressionToken CurrentToken { get => _currentToken; }

        private int _position;
        private RegularExpressionToken _currentToken;

        private static readonly Dictionary<char, RegularExpressionTokenName> SymbolToTokenNameMap = new()
        {
            { '(', RegularExpressionTokenName.LeftParenthesis },
            { ')', RegularExpressionTokenName.RightParenthesis },
            { '*', RegularExpressionTokenName.Star },
            { '+', RegularExpressionTokenName.Plus },
            { '?', RegularExpressionTokenName.Optional },
            { '|', RegularExpressionTokenName.Union },
            { '[', RegularExpressionTokenName.LeftSquareBracket },
            { ']', RegularExpressionTokenName.RightSquareBracket },
            { '-', RegularExpressionTokenName.Dash },
            { '\\', RegularExpressionTokenName.Escape },
        };

        public ParsingContext(string pattern)
        {
            Pattern = pattern;
            _position = 0;
            _currentToken = GetCurrentToken();
        }

        public string GetCurrentSubPattern(int start)
        {
            return Pattern[start.._position];
        }

        public void ConsumeToken(RegularExpressionTokenName name)
        {
            if (_currentToken.Name != name) throw new Exception($"Expected token {name} but found token {_currentToken.Name}.");

            _position++;
            _currentToken = GetCurrentToken();
        }

        private RegularExpressionToken GetCurrentToken()
        {
            if (_position >= Pattern.Length)
            {
                return new(RegularExpressionTokenName.End, '\0');
            }
            else
            {
                char currentSymbol = Pattern[_position];
                if (!SymbolToTokenNameMap.TryGetValue(currentSymbol, out RegularExpressionTokenName tokenName))
                {
                    tokenName = RegularExpressionTokenName.Literal;
                }
                else if (_position > 0 && _currentToken.Name == RegularExpressionTokenName.Escape)
                {
                    tokenName = RegularExpressionTokenName.Literal;
                }
                return new(tokenName, currentSymbol);
            }
        }
    }

    public class RegularExpressionParser
    {
        public IOperatorNode GetOperatorTree(string pattern)
        {
            ParsingContext context = new(pattern);
            return ParseExpression(context);
        }

        private IOperatorNode ParseExpression(ParsingContext context)
        {
            // An expression is a sequence of terms connected by the union operator.
            // Example : term1|term2|term3
            int start = context.Position;
            IOperatorNode node = ParseTerm(context);
            if (context.CurrentToken.Name == RegularExpressionTokenName.Union)
            {
                context.ConsumeToken(RegularExpressionTokenName.Union);
                IOperatorNode rightNode = ParseExpression(context);
                string pattern = context.GetCurrentSubPattern(start);
                node = new UnionNode(pattern, node, rightNode);
            }
            return node;
        }

        private IOperatorNode ParseTerm(ParsingContext context)
        {
            // A term is a sequence of factors concatenated together.
            // Example : factor1factor2factor3
            int start = context.Position;
            IOperatorNode node = ParseFactor(context);
            if (context.CurrentToken.Name == RegularExpressionTokenName.LeftParenthesis ||
                context.CurrentToken.Name == RegularExpressionTokenName.LeftSquareBracket ||
                context.CurrentToken.Name == RegularExpressionTokenName.Escape ||
                context.CurrentToken.Name == RegularExpressionTokenName.Literal)
            {
                // No token to consume for concatenation.
                //IOperatorNode rightNode = ParseExpression(context);
                IOperatorNode rightNode = ParseTerm(context);
                string pattern = context.GetCurrentSubPattern(start);
                node = new ConcatenationNode(pattern, node, rightNode);
            }
            return node;
        }

        private IOperatorNode ParseFactor(ParsingContext context)
        {
            // A factor is a symbol or a group modified by an operator.
            // Example : group*
            int start = context.Position;
            IOperatorNode node = ParseOperand(context);
            if (context.CurrentToken.Name == RegularExpressionTokenName.Star)
            {
                context.ConsumeToken(RegularExpressionTokenName.Star);
                string pattern = context.GetCurrentSubPattern(start);
                node = new StarNode(pattern, node);
            }
            else if (context.CurrentToken.Name == RegularExpressionTokenName.Plus)
            {
                context.ConsumeToken(RegularExpressionTokenName.Plus);
                string pattern = context.GetCurrentSubPattern(start);
                node = new PlusNode(pattern, node);
            }
            else if (context.CurrentToken.Name == RegularExpressionTokenName.Optional)
            {
                context.ConsumeToken(RegularExpressionTokenName.Optional);
                string pattern = context.GetCurrentSubPattern(start);
                node = new OptionalNode(pattern, node);
            }
            return node;
        }

        private IOperatorNode ParseOperand(ParsingContext context)
        {
            // An operand is a symbol or a group over an expression.
            // Example : (expression)
            return context.CurrentToken.Name switch
            {
                RegularExpressionTokenName.LeftParenthesis => ParseGroup(context),
                RegularExpressionTokenName.LeftSquareBracket => ParseCharacterClass(context),
                RegularExpressionTokenName.Escape => ParseEscape(context),
                RegularExpressionTokenName.Literal => ParseLiteral(context),
                _ => throw new Exception($"Found unexpected token {context.CurrentToken.Name}.")
            };
        }

        private GroupNode ParseGroup(ParsingContext context)
        {
            int start = context.Position;
            context.ConsumeToken(RegularExpressionTokenName.LeftParenthesis);
            IOperatorNode node = ParseExpression(context);
            context.ConsumeToken(RegularExpressionTokenName.RightParenthesis);
            string pattern = context.GetCurrentSubPattern(start); ;
            return new GroupNode(pattern, node);
        }

        private CharacterClassNode ParseCharacterClass(ParsingContext context)
        {
            int start = context.Position;
            context.ConsumeToken(RegularExpressionTokenName.LeftSquareBracket);

            HashSet<Range> ranges = [];
            HashSet<char> literals = [];
            LiteralNode? lastNode = ParsePotentiallyEscapedLiteral(context);
            while (context.CurrentToken.Name != RegularExpressionTokenName.RightSquareBracket)
            {
                if (context.CurrentToken.Name == RegularExpressionTokenName.Dash)
                {
                    if (lastNode is null) throw new Exception($"Found unexpected token {context.CurrentToken.Name}.");

                    context.ConsumeToken(RegularExpressionTokenName.Dash);
                    LiteralNode endNode = ParsePotentiallyEscapedLiteral(context);
                    Range range = new(lastNode.Literal, endNode.Literal);
                    ranges.Add(range);
                    lastNode = null;
                }
                else
                {
                    if (lastNode is not null)
                    {
                        literals.Add(lastNode.Literal);
                    }
                    lastNode = ParsePotentiallyEscapedLiteral(context);
                }
            }
            if (lastNode is not null)
            {
                literals.Add(lastNode.Literal);
            }

            context.ConsumeToken(RegularExpressionTokenName.RightSquareBracket);
            string pattern = context.GetCurrentSubPattern(start);
            return new CharacterClassNode(pattern, ranges, literals);
        }

        private LiteralNode ParsePotentiallyEscapedLiteral(ParsingContext context)
        {
            if (context.CurrentToken.Name == RegularExpressionTokenName.Escape)
            {
                return ParseEscape(context);
            }
            else
            {
                return ParseLiteral(context);
            }
        }

        private LiteralNode ParseEscape(ParsingContext context)
        {
            int start = context.Position;
            context.ConsumeToken(RegularExpressionTokenName.Escape);
            LiteralNode node = ParseLiteral(context);
            string pattern = context.GetCurrentSubPattern(start);
            return new(pattern, node.Literal);
        }

        private LiteralNode ParseLiteral(ParsingContext context)
        {
            char literal = context.CurrentToken.Value;
            LiteralNode literalNode = new(literal.ToString(), literal);
            context.ConsumeToken(RegularExpressionTokenName.Literal);
            return literalNode;
        }
    }
}
