using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(IkoTarget))]
public class TargetGroupCreator : MonoBehaviour
{
    [Header("Targets generation")]
    [SerializeField] private int MinCountInclusive = 2;
    [SerializeField] private int MaxCountInclusive = 5;
    [SerializeField] private float MinGroupRadius = 0.1f;
    [SerializeField] private float MaxGroupRadius = 0.1f;

    [Header("Targets split")]
    [SerializeField] private int MinCountForSplit = 3;
    [SerializeField] private float SplitOnTurnChance = 0.2f;
    [SerializeField] private int SplittedGroupMinCount = 1;
    [SerializeField] private int SplittedGroupMaxCount = 3;

    [Header("Prefabs")]
    [SerializeField] private TargetRevealer _targetPrefab;
    [SerializeField] private TargetGroupCreator _groupPrefab;

    private IkoTarget _target;
    private List<TargetRevealer> _ikoTargets = new List<TargetRevealer>();

    private void Start()
    {
        _target = GetComponent<IkoTarget>();
        _target.OnTrajectoryTurn.AddListener(OnTurned);
    }

    public void GenerateTargets()
    {
        var count = Random.Range(MinCountInclusive, MaxCountInclusive + 1);
        for (var i = 0; i < count; i++)
        {
            var target = Instantiate(_targetPrefab);
            target.transform.SetParent(transform, true);
            var r = Random.Range(MinGroupRadius, MaxGroupRadius);
            var a = Random.Range(0, 360.0f);
            var pos = Quaternion.Euler(0, 0, a) * new Vector2(0, r);
            target.transform.localPosition = pos;
        }
    }

    private void OnTurned()
    {
        if (_ikoTargets.Count > MinCountForSplit && 
            _ikoTargets.Count > SplittedGroupMinCount && 
            Random.Range(0, 1.0f) <= SplitOnTurnChance)
        {
            var count = Random.Range(
                SplittedGroupMinCount, 
                Mathf.Min(SplittedGroupMaxCount + 1, _ikoTargets.Count)
            );

            var groupSplited = Instantiate(_groupPrefab);
            groupSplited.transform.position = transform.position;
            groupSplited.transform.rotation = transform.rotation;
            groupSplited.transform.SetParent(transform.parent, true);
            groupSplited._target.CopyOther(_target);
            groupSplited._target.TurnTrajectory();

            for (int i = 0; i < count && i <= _ikoTargets.Count; i++)
            {
                var t = _ikoTargets[_ikoTargets.Count - 1];
                groupSplited._ikoTargets.Add(t);
                _ikoTargets.RemoveAt(_ikoTargets.Count - 1);
            }
        }
    }
}
