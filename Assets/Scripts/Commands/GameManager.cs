using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject CommandSelect;
    public GameObject RoleSelect;
    public GameObject MainPanel;
 
    [Header("Commands")]
    public Command[] CommandList;
    public RectTransform CommandButtonsUI;
    public Button CommandButtonPrefab;

    // state
    private Command _currentCommand;
    private int _personRole;


    void Start()
    {
        foreach (var c in CommandList)
        {
            var b = Instantiate(CommandButtonPrefab);
            b.onClick.AddListener(() => StartCommandScript(c));
            b.GetComponentInChildren<Text>().text = c.CommandName;
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

    }
}
