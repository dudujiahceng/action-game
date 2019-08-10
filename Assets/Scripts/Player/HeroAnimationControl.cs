using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationID : MonoBehaviour
{
    public static readonly int Stand = Animator.StringToHash("Stand");
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Stun_01 = Animator.StringToHash("Stun_01");
    public static readonly int Stun_02 = Animator.StringToHash("Stun_02");
    public static readonly int Stun_03 = Animator.StringToHash("Stun_03");
    public static readonly int Stun_04 = Animator.StringToHash("Stun_04");
    public static readonly int Weapon_Attack_01 = Animator.StringToHash("Weapon_Attack_01");
    public static readonly int AirBorne_Attack_1 = Animator.StringToHash("AirBorne_Attack_1");
    public static readonly int Combo2_1 = Animator.StringToHash("Combo2_1");
    public static readonly int Combo2_2 = Animator.StringToHash("Combo2_2");
    public static readonly int RotateAttack = Animator.StringToHash("RotateAttack");
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int Break = Animator.StringToHash("Break");
    public static readonly int AirBorne_1 = Animator.StringToHash("AirBorne_1");
    public static readonly int AirBorne_2 = Animator.StringToHash("AirBorne_2");
}
/*This class is used for change the motion of hero by status*/
public class HeroAnimationControl : MonoBehaviour{
    private Animator playerAnimator;
    //Stand
    public void MotionStand(bool flag)
    {
        playerAnimator.SetBool(AnimationID.Stand, flag);
    }
    //Walk
    public void MotionWalk(bool flag)
    {
        playerAnimator.SetBool(AnimationID.Walk, flag);
    }
    //Stun_01
    public void MotionStun_01(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Stun_01, flag);
    }
    //Stun_02
    public void MotionStun_02(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Stun_02, flag);
    }
    //Stun_03
    public void MotionStun_03(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Stun_03, flag);
    }
    //Stun_04
    public void MotionStun_04(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Stun_04, flag);
    }
    //Weapon_Attack_01
    public void MotionWeaponAttack_01(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Weapon_Attack_01, flag);
    }
    //AirBorne_Attack_1
    public void MotionAirBorne_Attack_1(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.AirBorne_Attack_1, flag);
    }
    //Combo2_1
    public void MotionCombo2_1(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Combo2_1, flag);
    }
    //Combo2_2
    public void MotionCombo2_2(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Combo2_2, flag);
    }
    //RotateAttack
    public void MotionRotateAttack(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.RotateAttack, flag);
    }
    //Dodge
    public void MotionDodge(bool flag)
    {
        StopAllMotion();
        playerAnimator.SetBool("Dodge", flag);
    }
    //Break
    public void MotionBreak(bool flag)
    {
        StopAttackMotion();
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Break, flag);
    }
    //Airborne
    public void MotionAirborne_1(bool flag)
    {
        Debug.Log("1");
        playerAnimator.SetBool(AnimationID.Break, false);
        playerAnimator.SetBool(AnimationID.AirBorne_1, flag);
    }
    public void MotionAirborne_2(bool flag)
    {
        Debug.Log("2");
        playerAnimator.SetBool(AnimationID.AirBorne_1, false);
        playerAnimator.SetBool(AnimationID.AirBorne_2, flag);
    }
    public void ActionDead(bool flag)
    {
        StopAttackMotion();
        StopAllMotion();
        playerAnimator.SetBool(AnimationID.Die, flag);
    }

    public void StopAllMotion()
    {
        MotionStand(false);
        MotionWalk(false);
    }
    public void StopAttackMotion()
    {
        MotionStun_01(false);
        MotionStun_02(false);
        MotionStun_03(false);
        MotionStun_04(false);
        MotionWeaponAttack_01(false);
        MotionAirBorne_Attack_1(false);
        MotionCombo2_1(false);
        MotionCombo2_2(false);
        MotionRotateAttack(false);
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }
}
