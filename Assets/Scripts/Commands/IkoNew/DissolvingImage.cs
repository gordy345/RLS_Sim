using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolvingImage : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvas;
    private float _creationTime;
    [SerializeField]
    private float _minAlpha;
    [SerializeField]
    private bool _isTarget;

    void Start()
    {
        _creationTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        //if (Time.realtimeSinceStartup > _creationTime + IkoController.Instance.TargetsDissolveTime)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        if (_canvas.alpha <= _minAlpha) return;
        float dissolveTime = IkoController.Instance.TargetsDissolveTime;
        if (!_isTarget) {
            dissolveTime = IkoController.Instance.InterferenceDissolveTime;
        }
        _canvas.alpha = Mathf.InverseLerp(
            _creationTime + dissolveTime,
            _creationTime,
            Time.realtimeSinceStartup
        );
    }
}
