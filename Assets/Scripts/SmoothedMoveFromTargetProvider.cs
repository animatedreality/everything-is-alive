using UnityEngine;
using Oculus.Interaction;

public class SmoothedMoveFromTargetProvider : MoveFromTargetProvider
{
    [SerializeField]
    private float _smoothTime = 0.1f;
    public MoveFromTargetProvider _originalProvider;
    private void Awake()
    {
        Debug.Log("Awake is CALLED");
        _originalProvider = GetComponent<MoveFromTargetProvider>();
    }

    public IMovement CreateMovement()
    {
        Debug.Log("CreateMovement is CALLED");
        IMovement originalMovement = _originalProvider.CreateMovement();
        return new SmoothMoveFromTarget(originalMovement, _smoothTime);
    }

    private class SmoothMoveFromTarget : IMovement
    {
        private IMovement _originalMovement;
        private float _smoothTime;
        private Vector3 _currentVelocity;
        private Vector3 _currentAngularVelocity;
        private Pose _currentPose;
        private Pose _targetPose;

        public SmoothMoveFromTarget(IMovement originalMovement, float smoothTime)
        {
            _originalMovement = originalMovement;
            _smoothTime = smoothTime;
            _currentPose = _originalMovement.Pose;
            _targetPose = _currentPose;
        }

        public void UpdateTarget(Pose target)
        {
            Debug.Log("UpdateTarget");
            _originalMovement.UpdateTarget(target);
            _targetPose = target;
        }

        public void Tick()
        {
            Debug.Log("Tick!!");
            _originalMovement.Tick();
            UpdateSmoothedPose();
        }

        private void UpdateSmoothedPose()
        {
            Debug.Log("UpdateSmoothedPose");
            Vector3 smoothedPosition = Vector3.SmoothDamp(_currentPose.position, _targetPose.position, ref _currentVelocity, _smoothTime);
            Quaternion smoothedRotation = QuaternionUtil.SmoothDamp(_currentPose.rotation, _targetPose.rotation, ref _currentAngularVelocity, _smoothTime);
            _currentPose = new Pose(smoothedPosition, smoothedRotation);

            Debug.Log($"Original: {_originalMovement.Pose.position}, Target: {_targetPose.position}, Smoothed: {smoothedPosition}");
        }

        public void MoveTo(Pose target)
        {
            _originalMovement.MoveTo(target);
            _targetPose = target;
        }

        public void StopAndSetPose(Pose pose)
        {
            _originalMovement.StopAndSetPose(pose);
            _currentPose = pose;
            _currentVelocity = Vector3.zero;
            _currentAngularVelocity = Vector3.zero;
            _targetPose = pose;
        }

        public bool Stopped => _originalMovement.Stopped;
        public Pose Pose => _currentPose;
    }
}