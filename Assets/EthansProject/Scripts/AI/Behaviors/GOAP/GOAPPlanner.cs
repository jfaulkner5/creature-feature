using System.Collections.Generic;
using UnityEngine;

namespace EthansProject
{
    /// <summary>
    /// Runs through all the available actions and pieces them together to find one 
    /// that result in completing a goal and uses the cost of them to determine the best one.
    /// </summary>
    public class GOAPPlanner
    {

        public Queue<GOAPAction> Plan(GameObject agent, HashSet<GOAPAction> availibleActions, HashSet<KeyValuePair<string, object>> worldState, HashSet<KeyValuePair<string, object>> goal)
        {
            foreach (GOAPAction action in availibleActions)
            {
                action.DoReset();
            }

            HashSet<GOAPAction> usableActions = new HashSet<GOAPAction>();
            foreach (GOAPAction action in availibleActions)
            {
                if (action.CheckProcPreconditions(agent))
                {
                    usableActions.Add(action);
                }
            }

            List<BranchNode> nodes = new List<BranchNode>();

            BranchNode start = new BranchNode(null, 0, worldState, null);
            bool success = BuildGraph(start, nodes, usableActions, goal);

            if (!success)
            {
                Debug.LogWarning("No plan was found for " + agent.name);
                return null;
            }

            BranchNode cheapest = null;
            foreach (BranchNode node in nodes)
            {
                if (cheapest == null)
                {
                    cheapest = node;
                }
                else
                {
                    if (node.runningCost < cheapest.runningCost)
                        cheapest = node;
                }
            }

            List<GOAPAction> result = new List<GOAPAction>();
            BranchNode newNode = cheapest;

            while (newNode != null)
            {
                if (newNode.action != null)
                {
                    result.Insert(0, newNode.action);
                }
                newNode = newNode.parent;
            }

            Queue<GOAPAction> queue = new Queue<GOAPAction>();
            foreach (GOAPAction action in result)
            {
                queue.Enqueue(action);
            }
            return queue;

        }

        private bool BuildGraph(BranchNode parent, List<BranchNode> nodes, HashSet<GOAPAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
        {
            bool foundOne = false;

            foreach (GOAPAction action in usableActions)
            {
                if (InState(action.Preconditions, parent.state))
                {
                    HashSet<KeyValuePair<string, object>> currentState = PopulateState(parent.state, action.Effects);
                    BranchNode newNode = new BranchNode(parent, parent.runningCost + action.cost, currentState, action);

                    if (InState(goal, currentState))
                    {
                        nodes.Add(newNode);
                        foundOne = true;
                    }
                    else
                    {
                        HashSet<GOAPAction> subSet = ActionSubSet(usableActions, action);
                        bool found = BuildGraph(newNode, nodes, subSet, goal);
                        if (found)
                        {
                            foundOne = true;
                        }
                    }
                }
            }

            return foundOne;
        }

        private HashSet<GOAPAction> ActionSubSet(HashSet<GOAPAction> actions, GOAPAction toRemove)
        {
            HashSet<GOAPAction> subset = new HashSet<GOAPAction>();
            foreach (GOAPAction action in actions)
            {
                if (!action.Equals(toRemove))
                {
                    subset.Add(action);
                }

            }
            return subset;
        }

        private bool InState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
        {
            bool allMatch = true;
            foreach (KeyValuePair<string, object> t in test)
            {
                bool match = false;

                foreach (KeyValuePair<string, object> s in state)
                {
                    if (s.Equals(t))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                    allMatch = false;

            }
            return allMatch;
        }

        private HashSet<KeyValuePair<string, object>> PopulateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
        {
            HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

            foreach (KeyValuePair<string, object> change in stateChange)
            {
                bool exist = false;

                foreach (KeyValuePair<string, object> s in state)
                {
                    if (s.Equals(change))
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    state.RemoveWhere((KeyValuePair<string, object> pair) => { return pair.Key.Equals(change.Key); });
                    KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                    state.Add(change);

                }
                else
                {
                    state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
                }
            }
            return state;
        }

        private class BranchNode
        {
            public BranchNode parent;
            public float runningCost;
            public HashSet<KeyValuePair<string, object>> state;
            public GOAPAction action;

            public BranchNode(BranchNode _parent, float _runningCost, HashSet<KeyValuePair<string, object>> _state, GOAPAction _action)
            {
                parent = _parent;
                runningCost = _runningCost;
                state = _state;
                action = _action;
            }
        }
    }
}