using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MenuItem", menuName = "ScriptableObjects/MenuItem", order = 2)]
public class MenuItem : ScriptableObject
{
    public string Name;
    public MenuItem[] childNodes;
    public AbstractBlock Prefab;
}
