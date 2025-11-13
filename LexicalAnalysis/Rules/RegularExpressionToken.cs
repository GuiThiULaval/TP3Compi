using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules
{
    public enum RegularExpressionTokenName
    {
        LeftParenthesis,
        RightParenthesis,
        Star,
        Plus,
        Optional,
        Union,
        LeftSquareBracket,
        RightSquareBracket,
        Dash,
        Escape,
        Literal,
        End
    }

    public class RegularExpressionToken(RegularExpressionTokenName name, char value)
    {
        public RegularExpressionTokenName Name { get; init; } = name;
        public char Value { get; init; } = value;
    }
}
