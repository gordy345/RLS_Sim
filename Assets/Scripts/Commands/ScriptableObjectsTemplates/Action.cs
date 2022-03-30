using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract class Action : ScriptableObject
{
    [Header("Action base fields")]
    public string ActionName;
    public bool isUnordored;
    public UnordoredActionGroup relatedActionGroup;

    public void OnEnable() => Reset();

    public abstract void Reset();
    public abstract bool IsInDefaultState();
    public abstract bool IsInRequiredState();

    public virtual bool RemoveIfMatchingDefaultState => false;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Action))]
public class ActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var action = target as Action;

        action.isUnordored = GUILayout.Toggle(action.isUnordored, "Is Unordored");

        if (action.isUnordored)
            action.relatedActionGroup = EditorGUILayout.ObjectField(action.relatedActionGroup, typeof(UnordoredActionGroup), false) as UnordoredActionGroup;

    }
}
 #endif