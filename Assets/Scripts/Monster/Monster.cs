using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour {
    protected CharacterController monsterController;
    protected Animator animator;
    public NavMeshAgent navi;
    public MonsterActionManager actionManager;
    public Transform weapon;
    public List<Transform> BladePoint;
    protected Material weaponMat;
    protected List<Queue<string>> actionCombo = new List<Queue<string>>();

    //Unique Components
    protected RemoteAttack fireballAttack;

    protected bool attackInterval = false;
    public bool AttackInterval
    {
        get
        {
            return attackInterval;
        }
        set
        {
            attackInterval = value;
        }
    }

    public abstract void AnimationPlay(string animationName, bool play);

    #region
    /// <summary>
    /// Initialize module
    /// </summary>
    //Initialize base properties
    public abstract void monsterInit();
    //Initialize body part
    protected List<Transform> bodyParts;
    protected void GetBodyPart()
    {
        Transform[] allChilds = GetComponentsInChildren<Transform>();
        foreach (var child in allChilds)
        {
            for(int i = 0; i != bodyParts.Count; ++i)
            {
                if (bodyParts[i].name == child.name)
                    bodyParts[i] = child;
            }              
        }
    }
    /// <summary>
    /// Initialize finish
    /// </summary>
    #endregion

    protected float walkSpeed;
    protected float runSpeed;
    protected float animationMoveSpeed;
    protected bool onGround = true;
    public bool isFly;
    public bool isDescend;
    private GameObject[] attackTargets;
    protected List<GameObject> targetsList = new List<GameObject>();
    public GameObject lockedTarget;

    //All targets
    protected void GetAllTarget()
    {
        attackTargets = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in attackTargets)
        {
            targetsList.Add(obj);
        }
        lockedTarget = CurrentTarget();
    }

    //Get current locked target
    public GameObject CurrentTarget()
    {
        if (targetsList.Count == 0)
            return null;
        else
        {
            foreach (GameObject obj in targetsList)
            {
                if (!obj)
                    targetsList.Remove(obj);
            }
            int index = Random.Range(0, targetsList.Count);
            lockedTarget = targetsList[index];
            return targetsList[index];
        }
    }
    public GameObject GetTarget()
    {
        return lockedTarget;
    }
    

    //Battle module
    protected float rigidityThres;
    protected bool isGuardBreak         = false;
    protected bool inStunState          = false;
    public float HP;
    protected float maxHP;
    protected float strength;
    protected float maxStrength;
    protected bool isDead               = false;
    protected bool unStoppable          = false;
    public bool airBorne                = false;
    protected bool airBorning           = false;
    public virtual void GetDamage(float damageNum)
    {
        HP -= damageNum;
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
        strength -= damageNum;
        if(strength <= 0)
        {
            strength    = 0;
            inStunState = true;
            Debug.Log("Stun");
            Invoke("StrengthRecover", 5f);
        }
    }

    protected void StrengthRecover()
    {
        strength = maxStrength;
        lockedTarget.GetComponent<PlayerController>().EnemyStrengthSlider.value = 1;
        if(strength >= maxStrength)
        {
            inStunState = false;
            CancelInvoke("StrengthRecover");
        }        
    }
    public Transform GetBladePoint(int index)
    {
        return BladePoint[index];
    }

    //UI
    public float HPSliderValue
    {
        get
        {
            return HP / maxHP;
        }
    }
    public float StrengthSliderValue
    {
        get
        {
            return strength / maxStrength;
        }
    }


    protected void Die()
    {
        isDead = true;
        AnimationPlay("Die", true);
        //actionManager.ClearAction();
    }
    //Be controlled
    public void UnStoppable()
    {
        if (unStoppable)
            unStoppable = false;
        else
            unStoppable = true;
    }
    public void AirBorne()
    {
        if(!unStoppable && strength <= 0)
        {
            HP                             -= 200;//奖励伤害
            airBorne                        = true;
            actionImplement                 = false;
            currentActionSerialsFinished    = true;
        }
    }
    public void AirBorneRecover()
    {
        airBorne = false;
    }

    /// <summary>
    /// Move and play animations
    /// </summary>
    //Move in animation
    public bool isMove = false;
    public bool unlimitMove = false;
    public void StartMove(float speed)
    {
        animationMoveSpeed = speed;
        isMove = true;
    }//Move in animation
    public void StopMove()
    {
        isMove = false;
    }//Stop move in animation
    public bool GetMoveState
    {
        get
        {
            return isMove;
        }
    }
    public bool UnlimitMove
    {
        get
        {
            return unlimitMove;
        }
        set
        {
            unlimitMove = value;
        }
    }
    public void StandPlay()
    {
        animator.SetBool("Stand", true);
        animator.SetBool("Walk", false);
    }
    public void WalkPlay()
    {
        animator.SetBool("Stand", false);
        animator.SetBool("Walk", true);
    }
    public void StopStandAndWalk()
    {
        animator.SetBool("Stand", false);
        animator.SetBool("Walk", false);
    }
    //Get speed or set a new speed
    public float WalkSpeed
    {
        get
        {
            return walkSpeed;
        }
        set
        {
            walkSpeed = value;
        }
    }
    public float RunSpeed
    {
        get
        {
            return runSpeed;
        }
        set
        {
            runSpeed = value;
        }
    }

    //Effect
    //Weapon emission
    protected bool emission = false;
    public void Emission()
    {
        if (emission)
            emission = false;
        else
            emission = true;
        weaponMat.SetShaderPassEnabled("Always", emission);
    }

    ///Monster action
    //Actions list
    protected List<ActionStruct> actionsList;
    protected abstract void CreateActionsList();
    //Move To Action
    
    public virtual void MoveAgent(float range)
    {
        //GetComponent<NavMeshAgent>().enabled = true;
        //navi.destination = lockedTarget.transform.position;
        if (Vector3.Magnitude(gameObject.transform.position - lockedTarget.transform.position) < range)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            animator.SetBool("Walk", false);
            animator.SetBool("Stand", true);
            actionManager.InRange = true;
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = true;
            transform.LookAt(new Vector3(lockedTarget.transform.position.x, 0, lockedTarget.transform.position.z));
            navi.destination = lockedTarget.transform.position;
            navi.speed = walkSpeed;
            animator.SetBool("Walk", true);
            animator.SetBool("Stand", false);
            actionManager.InRange = false;
        }
    }

    public bool currentActionSerialsFinished;
    public bool actionInterval;
    public bool actionImplement;
    public bool CurrentActionSerialsFinished
    {
        get
        {
            return currentActionSerialsFinished;
        }
        set
        {
            currentActionSerialsFinished = value;
        }
    }
    public bool ActionInterval
    {
        get
        {
            return actionInterval;
        }
        set
        {
            actionInterval = value;
        }
    }
    public bool ActionImplement
    {
        get
        {
            return actionImplement;
        }
        set
        {
            actionImplement = value;
        }
    }
    protected abstract void InitActionCombo();
    protected abstract void ImplementActions(int serialIndex);

}
