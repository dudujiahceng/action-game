using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planer{
    private class Node
    {
        public Node Parent;
        public float CostNum;
        public HashSet<KeyValuePair<string, object>> State;
        public Action Action;
        public Node(Node parent, float costNum, HashSet<KeyValuePair<string, object>> state, Action action)
        {
            Parent = parent;
            CostNum = costNum;
            State = state;
            Action = action;
        }
    }
    public Queue<Action> Plan(GameObject agent, HashSet<Action> availableActions, HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> goal)
    {
        foreach (var a in availableActions)
            a.doReset();
        HashSet<Action> usableActions = new HashSet<Action>();
        foreach (var a in availableActions)
            if (a.CheckProceduralPrecondition(agent))
                usableActions.Add(a);
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, currentState, null);
        bool success = BuildGraph(start, leaves, usableActions, goal);
        if (!success)
            return null;
        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.CostNum < cheapest.CostNum)
                    cheapest = leaf;
            }
        }
        List<Action> result = new List<Action>();
        Node n = cheapest;
        while(n != null)
        {
            if (n.Action != null)
                result.Insert(0, n.Action);
            n = n.Parent;
        }
        Queue<Action> queue = new Queue<Action>();
        foreach (Action a in result)
            queue.Enqueue(a);
        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, HashSet<Action> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        bool foundOne = false;
        foreach(Action action in usableActions)
        {
            if(InState(action.Preconditions, parent.State))
            {
                HashSet<KeyValuePair<string, object>> currentState = PopulateState(parent.State, action.Effects);
                Node node = new Node(parent, parent.CostNum + action.cost, currentState, action);
                if(InState(goal, currentState))
                {
                    leaves.Add(node);
                    foundOne = true;
                }
                else
                {
                    HashSet<Action> subSet = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subSet, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }
        return foundOne;
    }

    private HashSet<Action> ActionSubset(HashSet<Action> actions, Action removeTarget)
    {
        HashSet<Action> subset = new HashSet<Action>();
        foreach(Action action in actions)
        {
            if (!action.Equals(removeTarget))
                subset.Add(action);
        }
        return subset;
    }

    private bool InState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        bool allMatch = true;
        foreach (KeyValuePair<string, object> s in test)
        {
            bool match = false;
            foreach(KeyValuePair<string, object> s1 in state)
            {
                if(s1.Equals(s))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                allMatch = false;
                break;
            }
        }
        return allMatch;
    }

    private HashSet<KeyValuePair<string, object>> PopulateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)

    {
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

        foreach (var s in currentState)
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));

        foreach (var change in stateChange)
        {
            bool exists = false;

            foreach (var s in state)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            }
            else
            {
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }
}
