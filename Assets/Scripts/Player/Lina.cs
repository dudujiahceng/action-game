using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lina : Player
{
    private float walkSpeed = 8f;
    private float gravity = 3f;
    private Vector3 moveDirection;
    private Vector3 currentDirection;
    //public string currentMode;
    //public Transform Center;
    /// <summary>
    /// Move module
    /// </summary>
    void WalkAnimation()
    {
        GetComponent<HeroAnimationControl>().StopAllMotion();
        GetComponent<HeroAnimationControl>().MotionWalk(true);
    }
    void StandAnimation()
    {
        GetComponent<HeroAnimationControl>().StopAllMotion();
        GetComponent<HeroAnimationControl>().MotionStand(true);
    }

    private bool isMove;
    void Move()
    {
        moveDirection.y -= gravity * Time.deltaTime;
        heroController.Move(moveDirection);
        if (heroController.isGrounded)
        {
            if(Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.D))
            {
                isMove = true;
                WalkAnimation();
                moveDirection = transform.forward * walkSpeed * Time.deltaTime;
            }
            else
            {
                isMove = false;
                StandAnimation();
                moveDirection = Vector3.zero;
            }
        }
    }

    private bool attackMove;
    void AttackMove()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.z = Input.GetAxis("Vertical");
        transform.Translate(moveDirection * (float)walkSpeed * 0.4f * Time.deltaTime, Space.World);
    }
    /// <summary>
    /// Move module end
    /// </summary>


    /// <summary>
    /// Attack module
    /// </summary>
    /// 
    protected override AttackTree DesignAttackTree()
    {
        AttackTree root = new AttackTree();
        AttackTree layerL = NewAttackTreeLeaf(ref root, AnimationID.Stun_01, "Stun_01", true, false);
        AttackTree layerLL = NewAttackTreeLeaf(ref layerL, AnimationID.Stun_02, "Stun_02", true, false);
        AttackTree layerLLL = NewAttackTreeLeaf(ref layerLL, AnimationID.Stun_03, "Stun_03", true, false);
        AttackTree layerLR = NewAttackTreeLeaf(ref layerL, AnimationID.Stun_04, "Stun_04", false, false);
        AttackTree layerLLR = NewAttackTreeLeaf(ref layerLL, AnimationID.Weapon_Attack_01, "Weapon_Attack_01", false, false);
        AttackTree layerLRL = NewAttackTreeLeaf(ref layerLR, AnimationID.AirBorne_Attack_1, "AirBorne_Attack_1", true, false);
        AttackTree layerR = NewAttackTreeLeaf(ref root, AnimationID.Combo2_1, "Combo2_1", false, false);
        AttackTree layerRL = NewAttackTreeLeaf(ref layerR, AnimationID.Combo2_2, "Combo2_2", true, false);
        AttackTree layerRLL = NewAttackTreeLeaf(ref layerRL, AnimationID.RotateAttack, "RotateAttack", true, true);
        return root;
    }


    /// <summary>
    /// Attack module main function
    /// </summary>

    private bool actionLoop;
    void Attack()
    {
        //Could play next attack action or action already finished
        if(couldNextAttack)
        {
            AttackAnimaPlay(attackCommandQueue.Peek().AttackModeStr, false);
            attackCommandQueue.Dequeue();
            couldNextAttack = false;
        }
        if (attackCommandQueue.Count == 0)
        {
            if (attackFinishReady)
            {
                if(currentNode == null || returnAttackMode == currentNode.AttackModeStr)
                {
                    AttackFinish();
                    attackFinishReady = false;
                }
            }
        }
        else
        {
            if(!attackCommandQueue.Peek().isLoop)
            {
                AttackAnimaPlay(attackCommandQueue.Peek().AttackModeStr, true);
                inAttack = true;

            }
            else
            {
                if(Input.GetKey(attackCommandQueue.Peek().key?KeyCode.U:KeyCode.O) && !actionLoop)
                {
                    actionLoop = true;
                    attackMove = true;
                    AttackAnimaPlay(attackCommandQueue.Peek().AttackModeStr, true);
                    inAttack = true;
                }
                if(Input.GetKeyUp(attackCommandQueue.Peek().key ? KeyCode.U : KeyCode.O))
                {
                    LoopFinish();
                    attackMove = false;
                    actionLoop = false;
                }
            }
        }
        //If current node is null, couldn't input attack command 
        if (currentNode != null && !actionLoop)
            if (Input.GetKeyDown(KeyCode.U))
                GetAttackKey(true);
            else if (Input.GetKeyDown(KeyCode.O))
                GetAttackKey(false);
    }
    private void LoopFinish()
    {
        AttackAnimaPlay(attackCommandQueue.Peek().AttackModeStr, false);
        attackCommandQueue.Dequeue();
        if (attackCommandQueue.Count == 0)
            AttackFinish();
    }

    //When 'U' or 'O', transform current state to inAttack state and decide which attack mode to play;
    //XorY is true equals input U, else equals input O;
    void GetAttackKey(bool XorY)
    {
        currentNode = XorY ? currentNode.LeftChild : currentNode.RightChild;
        if (currentNode == null)
            return;
        else if(attackCommandQueue.Count == 0 || (attackCommandQueue.Count == 1 && !attackCommandQueue.Peek().isLoop))
        {
            attackCommandQueue.Enqueue(currentNode);
        }
    }
    //When attack animation complete finished, trigger this function
    private bool attackFinishReady = false;
    private string returnAttackMode;
    void AttackFinishReady(string attackMode)
    {
        attackFinishReady = true;
        returnAttackMode = attackMode;
    }
    void AttackFinish()
    {
        GetComponent<HeroAnimationControl>().MotionStand(true);
        GetComponent<HeroAnimationControl>().StopAttackMotion();
        if (attackCommandQueue.Count != 0) 
            attackCommandQueue.Dequeue();  //弹出最后一个动作
        HideWeapon();                      //隐藏武器
        inAttack          = false;         //结束攻击状态
        currentNode       = attackModeTree;//连击树回到根节点
        attackFinishReady = false;
        couldNextAttack   = false;
    }

    //Take and hide weapon
    public void TakeWeapon()
    {
        weapon.SetActive(true);
    }
    public void HideWeapon()
    {
        weapon.SetActive(false);
    }

    /// <summary>
    /// When play animation which need change position
    /// use StartAnimationMove and StopAnimationMove to control
    /// transform position
    /// </summary>
    void AttackAnimaPlay(string attackCase, bool play)
    {
        if(target != null)
        {
            Vector3 targetPosition = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            transform.LookAt(targetPosition);
        }
        switch (attackCase)
        {
            case "Stun_01":
                animatorPlay.MotionStun_01(play);
                break;
            case "Stun_02":
                animatorPlay.MotionStun_02(play);
                break;
            case "Stun_03":
                animatorPlay.MotionStun_03(play);
                break;
            case "Stun_04":
                animatorPlay.MotionStun_04(play);
                break;
            case "Weapon_Attack_01":
                animatorPlay.MotionWeaponAttack_01(play);
                break;
            case "AirBorne_Attack_1":
                animatorPlay.MotionAirBorne_Attack_1(play);
                break;
            case "Combo2_1":
                animatorPlay.MotionCombo2_1(play);
                break;
            case "Combo2_2":
                animatorPlay.MotionCombo2_2(play);
                break;
            case "RotateAttack":
                animatorPlay.MotionRotateAttack(play);
                break;
            case "":
                break;
        }
    }
    public bool animationMove = false;
    private float moveSpeed;
    public void StartMove(float speed)
    {
        animationMove = true;
        moveSpeed = speed;
    }//Move in animation
    public void StopMove()
    {
        animationMove = false;
    }            //Stop move in animation
    private void AnimationMove()
    {
        if (animationMove)
        {
            if (target == null || moveSpeed < 0)
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            else
            {
                if (Vector3.Magnitude(transform.position - target.transform.position) <= 1.5f)
                    transform.position += transform.forward * Time.deltaTime * 0;
                else
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
        }
    }      //Move control in animation


    //Attack index
    //The damage, attackPart and attack effect of every action are included in attackActionsList;
    private Dictionary<int, AttackAction> attackActionsList = new Dictionary<int, AttackAction>();
    //Effects
    public GameObject firstAttackEffect;
    //Initialize every attack action's damage, which part to make damage and which effect to create
    private void InitializeAttackDic()
    {
        //                                       Damage   part          effect
        attackActionsList.Add(1,  new AttackAction(15, rightHand, firstAttackEffect));       //Stun_01_right_fist
        attackActionsList.Add(2,  new AttackAction(15, leftHand, firstAttackEffect));        //Stun_01_left_fist
        attackActionsList.Add(3,  new AttackAction(25, leg, firstAttackEffect));             //Stun_02
        attackActionsList.Add(4,  new AttackAction(20, leg, firstAttackEffect));             //Stun_03
        attackActionsList.Add(5,  new AttackAction(20, rightHand, firstAttackEffect));       //Stun_04
        attackActionsList.Add(6,  new AttackAction(30, weaponBladePoint, firstAttackEffect));//AirBorne_Attack_1
        attackActionsList.Add(7,  new AttackAction(40, weaponBladePoint, firstAttackEffect));//Weapon_Attack_1
        attackActionsList.Add(8,  new AttackAction(20, weaponBladePoint, firstAttackEffect));//Combo2_1
        attackActionsList.Add(9,  new AttackAction(40, weaponBladePoint, firstAttackEffect));//Combo2_2
        attackActionsList.Add(10, new AttackAction(15, weaponBladePoint, null));             //RotateAttack
    }

    /// <summary>
    /// Attack Damage Calculate
    /// </summary>
    //打人
    public void AnyAttack(int attackIndex)
    {
        if(target != null && (target.transform.position - attackActionsList[attackIndex].attackPart.transform.position).magnitude < 5)
        {
            target.GetComponent<Monster>().GetDamage(attackActionsList[attackIndex].damage);
            EnemyHPSlider.value = target.GetComponent<Monster>().HPSliderValue;
            EnemyStrengthSlider.value = target.GetComponent<Monster>().StrengthSliderValue;
        }
        //Create attack effect
        if (attackActionsList[attackIndex].effect == null)
            return;
        GameObject effect = GameObject.Instantiate(attackActionsList[attackIndex].effect, attackActionsList[attackIndex].attackPart.transform.position, Quaternion.identity);
        if(attackIndex == 7)
        {
            Vector3 bladePointPos = new Vector3(weaponBladePoint.transform.position.x, 0, weaponBladePoint.transform.position.z);
            camera.GetComponent<WaveEffect>().startShockWave(bladePointPos);
        }
        Destroy(effect, 0.5f);
    }
    //可以把人打飞 
    public void AirBorneAttack()
    {
        if (target != null)
            target.GetComponent<Monster>().AirBorne();
    }          

    /// <summary>
    /// Dodge action
    /// </summary>
    private bool couldDodge;
    private bool isDodge;
    private void Dodge()
    {
        if(couldDodge && Input.GetKeyDown(KeyCode.Space))
        {
            isDodge     = true;
            couldDodge  = false;
            couldMove   = false;
            if (isMove)
            {
                animatorPlay.MotionWalk(false);
                animatorPlay.MotionDodge(true);
            }
            else
            {
                animatorPlay.MotionStand(false);
                animatorPlay.MotionDodge(true);
            }
        }
    }
    public void DodgeStart()
    {
        unBeatable = true;
    }
    public void DodgeFinish()
    {
        couldMove  = true;
        isDodge    = false;
        unBeatable = false;
        animatorPlay.MotionDodge(false);
        StartCoroutine(DodgeCoolDown());
    }
    private IEnumerator DodgeCoolDown()
    {
        yield return new WaitForSeconds(2.5f);
        couldDodge = true;
    }

    private void InitializeVariable()
    {
        //Base Properties
        HP                  = 1000;
        maxHP               = HP;
        strengthValue       = 100;
        maxStrengthValue    = strengthValue;
        //Move
        couldMove           = true;
        isMove              = false;
        attackMove          = false;
        //Attack
        isAirBorne            = false;
        couldNextAttack     = false;
        //Dodge
        isDodge             = false;
        unBeatable          = false;
        couldDodge          = true;
        //Attack Tree
        currentNode         = attackModeTree;
        attackCommandQueue  = new Queue<AttackTree>();
    }
    
    void Start()
    {
        GameObject mainCamera   = GameObject.FindGameObjectWithTag("MainCamera");
        camera                  = mainCamera.GetComponent<Camera>();
        playerAnimator          = GetComponent<Animator>();
        heroController          = GetComponent<CharacterController>();
        animatorPlay            = GetComponent<HeroAnimationControl>();
        attackModeTree          = DesignAttackTree();
        GetTarget();
        weapon.SetActive(false);
        InitializeAttackDic();//Initialize 
        InitializeVariable();//Initialize all variables of player
    }

    void Update()
    {
        if (target == null)
        {
            target = GetTarget();
        }
        if (!inAttack)
        {
            HideWeapon();
        }
    }
    void FixedUpdate()
    {
        if(!isDead && !isBreak)            //要是没死或人没晕
        {
            if (!inAttack && !isDodge)
            {
                Move();        //移动
            }
            if (attackMove)
                AttackMove();  //在攻击中移动(只有特定的攻击才可以)
            Attack();          //攻击
            if(!inAttack)
                Dodge();           //闪避
        }
        AnimationMove();   //动画中的位移
    }
}
