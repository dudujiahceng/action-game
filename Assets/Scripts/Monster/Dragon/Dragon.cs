using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Monster {
    public override void monsterInit()
    {
        walkSpeed           = 8;
        runSpeed            = 8;
        HP                  = 1000;
        maxHP               = HP;
        strength            = 200;
        maxStrength         = strength;
        actionsList         = new List<ActionStruct>();
        monsterController   = GetComponent<CharacterController>();
        animator            = GetComponent<Animator>();
        navi                = GetComponent<NavMeshAgent>();
        actionManager       = GetComponent<MonsterActionManager>();
        fireballAttack      = GetComponent<RemoteAttack>();

        isFly               = false;
        isDescend           = false;
        if (weapon != null)
        {
            weaponMat = weapon.GetComponent<MeshRenderer>().material;
            weaponMat.SetShaderPassEnabled("Always", false);
        }
    }

    /// <summary>
    /// Create a action combo
    /// </summary>
    protected override void CreateActionsList()
    {
        actionsList.Add(new ActionStruct(false, true,   5, 20,  "ClawsAttack"));
        actionsList.Add(new ActionStruct(true,  false,  10,0,   "FlyStationaryAscend", 2));
        actionsList.Add(new ActionStruct(true,  false,  10,0,   "FlyStationaryDescend", 1));
        actionsList.Add(new ActionStruct(true,  false,  0, 0,  "SpitFireBall", 4));
        actionsList.Add(new ActionStruct(false, false,  0, 0,   "FlyStationaryToLanding"));
    }
    //是否正在执行当前动作序列
    protected override void InitActionCombo()
    {
        actionInterval = false; //
        actionImplement = false;//Any action serial is implement
        currentActionSerialsFinished = false;//Current action serial finished or not
        Queue<string> combo_1 = new Queue<string>();
        combo_1.Enqueue("ClawsAttack");
        actionCombo.Add(combo_1);

        Queue<string> combo_2 = new Queue<string>();
        combo_2.Enqueue("FlyStationaryAscend");
        combo_2.Enqueue("SpitFireBall");
        combo_2.Enqueue("FlyStationaryToLanding");
        actionCombo.Add(combo_2);
    }
    protected override void ImplementActions(int serialIndex)
    {
        actionManager.GetActions(actionCombo[serialIndex]);
        actionManager.GetActionSerial();
    }
    //Move in animation
    private Vector3 moveDirection = new Vector3();
    public void AnimationMove()
    {
        transform.LookAt(new Vector3(lockedTarget.transform.position.x, 0, lockedTarget.transform.position.z));
        if (isMove || unlimitMove)
        {
            moveDirection.x = transform.forward.x;
            moveDirection.z = transform.forward.z;
            if (animationMoveSpeed < 0)
            {
                monsterController.Move(moveDirection * Time.deltaTime * animationMoveSpeed);

            }
            else
            {
                if (Vector3.Magnitude(transform.position - lockedTarget.transform.position) > 5f)
                {
                    monsterController.Move(moveDirection * Time.deltaTime * animationMoveSpeed);
                }
            }
        }
    }
    public override void GetDamage(float damageNum)
    {
        HP -= damageNum;
        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
    }
    public override void MoveAgent(float range)
    {
        if(!isFly)
        {
            base.MoveAgent(range);
        }
        else
        {

        }
    }

    //Fly module
    private RaycastHit hit;
    private Vector3 flyDirection;
    //起飞
    private void FlyMove(float height)
    {
        Physics.Raycast(transform.position, -transform.up, out hit);
        if(hit.distance <= height)
        {
            transform.LookAt(lockedTarget.transform);
            if(Vector3.Magnitude(lockedTarget.transform.position - transform.position) < 15)
            {
                flyDirection = -transform.forward;
            }
            flyDirection.y = 4;
            transform.position += flyDirection * Time.deltaTime * 4;
        }
    }
    //垂直降落
    private void FlyDescend()
    {
        Physics.Raycast(transform.position, -transform.up, out hit);
        if (hit.distance >= 0.5)
        {
            transform.LookAt(lockedTarget.transform);
            transform.position -= transform.up * Time.deltaTime * 20;
        }
    }
    public override void AnimationPlay(string animationName, bool play)
    {
        switch(animationName)
        {
            case "ClawsAttack":
                animator.SetBool("ClawsAttack", play);
                break;
            case "FlyStationaryAscend":
                animator.SetBool("FlyStationaryAscend", play);
                break;
            case "SpitFireBall":
                animator.SetBool("SpitFireBall", play);
                break;
            case "FlyStationaryToLanding":
                animator.SetBool("FlyStationaryToLanding", play);
                break;
            case "":
                break;
        }
    }
    void Awake()
    {
        monsterInit();
        GetAllTarget();
        CreateActionsList();
        InitActionCombo();
        actionManager.LoadActionsTest(actionsList);
    }

    void Update()
    {
        if (lockedTarget == null)
        {
            lockedTarget = CurrentTarget();
        }
        if (!isDead)
        {
            if (!actionImplement && !lockedTarget.GetComponent<Player>().GetLiftStatus)
            {
                actionImplement = true;
                currentActionSerialsFinished = false;
                ImplementActions(UnityEngine.Random.Range(1, actionCombo.Count));
            }
            else
            {
                actionManager.ImplementAction();
            }
        }
    }
    void FixedUpdate()
    {
        if (isFly)
            FlyMove(15);
        if (isDescend)
            FlyDescend();
        AnimationMove();
    }
}
