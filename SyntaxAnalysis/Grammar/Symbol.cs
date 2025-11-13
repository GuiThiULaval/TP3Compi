namespace SyntaxAnalysis.Grammar
{
    public enum SymbolType
    {
        Terminal,
        NonTerminal,
        Special,
    }

    public class Symbol(SymbolType type, string name)
    {
        public static readonly Symbol Epsilon = new(SymbolType.Special, "epsilon");
        public static readonly Symbol End = new(SymbolType.Special, "$");
        public static readonly Symbol AugmentedStart = new(SymbolType.NonTerminal, "S'");

        public SymbolType Type { get; } = type;
        public string Name { get; } = name;
    }
}
