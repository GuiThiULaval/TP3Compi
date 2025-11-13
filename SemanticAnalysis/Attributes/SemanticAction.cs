using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyntaxAnalysis.Grammar;

namespace SemanticAnalysis.Attributes
{
    public class SemanticAction
    {
        public IAttributeBinding Target { get; }
        public HashSet<IAttributeBinding> Sources { get; }

        private readonly Action<ParseNode> _action;

        public SemanticAction(IAttributeBinding target, HashSet<IAttributeBinding> sources, Action<ParseNode> action)
        {
            Target = target;
            Sources = sources;
            _action = action;
        }

        public void Execute(ParseNode node)
        {
            if (node.Symbol != Target.Symbol)
            {
                bool found = false;
                foreach (ParseNode child in node.Children)
                {
                    if (child.Symbol == Target.Symbol)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new Exception($"No symbol found for target={Target.Symbol.Name}.");
                }
            }

            if (node.Symbol.Type == SymbolType.Terminal || node.Symbol == Symbol.Epsilon)
            {
                throw new Exception("Actions cannot target a terminal or epsilon symbol.");
            }

            _action(node);
        }
    }
}
