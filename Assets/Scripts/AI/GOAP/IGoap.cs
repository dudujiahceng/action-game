using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoap{
    HashSet<KeyValuePair<string, object>> GetState();
    HashSet<KeyValuePair<string, object>> CreateGoalState();
    void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal);
    void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<Action> actions);
    void ActionsFinished();
    void PlanAborted(Action abortedAction);
    bool MoveAgent(Action targetAction);
}
