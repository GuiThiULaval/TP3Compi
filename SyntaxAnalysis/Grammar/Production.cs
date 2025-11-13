namespace SyntaxAnalysis.Grammar
{
    public class Production
    {
        public Symbol Head { get; }
        public List<Symbol> Body { get; }

        public bool IsEmpty => Body.Count == 1 && Body[0] == Symbol.Epsilon;

        public Production(Symbol head, List<Symbol> body)
        {
            Validate(head, body);

            Head = head;
            Body = body;
        }

        private static void Validate(Symbol head, List<Symbol> body)
        {
            if (head.Type != SymbolType.NonTerminal)
            {
                throw new Exception($"Head {head.Name} cannot be of type {head.Type}.");
            }

            if (body.Count == 0)
            {
                throw new Exception($"Body cannot be empty.");
            }

            if (body.Any(symbol => symbol.Type == SymbolType.Special))
            {
                if (body.Count > 1)
                {
                    throw new Exception($"Body cannot contain more than one special symbol.");
                }
                if (body[0] != Symbol.Epsilon)
                {
                    throw new Exception($"Epsilon must be a single symbol.");
                }
            }
        }

        public string GetDisplayText()
        {
            return $"{Head.Name} -> {string.Join("", Body.Select(s => s.Name))}";
        }
    }
}
