using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour {
    #region
    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;
    private bool inRange = false;
    public float cost = 0f;


    public GameObject target;
    protected Property actionAgentProperty;
    protected Monster actionAgent;
    protected Agent agentCompoent;

    //If action is a loop action
    protected bool isLoop = false;
    protected float loopTime;
    protected bool isDone = false;
    protected bool requiresInRange;
    protected bool actionStart = false;
    public bool couldNextAttack = false;
    protected bool actionFinish = false;
    public string actionName;
    public string nextActionName;

    //attack action base damage
    protected Transform weaponBladePoint;
    public float baseDamage = 0;
    public float increaseDamageCoff;
    #endregion

    /// <summary>
    /// Initialize action to perferm action module
    /// </summary>
    #region
    public void CouldNextAttack()
    {
        couldNextAttack = true;
    }
    public void ActionFinish()
    {
        actionFinish = true;
    }
    //Initialize a action
    public Action()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }
    public void doReset()
    {
        inRange = false;
        target = null;
        Reset();
    }
    public abstract void Reset();
    // 由代理检索动作最优目标，并返回目标是否存在，动作是否可以被执行 
    public abstract bool CheckProceduralPrecondition(GameObject agent);

    public bool IsDone()
    {
        return isDone;
    }
    //执行动作
    public bool Perform(GameObject agent)
    {
        DamageCondition();
        if (!isLoop)
        {
            if (!actionStart)
            {
                couldNextAttack = false;
                actionAgent.StopStandAndWalk();
                actionAgent.AnimationPlay(actionName, true);
                actionStart = true;
            }
            DoneAction();
        }
        else
        {
            if (!actionStart)
            {
                couldNextAttack = false;
                actionAgent.StopStandAndWalk();
                actionAgent.AnimationPlay(actionName, true);
                actionStart = true;
                StartCoroutine(timeCount());
            }
            DoneAction();
        }
        return true;
    }
    //Play next action or finish all actions;
    protected void DoneAction()
    {
        if (couldNextAttack)
        {
            if (nextActionName != "" || target == null)
            {
                actionAgent.AnimationPlay(actionName, false);
                actionAgent.AnimationPlay(nextActionName, true);
                isDone = true;
            }
        }
        if (actionFinish)
        {
            actionAgent.AnimationPlay(actionName, false);
            actionAgent.StandPlay();
            actionAgent.AttackInterval = true;
            //agentCompoent.ClearPlan();
            isDone = true;
        }
    }
    IEnumerator timeCount()
    {
        yield return new WaitForSeconds(loopTime);
        couldNextAttack = true;
    }
    //动作执行是否需要靠近目标
    public bool RequiresInRange()
    {
        return requiresInRange;
    }
    public void addPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }
    public void removePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach(KeyValuePair<string, object> kvp in preconditions)
        {
            if(kvp.Key.Equals(key))
            {
                remove = kvp;
                break;
            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            preconditions.Remove(remove);
    }
    public void addEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }
    public void removeEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach(KeyValuePair<string, object> kvp in effects)
        {
            if(kvp.Key.Equals(key))
            {
                remove = kvp;
                break;
            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }

    public bool IsInRange
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
    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }
    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }

    public void ClearPreconditions()
    {
        preconditions.Clear();
    }

    public void ClearEffects()
    {
        effects.Clear();
    }
    #endregion

    ///<summary>
    ///Attack action make damage module
    ///</summary>
    #region

    protected void DamageCondition()
    {

    }
    protected void CalculateDamage()
    {
        target.GetComponent<Player>().GetDamage(baseDamage);
    }
    #endregion
}
