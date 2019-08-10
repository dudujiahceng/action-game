using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct ActionStruct
{
    public bool isLoop;
    public bool requiresInRange;
    public float range;
    public float baseDamage;
    public string actionName;
    public ActionStruct(bool _isLoop, bool _requiresInRange, float _range, float _baseDamage, string _actionName)
    {
        isLoop          = _isLoop;
        requiresInRange = _requiresInRange;
        range           = _range;
        baseDamage      = _baseDamage;
        actionName      = _actionName;
    }
}
public class ActionBase : MonoBehaviour {
    #region
    private Monster actionAgent;
    private MonsterActionManager actionManager;
    private ActionBase curAction;
    private bool isLoop;
    private bool isDone = false;
    private bool inRange = false;
    private bool requiresInRange;
    private bool actionStart = false;

    public float loopTime;
    public bool couldNextAction = false;
    public bool actionFinish = false;
    public string actionName;
    public float range;
    public bool makeDamage;
    public float baseDamage;
    public Transform BladePoint;
    #endregion
    public ActionBase(Monster _agent, MonsterActionManager _manager,
        ActionStruct actionStruct)
    {
        actionAgent     = _agent;
        actionManager   = _manager;
        BladePoint      = actionAgent.GetBladePoint;
        isLoop          = actionStruct.isLoop;
        requiresInRange = actionStruct.requiresInRange;
        range           = actionStruct.range;
        baseDamage      = actionStruct.baseDamage;
        actionName      = actionStruct.actionName;
    }

    public string ActionName
    {
        get
        {
            return actionName;
        }
    }
    public float GetRange
    {
        get
        {
            return range;
        }
    }

    /// <summary>
    /// Initialize action to perferm action module
    /// </summary>
    #region

    public bool IsDone()
    {
        return isDone;
    }
    //重置动作
    public void Reset()
    {
        actionAgent.AnimationPlay(actionName, false);
        actionStart = false;
        couldNextAction = false;
        actionFinish = false;
        makeDamage = false;
        isDone = false;
    }
    //执行动作
    public void Perform(bool hasNextAction)
    {
        actionManager.CalculateDamage();
        if (!actionStart)
        {
            couldNextAction = false;
            actionAgent.StopStandAndWalk();
            actionAgent.AnimationPlay(actionName, true);
            actionStart = true;
            if(isLoop)
            {
                actionAgent.GetComponent<Monster>().Emission();//启用蓄力状态武器发光
                actionAgent.GetComponent<Monster>().UnStoppable();//启用蓄力不可打断
                actionManager.LoopTime(hasNextAction);
            }
        }
        if (couldNextAction)
        {
            if (hasNextAction)
            {
                actionAgent.AnimationPlay(actionName, false);
                isDone = true;
            }
        }
        if (actionFinish)
        {
            actionAgent.AnimationPlay(actionName, false);
            actionAgent.StandPlay();
            isDone = true;
        }
    }
    //动作执行是否需要靠近目标
    public bool RequiresInRange
    {
        get
        {
            return requiresInRange;
        }
    }
    #endregion

    ///<summary>
    ///Attack action make damage module
    ///</summary>
    #region
    #endregion
}

