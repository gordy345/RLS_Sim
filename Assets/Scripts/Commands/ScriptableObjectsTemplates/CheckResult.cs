using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckResult : MonoBehaviour
{
    public GameObject PassMessage;
    public GameObject FailMessage;
    public UnityEvent OnCloseToMenu;
    public UnityEvent OnCloseRestart;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
        PassMessage.SetActive(false);
        FailMessage.SetActive(false);
    }

    public void ShowPassMessage()
    {
        gameObject.SetActive(true);
        PassMessage.SetActive(true);
    }

    public void ShowFailMessage()
    {
        gameObject.SetActive(true);
        FailMessage.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        PassMessage.SetActive(false);
        FailMessage.SetActive(false);
        OnCloseToMenu?.Invoke();
    }

    public void Restart()
    {
        gameObject.SetActive(false);
        PassMessage.SetActive(false);
        FailMessage.SetActive(false);
        OnCloseRestart?.Invoke();
    }
}
