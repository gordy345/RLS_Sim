using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject CommandSelect;
    public GameObject RoleSelect;
    public GameObject MainPanel;
    public CheckResult CheckResultPanel;

    [Header("Tooltip")]
    public TooltipPanel Tooltip;
    public float TooltipDelay;
    public bool TooltipIsAllowed { get; private set; } = true;

    [Header("Commands")]
    public Command[] CommandList;
    public RectTransform CommandButtonsUI;
    public Button CommandButtonPrefab;

    [Header("Blocks")]
    public AbstractBlock DefaultBlock;
    private AbstractBlock CurrentBlock { get; set; }

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

        CurrentBlock = DefaultBlock;
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
        //clear state
        foreach (var a in actions)
        {
            a.Reset();
        }
        actions.Clear();
        CurrentBlock.UpdateUI();
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
            FailCheck("Count is not matching");
            return;
        }

        if (actions.Zip(req, (a, r) => new { a, r })
            .Any(t => t.a.GetInstanceID() != t.r.GetInstanceID())
            )
        {
            FailCheck("Actions are not matching");
            return;
        }


        if (!actions.All(a => a.IsInRequiredState()))
        {
            FailCheck("Some action is not in required state");
            return;
        }
        PassCheck();
    }

    private void FailCheck(string reason = "")
    {
        //Debug.Log("Check failed: " + reason);
        CheckResultPanel.gameObject.SetActive(true);
        CheckResultPanel.ShowFailMessage();
    }

    private void PassCheck()
    {
        //Debug.Log("Check passed");
        CheckResultPanel.gameObject.SetActive(true);
        CheckResultPanel.ShowPassMessage();
    }

    public void AddToState(Action a)
    {
        var last = actions.LastOrDefault();
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
        //Debug.Log("State length: " + actions.Count);
        //Debug.Log("Last action: " + actions.LastOrDefault()?.name);
    }
}
