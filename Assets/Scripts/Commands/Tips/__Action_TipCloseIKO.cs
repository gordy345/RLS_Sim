using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "__Action_TipCloseIKO", menuName = "ScriptableObjects/__Action_TipCloseIKO", order = 111)]
public class __Action_TipCloseIKO : InternalAction
{
    public override void DispatchAction()
    {
        GameManager.Instance.Tips_CloseIkoCalled.Invoke();
    }

    public override bool IsInDefaultState() => true;
    public override bool IsInRequiredState() => true;
    public override void Reset() { }
}
