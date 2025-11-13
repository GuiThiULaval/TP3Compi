using LexicalAnalysis.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LexicalAnalysis.Automata
{
    internal static class PartitioningAlgorithm
    {
        public static DFA Minimize(DFA automaton)
        {
            // Build partition.
            HashSet<HashSet<State>> partition = GetPartition(automaton);

            // Build states.
            Dictionary<State, string> acceptingStates = [];
            Dictionary<State, State> stateMap = [];
            foreach (HashSet<State> group in partition)
            {
                State minimalState = new(group);
                foreach (State state in group)
                {
                    stateMap.Add(state, minimalState);
                }
                if (automaton.AcceptingStates.TryGetValue(group.First(), out string? tokenName))
                {
                    acceptingStates.Add(minimalState, tokenName);
                }
            }

            // Add transitions.
            foreach (KeyValuePair<State, State> pair in stateMap)
            {
                foreach (char symbol in pair.Key.Transitions.Keys)
                {
                    HashSet<State> nextStates = pair.Key.Transitions[symbol];
                    State representative = nextStates.First();
                    pair.Value.AddTransition(symbol, stateMap[representative]);
                }
            }

            return new(stateMap[automaton.InitialState], acceptingStates);
        }

        private static HashSet<HashSet<State>> GetPartition(DFA automaton)
        {
            // Split states for initial partition.
            // Accepting states producing different tokens should be part of different groups.
            HashSet<State> states = automaton.GetAllStates();
            HashSet<State> unacceptingStates = states.Except(automaton.AcceptingStates.Keys).ToHashSet();
            HashSet<HashSet<State>> partition = [unacceptingStates];
            Dictionary<string, HashSet<State>> acceptingGroups = [];
            foreach (KeyValuePair<State, string> pair in automaton.AcceptingStates)
            {
                if (acceptingGroups.TryGetValue(pair.Value, out HashSet<State>? acceptingGroup))
                {
                    acceptingGroup.Add(pair.Key);
                }
                else
                {
                    acceptingGroups.Add(pair.Value, [pair.Key]);
                }
            }
            partition.UnionWith(acceptingGroups.Values);

            // Partition refinement.
            bool partitionOccured = true;
            while (partitionOccured)
            {
                partitionOccured = false;
                HashSet<HashSet<State>> newPartition = [];
                foreach (HashSet<State> group in partition)
                {
                    HashSet<HashSet<State>> subGroups = SplitIntoSubGroups(group, partition);
                    if (subGroups.Count > 1)
                    {
                        partitionOccured = true;
                    }
                    newPartition.UnionWith(subGroups);
                }
                partition = newPartition;
            }

            return partition;
        }

        private static HashSet<HashSet<State>> SplitIntoSubGroups(HashSet<State> group, HashSet<HashSet<State>> partition)
        {
            HashSet<HashSet<State>> subGroups = [];
            foreach (State state in group)
            {
                bool foundGroup = false;
                foreach (HashSet<State> subGroup in subGroups)
                {
                    bool distinguishableFromGroup = false;
                    foreach (State subGroupState in subGroup)
                    {
                        if (AreDistinguishable(state, subGroupState, partition))
                        {
                            distinguishableFromGroup = true;
                            break;
                        }
                    }
                    if (!distinguishableFromGroup)
                    {
                        foundGroup = true;
                        subGroup.Add(state);
                        break;
                    }
                }
                if (!foundGroup)
                {
                    subGroups.Add([state]);
                }
            }
            return subGroups;
        }

        private static bool AreDistinguishable(State firstState, State secondState, HashSet<HashSet<State>> partition)
        {
            if (!firstState.Transitions.Keys.ToHashSet().SetEquals(secondState.Transitions.Keys))
            {
                // Not same transitions.
                return true;
            }

            foreach (char symbol in firstState.Transitions.Keys)
            {
                bool firstHasNext = firstState.TryGetNextState(symbol, out State? firstNextState);
                bool secondHasNext = secondState.TryGetNextState(symbol, out State? secondNextState);

                if (firstHasNext && secondHasNext)
                {
                    bool found = false;
                    foreach (HashSet<State> group in partition)
                    {
                        if (group.Contains(firstNextState!) && group.Contains(secondNextState!))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        // Not in same group.
                        return true;
                    }
                }
                else if ((firstHasNext && !secondHasNext) || (!firstHasNext && secondHasNext))
                {
                    // Only one dead state.
                    return true;
                }
            }

            // No symbol can distinguish.
            return false;
        }
    }
}
