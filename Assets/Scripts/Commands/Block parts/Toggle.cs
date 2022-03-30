using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

public class Toggle : MonoBehaviour
{
    public GameObject StateOn;
    public GameObject StateOff;

    public BoolEvent OnToggle;

    private bool _isToggledOn;
    public bool IsToggledOn
    {
        get => _isToggledOn;
        set {
            SetStateNoEvent(value);
            OnToggle?.Invoke(value);
        }
    }

    public void SetStateNoEvent(bool value)
    {
        _isToggledOn = value;
        StateOn.SetActive(value);
        StateOff.SetActive(!value);
    }

    public void ToggleState()
    {
        IsToggledOn = !IsToggledOn;
    }

    private void Awake()
    {
        OnToggle = new BoolEvent();
        //OnToggle.AddListener(EmptyHandler);
    }
    // Start is called before the first frame update
    void Start()
    {
        IsToggledOn = false;
    }

    private static void EmptyHandler(bool _) { }
}
