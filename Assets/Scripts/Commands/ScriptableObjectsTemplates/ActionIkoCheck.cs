using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIkoCheck : Action
{
    public override bool IsInDefaultState() => true;

    public override bool IsInRequiredState() => true;

    public override void Reset() { }
}
