using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToggleAction", menuName = "ScriptableObjects/ToggleAction", order = 10)]
public class ToggleAction : Action
{
    [Header("ToggleAction fields")]
    [SerializeField]
    private bool _defaultState;
    [SerializeField]
    private bool _requiredState = true;

    public override bool RemoveIfMatchingDefaultState => true;
    public bool DefaultState => _defaultState;
    public bool RequiredState => _requiredState;

    public bool currentState;

    public override void Reset()
    {
        currentState = DefaultState;
    }

    public override bool IsInDefaultState() => currentState == DefaultState;

    public override bool IsInRequiredState() => currentState == RequiredState;

}

