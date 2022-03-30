using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Action : ScriptableObject
{
    public string ActionName;

    public abstract void Reset();
    public abstract bool IsInDefaultState();
    public abstract bool IsInRequiredState();

    public virtual bool RemoveIfMatchingDefaultState => false;
}

