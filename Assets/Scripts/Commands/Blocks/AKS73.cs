using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKS73 : AbstractBlock
{

    [Header("Actions")]
    public ToggleAction Network;

    [Header("Triggers")]
    public Toggle NetworkTrigger;

    public void NetworkAction(bool state) => TriggerEventInGM(Network, state);


    private void Start()
    {
        UpdateUI(false);

        NetworkTrigger.OnToggle.AddListener(NetworkAction);
    }

    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI(bool _)
    {
        NetworkTrigger.SetStateNoEvent(Network.currentState);
    }

}
