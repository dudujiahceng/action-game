using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player {

    #region state manage function
    void Init()
    {
        walkSpeed               = 8;
        GameObject mainCamera   = GameObject.FindGameObjectWithTag("MainCamera");
        camera                  = mainCamera.GetComponent<Camera>();
        playerAnimator          = GetComponent<Animator>();
        heroController          = GetComponent<CharacterController>();
        //animatorPlay            = GetComponent<HeroAnimationControl>();
        attackModeTree          = DesignAttackTree();
        InitMode();//Initialize player's state machine
        GetTarget();//Get target
        DesignAttackTree();
        InitializeAttackDic();
        InitializeVariable();
        ChangeMode(GAME_MODE_TYPE.IdleMode);
    }

    public void InitMode()
    {
        gameModeDict.Clear();
        gameModeDict = GameModeConfig.CreateGameModeDict(this);
    }

    public bool IsGameMode(GAME_MODE_TYPE type)
    {
        if (curGameMode != null && curGameMode.modeType == type)
            return true;
        else
            return false;
    }

    #endregion

    protected void MoveControll()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            ChangeMode(GAME_MODE_TYPE.WalkingMode);
        }
        else
        {
            ChangeMode(GAME_MODE_TYPE.IdleMode);
        }
    }

    protected override AttackTree DesignAttackTree()
    {
        AttackTree root     = new AttackTree();
        AttackTree layerL   = NewAttackTreeLeaf(ref root, "Stun_01", true, false);
        AttackTree layerLL  = NewAttackTreeLeaf(ref layerL, "Stun_02", true, false);
        AttackTree layerLLL = NewAttackTreeLeaf(ref layerLL, "Stun_03", true, false);
        AttackTree layerLR  = NewAttackTreeLeaf(ref layerL, "Stun_04", false, false);
        AttackTree layerLLR = NewAttackTreeLeaf(ref layerLL, "Weapon_Attack_01", false, false);
        AttackTree layerLRL = NewAttackTreeLeaf(ref layerLR, "AirBorne_Attack_1", true, false);
        AttackTree layerR   = NewAttackTreeLeaf(ref root, "Combo2_1", false, false);
        AttackTree layerRL  = NewAttackTreeLeaf(ref layerR, "Combo2_2", true, false);
        AttackTree layerRLL = NewAttackTreeLeaf(ref layerRL, "RotateAttack", true, true);
        return root;
    }
    private bool actionLoop;
    public string returnAttackMode;
    //private bool attackFinishReady = false;
    private Dictionary<int, AttackAction> attackActionsList = new Dictionary<int, AttackAction>();
    //Effects
    public GameObject firstAttackEffect;
    object[] _params = new object[2];
    
    private void Attack()
    {
        //Could play next attack action or action already finished
        if (couldNextAttack)
        {
            attackCommandQueue.Dequeue();
            couldNextAttack = false;
        }
        
        if (attackCommandQueue.Count == 0)
        {
            if (attackFinishReady)
            {
                if (currentNode == null || returnAttackMode == currentNode.AttackModeStr)
                {
                    ChangeMode(GAME_MODE_TYPE.IdleMode);
                    AttackFinish();
                }
            }
        }
        else
        {
            _params[0] = attackCommandQueue.Peek().AttackModeStr;
            _params[1] = target;
            inAttack = true;
            ChangeMode(GAME_MODE_TYPE.AttackMode, _params);
        }

        //If current node is null, couldn't input attack command 
        if (currentNode != null)
        {
            if (Input.GetKeyDown(KeyCode.U))
                GetAttackKey(true);
            else if (Input.GetKeyDown(KeyCode.O))
                GetAttackKey(false);
        }
    }
    void GetAttackKey(bool XorY)
    {
        currentNode = XorY ? currentNode.LeftChild : currentNode.RightChild;
        if (currentNode == null)
            return;
        else if (attackCommandQueue.Count == 0 || (attackCommandQueue.Count == 1 && !currentNode.isLoop))
        {
            attackCommandQueue.Enqueue(currentNode);
        }
    }
    
    //When attack animation complete finished, trigger this function
    //private string returnAttackMode;
    void AttackFinishReady(string attackMode)
    {
        attackFinishReady = true;
        returnAttackMode = attackMode;
    }
    public void AttackFinish()
    {
        if (attackCommandQueue.Count != 0)
            attackCommandQueue.Clear();       //弹出最后一个动作
        HideWeapon();                           //隐藏武器
        inAttack            = false;            //结束攻击状态
        currentNode         = attackModeTree;   //连击树回到根节点
        attackFinishReady   = false;
        couldNextAttack     = false;
        ChangeMode(GAME_MODE_TYPE.IdleMode);
        //playerAnimator.SetBool("Stand", true);
    }


    /// <summary>
    /// Attack Damage Calculate
    /// </summary>
    //打人
    public void AnyAttack(int attackIndex)
    {
        if (target != null && (target.transform.position - attackActionsList[attackIndex].attackPart.transform.position).magnitude < 5)
        {
            target.GetComponent<Monster>().GetDamage(attackActionsList[attackIndex].damage);
            EnemyHPSlider.value = target.GetComponent<Monster>().HPSliderValue;
            EnemyStrengthSlider.value = target.GetComponent<Monster>().StrengthSliderValue;
        }
        //Create attack effect
        if (attackActionsList[attackIndex].effect == null)
            return;
        GameObject effect = GameObject.Instantiate(attackActionsList[attackIndex].effect, attackActionsList[attackIndex].attackPart.transform.position, Quaternion.identity);
        if (attackIndex == 7)
        {
            Vector3 bladePointPos = new Vector3(weaponBladePoint.transform.position.x, 0, weaponBladePoint.transform.position.z);
            camera.GetComponent<WaveEffect>().startShockWave(bladePointPos);
        }
        Destroy(effect, 0.5f);
    }
    private void InitializeAttackDic()
    {
        //                                       Damage   part          effect
        attackActionsList.Add(1, new AttackAction(15, rightHand, firstAttackEffect));       //Stun_01_right_fist
        attackActionsList.Add(2, new AttackAction(15, leftHand, firstAttackEffect));        //Stun_01_left_fist
        attackActionsList.Add(3, new AttackAction(25, leg, firstAttackEffect));             //Stun_02
        attackActionsList.Add(4, new AttackAction(20, leg, firstAttackEffect));             //Stun_03
        attackActionsList.Add(5, new AttackAction(20, rightHand, firstAttackEffect));       //Stun_04
        attackActionsList.Add(6, new AttackAction(30, weaponBladePoint, firstAttackEffect));//AirBorne_Attack_1
        attackActionsList.Add(7, new AttackAction(40, weaponBladePoint, firstAttackEffect));//Weapon_Attack_1
        attackActionsList.Add(8, new AttackAction(20, weaponBladePoint, firstAttackEffect));//Combo2_1
        attackActionsList.Add(9, new AttackAction(40, weaponBladePoint, firstAttackEffect));//Combo2_2
        attackActionsList.Add(10,new AttackAction(15, weaponBladePoint, null));             //RotateAttack
    }

    public void TakeWeapon()
    {
        weapon.SetActive(true);
    }
    public void HideWeapon()
    {
        weapon.SetActive(false);
    }

    public bool animationMove = false;
    private float moveSpeed;
    //Move in animation
    public void StartMove(float speed)
    {
        animationMove = true;
        moveSpeed = speed;
    }
    //Stop move in animation
    public void StopMove()
    {
        animationMove = false;
    }            
    //Move control in animation
    private void AnimationMove()
    {
        if (animationMove)
        {
            if (target == null || moveSpeed < 0)
            {
                //transform.position += transform.forward * Time.deltaTime * moveSpeed;
                heroController.Move(transform.forward * Time.deltaTime * moveSpeed);//可以防止穿模
            }
            else
            {
                if (Vector3.Magnitude(transform.position - target.transform.position) <= 1.5f)
                    transform.position += transform.forward * Time.deltaTime * 0;
                else
                    heroController.Move(transform.forward * Time.deltaTime * moveSpeed);
            }
        }
    }
    //可以把人打飞 
    public void AirBorneAttack()
    {
        if (target != null)
            target.GetComponent<Monster>().AirBorne();
    }

    private void InitializeVariable()
    {
        //Base Properties
        HP                  = 2000;
        maxHP               = HP;
        strengthValue       = 1000;
        maxStrengthValue    = strengthValue;
        weapon.SetActive(false);
        //Move
        couldMove           = true;
        isMove              = false;
        //attackMove = false;
        //Attack
        isAirBorne          = false;
        couldNextAttack     = false;
        //Dodge
        isDodge             = false;
        unBeatable          = false;
        couldDodge          = true;
        //Attack Tree
        currentNode         = attackModeTree;
        attackCommandQueue  = new Queue<AttackTree>();
    }

    /// <summary>
    /// Dodge action
    /// </summary>
    private bool couldDodge;
    private bool isDodge;
    private void Dodge()
    {
        if (couldDodge && Input.GetKeyDown(KeyCode.Space))
        {
            isDodge = true;
            couldDodge = false;
            couldMove = false;
            ChangeMode(GAME_MODE_TYPE.DodgeMode);
        }
    }
    public void DodgeStart()
    {
        unBeatable = true;
    }
    public void DodgeFinish()
    {
        couldMove = true;
        isDodge = false;
        unBeatable = false;
        ChangeMode(GAME_MODE_TYPE.IdleMode);
        StartCoroutine(DodgeCoolDown());
    }
    private IEnumerator DodgeCoolDown()
    {
        yield return new WaitForSeconds(2.5f);
        couldDodge = true;
    }


    void Start () {
        Init();
	}

	void Update () {
        if(!isDead)//最高优先级
        {
            if (target == null)
            {
                target = GetTarget();
            }
            if (!isBreak)//第二优先级
            {
                if (!isDodge)
                {
                    Attack();
                    if (!inAttack)
                        MoveControll();
                }
                if (!inAttack)
                {
                    Dodge();
                }
            }
        }
    }

    void FixedUpdate()
    {
        AnimationMove();        //控制人物在动画中的移动
        curGameMode.Update();
    }
}
