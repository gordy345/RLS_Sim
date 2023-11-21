using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP71 : AbstractBlock
{

    [Header("Actions")]
    public ToggleAction Network;
    public ToggleAction Anod;

    [Header("Triggers")]
    public Toggle NetworkTrigger;
    public Toggle AnodTrigger;

    public void NetworkAction(bool state) => TriggerEventInGM(Network, state);
    public void AnodAction(bool state) => TriggerEventInGM(Anod, state);


    private void Start()
    {
        UpdateUI(false);

        NetworkTrigger.OnToggle.AddListener(NetworkAction);
        AnodTrigger.OnToggle.AddListener(AnodAction);
    }

    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI(bool _)
    {
        NetworkTrigger.SetStateNoEvent(Network.currentState);
        AnodTrigger.SetStateNoEvent(Anod.currentState);
    }

}
