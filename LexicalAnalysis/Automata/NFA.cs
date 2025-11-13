using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public class NFA(State initialState, Dictionary<State, string> acceptingStates)
        : FiniteAutomaton(initialState, acceptingStates)
    {
        public override bool TrySimulate(string input, [NotNullWhen(true)] out string? tokenName)
        {
            HashSet<State> currentStates = GetEpsilonClosure([InitialState]);

            foreach (char symbol in input)
            {
                currentStates = GetEpsilonClosureAfterTransition(currentStates, symbol);
            }

            if (currentStates.Count == 1)
            {
                State currentState = currentStates.First();
                if (AcceptingStates.TryGetValue(currentState, out tokenName))
                {
                    return true;
                }
            }

            tokenName = null;
            return false;
        }

        public HashSet<State> GetEpsilonClosure(HashSet<State> states)
        {
            HashSet<State> closure = new(states);
            Stack<State> unvisited = new(states);

            while (unvisited.Count > 0)
            {
                State currentState = unvisited.Pop();
                if (currentState.Transitions.TryGetValue(Alphabet.Epsilon, out HashSet<State>? value))
                {
                    foreach (State nextState in value)
                    {
                        if (!closure.Contains(nextState))
                        {
                            closure.Add(nextState);
                            unvisited.Push(nextState);
                        }
                    }
                }
            }

            return closure;
        }

        public HashSet<State> GetEpsilonClosureAfterTransition(HashSet<State> states, char symbol)
        {
            HashSet<State> closure = [];

            foreach (State state in states)
            {
                if (state.Transitions.TryGetValue(symbol, out HashSet<State>? nextStates))
                {
                    HashSet<State> stateClosure = GetEpsilonClosure(nextStates);
                    closure.UnionWith(stateClosure);
                }
            }

            return closure;
        }

        public static NFA Combine(IEnumerable<NFA> automata)
        {
            State initialState = new("init");
            Dictionary<State, string> acceptingStates = [];

            foreach (NFA automaton in automata)
            {
                initialState.AddTransition(Alphabet.Epsilon, automaton.InitialState);
                foreach (KeyValuePair<State, string> pair in automaton.AcceptingStates)
                {
                    if (!acceptingStates.TryAdd(pair.Key, pair.Value))
                    {
                        throw new Exception($"State {pair.Key.Name} already accept token {acceptingStates[pair.Key]}, cannot also accept token {pair.Value}.");
                    }
                }
            }

            return new(initialState, acceptingStates);
        }
    }
}
