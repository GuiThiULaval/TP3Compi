using LexicalAnalysis.Automata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Rules.OperatorNodes
{
    public record Fragment(State Entry, State Exit);

    public abstract class OperatorNode(string pattern) : IOperatorNode
    {
        public string Pattern { get; init; } = pattern;

        public NFA Compile(Alphabet alphabet, string tokenName)
        {
            Fragment fragment = CompileFragment();

            // Add other transition.
            State otherExit = new(tokenName);
            foreach (char symbol in alphabet.Symbols)
            {
                if (!fragment.Exit.Transitions.ContainsKey(symbol))
                {
                    fragment.Exit.AddTransition(symbol, otherExit);
                }
            }
            fragment.Exit.AddTransition(Alphabet.EndOfFile, otherExit);

            // Seal.
            Dictionary<State, string> acceptingStates = new()
            {
                { otherExit, tokenName },
            };
            return new(fragment.Entry, acceptingStates);
        }

        public abstract Fragment CompileFragment();
    }
}
