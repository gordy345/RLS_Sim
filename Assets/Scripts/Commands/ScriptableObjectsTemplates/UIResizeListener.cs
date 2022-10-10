using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIResizeListener : MonoBehaviour
{
    public UnityEvent OnResized;
    private void OnRectTransformDimensionsChange()
    {
        Debug.Log("resize");
        OnResized?.Invoke();
    }
}
