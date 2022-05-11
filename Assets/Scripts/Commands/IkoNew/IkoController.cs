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
    [SerializeField]
    public Transform TargetsFolder;
    [SerializeField]
    private CanvasGroup _group;

    [Header("Controls objects")]
    [SerializeField]
    private Scrollbar BrightnessController;
    [SerializeField]
    private Button StartButton;

    [Header("Settings")]
    [SerializeField]
    private float LineRotationSpeed;
    [SerializeField]
    private float MaxDistanceToTarget;
    [SerializeField]
    private float MinDistanceToTarget;
    [SerializeField]
    private float MaxTargetVel;
    [SerializeField]
    private float MinTargetVel;
    [SerializeField]
    private float TargetsDirectionSpreadAngle;
    public float TargetsDissolveTime;

    [Header("Prefabs")]
    [SerializeField]
    private IkoTarget IkoTargetPrefab;
    [SerializeField]
    private GameObject TargetSingle;
    [SerializeField]
    private GameObject TargetGroup;
    [SerializeField]
    private GameObject TargetSingleOurs;
    [SerializeField]
    private GameObject TargetGroupOurs;


    private const float _defaultBrightness = 0.5f;
    private IkoTarget _lastTarget;
    private bool _hasStarted;
    private int rounds = 0;

    public static IkoController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gridMat = Instantiate(Grid.material);
        Grid.material = _gridMat;
        _colorGrid = _gridMat.color;

        BrightnessController.onValueChanged.AddListener(BrightnessChanged);
        BrightnessController.value = _defaultBrightness;

        gameObject.SetActive(true);
        CloseIko();
    }

    void Update()
    {
        if (!_hasStarted) return;
        var angles = Line.transform.localEulerAngles;

        var lastAngle = angles.z;
        angles.z += LineRotationSpeed * Time.deltaTime;
        Line.transform.localEulerAngles = angles;
        if (lastAngle < 90 && angles.z >= 90)
        {
            rounds++;
            if (rounds == 4)
            {
                GenerateInterference();
            }
        }
    }

    public void BrightnessChanged(float value)
    {
        _colorGrid.a = value;
        _gridMat.color = _colorGrid;
        Grid.material = _gridMat;
    }

    public void OpenIko()
    {
        _group.alpha = 1.0f;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    public void CloseIko()
    {
        _group.alpha = 0.0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }

    public void GenerateTargets()
    {
        var dist = Random.Range(MinDistanceToTarget, MaxDistanceToTarget);
        var angle = Random.Range(0, 360f);
        var pos = Quaternion.Euler(0, 0, angle) * 
            new Vector2(0, dist) + 
            Line.transform.position;

        var velAmp = Random.Range(MinTargetVel, MaxTargetVel);
        var td = TargetsDirectionSpreadAngle / 2;
        var angleVel = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(
                Vector2.up,
                Line.transform.position - pos
            ) + Random.Range(-td, +td)
        );

        var vel = angleVel * new Vector2(0, velAmp);

        var instance = Instantiate(IkoTargetPrefab);

        instance.Center = Line.transform;
        instance.StartPos = pos;
        instance.MotionVel = vel;

        var isGroup = Random.Range(0f, 10f) <= 5;
        var isOur = Random.Range(0f, 10f) <= 5;

        instance.IsGroup = isGroup;
        instance.IsOurs = isOur;

        if (isGroup)
        {
            instance.TargetPrefab = TargetGroup;
            if (isOur)
                instance.TargetPrefabDetermined = TargetGroupOurs;
            else
                instance.TargetPrefabDetermined = TargetGroup;
        }
        else
        {
            instance.TargetPrefab = TargetSingle;
            if (isOur)
                instance.TargetPrefabDetermined = TargetSingleOurs;
            else
                instance.TargetPrefabDetermined = TargetSingle;
        }


        instance.transform.SetParent(TargetsFolder, false);

        instance.transform.localScale = Vector3.one;

        _lastTarget = instance;
    }

    public void GenerateInterference()
    {
        Debug.Log("??????");
    }

    public void StartTest()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        GenerateTargets();
        rounds = 0;
        StartButton.interactable = false;
    }

    public void Reset()
    {
        _hasStarted = false;
        rounds = 0;
        Destroy(_lastTarget);
        _lastTarget = null;
        Line.transform.localEulerAngles = new Vector3(0, 0, 90);
        StartButton.interactable = true;
    }
}
