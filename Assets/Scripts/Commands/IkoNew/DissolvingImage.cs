using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolvingImage : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvas;
    private float _creationTime;

    void Start()
    {
        _creationTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (Time.realtimeSinceStartup > _creationTime + IkoController.Instance.TargetsDissolveTime)
        {
            Destroy(gameObject);
            return;
        }
        _canvas.alpha = Mathf.InverseLerp(
            _creationTime + IkoController.Instance.TargetsDissolveTime,
            _creationTime,
            Time.realtimeSinceStartup
        );
    }
}
