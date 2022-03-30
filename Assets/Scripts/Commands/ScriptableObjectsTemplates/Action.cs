using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "ScriptableObjects/Action", order = 1)]
public abstract class Action : ScriptableObject
{
    public string ActionName;
    public string ActionDescription;

    public abstract void Reset();
    public abstract bool IsInDefaultState();
    public abstract bool IsInRequiredState();

    public virtual bool RemoveIfMatchingDefaultState => false;
}

