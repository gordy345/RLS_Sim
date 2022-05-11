using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IkoController : MonoBehaviour
{
    [Header("Display Objects")]
    [SerializeField]
    private Image Grid;
    private Material _gridMat;
    private Color _colorGrid;
    [SerializeField]
    private GameObject Line;

    [Header("Controls objects")]
    [SerializeField]
    private Scrollbar BrightnessController;

    [Header("Settings")]
    [SerializeField]
    private float LineRotationSpeed;
    private const float _defaultBrightness = 0.5f;
    private float _closeTime = -1f;
    private bool _open = false;

    public static IkoController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _closeTime = Time.time;
    }

    void Start()
    {
        gameObject.SetActive(false);
        _gridMat = Instantiate(Grid.material);
        Grid.material = _gridMat;
        _colorGrid = _gridMat.color;

        BrightnessController.onValueChanged.AddListener(BrightnessChanged);
        BrightnessController.value = _defaultBrightness;
    }

    void Update()
    {
        var angles = Line.transform.localEulerAngles;
        angles.z += LineRotationSpeed * Time.deltaTime;
        Line.transform.localEulerAngles = angles;
    }

    public void BrightnessChanged(float value)
    {
        _colorGrid.a = value;
        _gridMat.color = _colorGrid;
        Grid.material = _gridMat;
    }

    public void OpenIko()
    {
        gameObject.SetActive(true);
        if (!_open)
        {
            _open = true;
            return;
        }
        var dt = Time.time - _closeTime;
        var angles = Line.transform.localEulerAngles;
        angles.z += LineRotationSpeed * dt;
        Line.transform.localEulerAngles = angles;
    }

    public void CloseIko()
    {
        gameObject.SetActive(false);
        _closeTime = Time.time;
    }
}
