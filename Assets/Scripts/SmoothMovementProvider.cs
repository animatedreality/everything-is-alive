using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class SmoothMovementProvider : MonoBehaviour, IMovementProvider
{
    [SerializeField]
    private float _smoothTime = 0.1f;

    private IMovementProvider _originalProvider;

    public void Initialize(IMovementProvider originalProvider)
    {
        _originalProvider = originalProvider;
    }

    public IMovement CreateMovement()
    {
        return new SmoothMovement(_originalProvider.CreateMovement(), _smoothTime);
    }
}

public class SmoothMovement : IMovement
{
    private IMovement _originalMovement;
    private float _smoothTime;
    private Vector3 _currentVelocity;
    private Vector3 _currentAngularVelocity;

    public Pose Pose => _smoothPose;
    public bool Stopped => _originalMovement.Stopped;

    private Pose _smoothPose;

    public SmoothMovement(IMovement originalMovement, float smoothTime)
    {
        _originalMovement = originalMovement;
        _smoothTime = smoothTime;
        _smoothPose = originalMovement.Pose;
    }

    public void MoveTo(Pose target)
    {
        _originalMovement.MoveTo(target);
    }

    public void UpdateTarget(Pose target)
    {
        _originalMovement.UpdateTarget(target);
    }

    public void StopAndSetPose(Pose pose)
    {
        _originalMovement.StopAndSetPose(pose);
        _smoothPose = pose;
        _currentVelocity = Vector3.zero;
        _currentAngularVelocity = Vector3.zero;
    }

    public void Tick()
    {
        _originalMovement.Tick();
        Pose targetPose = _originalMovement.Pose;

        _smoothPose.position = Vector3.SmoothDamp(_smoothPose.position, targetPose.position, ref _currentVelocity, _smoothTime);
        _smoothPose.rotation = QuaternionUtil.SmoothDamp(_smoothPose.rotation, targetPose.rotation, ref _currentAngularVelocity, _smoothTime);
    }
}

public static class QuaternionUtil
{
    public static Quaternion SmoothDamp(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }
}