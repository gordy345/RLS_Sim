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
    private RectTransform _buttonsHolder;

    public Toggle ButtonPrefab;

    private UnityEvent _updateActions;

    public float ButtonSizeWPadding;

    private Dictionary<ToggleAction, Text> _toggles = new Dictionary<ToggleAction, Text>();
    private Stack<ToggleAction> _currentActions = new Stack<ToggleAction>();

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

            var t = instance.transform.Find("SelectionNum").GetComponent<Text>();
            _toggles.Add(a, t);
        }

        var contentSize = _possibleActions.Length * ButtonSizeWPadding + 20;
        _buttonsHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentSize);
    }

    private void TriggerActionInGM(ToggleAction a, bool state)
    {
        a.currentState = state;
        if (!_currentActions.Contains(a))
        {
            _toggles[a].text = (_currentActions.Count + 1).ToString();
            _currentActions.Push(a);
        }
        else
        {
            var resetStack = new Stack<ToggleAction>();
            ToggleAction last;
            while(_currentActions.Count > 0)
            {
                last = _currentActions.Pop();
                _toggles[last].text = string.Empty;
                if (last != a)
                {
                    resetStack.Push(last);
                }
                else
                {
                    break;
                }
            }
            while (resetStack.Count > 0)
            {
                var t = resetStack.Pop();
                _toggles[t].text = (_currentActions.Count + 1).ToString();
                _currentActions.Push(t);
            }
        }
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI()
    {
        _updateActions.Invoke();
    }
}
