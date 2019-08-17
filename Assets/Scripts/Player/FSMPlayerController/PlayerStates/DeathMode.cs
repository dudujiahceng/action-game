using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMode : BaseMode {

	public DeathMode()
    {
        modeType = GAME_MODE_TYPE.DeathMode;
    }

    public override void Begin(object[] _params)
    {
        playerAnimator.SetBool("Die", true);
        base.Begin(_params);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void End()
    {
        playerAnimator.SetBool("Die", false);
        base.End();
    }

    public override void InputHandle()
    {
        
    }
}
