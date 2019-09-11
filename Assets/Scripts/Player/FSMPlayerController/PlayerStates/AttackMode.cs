using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMode : BaseMode {
    private string attackName;
    private GameObject target;
    private Vector3 lookAtPosition;
    private object[] thisParams;
    public AttackMode()
    {
        modeType        = GAME_MODE_TYPE.AttackMode;
        attackName      = null;
        target          = null;
        lookAtPosition  = Vector3.zero;
        thisParams      = null;
    }
    public override void Begin(object[] _params)
    {
        base.Begin(_params);
        thisParams = _params;
        if (thisParams[0] != null)
            attackName = thisParams[0].ToString();
        if (thisParams[1] != null)
            target = (GameObject)thisParams[1];
        playerAnimator.SetBool(attackName, true);
        //Look at target before attack
        if(target != null)
        {
            lookAtPosition.x = target.transform.position.x;
            lookAtPosition.z = target.transform.position.z;
            player.transform.LookAt(lookAtPosition);
        }
        player.inAttack = true;
    }

    public override void Update()
    {
        base.Update();
    }
    public override void End()
    {
        base.End();
        player.inAttack = false;
        playerAnimator.SetBool(attackName, false);
    }

    public override void InputHandle()
    {

    }
}
