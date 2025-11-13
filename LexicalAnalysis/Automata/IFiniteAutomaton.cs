using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public interface IFiniteAutomaton
    {
        State InitialState { get; }
        Dictionary<State, string> AcceptingStates { get; }

        HashSet<State> GetAllStates();
        bool TrySimulate(string input, [NotNullWhen(true)] out string? tokenName);
    }
}
