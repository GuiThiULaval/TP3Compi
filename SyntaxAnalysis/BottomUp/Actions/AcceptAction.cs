using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.BottomUp.Actions
{
    public class AcceptAction : IParsingAction
    {
        public string Name { get; } = "acc";

        public void Execute(ParsingContext context)
        {
            context.Accepted = true;
        }
    }
}
