using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS74 : AbstractBlock
{
    [Header("Actions")]
    public ToggleAction Commandier2_a;
    public ToggleAction Commutator2_a;
    public ToggleAction Vent2_a;
    public ToggleAction Commandier1_a;
    public ToggleAction Commutator1_a;
    public ToggleAction Vent1_a;

    [Header("Triggers")]
    public Toggle Commandier2;
    public Toggle Commutator2;
    public Toggle Vent2;
    public Toggle Commandier1;
    public Toggle Commutator1;
    public Toggle Vent1;

    public void Commandier2Action(bool state) => TriggerEventInGM(Commandier2_a, state);
    public void Commutator2Action(bool state) => TriggerEventInGM(Commutator2_a, state);
    public void Vent2Action(bool state) => TriggerEventInGM(Vent2_a, state);
    public void Commutator1Action(bool state) => TriggerEventInGM(Commutator1_a, state);
    public void Commandier1Action(bool state) => TriggerEventInGM(Commandier1_a, state);

    public void Vent1Action(bool state) => TriggerEventInGM(Vent1_a, state);

    private void Start()
    {
        UpdateUI(false);

        Commandier2.OnToggle.AddListener(Commandier2Action);
        Commutator2.OnToggle.AddListener(Commutator2Action);
        Vent2.OnToggle.AddListener(Vent2Action);
        Commandier1.OnToggle.AddListener(Commandier1Action);
        Commutator1.OnToggle.AddListener(Commutator1Action);
        Vent1.OnToggle.AddListener(Vent1Action);

    }


    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI(bool _)
    {
        Commandier2.SetStateNoEvent(Commandier2_a.currentState);
        Commutator2.SetStateNoEvent(Commutator2_a.currentState);
        Vent2.SetStateNoEvent(Vent2_a.currentState);
        Commandier1.SetStateNoEvent(Commandier1_a.currentState);
        Commutator1.SetStateNoEvent(Commutator1_a.currentState);
        Vent1.SetStateNoEvent(Vent1_a.currentState);
    }
}
