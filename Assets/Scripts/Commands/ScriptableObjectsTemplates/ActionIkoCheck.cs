using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionIkoCheck", menuName = "ScriptableObjects/ActionIkoCheck", order = 12)]
public class ActionIkoCheck : Action
{
    public override bool IsInDefaultState() => true;

    public override bool IsInRequiredState() => true;

    public override void Reset() { }
}
