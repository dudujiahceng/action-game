using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    private FSM stateMachine;
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    public List<Action> allActions;
    private HashSet<Action> availableActions;
    private Queue<Action> currentActions;
    private Queue<Action> plan;
    private bool actionBreak = false;
    private HashSet<KeyValuePair<string, object>> currentState;
    private HashSet<KeyValuePair<string, object>> goal;

    private IGoap dataProvider;
    private Planer planer;

    public bool ActionBreak
    {
        get
        {
            return actionBreak;
        }
        set
        {
            actionBreak = value;
        }
    }
    private bool HashActionPlan
    {
        get
        {
            return currentActions.Count > 0;
        }
    }

    void Start()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<Action>();
        currentActions = new Queue<Action>();
        planer = new Planer();
        InitDataProvider();
        InitIdleState();
        InitMoveToState();
        InitPerformActionState();
        stateMachine.PushState(idleState);
        LoadActions();
    }

    void Update()
    {
        stateMachine.Update(this.gameObject);
    }

    private void InitIdleState()
    {
        idleState = (fsm, go) =>
        {
            currentState = dataProvider.GetState();
            goal = dataProvider.CreateGoalState();
            plan = planer.Plan(gameObject, availableActions, currentState, goal);
            if(plan != null)
            {
                currentActions = plan;
                dataProvider.PlanFound(goal, plan);
                fsm.PopState();
                fsm.PushState(performActionState);
            }
            else
            {
                dataProvider.PlanFailed(goal);
                fsm.PopState();
                fsm.PushState(idleState);
            }
        };
    }

    private void InitMoveToState()
    {
        moveToState = (fsm, go) =>
        {
            Action action = currentActions.Peek();
            if(action.RequiresInRange() && action.target == null)
            {
                fsm.PopState();
                fsm.PopState();
                fsm.PushState(idleState);
                return;
            }
            if (dataProvider.MoveAgent(action))
                fsm.PopState();
        };
    }

    private void InitPerformActionState()
    {
        performActionState = (fsm, go) => 
        {
            if (ActionBreak)
            {
                fsm.ClearState();
                fsm.PushState(idleState);
            }
            if (!HashActionPlan)
            {
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
                return;
            }

            Action action = currentActions.Peek();

            if (action.IsDone())
            {
                currentActions.Dequeue();
            }
            if(HashActionPlan)
            {
                action = currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.IsInRange : true;
                if(inRange)
                {
                    bool success = action.Perform(go);
                    if(!success)
                    {
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.PlanAborted(action);
                    }
                }
                else
                {
                    fsm.PushState(moveToState);
                }
            }
            else
            {
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
            }
        };
    }

    private void InitDataProvider()
    {
        foreach(Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if(typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    private void LoadActions()
    {
        Action[] actions = gameObject.GetComponents<Action>();
        foreach (var a in actions)
        {

            availableActions.Add(a);
            allActions.Add(a);
        }
    }

    public void ClearPlan()
    {
        currentActions.Clear();
    }
}
