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

    [Header("IKO")]
    public GameObject OpenIkoButton;
    [SerializeField]
    private IkoController _iko;

    [Header("Tooltip")]
    public TooltipPanel Tooltip;
    public float TooltipDelay;
    public bool TooltipIsAllowed { get; set; } = true;

    [Header("Commands")]
    public Command[] CommandList;
    public RectTransform CommandButtonsUI;
    public Button CommandButtonPrefab;

    [Header("Role Select Buttons")]
    public RectTransform ButtonsHolder;
    public GameObject RoleButton1;
    public GameObject RoleButton2;
    public GameObject RoleButton3;
    public GameObject RoleButton4;
    public GameObject RoleButton5;
    public float RoleButtonWidth;
    public float RoleButtonsSpacing;



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
#if !UNITY_EDITOR
            if (c.IsTestOnly) continue;
#endif
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
        _iko?.gameObject.SetActive(true);
        _iko?.CloseIko();
    }

    private void StartCommandScript(Command command)
    {
        TooltipIsAllowed = false;
        _currentCommand = command;
        CommandSelect.SetActive(false);

        float totalWidth = -RoleButtonsSpacing;

        RoleButton1.SetActive(command.Enabled_p1);
        RoleButton2.SetActive(command.Enabled_p2);
        RoleButton3.SetActive(command.Enabled_p3);
        RoleButton4.SetActive(command.Enabled_p4);
        RoleButton5.SetActive(command.Enabled_p5);

        if (command.Enabled_p1) totalWidth += RoleButtonWidth + RoleButtonsSpacing;
        if (command.Enabled_p2) totalWidth += RoleButtonWidth + RoleButtonsSpacing;
        if (command.Enabled_p3) totalWidth += RoleButtonWidth + RoleButtonsSpacing;
        if (command.Enabled_p4) totalWidth += RoleButtonWidth + RoleButtonsSpacing;
        if (command.Enabled_p5) totalWidth += RoleButtonWidth + RoleButtonsSpacing;

        ButtonsHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

        RoleSelect.SetActive(true);
    }

    public void SetRole(int r)
    {
        _personRole = r;
        OpenIkoButton.SetActive(_currentCommand.ShowIkoButton);
        RoleSelect.SetActive(false);
        MainPanel.SetActive(true);
        MainPanel.OpenDefaultBlock();

        IkoController.Instance.gameObject.SetActive(true);
        if (_currentCommand.ShowIkoButton) IkoController.Instance.OpenIko();
        else IkoController.Instance.CloseIko();
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
        IkoController.Instance?.CloseIko();
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
        IkoController.Instance?.CloseIko();
        Restart();
    }

    public void Restart()
    {
        foreach (var a in actions)
        {
            a.Reset();
        }
        actions.Clear();
        MainPanel.UpdateCurrentBlockUI(true);
        IkoController.Instance?.Restart();
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
            FailCheck();
            return;
        }

        var i = 0;
        foreach (var a in actions) 
        {
            var r = req[i];
            if (
                a.ActionName != r.ActionName &&
                a.GetInstanceID() != r.GetInstanceID()
                )
            {
                FailCheck();
                return;
            }

            i++;
        }


        if (actions.Any(a => !a.IsInRequiredState()))
        {
            FailCheck();
            return;
        }
        PassCheck();
    }

    public void FailCheck()
    {
        CheckResultPanel.gameObject.SetActive(true);
        CheckResultPanel.ShowFailMessage();
    }

    public void PassCheck()
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

        if (last == a || last?.name == a.name)
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
