using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class MultistateToggleState
{
    public Button DirectionBtn;
    public string StateName;
    public GameObject DisplayObject;
    public Transform StatePos;
    [HideInInspector]
    public float angle;
    [HideInInspector]
    public int index;
}

public class MultistateToggle : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private List<MultistateToggleState> _states;
    [SerializeField]
    private Transform _rotationCenter;

    public event UnityAction<string> OnStateChange;

    private const int _defaultState = 0;
    private int _currentState = _defaultState;
    private Camera _cam;

    private void Start()
    {
        for (int i = 0; i < _states.Count; i++)
        {
            var s = _states[i];
            s.index = i;
            s.DirectionBtn.onClick.AddListener(() => OnButtonClick(s.index));
            s.angle = Vector2.SignedAngle(
                Vector2.up,
                (Vector2)(s.StatePos.position - _rotationCenter.position)
            );
        }
        Reset();
        _cam = Camera.main;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var currentVec = (Vector2)(
            _cam.ScreenToWorldPoint(Input.mousePosition) - _rotationCenter.position
        );
        var angle = Vector2.SignedAngle(
            Vector2.up,
            currentVec
        );
        var selected = _states.OrderBy(s => Mathf.Abs(s.angle - angle)).FirstOrDefault();

        if (_currentState != selected.index)
        {
            _currentState = selected.index;
            OnStateChange?.Invoke(selected.StateName);
            UpdateUI();
        }
    }

    public void Reset()
    {
        _currentState = _defaultState;
        UpdateUI();
    }

    private void OnButtonClick(int index)
    {
        _currentState = index;
        OnStateChange?.Invoke(_states[index].StateName);
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var s in _states)
        {
            s.DisplayObject.SetActive(false);
        }
        _states[_currentState].DisplayObject.SetActive(true);
    }

    public void SetStateNoEvent(string stateName)
    {
        var state = _states.Where(s => s.StateName == stateName).FirstOrDefault();
        if (state == null) return;
        _currentState = _states.IndexOf(state);
        UpdateUI();
    }
}
