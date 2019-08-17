using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunMode : BaseMode {

	 public StunMode()
    {
        modeType = GAME_MODE_TYPE.StunMode;
    }

    public override void Begin(object[] _params)
    {
        playerAnimator.SetBool("Break", true);
        base.Begin(_params);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void End()
    {
        playerAnimator.SetBool("Break", false);
        base.End();
    }

    public override void InputHandle()
    {

    }
}
