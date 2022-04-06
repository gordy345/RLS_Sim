using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnordoredActionGroup", menuName = "ScriptableObjects/Unordored Action Group", order = 4)]
public class UnordoredActionGroup : Action
{
    [Header("UnordoredActionGroup")]
    [SerializeField]
    public List<Action> RequiredActions = new List<Action>();

    private List<Action> CurrentActions { get; set; } = new List<Action>();

    public override bool IsInDefaultState() => RequiredActions == null || RequiredActions.Count == 0;

    public override bool IsInRequiredState() =>
        RequiredActions.Count == CurrentActions.Count &&
        RequiredActions.All(
            r => CurrentActions.Contains(r)
        ) &&
        CurrentActions.All(a => a.IsInRequiredState());

    public override void Reset()
    {
        foreach (var action in CurrentActions)
        {
            action.Reset();
        }
        CurrentActions.Clear();
    }

    public void AddAction(Action a)
    {
        if (CurrentActions.Any(o => o.GetInstanceID() == a.GetInstanceID()))
        {
            CurrentActions.RemoveAll(o => o.GetInstanceID() == a.GetInstanceID());
            if (!a.IsInDefaultState())
            {
                CurrentActions.Add(a);
            }
        }
        else
        {
            CurrentActions.Add(a);
        }
    }
}
