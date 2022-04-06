using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Machine1 : AbstractBlock
{
    [SerializeField]
    private ToggleAction[] _possibleActions;

    [SerializeField]
    private Transform _buttonsHolder;

    public Toggle ButtonPrefab;

    private UnityEvent _updateActions;

    private void Start()
    {
        _updateActions = new UnityEvent();
        foreach (var a in _possibleActions)
        {
            var instance = Instantiate(ButtonPrefab);
            instance.transform.SetParent(_buttonsHolder, false);
            instance.GetComponentInChildren<Text>().text = a.ActionName;
            instance.OnToggle.AddListener(s => TriggerActionInGM(a, s));
            instance.SetStateNoEvent(a.currentState);

            _updateActions.AddListener(() => instance.SetStateNoEvent(a.currentState));
        }
    }

    private void TriggerActionInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI()
    {
        _updateActions.Invoke();
    }
}
