using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POS72 : AbstractBlock
{
    [SerializeField]
    private MultistateToggle _workModeToggle;
    [SerializeField]
    private MultistateToggleAction _workModeAction;
    [SerializeField]
    private string _strobModeName;

    [Header("Actions")]
    public ToggleAction HighVoltage;
    public ToggleAction Power;
    public ToggleAction Rotation;
    public ToggleAction Speed;

    [Header("Triggers")]
    public Toggle HighVoltageTrigger;
    public Toggle PowerTrigger;
    public Toggle RotationTrigger;
    public Toggle SpeedTrigger;

    public void HighVoltageAction(bool state) => TriggerEventInGM(HighVoltage, state);
    public void PowerAction(bool state) => TriggerEventInGM(Power, state);
    public void RotationAction(bool state) => TriggerEventInGM(Rotation, state);
    public void SpeedAction(bool state) => TriggerEventInGM(Speed, state);

    private void Start()
    {
        UpdateUI(false);
        _workModeToggle.OnStateChange += HandleWorkMode;

        HighVoltageTrigger.OnToggle.AddListener(HighVoltageAction);
        PowerTrigger.OnToggle.AddListener(PowerAction);
        RotationTrigger.OnToggle.AddListener(RotationAction);
        SpeedTrigger.OnToggle.AddListener(SpeedAction);
    }

    public override void UpdateUI(bool clearState)
    {
        if (clearState)
        {
            _workModeAction.Reset();
        }
        _workModeToggle.SetStateNoEvent(_workModeAction.CurrentState);
        HighVoltageTrigger.SetStateNoEvent(HighVoltage.currentState);
        PowerTrigger.SetStateNoEvent(Power.currentState);
        RotationTrigger.SetStateNoEvent(Rotation.currentState);
        SpeedTrigger.SetStateNoEvent(Speed.currentState);
    }

    private void TriggerEventInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    private void HandleWorkMode(string state)
    {
        _workModeAction.CurrentState = state;
        if (state == _strobModeName)
        {
            //Debug.Log("strob mode");
            IkoController.Instance.EnableStrobControl();
        }
        else
        {
            IkoController.Instance.DisableStrobControl();
        }
        GameManager.Instance.AddToState(_workModeAction);
    }
}
