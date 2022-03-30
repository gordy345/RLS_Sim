using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlock : AbstractBlock
{
    public Toggle T1;
    public Toggle T2;

    public ToggleAction T1_a;
    public ToggleAction T2_a;

    public void Action1(bool state) => TriggerEventInGM(T1_a, state);
    public void Action2(bool state) => TriggerEventInGM(T2_a, state);


    private void Start()
    {
        T1.OnToggle.AddListener(Action1);
        T2.OnToggle.AddListener(Action2);
        UpdateUI();
    }

    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI()
    {
        T1.SetStateNoEvent(T1_a.currentState);
        T2.SetStateNoEvent(T2_a.currentState);
    }
}
