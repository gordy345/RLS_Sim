using UnityEngine;
using UnityEngine.UI;

// Attach this component to a UI object which will be the tooltip visual
// Must include some text somewhere of course, and optionally a CanvasGroup if you want fading
public class TooltipPanel : MonoBehaviour
{
    [Tooltip("The text object which will display the tooltip string")]
    public Text tooltipText;
    [Tooltip("Whether to move the tooltip with the mouse after it's visible")]
    public bool moveWithMouse;
    [Tooltip("The distance from the mouse position the tooltip will appear at (relative to tooltip pivot)")]
    public Vector2 positionOffset;
    [Tooltip("The margins from the edge of the screen which the tooltip will stay within")]
    public Vector2 margins;


    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private RectTransform triggerObject;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (moveWithMouse)
            Reposition();
        if (Input.anyKeyDown)
        {
            gameObject.SetActive(false);
        }
    }

    private void Reposition()
    {
        Vector2 screenPos = Input.mousePosition;
        // world position origin is wherever the pivot is
        Vector2 newPos = screenPos + positionOffset;
        float maxX = Screen.width - margins.x;
        float minX = margins.x;
        float maxY = Screen.height - margins.y;
        float minY = margins.y;
        float rightEdge = newPos.x + (1f - rectTransform.pivot.x) * rectTransform.rect.width * parentCanvas.scaleFactor;
        if (rightEdge > maxX)
        {
            newPos.x -= rightEdge - maxX;
        }
        float leftEdge = newPos.x - rectTransform.pivot.x * rectTransform.rect.width * parentCanvas.scaleFactor;
        if (leftEdge < minX)
        {
            newPos.x += minX - leftEdge;
        }

        // y is measured from top
        float topEdge = newPos.y + (1f - rectTransform.pivot.y) * rectTransform.rect.height * parentCanvas.scaleFactor;
        if (topEdge > maxY)
        {
            newPos.y -= topEdge - maxY;
        }

        float bottomEdge = newPos.y - rectTransform.pivot.y * rectTransform.rect.height * parentCanvas.scaleFactor;
        if (bottomEdge < minY)
        {
            newPos.y += minY - bottomEdge;
        }

        var cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;

        if (triggerObject != null)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(triggerObject, newPos, cam, out var worldPoint);
            rectTransform.position = worldPoint;
        }
    }
    public void Show(string text, RectTransform triggeredBy)
    {
        if (gameObject.activeSelf)
            return;

        triggerObject = triggeredBy;
        tooltipText.text = text;



        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        Reposition();
    }

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
    }
}