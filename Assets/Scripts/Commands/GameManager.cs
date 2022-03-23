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

    [Header("Tooltip")]
    public TooltipPanel Tooltip;
    public float TooltipDelay;

    [Header("Commands")]
    public Command[] CommandList;
    public RectTransform CommandButtonsUI;
    public Button CommandButtonPrefab;

    // state
    private Command _currentCommand;
    private int _personRole;
    private List<Action> actions = new List<Action>();

    public static GameManager Instance { get; private set; }

    void Start()
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
    }

    private void StartCommandScript(Command command)
    {
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
    }

    public void BackToRoleSelect()
    {
        _personRole = 0;
        CommandSelect.SetActive(false);
        RoleSelect.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void Restart()
    {
        //clear state
        actions.Clear();
    }

    public void CheckOrder(bool showUI = false)
    {
        if (showUI)
        {
            // ui
        }
        // check
    }

    public void AddToState(Action a)
    {
        var last = actions.LastOrDefault();
        if (last?.name == a.name)
        {
            actions.Remove(last);
            if (!a.IsInDefaultState())
                actions.Add(a);
        }
        else
        {
            actions.Add(a);
        }
    }
}
