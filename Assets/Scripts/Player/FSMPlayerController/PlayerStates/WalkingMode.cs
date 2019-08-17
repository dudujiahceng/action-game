using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMode : BaseMode {
    private Vector3 moveDirection;
    

    public WalkingMode()
    {
        modeType = GAME_MODE_TYPE.WalkingMode;
    }
    public override void Begin(object[] _params)
    {
        player.isMove = true;
        playerAnimator.SetBool("Walk", true);
        base.Begin(_params);
    }

    public override void Update()
    {
        Move();
        base.Update();
    }
    public override void End()
    {
        playerAnimator.SetBool("Walk", false);
        player.isMove = false;
        base.End();
    }

    public override void InputHandle()
    {
        
    }

    
    void Move()
    {
        playerController.Move(moveDirection);
        moveDirection = player.transform.forward * player.walkSpeed * Time.deltaTime;
    }
}
