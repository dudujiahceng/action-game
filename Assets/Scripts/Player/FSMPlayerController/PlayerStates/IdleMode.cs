using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMode : BaseMode {

    public IdleMode()
    {
        modeType = GAME_MODE_TYPE.IdleMode;
    }
    public override void Begin(object[] _params)
    {
        playerAnimator.SetBool("Stand", true);
        base.Begin(_params);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void End()
    {
        playerAnimator.SetBool("Stand", false);
        base.End();
    }

    public override void InputHandle()
    {

    }
}
