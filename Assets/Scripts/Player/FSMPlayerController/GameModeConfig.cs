using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_MODE_TYPE
{
    IdleMode,
    WalkingMode,
    DeathMode,
    AttackMode,
    DodgeMode,
    StunMode
}
public class GameModeConfig : MonoBehaviour {
    public static Dictionary<GAME_MODE_TYPE, BaseMode> CreateGameModeDict(PlayerController player)
    {
        var dict = new Dictionary<GAME_MODE_TYPE, BaseMode>()
        {
            {GAME_MODE_TYPE.IdleMode,       new IdleMode() },
            {GAME_MODE_TYPE.WalkingMode,    new WalkingMode()},
            {GAME_MODE_TYPE.DeathMode,      new DeathMode() },
            {GAME_MODE_TYPE.AttackMode,     new AttackMode() },
            {GAME_MODE_TYPE.DodgeMode,      new DodgeMode() },
            {GAME_MODE_TYPE.StunMode,       new StunMode() }
        };

        foreach(var item in dict)
        {
            item.Value.Init(player);
        }
        return dict;
    }
}
