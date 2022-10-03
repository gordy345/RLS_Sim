using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IkoTarget : MonoBehaviour
{
    public Vector2 MotionVel;
    public Vector2 StartPos;

    private float _lineLen;
    private Vector2 _lineStartPos;

    private bool _isOurs;
    public bool IsOurs
    {
        get => _isOurs;
        set
        {
            _isOurs = value;
            foreach (var t in GetComponentsInChildren<TargetRevealer>())
            {
                t.IsOurs = value;
            }
        }
    }
    public bool IsGroup { get; set; }

    public Vector2 CurrentPos { get; private set; }

    public UnityEvent OnTrajectoryTurn = new UnityEvent();

    private void Start()
    {
        transform.position = StartPos;
        _lineLen = Random.Range(
            IkoController.Instance.MinTargetLineLength,
            IkoController.Instance.MaxTargetLineLength
        );
        _lineStartPos = StartPos;
    }

    private void FixedUpdate()
    {
        Vector2 newPos;
        newPos.x = transform.position.x;
        newPos.y = transform.position.y;
        CurrentPos = newPos + MotionVel * Time.deltaTime;
        transform.position = CurrentPos;

        if ((CurrentPos - _lineStartPos).magnitude >= _lineLen)
        {
            OnTrajectoryTurn?.Invoke();
            TurnTrajectory();
        }
    }

    public void CopyOther(IkoTarget target)
    {
        MotionVel = target.MotionVel;
        StartPos = target.StartPos;
        _lineLen = target._lineLen;
        _lineStartPos = target._lineStartPos;
        CurrentPos = target.CurrentPos;

        IsOurs = target.IsOurs;
        IsGroup = target.IsGroup;
    }

    public void TurnTrajectory()
    {
        _lineStartPos = CurrentPos;
        var angle = Random.Range(
            -IkoController.Instance._maxTargetAngleDeviation,
            +IkoController.Instance._maxTargetAngleDeviation
        );
        MotionVel = Quaternion.Euler(0, 0, angle) * MotionVel;
    }
}
