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
        _relatedButton.onClick.AddListener(OnClicked);
        switch (_tipType)
        {
            case TipType.Close:
                GameManager.Instance.Tips_CloseIkoCalled.AddListener(OnInvoked);
                break;
            case TipType.Open:
                GameManager.Instance.Tips_OpenIkoCalled.AddListener(OnInvoked);
                break;
        }
        canvas.alpha = 0;
    }

    private void OnInvoked()
    {
        IEnumerator delay()
        {
            yield return new WaitForSeconds(_tipDelay);
            canvas.alpha = 1;
        }
        StartCoroutine(delay());
    }

    private void OnClicked()
    {
        canvas.alpha = 0;
    }

    public enum TipType
    {
        Close,
        Open,
    }
}
