using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoundDetector : MonoBehaviour
{
    private void Start()
    {
        IkoController.Instance.OnReset += Reset;
    }

    private int _rounds;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _rounds++;
        IkoController.Instance.OnRound(_rounds);
    }

    private void Reset()
    {
        _rounds = 0;
    }
}
