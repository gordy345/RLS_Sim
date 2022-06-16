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

    private void Start()
    {
        _workModeToggle.OnStateChange += HandleWorkMode;
    }

    public override void UpdateUI(bool clearState)
    {
        if (clearState)
        {
            _workModeAction.Reset();
        }
        _workModeToggle.SetStateNoEvent(_workModeAction.CurrentState);
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
