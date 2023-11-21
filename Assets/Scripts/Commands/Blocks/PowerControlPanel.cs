using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerControlPanel : AbstractBlock
{
    [Header("Actions")]
    public ToggleAction RemoteControl;
    public ToggleAction Lighting;
    public ToggleAction CheckPKI;
    public ToggleAction Battery;
    public ToggleAction Launch;
    public ToggleAction Excitement;
    public ToggleAction Network;

    [Header("Triggers")]
    public Toggle RemoteControlTrigger;
    public Toggle LightingTrigger;
    public Toggle CheckPKITrigger;
    public Toggle BatteryTrigger;
    public Toggle LaunchTrigger;
    public Toggle ExcitementTrigger;
    public Toggle NetworkTrigger;

    public void RemoteControlAction(bool state) => TriggerEventInGM(RemoteControl, state);
    public void LightingAction(bool state) => TriggerEventInGM(Lighting, state);
    public void CheckPKIAction(bool state) => TriggerEventInGM(CheckPKI, state);

    public void BatteryAction(bool state) => TriggerEventInGM(Battery, state);
    public void StartAction(bool state) => TriggerEventInGM(Launch, state);
    public void ExcitementAction(bool state) => TriggerEventInGM(Excitement, state);
    public void NetworkAction(bool state) => TriggerEventInGM(Network, state);
   

    private void Start()
    {
        UpdateUI(false);

        RemoteControlTrigger.OnToggle.AddListener(RemoteControlAction);
        LightingTrigger.OnToggle.AddListener(LightingAction);
        CheckPKITrigger.OnToggle.AddListener(CheckPKIAction);
        BatteryTrigger.OnToggle.AddListener(BatteryAction);
        LaunchTrigger.OnToggle.AddListener(StartAction);
        ExcitementTrigger.OnToggle.AddListener(ExcitementAction);
        NetworkTrigger.OnToggle.AddListener(NetworkAction);
    }


    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI(bool _)
    {
        RemoteControlTrigger.SetStateNoEvent(RemoteControl.currentState);
        LightingTrigger.SetStateNoEvent(Lighting.currentState);
        CheckPKITrigger.SetStateNoEvent(CheckPKI.currentState);
        BatteryTrigger.SetStateNoEvent(Battery.currentState);
        LaunchTrigger.SetStateNoEvent(Launch.currentState);
        ExcitementTrigger.SetStateNoEvent(Excitement.currentState);
        NetworkTrigger.SetStateNoEvent(Network.currentState);
    }

}
