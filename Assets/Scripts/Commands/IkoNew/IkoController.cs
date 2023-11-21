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
    public float InterferenceDissolveTime;
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
    private GameObject NipInterferencePrefab;
    [SerializeField]
    private float _passiveIntRadius;

    [Header("Test buttons")]
    [SerializeField]
    private List<Button> singleTargetButtons;
    [SerializeField]
    private List<Button> groupTargetButtons;
    [SerializeField]
    private List<Button> oursTargetButtons;
    [SerializeField]
    private List<Button> othersTargetButtons;
    [SerializeField]
    private List<Button> InterferenceButtons1;
    [SerializeField]
    private List<Button> InterferenceButtons2;
    [SerializeField]
    private List<Button> InterferenceButtons3;
    [SerializeField]
    private List<Button> InterferenceButtons4;

    [Header("Button Colors")]
    [SerializeField]
    private Color _colorDisabledChecked;
    [SerializeField]
    private Color _colorDisabledUnchecked;
    [SerializeField]
    private Color _colorDisabledCheckedValid;
    [SerializeField]
    private Color _colorDisabledCheckedInvalid;


    private const float _defaultBrightness = 0.5f;
    private List<IkoTarget> targets = new List<IkoTarget>();
    private float[] interferencesDistToCenter = new float[4];
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
    private int _reqiredAnswers = 12;


    List<bool?> isGroupAnswers = new List<bool?>();
    List<bool?> isOursAnswers = new List<bool?>();
    List<InterferenceType> interferenceAnswers = new List<InterferenceType>();
    List<InterferenceType> interferenceGeneratedTypes = new List<InterferenceType>();


    private int Answers
    {
        get => _currentAnswers;
        set
        {
            _currentAnswers = value;
            if (_currentAnswers >= _reqiredAnswers)
            {
                // group-single
                for (int i = 0; i < singleTargetButtons.Count; ++i)
                {
                    var singleTargetButton = singleTargetButtons[i];
                    var groupTargetButton = groupTargetButtons[i];
                    var cols_s = singleTargetButton.colors;
                    var cols_g = groupTargetButton.colors;
                    bool? isGroupAnswer = isGroupAnswers[i];
                    if (isGroupAnswer.Value)
                    {
                        cols_g.disabledColor = targets[i].IsGroup ?
                            _colorDisabledCheckedValid :
                            _colorDisabledCheckedInvalid;
                        cols_s.disabledColor = _colorDisabledUnchecked;
                    }
                    else
                    {
                        cols_g.disabledColor = _colorDisabledUnchecked;
                        cols_s.disabledColor = !targets[i].IsGroup ?
                            _colorDisabledCheckedValid :
                            _colorDisabledCheckedInvalid;
                    }
                    singleTargetButton.colors = cols_s;
                    groupTargetButton.colors = cols_g;
                }

                // ours-others
                for (int i = 0; i < oursTargetButtons.Count; ++i)
                {
                    var oursTargetButton = oursTargetButtons[i];
                    var othersTargetButton = othersTargetButtons[i];
                    var cols_o = oursTargetButton.colors;
                    var cols_t = othersTargetButton.colors;
                    bool? isOursAnswer = isOursAnswers[i];
                    if (isOursAnswer.Value)
                    {
                        cols_o.disabledColor = targets[i].IsOurs ?
                            _colorDisabledCheckedValid :
                            _colorDisabledCheckedInvalid;
                        cols_t.disabledColor = _colorDisabledUnchecked;
                    }
                    else
                    {
                        cols_o.disabledColor = _colorDisabledUnchecked;
                        cols_t.disabledColor = !targets[i].IsOurs ?
                            _colorDisabledCheckedValid :
                            _colorDisabledCheckedInvalid;
                    }
                    oursTargetButton.colors = cols_o;
                    othersTargetButton.colors = cols_t;
                }

                // interference type
                for (int i = 0; i < interferenceAnswers.Count; ++i)
                {
                    var interferenceTypeIndex = (int)interferenceAnswers[i] - 1;
                    List<Button> interferenceButtons;
                    if (i == 0)
                    {
                        interferenceButtons = InterferenceButtons1;
                    }
                    else if (i == 1)
                    {
                        interferenceButtons = InterferenceButtons2;
                    }
                    else if (i == 2)
                    {
                        interferenceButtons = InterferenceButtons3;
                    }
                    else
                    {
                        interferenceButtons = InterferenceButtons4;
                    }
                    var selected = interferenceButtons[interferenceTypeIndex];
                    var col_sel = selected.colors;
                    if (interferenceAnswers[i] == interferenceGeneratedTypes[i])
                        col_sel.disabledColor = _colorDisabledCheckedValid;
                    else
                        col_sel.disabledColor = _colorDisabledCheckedInvalid;

                    selected.colors = col_sel;
                }

                if (Mistakes < _maxMistakes)
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
    [Range(0, 1)]
    private float _strobTolerance;
    [SerializeField]
    private float _interferenceFadeDist;
    [SerializeField]
    private float _minInterferenceBrightness;
    [SerializeField]
    private ActionIkoCheck _strobCheckAction;

    private float _ikoRadius;
    private float _interferenceDistToCenter;

    private bool _isStrobValid = false;

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
        for (int i = 0; i < 4; ++i)
        {
            generateTarget(i);
        }
        Debug.Log(targets.Count);
    }

    private void generateTarget(int targetIdx)
    {
        var dist = Random.Range(MinDistanceToTarget, MaxDistanceToTarget);
        float angle;
        if (targetIdx == 0) {
            angle = Random.Range(30, 60);
        } else if (targetIdx == 1) {
            angle = Random.Range(120, 150);
        } else if (targetIdx == 2) {
            angle = Random.Range(210, 240);
        } else {
            angle = Random.Range(300, 330);
        }
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

        Debug.Log("target with idx " + targetIdx + ": isOurs = " + isOur + " isGroup = " + isGroup);

        instance.transform.SetParent(TargetsFolder, false);
        instance.transform.localScale = Vector3.one;

        targets.Add(instance);
    }

    public void StartTest()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        GenerateTargets();
        StartButton.interactable = false;

        foreach (var button in singleTargetButtons)
        {
            button.interactable = true;
        }
        foreach (var button in groupTargetButtons)
        {
            button.interactable = true;
        }
    }

    public void Restart()
    {
        _hasStarted = false;
        foreach (var target in targets)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }
        targets = new List<IkoTarget>();
        LineObject.transform.localEulerAngles = new Vector3(0, 0, 90);
        StartButton.interactable = true;
        OnReset?.Invoke();

        WorkMode = IkoWorkMode.Rpm6;

        ResetButtonsColors();
        SetInteractableFalseForButtons();

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
        for (int i = 0; i < 4; ++i)
        {
            interferencesDistToCenter[i] = 0;
        }
        isGroupAnswers = new List<bool?>();
        for (int i = 0; i < singleTargetButtons.Count; ++i)
        {
            isGroupAnswers.Add(null);
        }
        isOursAnswers = new List<bool?>();
        for (int i = 0; i < singleTargetButtons.Count; ++i)
        {
            isOursAnswers.Add(null);
        }
        interferenceAnswers = new List<InterferenceType>();
        for (int i = 0; i < singleTargetButtons.Count; ++i)
        {
            interferenceAnswers.Add(InterferenceType.None);
        }
        interferenceGeneratedTypes = new List<InterferenceType>();
        for (int i = 0; i < singleTargetButtons.Count; ++i)
        {
            interferenceGeneratedTypes.Add(InterferenceType.None);
        }
    }

    public void OnRound(int round)
    {
        switch (round)
        {
            case 3:
                foreach (var button in oursTargetButtons)
                {
                    button.interactable = true;
                }
                foreach (var button in othersTargetButtons)
                {
                    button.interactable = true;
                }
                break;
            case 4:
                GenerateInterferences();
                break;
            case 5:
                List<List<Button>> allInterferenceButtons = new List<List<Button>>{
                    InterferenceButtons1, InterferenceButtons2, InterferenceButtons3, InterferenceButtons4};
                foreach (var buttons in allInterferenceButtons)
                {
                    foreach (var b in buttons)
                    {
                        b.interactable = true;
                    }
                }
                break;
            default:
                break;
        }
    }

    #region Buttons

    public void Test_GroupSingle1(bool isGroup)
    {
        testGroupSingleTarget(isGroup, 0);
    }

    public void Test_GroupSingle2(bool isGroup)
    {
        testGroupSingleTarget(isGroup, 1);
    }

    public void Test_GroupSingle3(bool isGroup)
    {
        testGroupSingleTarget(isGroup, 2);
    }

    public void Test_GroupSingle4(bool isGroup)
    {
        testGroupSingleTarget(isGroup, 3);
    }

    private void testGroupSingleTarget(bool isGroup, int targetIdx)
    {
        Debug.Log("checking target number " + targetIdx + ". isGroup=" + isGroup);
        Button singleTargetButton = singleTargetButtons[targetIdx];
        Button groupTargetButton = groupTargetButtons[targetIdx];
        singleTargetButton.interactable = false;
        groupTargetButton.interactable = false;
        var cols_s = singleTargetButton.colors;
        var cols_g = groupTargetButton.colors;
        if (isGroup)
        {
            cols_g.disabledColor = _colorDisabledChecked;
            cols_s.disabledColor = _colorDisabledUnchecked;
        }
        else
        {
            cols_g.disabledColor = _colorDisabledUnchecked;
            cols_s.disabledColor = _colorDisabledChecked;
        }

        if (isGroup != targets[targetIdx].IsGroup) Mistakes++;

        groupTargetButton.colors = cols_g;
        singleTargetButton.colors = cols_s;

        isGroupAnswers[targetIdx] = isGroup;
        Answers++;
    }

    public void Test_TheirOurs1(bool isOurs)
    {
        TestTheirOursTarget(isOurs, 0);
    }

    public void Test_TheirOurs2(bool isOurs)
    {
        TestTheirOursTarget(isOurs, 1);
    }

    public void Test_TheirOurs3(bool isOurs)
    {
        TestTheirOursTarget(isOurs, 2);
    }

    public void Test_TheirOurs4(bool isOurs)
    {
        TestTheirOursTarget(isOurs, 3);
    }

    public void TestTheirOursTarget(bool isOurs, int targetIdx)
    {
        Debug.Log("checking target number " + targetIdx + ". isOurs=" + isOurs);
        Button oursTargetButton = oursTargetButtons[targetIdx];
        Button othersTargetButton = othersTargetButtons[targetIdx];
        oursTargetButton.interactable = false;
        othersTargetButton.interactable = false;

        var cols_o = oursTargetButton.colors;
        var cols_t = othersTargetButton.colors;
        if (isOurs)
        {
            cols_o.disabledColor = _colorDisabledChecked;
            cols_t.disabledColor = _colorDisabledUnchecked;
        }
        else
        {
            cols_o.disabledColor = _colorDisabledUnchecked;
            cols_t.disabledColor = _colorDisabledChecked;
        }

        if (isOurs != targets[targetIdx].IsOurs) Mistakes++;

        oursTargetButton.colors = cols_o;
        othersTargetButton.colors = cols_t;

        isOursAnswers[targetIdx] = isOurs;
        Answers++;
    }

    public void Test_Interference1(int interferenceType)
    {
        Test_Interference(interferenceType, 1);
    }

    public void Test_Interference2(int interferenceType)
    {
        Test_Interference(interferenceType, 2);
    }

    public void Test_Interference3(int interferenceType)
    {
        Test_Interference(interferenceType, 3);
    }

    public void Test_Interference4(int interferenceType)
    {
        Test_Interference(interferenceType, 4);
    }

    public void Test_Interference(int interferenceType, int targetNumber)
    {
        Debug.Log("testing interference. targetNumber = " + targetNumber + " type = " + interferenceType);
        var interferenceButtonIdx = interferenceType - 1;
        List<Button> buttons;
        if (targetNumber == 1)
        {
            buttons = InterferenceButtons1;
        }
        else if (targetNumber == 2)
        {
            buttons = InterferenceButtons2;
        }
        else if (targetNumber == 3)
        {
            buttons = InterferenceButtons3;
        }
        else
        {
            buttons = InterferenceButtons4;
        }
        var selected = buttons[interferenceButtonIdx];

        var col_sel = selected.colors;
        var col_notSel = selected.colors;

        col_sel.disabledColor = _colorDisabledChecked;
        col_notSel.disabledColor = _colorDisabledUnchecked;

        foreach (var b in buttons)
        {
            b.interactable = false;
            b.colors = col_notSel;
        }
        selected.colors = col_sel;
        var interferenceAnswer = (InterferenceType)interferenceType;
        interferenceAnswers[targetNumber - 1] = interferenceAnswer;

        if (interferenceAnswer != interferenceGeneratedTypes[targetNumber - 1]) {
            Mistakes++;
            Debug.Log("wrong interference type, right = " + interferenceGeneratedTypes[targetNumber - 1]);
        }
        Answers++;
    }

    private void ResetButtonsColors()
    {
        var cols = oursTargetButtons[0].colors;
        cols.disabledColor = _colorDisabledUnchecked;

        List<List<Button>> allTargetButtons = new List<List<Button>>{
                    singleTargetButtons, groupTargetButtons, oursTargetButtons, othersTargetButtons};

        foreach (var buttons in allTargetButtons)
        {
            foreach (var b in buttons)
            {
                b.colors = cols;
            }
        }

        List<List<Button>> allInterferenceButtons = new List<List<Button>>{
                    InterferenceButtons1, InterferenceButtons2, InterferenceButtons3, InterferenceButtons4};
        foreach (var buttons in allInterferenceButtons)
        {
            foreach (var b in buttons)
            {
                b.colors = cols;
            }
        }

    }

    private void SetInteractableFalseForButtons()
    {
        List<List<Button>> allTargetButtons = new List<List<Button>>{
                    singleTargetButtons, groupTargetButtons, oursTargetButtons, othersTargetButtons};
        foreach (var buttons in allTargetButtons)
        {
            foreach (var b in buttons)
            {
                b.interactable = false;
            }
        }

        List<List<Button>> allInterferenceButtons = new List<List<Button>>{
                    InterferenceButtons1, InterferenceButtons2, InterferenceButtons3, InterferenceButtons4};
        foreach (var buttons in allInterferenceButtons)
        {
            foreach (var b in buttons)
            {
                b.interactable = false;
            }
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

    public void GenerateInterferences()
    {
        for (int i = 0; i < 4; ++i)
        {
            GenerateInterference(i);
        }
        Debug.Log("Interferences count = " + interferenceGeneratedTypes.Count);
    }

    public void GenerateInterference(int targetIdx)
    {
        var target = targets[targetIdx];
        var offset = target.MotionVel;
        offset.Normalize();
        offset *= _passiveIntRadius;
        var instance = Instantiate(getRandomInterferencePrefab(targetIdx));
        instance.transform.position = target.CurrentPos +
            offset +
            InterferenceTimeOffset * target.MotionVel;
        instance.transform.SetParent(InterferenceFolder, true);
        instance.transform.localScale = Vector3.one;

        var rotation = Random.Range(0, 360f);
        instance.transform.rotation = Quaternion.Euler(0, 0, rotation);

        _interferenceDistToCenter = ((Vector2)instance.transform.position -
            (Vector2)LineObject.transform.position)
            .magnitude;
        interferencesDistToCenter[targetIdx] = _interferenceDistToCenter;
    }

    private GameObject getRandomInterferencePrefab(int targetIdx) {
        List<InterferenceType> enabledTypes = new List<InterferenceType>{
            InterferenceType.Passive, InterferenceType.NonlinearImpulse};
        var randomIdx = (int) Random.Range(0, enabledTypes.Count);
        var randomInterferenceType = enabledTypes[randomIdx];
        interferenceGeneratedTypes[targetIdx] = randomInterferenceType;

        if (randomInterferenceType == InterferenceType.Passive) {
            Debug.Log("Generating passive interference for target with idx " + targetIdx);
            return PassiveInterferencePrefab;
        }
        Debug.Log("Generating NIP interference for target with idx " + targetIdx);
        return NipInterferencePrefab;
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


        if (!_isStrobValid && (alpha <= _minInterferenceBrightness + _strobTolerance))
        {
            _isStrobValid = true;
            GameManager.Instance.AddToState(_strobCheckAction);
        }
        else if (_isStrobValid && (alpha > _minInterferenceBrightness + _strobTolerance))
        {
            _isStrobValid = false;
            GameManager.Instance.RemoveFromState(_strobCheckAction);
        }
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
