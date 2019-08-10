using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnikaProperty : Property {
    public List<string> attackAnimationsList = new List<string>();
    private void AddAttackAnimation()
    {
        attackAnimationsList.Add("ForwardAttack_1");
        attackAnimationsList.Add("ForwardAttack_2");
        attackAnimationsList.Add("ForwardAttack_3");
        attackAnimationsList.Add("SickleAttack_1");
        attackAnimationsList.Add("SickleAttack_2");
        attackAnimationsList.Add("SickleAttack_3");
        attackAnimationsList.Add("SickleAttack_4");
        attackAnimationsList.Add("BackJump");
    }
    void Awake()
    {
        AddAttackAnimation();
    }
	void Start () {
        walkSpeed = 8;
        GetAllTarget();
	}
}
