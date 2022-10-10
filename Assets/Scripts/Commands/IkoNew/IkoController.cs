using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IkoController : MonoBehaviour
{
    [Header("Display Objects")]
    [SerializeField]
    private CanvasGroup Grid;
    [SerializeField]
    private GameObject LineObject;
    [SerializeField]
    private GameObject EdgeObject;
    [SerializeField]
    public Transform TargetsFolder;
    [SerializeField]
    private CanvasGroup _ikoPanel;
    [SerializeField]
    private Transform InterferenceFolder;

    [Header("Controls objects")]
    [SerializeField]
    private Scrollbar BrightnessController;
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button Rpm6_Btn;
    [SerializeField]
    private Button Rpm12_Btn;

    [Header("Settings")]
    [SerializeField]
    private float LineRotationSpeed_6rpm = -36f;
    [SerializeField]
    private float LineRotationSpeed_12rpm = -72f;
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

    #region Work mode

    [System.Serializable]
    public enum IkoWorkMode
    {
        Rpm6,
        Rpm12,
    }

    private IkoWorkMode _mode;
    public IkoWorkMode WorkMode 
    {
        get => _mode;
        set
        {
            _mode = value;
            switch (value)
            {
                case IkoWorkMode.Rpm12:
                    Rpm12_Btn.interactable = false;
                    Rpm6_Btn.interactable = true;
                    break;
                case IkoWorkMode.Rpm6:
                default:
                    Rpm12_Btn.interactable = true;
                    Rpm6_Btn.interactable = false;
                    break;
            }
        }
    }

    private float LineRotationSpeed
    {
        get
        {
            switch (WorkMode)
            {
                case IkoWorkMode.Rpm12:
                    return LineRotationSpeed_12rpm;
                case IkoWorkMode.Rpm6:
                default:
                    return LineRotationSpeed_6rpm;
            }
        }
    }

    #endregion

    public float MinTargetLineLength;
    public float MaxTargetLineLength;
    public float _maxTargetAngleDeviation;

    [Header("Target Prefabs")]
    [SerializeField]
    private IkoTarget SingleTargetPrefab;
    [SerializeField]
    private IkoTarget GroupTargetPrefab;

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
            {
                _hasStarted = false;
                GameManager.Instance.FailCheck();
            }
        }
    }
    private int _currentAnswers = 0;
    private int _reqiredAnswers = 3;
    private int Answers
    {
        get => _currentAnswers;
        set
        {
            _currentAnswers = value;
            if (_currentAnswers >= _reqiredAnswers && Mistakes < _maxMistakes)
            {
                //CloseIko();
                GameManager.Instance.AddToState(CheckPoint);
            }
        }
    }
    [SerializeField]
    private ActionIkoCheck CheckPoint = null;

    [Header("Strob control")]
    [SerializeField]
    private GameObject _strobContainer;
    [SerializeField]
    private Slider _strobSlider;
    [SerializeField]
    private float _strobTolerance;
    [SerializeField]
    private float _interferenceFadeDist;
    [SerializeField]
    private float _minInterferenceBrightness;
    [SerializeField]
    private ActionIkoCheck _strobCheckAction;

    private float _ikoRadius;

    private float _interferenceDistToCenter;

    public static Vector3 IkoCenter => Instance?.LineObject.transform.position ?? Vector3.zero;

    public static IkoController Instance { get; private set; }

    public event UnityAction OnReset;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WorkMode = IkoWorkMode.Rpm6;
        _ikoRadius = ((Vector2)LineObject.transform.position - 
            (Vector2)EdgeObject.transform.position)
            .magnitude;

        BrightnessController.onValueChanged.AddListener(BrightnessChanged);
        BrightnessController.value = _defaultBrightness;

        gameObject.SetActive(true);
        CloseIko();

        _strobSlider.onValueChanged.AddListener(OnStrobStartValueChange);

        Rpm6_Btn.onClick.AddListener(() => WorkMode = IkoWorkMode.Rpm6);
        Rpm12_Btn.onClick.AddListener(() => WorkMode = IkoWorkMode.Rpm12);

        Restart();
    }

    void Update()
    {
        if (!_hasStarted) return;
        var angles = LineObject.transform.localEulerAngles;

        var lastAngle = angles.z;
        angles.z += LineRotationSpeed * Time.deltaTime;
        LineObject.transform.localEulerAngles = angles;
    }

    public void BrightnessChanged(float value)
    {
        Grid.alpha = value;
    }

    public void OpenIko()
    {
        _ikoPanel.alpha = 1.0f;
        _ikoPanel.interactable = true;
        _ikoPanel.blocksRaycasts = true;
    }

    public void CloseIko()
    {
        _ikoPanel.alpha = 0.0f;
        _ikoPanel.interactable = false;
        _ikoPanel.blocksRaycasts = false;
    }

    public void GenerateTargets()
    {
        var dist = Random.Range(MinDistanceToTarget, MaxDistanceToTarget);
        var angle = Random.Range(0, 360f);
        var pos = Quaternion.Euler(0, 0, angle) * 
            new Vector2(0, dist) + 
            LineObject.transform.position;

        var velAmp = Random.Range(MinTargetVel, MaxTargetVel);
        var td = TargetsDirectionSpreadAngle / 2;
        var angleVel = Quaternion.Euler(
            0,
            0,
            Vector2.SignedAngle(
                Vector2.up,
                LineObject.transform.position - pos
            ) + Random.Range(-td, +td)
        );

        var vel = angleVel * new Vector2(0, velAmp);

        IkoTarget instance;

        var isGroup = Random.Range(0f, 10f) <= 5;
        var isOur = Random.Range(0f, 10f) <= 5;


        if (isGroup)
        {
            instance = Instantiate(GroupTargetPrefab);
            instance.GetComponent<TargetGroupCreator>().GenerateTargets();
        }
        else
        {
            instance = Instantiate(SingleTargetPrefab);
        }

        instance.IsGroup = isGroup;
        instance.IsOurs = isOur;
        instance.StartPos = pos;
        instance.MotionVel = vel;

        instance.transform.SetParent(TargetsFolder, false);

        instance.transform.localScale = Vector3.one;

        _lastTarget = instance;
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

    public void Restart()
    {
        _hasStarted = false;
        if (_lastTarget != null) Destroy(_lastTarget);
        _lastTarget = null;
        LineObject.transform.localEulerAngles = new Vector3(0, 0, 90);
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

        foreach (Transform obj in InterferenceFolder)
        {
            Destroy(obj.gameObject);
        }

        void DestroyRecursive(Transform t)
        {
            foreach (Transform obj in t)
            {
                DestroyRecursive(obj);
            }
            Destroy(t.gameObject);
        }

        foreach (Transform obj in TargetsFolder)
        {
            DestroyRecursive(obj);
        }

        Mistakes = 0;
        Answers = 0;
        _strobSlider.value = 0;
        PassiveInterferenceLevel = 1f;
        _strobContainer.SetActive(false);
        _interferenceDistToCenter = 0;
        
    }

    public void OnRound(int round)
    {
        switch(round) {
            case 3:
                _buttonOthersTarget.interactable = true;
                _buttonOursTarget.interactable = true;
                break;
            case 4:
                GenerateInterference();
                break;
            case 5:
                foreach (var b in InterferenceButtons)
                {
                    b.interactable = true;
                }
                break;
            default:
                break;
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
        Answers++;
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
        Answers++;
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

        Answers++;
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

    public void GenerateInterference()
    {
        var offset = _lastTarget.MotionVel;
        offset.Normalize();
        offset *= _passiveIntRadius;
        var instance = Instantiate(PassiveInterferencePrefab);
        instance.transform.position = _lastTarget.CurrentPos +
            offset +
            InterferenceTimeOffset * _lastTarget.MotionVel;
        instance.transform.SetParent(InterferenceFolder, true);
        instance.transform.localScale = Vector3.one;

        var rotation = Random.Range(0, 360f);
        instance.transform.rotation = Quaternion.Euler(0, 0, rotation);

        _interferenceDistToCenter = ((Vector2)instance.transform.position -
            (Vector2)LineObject.transform.position)
            .magnitude;
    }

    #endregion

    #region strob actions

    public void EnableStrobControl()
    {
        _strobContainer.SetActive(true);
    }

    public void DisableStrobControl()
    {
        _strobContainer.SetActive(false);
    }

    private void OnStrobStartValueChange(float value)
    {
        var pos = Mathf.Lerp(0, _ikoRadius, value);

        var dist = Mathf.Abs(_interferenceDistToCenter - pos);
        var lerpVal = Mathf.Clamp01(
            Mathf.InverseLerp(_interferenceFadeDist * _ikoRadius, 0, dist)
        );
        var alpha = Mathf.Lerp(1, _minInterferenceBrightness, lerpVal);
        PassiveInterferenceLevel = alpha;
        //Debug.Log($"strob start changed: {value}, new alpha: {alpha} (lerpVal: {lerpVal})");
    }

    public void ValidateStrob()
    {
        if (PassiveInterferenceLevel <= _minInterferenceBrightness + _strobTolerance)
        {
            GameManager.Instance.AddToState(_strobCheckAction);
        }
        GameManager.Instance.CheckOrder();
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
