namespace SyntaxAnalysis.Grammar
{
    public class CreateProductions
    {

        public static ContextFreeGrammar CreateGrammar()
        {
            var Prog = new Symbol(SymbolType.NonTerminal, "Prog");
            var StmtList = new Symbol(SymbolType.NonTerminal, "StmtList");
            var Stmt = new Symbol(SymbolType.NonTerminal, "Stmt");
            var Params = new Symbol(SymbolType.NonTerminal, "Params");
            var ParamTail = new Symbol(SymbolType.NonTerminal, "ParamTail");
            var ArgList = new Symbol(SymbolType.NonTerminal, "ArgList");
            var ArgTail = new Symbol(SymbolType.NonTerminal, "ArgTail");
            var Expr = new Symbol(SymbolType.NonTerminal, "Expr");
            var EPrime = new Symbol(SymbolType.NonTerminal, "E'");
            var Term = new Symbol(SymbolType.NonTerminal, "Term");
            var TPrime = new Symbol(SymbolType.NonTerminal, "T'");
            var Pow = new Symbol(SymbolType.NonTerminal, "Pow");
            var PowTail = new Symbol(SymbolType.NonTerminal, "PowTail");
            var Factor = new Symbol(SymbolType.NonTerminal, "Factor");
            var FactorTail = new Symbol(SymbolType.NonTerminal, "FactorTail");
            var id = new Symbol(SymbolType.Terminal, "id");
            var num = new Symbol(SymbolType.Terminal, "num");
            var plus = new Symbol(SymbolType.Terminal, "+");
            var minus = new Symbol(SymbolType.Terminal, "-");
            var star = new Symbol(SymbolType.Terminal, "*");
            var slash = new Symbol(SymbolType.Terminal, "/");
            var caret = new Symbol(SymbolType.Terminal, "^");
            var openPar = new Symbol(SymbolType.Terminal, "(");
            var closePar = new Symbol(SymbolType.Terminal, ")");
            var eq = new Symbol(SymbolType.Terminal, "=");
            var semi = new Symbol(SymbolType.Terminal, ";");
            var comma = new Symbol(SymbolType.Terminal, ",");
            var kw_let = new Symbol(SymbolType.Terminal, "let");
            var kw_fn = new Symbol(SymbolType.Terminal, "fn");
            var kw_print = new Symbol(SymbolType.Terminal, "print");
            var P = new HashSet<Production>
            {
                new(Prog, [StmtList]),
                new(StmtList, [Stmt, StmtList]),
                new(StmtList, [Symbol.Epsilon]),
                new(Stmt, [kw_let, id, eq, Expr, semi]),
                new(Stmt, [kw_fn, id, openPar, Params, closePar, eq, Expr, semi]),
                new(Stmt, [kw_print, Expr, semi]),
                new(Params, [id, ParamTail]),
                new(Params, [Symbol.Epsilon]),
                new(ParamTail, [comma, id, ParamTail]),
                new(ParamTail, [Symbol.Epsilon]),
                new(ArgList, [Expr, ArgTail]),
                new(ArgList, [Symbol.Epsilon]),
                new(ArgTail, [comma, Expr, ArgTail]),
                new(ArgTail, [Symbol.Epsilon]),
                new(Expr, [Term, EPrime]),
                new(EPrime, [plus, Term, EPrime]),
                new(EPrime, [minus, Term, EPrime]),
                new(EPrime, [Symbol.Epsilon]),
                new(Term, [Pow, TPrime]),
                new(TPrime, [star, Pow, TPrime]),
                new(TPrime, [slash, Pow, TPrime]),
                new(TPrime, [Symbol.Epsilon]),
                new(Pow, [Factor, PowTail]),
                new(PowTail, [caret, Pow]),
                new(PowTail, [Symbol.Epsilon]),
                new(Factor, [openPar, Expr, closePar]),
                new(Factor, [num]),
                new(Factor, [id, FactorTail]),
                new(FactorTail, [openPar, ArgList, closePar]),
                new(FactorTail, [Symbol.Epsilon])
            };
            var terminals = new HashSet<Symbol>([id, num, plus, minus, star, slash, caret, openPar, closePar, eq, semi, comma, kw_let, kw_fn, kw_print, Symbol.End
            ]);
            var nonTerminals = new HashSet<Symbol>([Prog, StmtList, Stmt, Params, ParamTail, ArgList, ArgTail, Expr, EPrime, Term, TPrime, Pow, PowTail, Factor, FactorTail
            ]);
            return new ContextFreeGrammar(Prog, P, terminals, nonTerminals);
        }
    }
}
