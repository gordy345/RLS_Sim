using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "__Action_TipOpenIKO", menuName = "ScriptableObjects/__Action_TipOpenIKO", order = 110)]
public class __Action_TipOpenIKO : InternalAction
{
    public override void DispatchAction()
    {
        GameManager.Instance.Tips_OpenIkoCalled.Invoke();
    }

    public override bool IsInDefaultState() => true;
    public override bool IsInRequiredState() => true;
    public override void Reset() { }
}
