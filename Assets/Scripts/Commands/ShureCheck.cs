using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShureCheck : MonoBehaviour
{
    public static ShureCheck Instance { get; private set; }

    [SerializeField] private Button _buttonYes;
    [SerializeField] private Button _buttonNo;
    [SerializeField] private Button _bg;

    private bool _anyPressed;
    private bool _yesPressed;

    private bool _enabled = false;

    public ShureCheck()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_enabled)
            gameObject.SetActive(false);

        _buttonYes.onClick.AddListener(YesClicked);
        _buttonNo.onClick.AddListener(NoClicked);
        _bg.onClick.AddListener(NoClicked);

        _buttonYes.onClick.AddListener(Close);
        _buttonNo.onClick.AddListener(Close);
        _bg.onClick.AddListener(Close);
    }

    public static async Task<bool> CheckIfShure()
    {
        Instance._enabled = true;
        Instance.gameObject.SetActive(true);
        return await Instance.CheckShure();
    }

    private async Task<bool> CheckShure()
    {
        _anyPressed = false;
        _yesPressed = false;

        while (!_anyPressed)
        {
            await Task.Yield();
        }
        Close();

        return _yesPressed;
    }

    private void NoClicked() => _anyPressed = true;
    private void YesClicked()
    {
        _anyPressed = true;
        _yesPressed = true;
    }

    private void Close()
    {
        IEnumerator c()
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }
        StartCoroutine(c());
    }
}
