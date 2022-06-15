using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultistateToggleAction", menuName = "ScriptableObjects/MultistateToggleAction", order = 14)]
public class MultistateToggleAction : Action
{
    [Header("MultistateToggleAction fields")]
    [SerializeField]
    private string _defaultState = "";
    [SerializeField]
    private string _requiredState = "";

    public string CurrentState;

    private void Awake()
    {
        CurrentState = _defaultState;
    }

    public override bool IsInDefaultState() => CurrentState == _defaultState;

    public override bool IsInRequiredState() => CurrentState == _requiredState;

    public override void Reset() => CurrentState = _defaultState;
}
