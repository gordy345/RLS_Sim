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

    // Start is called before the first frame update
    void Start()
    {
        _gridMat = Instantiate(Grid.material);
        Grid.material = _gridMat;
        _colorGrid = _gridMat.color;

        BrightnessController.onValueChanged.AddListener(BrightnessChanged);
        BrightnessController.value = _defaultBrightness;
    }

    // Update is called once per frame
    void Update()
    {
        var angles = Line.transform.localEulerAngles;
        angles.z += LineRotationSpeed * Time.deltaTime;
        angles.z %= 360;
        Line.transform.localEulerAngles = angles;
    }

    public void BrightnessChanged(float value)
    {
        _colorGrid.a = value;
        _gridMat.color = _colorGrid;
        Grid.material = _gridMat;
    }
}
