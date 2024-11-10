using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public enum CreatureState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        Crouch = 3,
        Attack = 4,
        Interactive = 5,
        GetHit = 6,
        Die = 7,
    }
}