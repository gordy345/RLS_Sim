using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Command", menuName = "ScriptableObjects/Command", order = 0)]
public class Command : ScriptableObject
{
    public string CommandName;

    [Header("Pos 1")]
    public bool Enabled_p1 = true;
    public string CommandDescriptionPos1;
    public Action[] ActionsPos1;

    [Header("Pos 2")]
    public bool Enabled_p2 = true;
    public string CommandDescriptionPos2;
    public Action[] ActionsPos2;

    [Header("Pos 3")]
    public bool Enabled_p3 = true;
    public string CommandDescriptionPos3;
    public Action[] ActionsPos3;

    [Header("Pos 4")]
    public bool Enabled_p4 = true;
    public string CommandDescriptionPos4;
    public Action[] ActionsPos4;

    [Header("Pos 5")]
    public bool Enabled_p5 = true;
    public string CommandDescriptionPos5;
    public Action[] ActionsPos5;
}
