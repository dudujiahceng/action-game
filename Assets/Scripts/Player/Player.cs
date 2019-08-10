using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player : MonoBehaviour {
    protected struct AttackAction
    {
        public float damage;
        public GameObject attackPart;
        public GameObject effect;
        public AttackAction(float _damage, GameObject _attackPart, GameObject _effect)
        {
            damage = _damage;
            attackPart = _attackPart;
            effect = _effect;
        }
    }
    #region
    //UI
    public Slider HPSlider;
    public Slider Strength;

    public Slider EnemyHPSlider;
    public Slider EnemyStrengthSlider;
    //
    protected Animator playerAnimator;
    protected HeroAnimationControl animatorPlay;
    public AnimationState animaSta;
    public bool inAttack = false;
    protected CharacterController heroController;
    protected GameObject target;
    protected Camera camera;
    //Base property module
    public float HP;
    protected float maxHP;
    public float strengthValue;
    protected float maxStrengthValue;

    protected bool couldMove;
    protected bool isDead;
    public bool isBreak;
    public bool isAirBorne;
    //Player's attack method
    public GameObject leftHand, rightHand, leg;
    public GameObject weapon, weaponBladePoint;
    protected bool unBeatable;
    //Battle module
    #endregion
    protected void GetAttackPart()
    {
        Transform[] allChilds = GetComponentsInChildren<Transform>();
        foreach(var child in allChilds)
        {
            if (child.name == "LeftHand")
                leftHand = child.gameObject;
            if (child.name == "RightHand")
                rightHand = child.gameObject;
            if (child.name == "Leg")
                leg = child.gameObject;
            if (child.name == "Weapon")
                weapon = child.gameObject;
            if (child.name == "BladePoint")
                weaponBladePoint = child.gameObject;
        }
    }

    //Get target
    protected GameObject GetTarget()
    {
        GameObject[] currentTargets = GameObject.FindGameObjectsWithTag("Target");
        if (currentTargets.Length == 0)
            return null;
        float minDistance = 100;
        GameObject newTarget = null;
        foreach (GameObject tar in currentTargets)
        {
            if(Vector3.Magnitude(transform.position - tar.transform.position) <= minDistance)
            {
                minDistance = Vector3.Magnitude(transform.position - tar.transform.position);
                newTarget = tar;
            }
        }
        return newTarget;
    }


    /// <summary>
    /// Creat an attack tree
    /// </summary>
    /// initialize attack animation tree
    protected AttackTree attackModeTree = new AttackTree(); //Attack tree's root
    protected Queue<AttackTree> attackCommandQueue;         //Attack Command queue
    protected AttackTree currentNode;                       //Current attack tree node
    protected bool couldNextAttack;                         //Current is node ready to point next node 
    public AttackTree NewAttackTreeLeaf(ref AttackTree parentLeaf, int attackMode, string attackModeStr, bool leftOrRight, bool isLoop)
    {
        AttackTree newLeaf          = new AttackTree();
        newLeaf.Parent              = parentLeaf;
        if (leftOrRight)
            parentLeaf.LeftChild    = newLeaf;
        else
            parentLeaf.RightChild   = newLeaf;
        newLeaf.AttackMode          = attackMode;
        newLeaf.attackModeStr       = attackModeStr;
        newLeaf.isLoop              = isLoop;
        newLeaf.key                 = leftOrRight;
        return newLeaf;
    }
    // Creat an attack tree function DesignAttackTree()
    protected abstract AttackTree DesignAttackTree();       //Create a specific attack tree in derive class
    public void CouldNextAttack()
    {
        couldNextAttack = true;
    }


    public virtual void GetDamage(float damage)
    {
        if (!unBeatable)
        {
            if (HP > 0)
                HP -= damage;
            if (HP <= 0)
            {
                HP = 0;
                Die();
            }
            if (strengthValue > 0)
                strengthValue -= damage;
            if (strengthValue <= 0)
            {
                strengthValue = 0;
                if (!isBreak)
                    BreakStatus();
            }
            HPSlider.value = HP / maxHP;
        }
    }
    protected void Die()
    {
        isDead = true;
        weapon.SetActive(false);
        GetComponent<HeroAnimationControl>().ActionDead(true);
    }

    /// <summary>
    /// Negative status module
    /// a:stun, b:airborne
    /// </summary>
    protected void BreakStatus()
    {
        isBreak = true;
        currentNode = attackModeTree; //Attack node return to root;
        attackCommandQueue.Clear();
        animatorPlay.MotionBreak(true);
        StartCoroutine(BreakRecover());
    }
    public bool GetBreakStutas
    {
        get
        {
            return isBreak;
        }
    }
    IEnumerator BreakRecover()
    {
        yield return new WaitForSeconds(5);
        isBreak = false;
        strengthValue = maxStrengthValue;
        animatorPlay.MotionBreak(false);
        animatorPlay.MotionStand(true);
        strengthValue = maxStrengthValue;
    }
    public bool GetLiftStatus
    {
        get
        {
            return isDead;
        }
    }
    
}
