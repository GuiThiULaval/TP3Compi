using LexicalAnalysis.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalAnalysis.Automata
{
    public static class PowersetConstructionAlgorithm
    {
        public static DFA Convert(NFA automaton)
        {
            Dictionary<HashSet<State>, State> stateMap = [];

            // Add initial state.
            HashSet<State> initialClosure = automaton.GetEpsilonClosure([automaton.InitialState]);
            State initialState = new(initialClosure);
            stateMap.Add(initialClosure, initialState);

            Dictionary<State, string> acceptingStates = [];
            AddAcceptingState(automaton, acceptingStates, initialState, initialClosure);

            // Add other states.
            Stack<HashSet<State>> unmarked = new([initialClosure]);
            while (unmarked.Count > 0)
            {
                HashSet<State> currentClosure = unmarked.Pop();
                HashSet<char> alphabetClosure = [];
                foreach (State state in currentClosure)
                {
                    alphabetClosure.UnionWith(state.Transitions.Keys);
                }
                alphabetClosure.Remove(Alphabet.Epsilon);

                foreach (char symbol in alphabetClosure)
                {
                    HashSet<State> nextClosure = automaton.GetEpsilonClosureAfterTransition(currentClosure, symbol);
                    if (nextClosure.Count > 0)
                    {
                        bool found = false;
                        foreach (HashSet<State> closure in stateMap.Keys)
                        {
                            if (closure.SetEquals(nextClosure))
                            {
                                stateMap[currentClosure].AddTransition(symbol, stateMap[closure]);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            State newState = new(nextClosure);
                            stateMap[currentClosure].AddTransition(symbol, newState);
                            stateMap.Add(nextClosure, newState);
                            unmarked.Push(nextClosure);
                            AddAcceptingState(automaton, acceptingStates, newState, nextClosure);
                        }
                    }
                }
            }

            return new(initialState, acceptingStates);
        }

        private static void AddAcceptingState(NFA automaton, Dictionary<State, string> acceptingStates, State currentState, HashSet<State> closure)
        {
            if (closure.Count == 1)
            {
                State state = closure.First();
                if (automaton.AcceptingStates.TryGetValue(state, out string? tokenName))
                {
                    acceptingStates.Add(currentState, tokenName);
                }
            }
        }
    }
}
