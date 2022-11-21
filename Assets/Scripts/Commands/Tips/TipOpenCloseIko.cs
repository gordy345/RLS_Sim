using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TipOpenCloseIko : MonoBehaviour
{
    [SerializeField] float _tipDelay;
    [SerializeField] private Button _relatedButton = null!;
    [SerializeField] TipType _tipType;
    private CanvasGroup canvas;

    private void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        _relatedButton.onClick.AddListener(Close);
        switch (_tipType)
        {
            case TipType.Close:
                GameManager.Instance.Tips_CloseIkoCalled.AddListener(Open);
                break;
            case TipType.Open:
                GameManager.Instance.Tips_OpenIkoCalled.AddListener(Open);
                break;
        }
        GameManager.Instance.OnReset.AddListener(Close);
        canvas.alpha = 0;
    }

    private void Open()
    {
        IEnumerator delay()
        {
            yield return new WaitForSeconds(_tipDelay);
            canvas.alpha = 1;
        }
        StartCoroutine(delay());
    }

    private void Close()
    {
        canvas.alpha = 0;
    }

    public enum TipType
    {
        Close,
        Open,
    }
}
