using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestVariantAction", menuName = "ScriptableObjects/TestVariantAction", order = 11)]
public class TestVariantAction : ToggleAction
{
    public int SelectedIndex { get; set; } = -1;
}
