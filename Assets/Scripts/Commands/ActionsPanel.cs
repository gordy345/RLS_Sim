using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionsPanel : MonoBehaviour
{
    [Header("SubMenus")]
    public RectTransform Panel1;
    public RectTransform Panel2;
    public RectTransform Panel3;
    public RawImage RaycastTarget;
    public RectTransform WorkingArea;

    [Header("Prefabs")]
    public Button ButtonPrefab;
    public MenuItem[] Panel1Items;
    const int BUTTON_HEIGHT = 35;
    const int BOTTOM_PADDING = 10;

    [Header("Blocks")]
    public AbstractBlock DefaultBlockPrefab;
    public AbstractBlock CurrentBlock { get; set; }

    // state, for debug only
    private MenuItem _itemL1;
    private MenuItem _itemL2;
    private MenuItem _itemL3;

    void Start()
    {
        CurrentBlock = Instantiate(DefaultBlockPrefab);
        UpdateCurrentBlock();
        CloseMenus();
        RerenderPanel(Panel1, Panel1Items, 1);
    }

    public void SetActive(bool state) => gameObject.SetActive(state);

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
        //if (btnLayer == 3)
        //{
        //    b.onClick.AddListener(() => OpenBlock(item));
        //}
        //else
        //{
        b.onClick.AddListener(() => OpenMenus(btnLayer + 1, item));
        //}
        b.GetComponentInChildren<Text>().text = item.Name;
    }

    public void ToggleMenus()
    {
        if (RaycastTarget.enabled)
        {
            CloseMenus();
        }
        else
        {
            Panel1.gameObject.SetActive(true);
            Panel2.gameObject.SetActive(false);
            Panel3.gameObject.SetActive(false);
            RaycastTarget.enabled = true;
        }
    }

    public void OpenMenus(int layer, MenuItem item)
    {
        if (layer == 2)
        {
            _itemL1 = item;
            RerenderPanel(Panel2, item.childNodes, layer);
            Panel1.gameObject.SetActive(true);
            Panel2.gameObject.SetActive(true);
            Panel3.gameObject.SetActive(false);
        }
        if (layer == 3)
        {
            _itemL2 = item;
            RerenderPanel(Panel3, item.childNodes, layer);
            Panel1.gameObject.SetActive(true);
            Panel2.gameObject.SetActive(true);
            Panel3.gameObject.SetActive(true);
        }
        if (layer == 4)
        {
            _itemL3 = item;
        }
        if (item.Prefab != null)
        {
            OpenBlock(item);
        }
    }

    public void CloseMenus()
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
        RaycastTarget.enabled = false;
        _itemL1 = null;
        _itemL2 = null;
        _itemL3 = null;
    }

    public void UpdateCurrentBlockUI()
    {
        CurrentBlock?.UpdateUI();
    }

    public void OpenDefaultBlock()
    {
        CurrentBlock = Instantiate(DefaultBlockPrefab);
        UpdateCurrentBlock();
    }

    private void OpenBlock(MenuItem item)
    {
        if (item.Prefab != null)
        {
            CurrentBlock = Instantiate(item.Prefab);
            UpdateCurrentBlock();
        }
        if (item.childNodes == null || 
            item.childNodes.Length == 0 || 
            item == _itemL3) 
            CloseMenus();
    }

    private void RerenderPanel(RectTransform panel, MenuItem[] items, int layer)
    {
        foreach (Transform obj in panel)
        {
            Destroy(obj.gameObject);
        }
        foreach (var item2 in items)
        {
            CreateButton(layer, item2);
        }
        panel.sizeDelta = new Vector2(
            panel.sizeDelta.x,
            (items.Length * BUTTON_HEIGHT + BOTTOM_PADDING)
        );
    }

    private void ClearWorkArea()
    {
        foreach (Transform t in WorkingArea)
        {
            Destroy(t.gameObject);
        }
    }

    private void UpdateCurrentBlock()
    {
        ClearWorkArea();
        CurrentBlock.transform.SetParent(WorkingArea.transform, false);

        IEnumerator c()
        {
            yield return new WaitForEndOfFrame();
            CurrentBlock.UpdateUI();
        }

        StartCoroutine(c());
    }
}
