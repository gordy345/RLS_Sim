using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Tooltip("The text to display in the tooltip")]
    public string text;
    private float Delay => GameManager.Instance.TooltipDelay;
    private TooltipPanel TooltipPanel => GameManager.Instance.Tooltip;


    [SerializeField]
    private RectTransform _rectTransform;

    private Coroutine _coroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GameManager.Instance.TooltipIsAllowed) return;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData) => HideTooltip();

    private void ShowTooltip()
    {
        IEnumerator c()
        {
            yield return new WaitForSecondsRealtime(Delay);
            if (TooltipPanel != null)
            {
                TooltipPanel.Show(text, _rectTransform);
            }
        }
        _coroutine = StartCoroutine(c());
    }

    private void HideTooltip()
    {
        if (TooltipPanel != null)
        {
            TooltipPanel.Hide();
        }
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData) => HideTooltip();
}