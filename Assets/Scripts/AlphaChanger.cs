using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaChanger : MonoBehaviour 
{
    [SerializeField] GameObject LineSprite;
    [SerializeField] GameObject LineAlpha;

    [SerializeField] GameObject GrindSprite;
    [SerializeField] GameObject GrindAlpha;

    public void LineAlphaChanger()
    {
        Color color = LineSprite.GetComponent<Renderer>().material.color;
        color.a = LineAlpha.GetComponent<Slider>().value / 100;
        LineSprite.GetComponent<Renderer>().material.color = color;
    }

    public void GrindAlphaChanger()
    {
        Color color = GrindSprite.GetComponent<Renderer>().material.color;
        color.a = GrindAlpha.GetComponent<Slider>().value / 100;
        GrindSprite.GetComponent<Renderer>().material.color = color;
    }
}
