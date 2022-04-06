using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BothMachines : AbstractBlock
{
    [SerializeField]
    private Image MachinesImage;
    [SerializeField]
    private Color RecolorCol;

    public override void UpdateUI() { }

    private void Start()
    {
        GameManager.Instance.OnRecolor.AddListener(Recolor);
    }

    private void Recolor()
    {
        Debug.Log("recolor");
        MachinesImage.color = RecolorCol;
    }
}
