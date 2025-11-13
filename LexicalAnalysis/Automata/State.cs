using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public class State
    {
        public string Name { get; init; }
        public Dictionary<char, HashSet<State>> Transitions { get; init; }

        public State(string name)
        {
            Name = name;
            Transitions = [];
        }

        public State(HashSet<State> states)
        {
            Name = string.Join(" - ", states.Select(state => state.Name));
            Transitions = [];
        }

        public void AddTransition(char symbol, State nextState)
        {
            if (Transitions.TryGetValue(symbol, out HashSet<State>? nextStates))
            {
                nextStates.Add(nextState);
            }
            else
            {
                Transitions.Add(symbol, [nextState]);
            }
        }

        public bool TryGetNextState(char symbol, [NotNullWhen(true)] out State? nextState)
        {
            if (TryGetNextStates(symbol, out HashSet<State>? nextStates))
            {
                if (nextStates.Count == 1)
                {
                    nextState = nextStates.First();
                    return true;
                }
            }

            nextState = null;
            return false;
        }

        public bool TryGetNextStates(char symbol, [NotNullWhen(true)] out HashSet<State>? nextStates)
        {
            return Transitions.TryGetValue(symbol, out nextStates);
        }
    }
}
