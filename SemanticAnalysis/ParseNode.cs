using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SyntaxAnalysis.Grammar;

namespace SemanticAnalysis
{
    public class ParseNode
    {
        public Symbol Symbol { get; }
        public Production? Production { get; }
        public List<ParseNode> Children { get; } = [];

        private readonly Dictionary<ISemanticAttribute, object?> _attributes = [];

        public ParseNode(Symbol symbol, Production? production)
        {
            Symbol = symbol;
            Production = production;
        }

        public T? GetAttributeValue<T>(SemanticAttribute<T> attribute)
        {
            return (T?)_attributes[attribute];
        }

        public void SetAttributeValue<T>(SemanticAttribute<T> attribute, T value)
        {
            _attributes[attribute] = value;
        }

        public void SetLexicalValue(object? value)
        {
            _attributes[SemanticAttribute.LEXICAL_VALUE] = value;
        }

        public ParseNode GetBindedNode(IAttributeBinding binding)
        {
            if (Symbol == binding.Symbol && binding.Subscript == 0)
            {
                return this;
            }

            Dictionary<Symbol, int> subscripts = [];
            subscripts[Symbol] = 0;

            foreach (ParseNode child in Children)
            {
                if (child.Symbol == binding.Symbol)
                {
                    if (!subscripts.ContainsKey(child.Symbol))
                    {
                        subscripts[child.Symbol] = 0;
                    }
                    else
                    {
                        subscripts[Symbol]++;
                    }

                    if (subscripts[Symbol] == binding.Subscript)
                    {
                        return child;
                    }
                }
            }

            throw new Exception($"Could not find node for symbol={binding.Symbol} and subscript={binding.Subscript}.");
        }
    }
}
