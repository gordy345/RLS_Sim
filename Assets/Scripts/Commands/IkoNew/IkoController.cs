using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField]
    private Transform InterferenceFolder;

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
    [SerializeField]
    private float InterferenceTimeOffset;

    public float MinTargetLineLength;
    public float MaxTargetLineLength;
    public float _maxTargetAngleDeviation;

    [Header("Target Prefabs")]
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

    [Header("Interference Prefabs")]
    [SerializeField]
    private GameObject PassiveInterferencePrefab;
    [SerializeField]
    private float _passiveIntRadius;

    [Header("Test buttons")]
    [SerializeField]
    private Button _buttonSingleTarget;
    [SerializeField]
    private Button _buttonGroupTarget;
    [SerializeField]
    private Button _buttonOursTarget;
    [SerializeField]
    private Button _buttonOthersTarget;
    [SerializeField]
    private List<Button> InterferenceButtons;

    [Header("Button Colors")]
    [SerializeField]
    private Color _colorDisabledUnchecked;
    [SerializeField]
    private Color _colorDisabledCheckedValid;
    [SerializeField]
    private Color _colorDisabledCheckedInvalid;


    private const float _defaultBrightness = 0.5f;
    private IkoTarget _lastTarget;
    private bool _hasStarted;

    [Header("Mistakes Check")]
    [SerializeField]
    private int _maxMistakes = 2;
    private int _currentMistakes = 0;
    private int Mistakes
    {
        get => _currentMistakes;
        set
        {
            _currentMistakes = value;
            if (_currentMistakes >= _maxMistakes)
                GameManager.Instance.FailCheck();
        }
    }


    public static IkoController Instance { get; private set; }

    public event UnityAction OnReset;

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

        Reset();
    }

    void Update()
    {
        if (!_hasStarted) return;
        var angles = Line.transform.localEulerAngles;

        var lastAngle = angles.z;
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
        var offset = _lastTarget.MotionVel;
        offset.Normalize();
        offset *= _passiveIntRadius;
        var instance = Instantiate(PassiveInterferencePrefab);
        instance.transform.position = _lastTarget.currentPos + 
            offset + 
            InterferenceTimeOffset * _lastTarget.MotionVel;
        instance.transform.SetParent(InterferenceFolder, true);
        instance.transform.localScale = Vector3.one;

        var rotation = Random.Range(0, 360f);
        instance.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void StartTest()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        GenerateTargets();
        StartButton.interactable = false;

        _buttonGroupTarget.interactable = true;
        _buttonSingleTarget.interactable = true;
    }

    public void Reset()
    {
        _hasStarted = false;
        if (_lastTarget != null) Destroy(_lastTarget);
        _lastTarget = null;
        Line.transform.localEulerAngles = new Vector3(0, 0, 90);
        StartButton.interactable = true;
        OnReset?.Invoke();

        ResetButtonsColors();
        _buttonGroupTarget.interactable = false;
        _buttonSingleTarget.interactable = false;
        _buttonOthersTarget.interactable = false;
        _buttonOursTarget.interactable = false;

        foreach (var b in InterferenceButtons)
        {
            b.interactable = false;
        }

        Mistakes = 0;
    }

    public void OnRound(int round)
    {
        if (round == 3)
        {
            _buttonOthersTarget.interactable = true;
            _buttonOursTarget.interactable = true;
        }
        if (round == 4)
        {
            GenerateInterference();

            foreach (var b in InterferenceButtons)
            {
                b.interactable = true;
            }
        }
    }

    #region Buttons

    public void Test_GroupSingle(bool isGroup)
    {
        //Debug.Log($"is correct answer: {_lastTarget.IsGroup == isGroup}");
        _buttonSingleTarget.interactable = false;
        _buttonGroupTarget.interactable = false;

        var cols_g = _buttonGroupTarget.colors;
        var cols_s = _buttonSingleTarget.colors;
        if (isGroup)
        {
            cols_g.disabledColor = _lastTarget.IsGroup ?
                _colorDisabledCheckedValid :
                _colorDisabledCheckedInvalid;
            cols_s.disabledColor = _colorDisabledUnchecked;
        }
        else
        {
            cols_g.disabledColor = _colorDisabledUnchecked;
            cols_s.disabledColor = !_lastTarget.IsGroup ?
                _colorDisabledCheckedValid :
                _colorDisabledCheckedInvalid;
        }

        if (isGroup != _lastTarget.IsGroup) Mistakes++;

        _buttonGroupTarget.colors = cols_g;
        _buttonSingleTarget.colors = cols_s;
    }

    public void Test_TheirOurs(bool isOurs)
    {
        //Debug.Log($"is correct answer: {_lastTarget.IsOurs == isOurs}");
        _buttonOthersTarget.interactable = false;
        _buttonOursTarget.interactable = false;

        var cols_o = _buttonOursTarget.colors;
        var cols_t = _buttonOthersTarget.colors;
        if (isOurs)
        {
            cols_o.disabledColor = _lastTarget.IsOurs?
                _colorDisabledCheckedValid :
                _colorDisabledCheckedInvalid;
            cols_t.disabledColor = _colorDisabledUnchecked;
        }
        else
        {
            cols_o.disabledColor = _colorDisabledUnchecked;
            cols_t.disabledColor = !_lastTarget.IsOurs ?
                _colorDisabledCheckedValid :
                _colorDisabledCheckedInvalid;
        }

        if (isOurs != _lastTarget.IsOurs) Mistakes++;

        _buttonOursTarget.colors = cols_o;
        _buttonOthersTarget.colors = cols_t;
    }


    public void Test_Interference(int interferenceType)
    {
        var index = interferenceType - 1;
        var selected = InterferenceButtons[index];

        var col_sel = selected.colors;
        var col_notSel = selected.colors;

        const InterferenceType generated = InterferenceType.Passive;
        if ((InterferenceType)interferenceType == generated)
            col_sel.disabledColor = _colorDisabledCheckedValid;
        else
            col_sel.disabledColor = _colorDisabledCheckedInvalid;

        col_notSel.disabledColor = _colorDisabledUnchecked;

        foreach (var b in InterferenceButtons)
        {
            b.interactable = false;
            b.colors = col_notSel;
        }
        selected.colors = col_sel;

    }

    private void ResetButtonsColors()
    {
        var cols = _buttonOursTarget.colors;
        cols.disabledColor = _colorDisabledUnchecked;

        _buttonGroupTarget.colors = cols;
        _buttonOthersTarget.colors = cols;
        _buttonOursTarget.colors = cols;
        _buttonSingleTarget.colors = cols;

        foreach (var b in InterferenceButtons)
        {
            b.colors = cols;
        }
    }

    #endregion

    #region Interference

    public event UnityAction<float> OnPassiveIntChange;

    private float _passiveIntLevel = 1f;
    public float PassiveInterferenceLevel
    {
        get { return _passiveIntLevel; }
        set
        {
            _passiveIntLevel = value;
            OnPassiveIntChange?.Invoke(value);
        }
    }

    public void TestInt()
    {
        PassiveInterferenceLevel = 0.1f;
    }

    #endregion
}

[System.Serializable]
public enum InterferenceType
{
    None = 0,
    Passive = 1,
    LocalObjects = 2,
    NonlinearImpulse = 3,
    ActiveNoice = 4,
    ResponseImpulse = 5,
    ActiveImitating = 6,
}