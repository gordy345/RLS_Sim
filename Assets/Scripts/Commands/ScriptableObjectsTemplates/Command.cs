using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Command", menuName = "ScriptableObjects/Command", order = 0)]
public class Command : ScriptableObject
{
    public string CommandName;

    [Header("Pos 1")]
    public string CommandDescriptionPos1;
    public Action[] ActionsPos1;

    [Header("Pos 2")]
    public string CommandDescriptionPos2;
    public Action[] ActionsPos2;

    [Header("Pos 3")]
    public string CommandDescriptionPos3;
    public Action[] ActionsPos3;

    [Header("Pos 4")]
    public string CommandDescriptionPos4;
    public Action[] ActionsPos4;

    [Header("Pos 5")]
    public string CommandDescriptionPos5;
    public Action[] ActionsPos5;
}
