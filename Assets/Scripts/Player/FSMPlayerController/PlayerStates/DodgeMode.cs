using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeMode : BaseMode {
    public DodgeMode()
    {
        modeType = GAME_MODE_TYPE.DodgeMode;
    }
    public override void Begin(object[] _params)
    {
        playerAnimator.SetBool("Dodge", true);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void End()
    {
        base.End();
        playerAnimator.SetBool("Dodge", false);
    }

    public override void InputHandle()
    {

    }
}
