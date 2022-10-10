using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectManyTestBlock : AbstractBlock
{
    [SerializeField]
    private TestVariantAction[] _possibleActions;

    [SerializeField]
    private RectTransform _buttonsHolder;

    public Toggle ButtonPrefab;

    private UnityEvent _updateActions;

    public float ButtonsPadding = 0;

    private Dictionary<TestVariantAction, Text> _toggles = new Dictionary<TestVariantAction, Text>();
    private Stack<TestVariantAction> _currentActions = new Stack<TestVariantAction>();

    private void Start()
    {
        float btnsHeight = ButtonsPadding;
        _updateActions = new UnityEvent();

        var btnTextTransform = ButtonPrefab.GetComponentInChildren<Text>().transform as RectTransform;
        var btnTextPadding = btnTextTransform.offsetMin.y - btnTextTransform.offsetMax.y;

        var btns = new List<Toggle>();

        foreach (var a in _possibleActions)
        {
            var instance = Instantiate(ButtonPrefab);
            instance.transform.SetParent(_buttonsHolder, false);
            instance.OnToggle.AddListener(s => TriggerActionInGM(a, s));
            instance.SetStateNoEvent(a.currentState);

            _updateActions.AddListener(() => instance.SetStateNoEvent(a.currentState));

            instance.GetComponentInChildren<Text>().text = a.ActionName;
            instance.GetComponent<TooltipTrigger>().text = a.ActionName;

            var t = instance.transform.Find("SelectionNum").GetComponent<Text>();
            _toggles.Add(a, t);
            btns.Add(instance);
        }

        IEnumerator corout()
        {
            yield return new WaitForEndOfFrame();
            foreach (var instance in btns)
            {
                var textObj = instance.GetComponentInChildren<Text>();
                var height = textObj.preferredHeight + btnTextPadding + 1f;

                (instance.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                btnsHeight += height + ButtonsPadding;
            }

            _buttonsHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, btnsHeight);
        }

        StartCoroutine(corout());

        GameManager.Instance.TooltipIsAllowed = true;
    }


    private void OnDestroy()
    {
        GameManager.Instance.TooltipIsAllowed = false;
    }

    private void TriggerActionInGM(TestVariantAction a, bool state)
    {
        a.currentState = state;
        if (!_currentActions.Contains(a))
        {
            a.SelectedIndex = _currentActions.Count;
            _toggles[a].text = (_currentActions.Count + 1).ToString();
            _currentActions.Push(a);
        }
        else
        {
            var resetStack = new Stack<TestVariantAction>();
            TestVariantAction last;
            while(_currentActions.Count > 0)
            {
                last = _currentActions.Pop();
                _toggles[last].text = string.Empty;
                last.SelectedIndex = -1;
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
                t.SelectedIndex = _currentActions.Count;
                _toggles[t].text = (_currentActions.Count + 1).ToString();
                _currentActions.Push(t);
            }
        }
        GameManager.Instance.AddToState(a);
    }

    public override void UpdateUI(bool clearState)
    {
        if (clearState)
        {
            _currentActions.Clear();
            foreach (var a in _possibleActions)
            {
                a.Reset();
            }
        }
        foreach (var t in _toggles)
        {
            t.Value.text = string.Empty;
        }
        _updateActions.Invoke();
        foreach (var a in _possibleActions.OrderBy(a => a.SelectedIndex))
        {
            if (a.currentState && a.SelectedIndex >= 0)
            {
                _toggles[a].text = (a.SelectedIndex + 1).ToString();
                _currentActions.Push(a);
            }
        }
    }
}
