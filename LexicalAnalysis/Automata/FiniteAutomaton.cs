using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public abstract class FiniteAutomaton(State initialState, Dictionary<State, string> acceptingStates) : IFiniteAutomaton
    {
        public State InitialState { get; } = initialState;
        public Dictionary<State, string> AcceptingStates { get; } = acceptingStates;

        public HashSet<State> GetAllStates()
        {
            HashSet<State> visited = [];
            Stack<State> unvisited = new([InitialState]);

            do
            {
                State currentState = unvisited.Pop();
                if (visited.Add(currentState))
                {
                    foreach (HashSet<State> nextStates in currentState.Transitions.Values)
                    {
                        foreach (State nextState in nextStates)
                        {
                            if (!visited.Contains(nextState))
                            {
                                unvisited.Push(nextState);
                            }
                        }
                    }
                }
            }
            while (unvisited.Count > 0);

            return visited;
        }

        public abstract bool TrySimulate(string input, [NotNullWhen(true)] out string? tokenName);
    }
}
