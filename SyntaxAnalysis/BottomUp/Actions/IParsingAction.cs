using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.BottomUp.Actions
{
    public interface IParsingAction
    {
        string Name { get; }

        void Execute(ParsingContext context);
    }
}
