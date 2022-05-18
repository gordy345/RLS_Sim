using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InterferenceType
{
    None,
    Passive,
    LocalObjects,
}

[RequireComponent(typeof(Collider2D))]
public class InerferenceTarget : MonoBehaviour
{
    public CanvasGroup _targetPrefab;
    [SerializeField]
    private InterferenceType _type;

    private bool _triggered;
    private CanvasGroup _instance;

    private void Start()
    {
        switch (_type)
        {
            case InterferenceType.Passive:
            case InterferenceType.LocalObjects:
                {
                    IkoController.Instance.OnPassiveIntChange += ChangeBrightness;
                    break;
                }
            case InterferenceType.None:
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered) return;
        if (collision.tag != "Line") return;
        _triggered = true;
        _instance = Instantiate(_targetPrefab);
        _instance.transform.SetParent(transform, false);
        _instance.transform.localScale = Vector3.one;
    }

    private void ChangeBrightness(float brightness)
    {
        _instance.alpha = brightness;
    }

}
