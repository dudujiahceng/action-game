using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMode{

    public GAME_MODE_TYPE modeType;
    protected PlayerController player;
    protected Animator playerAnimator;
    protected CharacterController playerController;

    public virtual void Init(PlayerController _player)
    {
        player              = _player;
        playerAnimator      = _player.GetComponent<Animator>();
        playerController    = _player.GetComponent<CharacterController>();
    }

    public virtual void Begin(object[] _params)
    {
        //Debug.Log(modeType.ToString() + "|Begin");
    }

	public virtual void Update () {
        //Debug.Log(modeType.ToString() + "|Update");
    }

    public virtual void End()
    {
        //Debug.Log(modeType.ToString() + "|End");
    }

    public virtual void InputHandle()
    {

    }
}
