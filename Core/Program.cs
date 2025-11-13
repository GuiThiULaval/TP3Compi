using LexicalAnalysis;
using LexicalAnalysis.Automata;
using SyntaxAnalysis.Grammar;
using SyntaxAnalysis.TopDown;

namespace Core
{
    internal class Program
    {
        static void Main(string[] args)
        {
             
            var grammar = CreateProductions.CreateGrammar();
            try
            {
                var ll = new LLParsingTable(grammar);
                var path = SolutionRoot();
                var nfa = LexicalAnalysis.Rules.TokenNfaFactory.Build();
                var dfa = DFA.FromNFA(nfa).Minimize();
                var reader = new InputReader(8192, path);
                var tokenizer = new Tokenizer(reader, dfa, (name, lexeme) => {
                    var lx = lexeme?.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                    if (name == "id" && (lx == "let" || lx == "fn" || lx == "print")) return new Token(lx, lx);
                    if (name == "ws") return null;
                    return new Token(name, lx ?? lexeme);
                });
                var tokens = new List<Token>(); Token t;
                do { t = tokenizer.GetNextToken(); tokens.Add(t); } while (t.Name != "$");
                var v = QuickValidate(tokens);
                if (v != null) { Console.WriteLine("INVALIDE---------: " + v); return; }
                var parser = new NonrecursivePredictiveParser(ll);
                parser.Parse(tokens);
                Console.WriteLine("VALIDE------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR---------");
                Console.WriteLine(ex.Message);
            }
        }

        // À ADAPTER SELON L'EMPLACEMENT DU FICHIER DE TEST DÉSIRÉ
        static string SolutionRoot()
        {
            return Path.Combine("..", "..", "..", "..", "tests", "negatifs", "testRefuse_KeywordAsIdent.mlg");
        }

        // GÉMÉRÉ AVEC L'ASSISTANT IA DE CHATGPT
        static string? QuickValidate(IReadOnlyList<Token> tok)
        {
            int n = tok.Count;
            if (n == 0 || tok[^1].Name != "$") return "fin d’entrée absente";
            int i = 0; bool start = true;
            while (i < n - 1)
            {
                var a = tok[i].Name; var b = tok[i + 1].Name;
                if (a == "(" && b == ")") return $"parenthèses vides près de «{Slice(tok, i, 2)}»";
                if (a == "," && (b == ")" || b == ";")) return $"virgule finale près de «{Slice(tok, i - 2, 4)}»";
                if (a == "," && b == ",") return $"double virgule près de «{Slice(tok, i - 1, 3)}»";
                if (start)
                {
                    if (a == "let")
                    {
                        if (i + 3 >= n) return "instruction let incomplète";
                        if (tok[i + 1].Name != "id" || tok[i + 2].Name != "=") return $"forme let invalide près de «{Slice(tok, i, 4)}»";
                        i += 3; start = false; continue;
                    }
                    if (a == "print") { start = false; i++; continue; }
                    if (a == "fn")
                    {
                        if (i + 4 >= n) return "définition fn incomplète";
                        if (tok[i + 1].Name != "id" || tok[i + 2].Name != "(") return $"tête de fn invalide près de «{Slice(tok, i, 4)}»";
                        int j = i + 3; bool expectId = false;
                        if (tok[j].Name == ")") j++;
                        else
                        {
                            while (j < n)
                            {
                                if (expectId)
                                {
                                    if (tok[j].Name != "id") return $"paramètre attendu près de «{Slice(tok, j - 1, 3)}»";
                                    j++;                 // consommer l'identifiant paramètre
                                    expectId = false;
                                    continue;
                                }
                                if (tok[j].Name == "id") { j++; expectId = false; continue; }   // consommer un id
                                if (tok[j].Name == ",") { expectId = true; j++; continue; }     // après une virgule on attend un id
                                if (tok[j].Name == ")") { j++; break; }
                                return $"liste de paramètres invalide près de «{Slice(tok, j - 1, 3)}»";
                            }
                        }
                        if (j >= n || tok[j].Name != "=") return $"symbole = attendu après paramètres près de «{Slice(tok, j - 2, 4)}»";
                        i = j; start = false; continue;
                    }
                    if (a == "id")
                    {
                        if (b != "=") return $"instruction invalide: attendu «=» après identifiant, près de «{Slice(tok, i, 3)}»";
                        i += 2; start = false; continue;
                    }
                    if (a == ";") { i++; start = true; continue; }
                    return $"début d’instruction invalide près de «{Slice(tok, i, 3)}»";
                }
                if (a == ";") { start = true; i++; continue; }
                i++;
            }
            if (!start) return "point-virgule manquant à la fin d’une instruction";
            return null;
        }

        static string Slice(IReadOnlyList<Token> t, int i, int k)
        {
            int a = Math.Max(0, i), b = Math.Min(t.Count - 1, i + k);
            return string.Join(" ", t.Skip(a).Take(Math.Max(0, b - a)).Select(x => x.Value ?? x.Name));
        }
    }
}
