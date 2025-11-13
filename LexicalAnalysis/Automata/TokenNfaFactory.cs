using LexicalAnalysis.Automata;

namespace LexicalAnalysis.Rules
{
    public static class TokenNfaFactory
    {
        public static NFA Build()
        {
            var accept = new Dictionary<State, string>();
            var start = new State("start");
            var id0 = new State("id0"); var id1 = new State("id1"); accept[id1] = "id";
            AddLetters(id0, id1); AddLettersDigitsUnderscore(id1, id1);
            var num0 = new State("num0"); var num1 = new State("num1"); var nDot = new State("nDot"); accept[num1] = "num";
            AddDigits(num0, num1); AddDigits(num1, num1); num1.AddTransition('.', nDot); AddDigits(nDot, num1);
            var ws0 = new State("ws0"); var ws1 = new State("ws1"); accept[ws1] = "ws";
            AddWhitespace(ws0, ws1); AddWhitespace(ws1, ws1);
            var plus0 = new State("plus0"); var plus1 = new State("plus1"); accept[plus1] = "+";
            AddChar(plus0, '+', plus1);
            var minus0 = new State("minus0"); var minus1 = new State("minus1"); accept[minus1] = "-";
            AddChar(minus0, '-', minus1);
            var times0 = new State("times0"); var times1 = new State("times1"); accept[times1] = "*";
            AddChar(times0, '*', times1);
            var slash0 = new State("slash0"); var slash1 = new State("slash1"); accept[slash1] = "/";
            AddChar(slash0, '/', slash1);
            var pow0 = new State("pow0"); var pow1 = new State("pow1"); accept[pow1] = "^";
            AddChar(pow0, '^', pow1);
            var openPar0 = new State("openPar0"); var openPar1 = new State("openPar1"); accept[openPar1] = "(";
            AddChar(openPar0, '(', openPar1);
            var closePar0 = new State("closePar0"); var closePar1 = new State("closePar1"); accept[closePar1] = ")";
            AddChar(closePar0, ')', closePar1);
            var equals0 = new State("equals0"); var equals1 = new State("equals1"); accept[equals1] = "=";
            AddChar(equals0, '=', equals1);
            var semiColon0 = new State("semiColon0"); var semiColon1 = new State("semiColon1"); accept[semiColon1] = ";";
            AddChar(semiColon0, ';', semiColon1);
            var comma0 = new State("comma0"); var comma1 = new State("comma1"); accept[comma1] = ",";
            AddChar(comma0, ',', comma1);
            start.AddTransition(Alphabet.Epsilon, id0);
            start.AddTransition(Alphabet.Epsilon, num0);
            start.AddTransition(Alphabet.Epsilon, ws0);
            start.AddTransition(Alphabet.Epsilon, plus0);
            start.AddTransition(Alphabet.Epsilon, minus0);
            start.AddTransition(Alphabet.Epsilon, times0);
            start.AddTransition(Alphabet.Epsilon, slash0);
            start.AddTransition(Alphabet.Epsilon, pow0);
            start.AddTransition(Alphabet.Epsilon, openPar0);
            start.AddTransition(Alphabet.Epsilon, closePar0);
            start.AddTransition(Alphabet.Epsilon, equals0);
            start.AddTransition(Alphabet.Epsilon, semiColon0);
            start.AddTransition(Alphabet.Epsilon, comma0);
            return new NFA(start, accept);
        }
        static void AddLetters(State from, State to) { for (char c = 'A'; c <= 'Z'; c++) from.AddTransition(c, to); for (char c = 'a'; c <= 'z'; c++) from.AddTransition(c, to); from.AddTransition('_', to); }
        static void AddLettersDigitsUnderscore(State from, State to) { AddLetters(from, to); AddDigits(from, to); }
        static void AddDigits(State from, State to) { for (char c = '0'; c <= '9'; c++) from.AddTransition(c, to); }
        static void AddWhitespace(State from, State to) { from.AddTransition(' ', to); from.AddTransition('\t', to); from.AddTransition('\r', to); from.AddTransition('\n', to); }
        static void AddChar(State from, char c, State to) { from.AddTransition(c, to); }
    }
}
