using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterActionManager : MonoBehaviour {
    private Monster actionAgent;
    public Queue<string> actionsName = new Queue<string>();
    private Queue<ActionBase> actions = new Queue<ActionBase>();
    private List<ActionBase> availableActions = new List<ActionBase>();
    private bool isReset;
    private bool inRange;

    void Start()
    {
        //Get action implement agent
        actionAgent = GetComponent<Monster>();
        //Load all actions in agent
        isReset = false;
    }

    void Update()
    {
        
    }

    public bool InRange
    {
        get
        {
            return inRange;
        }
        set
        {
            inRange = value;
        }
    }

    //Clear current implement action serials
    public void ClearAction()
    {
        actionAgent.CurrentActionSerialsFinished = true;
        actionAgent.ActionImplement = false;
    }
    //Get current implement action serials from Monster gameObject
    public void GetActions(Queue<string> receivedAction)
    {
        actionsName = receivedAction;
    }
    //Load action serials to a queue
    public void GetActionSerial()
    {
        actions.Clear();
        foreach(var name in actionsName)
        {
            foreach(var a in availableActions)
            {
                if (a.ActionName == name)
                {
                    a.Reset();
                    actions.Enqueue(a);
                }
            }
        }
        inRange = false;
    }
    //Call this function in Monster Update function 
    public void ImplementAction()
    {
        if(!actionAgent.CurrentActionSerialsFinished)
        {
            if(actions.Count > 0)
            {
                ActionBase curAction = actions.Peek(); 
                if (!isReset)
                {
                    curAction.Reset();
                    isReset = true;
                }             
                if (!curAction.RequiresInRange)
                {
                    curAction.Perform(actions.Count > 1);
                }
                else
                {
                    if (inRange)
                    {
                        curAction.Perform(actions.Count > 1);
                    }
                    else
                    {
                        actionAgent.MoveAgent(curAction.GetRange);
                    }
                }
                if (curAction.IsDone())
                {
                    actions.Dequeue();
                    isReset = false;
                }
            }
            else
            {
                actionAgent.CurrentActionSerialsFinished = true;
                actionAgent.ActionImplement = false;
            }
        }
        else
        {
            actionAgent.StandPlay();
        }
    }

    public void CouldNextAttack(string returnActionName)
    {
        if (actions.Peek().actionName == returnActionName)
            actions.Peek().couldNextAction = true;
    }
    public void ActionFinish(string returnActionName)
    {
        if (actions.Peek().actionName == returnActionName)
            actions.Peek().actionFinish = true;
    }

    private float timer = 0;
    public void LoopTime(bool hasNextAction)
    {
        StartCoroutine(ActionLoop(hasNextAction));
    }
    IEnumerator ActionLoop(bool hasNextAction)
    {
        yield return new WaitForSeconds(2);
        if (hasNextAction)
            actions.Peek().couldNextAction = true;
        else
            actions.Peek().actionFinish = true;
    }

    public void LoadActionsTest(List<ActionStruct> actionsList)
    {
        foreach(var action in actionsList)
        {
            availableActions.Add(new ActionBase(GetComponent<Monster>(), this, action));
        }
    }
    //Calculate damage of current action
    public void MakeDamage()
    {
        actions.Peek().makeDamage = true;
    }
    public void CalculateDamage()
    {
        if (actions.Peek().makeDamage)
        {
            if (actionAgent.GetTarget() != null && (actionAgent.GetTarget().transform.position - actions.Peek().BladePoint.transform.position).magnitude < 5)
            {
                actionAgent.GetTarget().GetComponent<Player>().GetDamage(actions.Peek().baseDamage);
            }
            actions.Peek().makeDamage = false;
        }
    }

}
