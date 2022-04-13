using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject CommandSelect;
    public GameObject RoleSelect;
    public ActionsPanel MainPanel;
    public CheckResult CheckResultPanel;

    [Header("Tooltip")]
    public TooltipPanel Tooltip;
    public float TooltipDelay;
    public bool TooltipIsAllowed { get; set; } = true;

    [Header("Commands")]
    public Command[] CommandList;
    public RectTransform CommandButtonsUI;
    public Button CommandButtonPrefab;

    // state
    private Command _currentCommand;
    private int _personRole;
    private List<Action> actions = new List<Action>();

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        foreach (var c in CommandList)
        {
            var b = Instantiate(CommandButtonPrefab);
            b.onClick.AddListener(() => StartCommandScript(c));
            b.GetComponentInChildren<Text>().text = c.CommandName;
            var trigger = b.GetComponent<TooltipTrigger>();
            trigger.text = c.CommandName;
            b.transform.SetParent(CommandButtonsUI.transform, false);
        }
        CommandSelect.SetActive(true);
        RoleSelect.SetActive(false);
        MainPanel.SetActive(false);
        TooltipIsAllowed = true;
    }

    private void Start()
    {
        Tooltip.Show("", null);
        Tooltip.Hide();
    }

    private void StartCommandScript(Command command)
    {
        TooltipIsAllowed = false;
        _currentCommand = command;
        CommandSelect.SetActive(false);
        RoleSelect.SetActive(true);
    }

    public void SetRole(int r)
    {
        _personRole = r;
        RoleSelect.SetActive(false);
        MainPanel.SetActive(true);
        MainPanel.OpenDefaultBlock();
    }

    public void BackToCommandSelect()
    {
        _currentCommand = null;
        _personRole = 0;
        CommandSelect.SetActive(true);
        RoleSelect.SetActive(false);
        MainPanel.SetActive(false);
        TooltipIsAllowed = true;
        HideTooltipAfterFrame();
        Restart();
    }

    private void HideTooltipAfterFrame()
    {
        IEnumerator c()
        {
            yield return new WaitForEndOfFrame();
            Tooltip.Hide();
        }
        StartCoroutine(c());
    }

    public void BackToRoleSelect()
    {
        _personRole = 0;
        CommandSelect.SetActive(false);
        RoleSelect.SetActive(true);
        MainPanel.SetActive(false);
        Restart();
    }

    public void Restart()
    {
        foreach (var a in actions)
        {
            a.Reset();
        }
        actions.Clear();
        MainPanel.UpdateCurrentBlockUI();
    }

    public void CheckOrder()
    {
        Action[] req;
        switch (_personRole)
        {
            case 1: req = _currentCommand.ActionsPos1; break;
            case 2: req = _currentCommand.ActionsPos2; break;
            case 3: req = _currentCommand.ActionsPos3; break;
            case 4: req = _currentCommand.ActionsPos4; break;
            case 5: req = _currentCommand.ActionsPos5; break;
            default: return;
        }

        if (actions.Count != req.Length)
        {
            //Debug.Log("Fail on items count check");
            FailCheck();
            return;
        }

        var i = 0;
        foreach (var a in actions) 
        {
            var r = req[i];
            //Debug.Log("action: " + a.ActionName + ", id: " + a.GetInstanceID() + " is in group: " + a.isUnordored +
            //    " | reqired: " + r.ActionName + ", id: " + r.GetInstanceID() + " | ref equal: " + (a == r));

            if (
                a.ActionName != r.ActionName &&
                a.GetInstanceID() != r.GetInstanceID()
                )
            {
                //Debug.Log("Fail on items validation");
                FailCheck();
                return;
            }

            i++;
        }


        if (actions.Any(a => !a.IsInRequiredState()))
        {
            //Debug.Log("Fail on items state validation");
            FailCheck();
            return;
        }
        PassCheck();
    }

    private void FailCheck()
    {
        CheckResultPanel.gameObject.SetActive(true);
        CheckResultPanel.ShowFailMessage();
    }

    private void PassCheck()
    {
        CheckResultPanel.gameObject.SetActive(true);
        CheckResultPanel.ShowPassMessage();
    }

    public void AddToState(Action a)
    {
        var last = actions.LastOrDefault();
        if (a.isUnordored)
        {
            UnordoredActionGroup g;
            if (!(last is UnordoredActionGroup))
            {
                g = Instantiate(a.relatedActionGroup);
            }
            else
            {
                actions.Remove(last);
                g = (UnordoredActionGroup)last;
            }

            g.AddAction(a);

            if (!g.IsInDefaultState())
                actions.Add(g);

            return;
        }

        if (last?.name == a.name)
        {
            actions.Remove(last);
            if (!a.IsInDefaultState() || !a.RemoveIfMatchingDefaultState)
                actions.Add(a);
        }
        else
        {
            actions.Add(a);
        }
    }
}
