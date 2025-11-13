using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public class DFA(State initialState, Dictionary<State, string> acceptingStates)
        : FiniteAutomaton(initialState, acceptingStates)
    {
        public static DFA FromNFA(NFA automaton)
        {
            return PowersetConstructionAlgorithm.Convert(automaton);
        }

        public override bool TrySimulate(string input, [NotNullWhen(true)] out string? tokenName)
        {
            State? currentState = InitialState;

            foreach (char symbol in input)
            {
                if (!currentState.TryGetNextState(symbol, out currentState))
                {
                    tokenName = null;
                    return false;
                }
            }

            return AcceptingStates.TryGetValue(currentState, out tokenName);
        }

        public DFA Minimize()
        {
            return PartitioningAlgorithm.Minimize(this);
        }
    }
}
