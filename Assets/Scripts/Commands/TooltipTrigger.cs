using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("The text to display in the tooltip")]
    public string text;
    private float delay => GameManager.Instance.TooltipDelay;
    private TooltipPanel tooltipPanel => GameManager.Instance.Tooltip;

    private bool mouseIsHovering;
    private bool tooltipIsShown;
    private float mouseHoverTime;
    [SerializeField]
    private RectTransform rectTransform;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsHovering = true;
        mouseHoverTime = Time.unscaledTime;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsHovering = false;
        HideTooltip();
    }

    private void Update()
    {
        if (mouseIsHovering && !tooltipIsShown)
        {
            if (Time.unscaledTime >= mouseHoverTime + delay)
                ShowTooltip();
        }
    }

    private void ShowTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipIsShown = true;
            tooltipPanel.Show(text, rectTransform);
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipIsShown = false;
            tooltipPanel.Hide();
        }
    }

}