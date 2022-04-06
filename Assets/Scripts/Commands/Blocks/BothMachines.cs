using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BothMachines : AbstractBlock
{
    [SerializeField]
    private Image MachinesImage;

    [HideInInspector]
    [SerializeField]
    private Color RecolorCol;

    public override void UpdateUI() { }

    private bool _l_pressed;
    private bool _g_pressed;
    private bool _b_pressed;


    private void Update()
    {
        // ru layout
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                _l_pressed = true;
            }
            else if (_l_pressed &&
                Input.GetKeyDown(KeyCode.U))
            {
                _g_pressed = true;
            }
            else if (_l_pressed &&
                _g_pressed &&
                Input.GetKeyDown(KeyCode.Comma))
            {
                _b_pressed = true;
            }
            else if (_l_pressed &&
                _g_pressed &&
                _b_pressed &&
                Input.GetKeyDown(KeyCode.N))
            {
                _l_pressed = false;
                _g_pressed = false;
                _b_pressed = false;
                MachinesImage.color = RecolorCol;
            }
            else
            {
                _l_pressed = false;
                _g_pressed = false;
                _b_pressed = false;
            }
        }
    }
}
