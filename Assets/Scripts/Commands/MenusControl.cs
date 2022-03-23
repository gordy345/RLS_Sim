using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenusControl : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public RectTransform Panel1;
    public RectTransform Panel2;
    public RectTransform Panel3;

    [Header("Prefabs")]
    public Button ButtonPrefab;
    public MenuItem[] Panel1Items;
    const int BUTTON_HEIGHT = 35;
    const int BOTTOM_PADDING = 10;

    //state
    private MenuItem _itemL1;
    private MenuItem _itemL2;
    private MenuItem _itemL3;

    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();

        foreach (Transform obj in Panel1)
        {
            Destroy(obj.gameObject);
        }
        foreach (var item in Panel1Items)
        {
            CreateButton(1, item);
        }
        ResizePanel(Panel1);
    }

    private void CreateButton(int btnLayer, MenuItem item)
    {
        var b = Instantiate(ButtonPrefab);
        Transform parent;
        switch (btnLayer)
        {
            case 1: parent = Panel1; break;
            case 2: parent = Panel2; break;
            case 3: parent = Panel3; break;
            default: parent = Panel1; break;
        }
        b.transform.SetParent(parent, false);
        if (btnLayer == 3)
        {
            b.onClick.AddListener(() => OpenBlock(item));
        }
        else
        {
            b.onClick.AddListener(() => OpenMenus(btnLayer + 1, item));
        }
        b.GetComponentInChildren<Text>().text = item.Name;
    }

    public void OpenMenus()
    {
        Panel1.gameObject.SetActive(true);
        Panel2.gameObject.SetActive(false);
        Panel3.gameObject.SetActive(false);
    }

    public void OpenMenus(int layer, MenuItem item)
    {
        if (layer == 2)
        {
            _itemL1 = item;
            foreach (Transform obj in Panel2)
            {
                Destroy(obj.gameObject);
            }
            foreach (var item2 in item.childNodes)
            {
                CreateButton(layer, item2);
            }
            ResizePanel(Panel2);
            Panel1.gameObject.SetActive(true);
            Panel2.gameObject.SetActive(true);
            Panel3.gameObject.SetActive(false);
        }
        if (layer == 3)
        {
            _itemL2 = item;
            foreach (Transform obj in Panel3)
            {
                Destroy(obj.gameObject);
            }
            foreach (var item2 in item.childNodes)
            {
                CreateButton(layer, item2);
            }
            ResizePanel(Panel3);
            Panel1.gameObject.SetActive(true);
            Panel2.gameObject.SetActive(true);
            Panel3.gameObject.SetActive(true);
        }
    }

    private void OpenBlock(MenuItem item)
    {
        _itemL3 = item;
        Debug.Log($"OpenBlock: {item.Name}");
        CloseMenus();
    }

    private void ResizePanel(RectTransform panel)
    {
        panel.sizeDelta = new Vector2(
            panel.sizeDelta.x,
            (panel.childCount * BUTTON_HEIGHT + BOTTOM_PADDING)
        );
    }

    private void CloseMenus()
    {
        Panel1.gameObject.SetActive(false);
        Panel2.gameObject.SetActive(false);
        Panel3.gameObject.SetActive(false);

        foreach (Transform obj in Panel2)
        {
            Destroy(obj.gameObject);
        }
        foreach (Transform obj in Panel3)
        {
            Destroy(obj.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPressRaycast.gameObject == gameObject)
        {
            CloseMenus();
        }
    }
}
