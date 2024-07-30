using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    protected override void Init()
    {
        State = CreatureState.Moving;

        base.Init();
    }

    protected override void UpdateAnimation()
    {
    }
}