using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnikaTest : Monster {
    private bool LongDistance = true;
    
    public override void monsterInit()
    {
        walkSpeed       = 4;
        runSpeed        = 8;
        HP              = 1000;
        maxHP           = HP;
        strength        = 200;
        maxStrength     = strength;
        actionsList     = new List<ActionStruct>();
        animator        = GetComponent<Animator>();
        navi            = GetComponent<NavMeshAgent>();
        actionManager   = GetComponent<MonsterActionManager>();
        if(weapon != null)
        {
            weaponMat   = weapon.GetComponent<MeshRenderer>().material;
            weaponMat.SetShaderPassEnabled("Always", false);
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
    /// <summary>
    /// Create a action combo
    /// </summary>
    protected override void CreateActionsList()
    {
        actionsList.Add(new ActionStruct(false, false,  0, 0,   "AirBorneAction_1"));
        actionsList.Add(new ActionStruct(false, false,  0, 0,   "AirBorneAction_2"));
        actionsList.Add(new ActionStruct(false, true,   2, 20,  "AllKill_1"));
        actionsList.Add(new ActionStruct(false, false,  2, 20,  "AllKill_2"));
        actionsList.Add(new ActionStruct(false, false,  2, 40,  "AllKill_3"));
        actionsList.Add(new ActionStruct(false, false,  2, 40,  "AllKill_4"));
        actionsList.Add(new ActionStruct(false, true,   2, 0,   "AttackAction_1"));
        actionsList.Add(new ActionStruct(false, false,  2, 20,  "AttackAction_2"));
        actionsList.Add(new ActionStruct(false, false,  2, 0,   "AttackAction_3"));
        actionsList.Add(new ActionStruct(false, false,  2, 20,  "AttackAction_4"));
        actionsList.Add(new ActionStruct(false, false,  2, 0,   "BackJump"));
        actionsList.Add(new ActionStruct(false, false,  2, 80,   "HeavyAttack_1"));
        actionsList.Add(new ActionStruct(true,  true,   2, 0,   "PowerStorage"));
        actionsList.Add(new ActionStruct(false, true,   2, 20,  "RotateAttack_1"));
        actionsList.Add(new ActionStruct(false, false,  2, 30,  "RotateAttack_2"));
    }
    //是否正在执行当前动作序列
    protected override void InitActionCombo()
    {
        actionInterval = false; //
        actionImplement = false;//Any action serial is implement
        currentActionSerialsFinished = false;//Current action serial finished or not
        Queue<string> airBorne = new Queue<string>();//Airborne Action serial 0
        airBorne.Enqueue("AirBorneAction_1");
        airBorne.Enqueue("AirBorneAction_2");
        actionCombo.Add(airBorne);
        Queue<string> combo_1 = new Queue<string>();//Action serial 1
        combo_1.Enqueue("AttackAction_1");
        combo_1.Enqueue("AttackAction_2");
        combo_1.Enqueue("AttackAction_3");
        combo_1.Enqueue("AttackAction_4");
        actionCombo.Add(combo_1);
        Queue<string> combo_2 = new Queue<string>();//Action serial 2
        combo_2.Enqueue("PowerStorage");
        combo_2.Enqueue("HeavyAttack_1");
        actionCombo.Add(combo_2);
        Queue<string> combo_3 = new Queue<string>();//Action serial 3
        combo_3.Enqueue("RotateAttack_1");
        combo_3.Enqueue("BackJump");
        combo_3.Enqueue("RotateAttack_2");
        actionCombo.Add(combo_3);
        Queue<string> combo_4 = new Queue<string>();//Action serial 4
        combo_4.Enqueue("AttackAction_1");
        combo_4.Enqueue("AttackAction_2");
        combo_4.Enqueue("BackJump");
        combo_4.Enqueue("RotateAttack_2");
        actionCombo.Add(combo_4);
        Queue<string> combo_5 = new Queue<string>();//Action serial 5
        combo_5.Enqueue("AllKill_1");
        combo_5.Enqueue("AllKill_2");
        combo_5.Enqueue("AllKill_3");
        combo_5.Enqueue("AllKill_4");
        actionCombo.Add(combo_5);
        Queue<string> combo_6 = new Queue<string>();//Action serial 6
        combo_6.Enqueue("RotateAttack_1");
        combo_6.Enqueue("BackJump");
        combo_6.Enqueue("RotateAttack_2");
        combo_6.Enqueue("AllKill_3");
        combo_6.Enqueue("AllKill_4");
        actionCombo.Add(combo_6);
        Queue<string> combo_7 = new Queue<string>();//Action serial 7
        combo_7.Enqueue("AttackAction_1");
        combo_7.Enqueue("AttackAction_2");
        combo_7.Enqueue("PowerStorage");
        combo_7.Enqueue("HeavyAttack_1");
        actionCombo.Add(combo_7);
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
            if(animationMoveSpeed < 0)
            {
                transform.position += moveDirection * Time.deltaTime * animationMoveSpeed;
            }
            else
            {
                if (Vector3.Magnitude(transform.position - lockedTarget.transform.position) > 2.5f)
                {
                    transform.position += moveDirection * Time.deltaTime * animationMoveSpeed;
                }
            }
        }
    }
    //Move in animation

    //Animation play
    public override void AnimationPlay(string animationName, bool play)
    {
        switch (animationName)
        {
            case "AttackAction_1":
                animator.SetBool("AttackAction_1", play);
                break;
            case "AttackAction_2":
                animator.SetBool("AttackAction_2", play);
                break;
            case "AttackAction_3":
                animator.SetBool("AttackAction_3", play);
                break;
            case "AttackAction_4":
                animator.SetBool("AttackAction_4", play);
                break;
            case "PowerStorage":
                animator.SetBool("PowerStorage", play);
                break;
            case "HeavyAttack_1":
                animator.SetBool("HeavyAttack_1", play);
                break;
            case "RotateAttack_1":
                animator.SetBool("RotateAttack_1", play);
                break;
            case "BackJump":
                animator.SetBool("BackJump", play);
                break;
            case "RotateAttack_2":
                animator.SetBool("RotateAttack_2", play);
                break;
            case "AllKill_1":
                animator.SetBool("AllKill_1", play);
                break;
            case "AllKill_2":
                animator.SetBool("AllKill_2", play);
                break;
            case "AllKill_3":
                animator.SetBool("AllKill_3", play);
                break;
            case "AllKill_4":
                animator.SetBool("AllKill_4", play);
                break;
            case "AirBorneAction_1":
                animator.SetBool("AirBorneAction_1", play);
                break;
            case "AirBorneAction_2":
                animator.SetBool("AirBorneAction_2", play);
                break;
            case "Die":
                animator.SetBool("Die", play);
                break;
            case "":
                break;
        }
    }
    void Update()
    {
        if (lockedTarget == null)
        {
            lockedTarget = CurrentTarget();
        }
        if (!isDead && !airBorne)
        {
            if (!actionImplement && !lockedTarget.GetComponent<Player>().GetLiftStatus)
            {
                actionImplement = true;
                currentActionSerialsFinished = false;
                if (lockedTarget.GetComponent<Player>().GetBreakStutas)
                    ImplementActions(5);
                else           
                    ImplementActions(Random.Range(1, actionCombo.Count));
            }
            else
            {
                actionManager.ImplementAction();
            }
        }
        else if(airBorne)
        {
            if(!airBorning)
            {
                airBorning = true;
                actionImplement = true;
                currentActionSerialsFinished = false;
                ImplementActions(0);//击飞动画
            }
            else
            {
                actionManager.ImplementAction();
            }
        }
    }
    void FixedUpdate()
    {
        AnimationMove();
    }
}
