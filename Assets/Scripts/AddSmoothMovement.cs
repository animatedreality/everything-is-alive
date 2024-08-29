using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class AddSmoothMovement : MonoBehaviour, IMovementProvider
{
    public Transform GrabInteractable;
    DistanceHandGrabInteractable distanceHandGrabInteractable;
    DistanceGrabInteractable distanceGrabInteractable;
    HandGrabInteractable handGrabInteractable;

    [SerializeField]
    private float _smoothTime = 0.1f;

    private IMovementProvider _originalProvider;

    void Start()
    {
        distanceHandGrabInteractable = GrabInteractable.GetComponent<DistanceHandGrabInteractable>();
        distanceGrabInteractable = GrabInteractable.GetComponent<DistanceGrabInteractable>();
        handGrabInteractable = GrabInteractable.GetComponent<HandGrabInteractable>();
        if(!distanceHandGrabInteractable)
            Debug.LogError("No DistanceHandGrabInteractable found on the GrabInteractable GameObject.");
        if(!distanceGrabInteractable)
            Debug.LogError("No DistanceGrabInteractable found on the GrabInteractable GameObject.");
        if(!handGrabInteractable)
            Debug.LogError("No HandGrabInteractable found on the GrabInteractable GameObject.");
        InitializeSmoothMovement();
    }

    private void InitializeSmoothMovement()
    {
        if (distanceHandGrabInteractable != null)
        {
            IMovementProvider movementProvider = distanceHandGrabInteractable.GetComponent<IMovementProvider>();
            Initialize(movementProvider);
            distanceHandGrabInteractable.InjectOptionalMovementProvider(this);
        }

        if (distanceGrabInteractable != null)
        {
            IMovementProvider movementProvider = distanceGrabInteractable.GetComponent<IMovementProvider>();
            Initialize(movementProvider);
            distanceGrabInteractable.InjectOptionalMovementProvider(this);
        }

        if (handGrabInteractable != null)
        {
            IMovementProvider movementProvider = handGrabInteractable.GetComponent<IMovementProvider>();
            Initialize(movementProvider);
            handGrabInteractable.InjectOptionalMovementProvider(this);
        }
    }

    public void Initialize(IMovementProvider originalProvider)
    {
        _originalProvider = originalProvider;
    }

    public IMovement CreateMovement()
    {
        return new SmoothMovement(_originalProvider.CreateMovement(), _smoothTime);
    }

    private class SmoothMovement : IMovement
    {
        private IMovement _originalMovement;
        private float _smoothTime;
        private Vector3 _currentVelocity;
        private Vector3 _currentAngularVelocity;

        public Pose Pose => _smoothPose;
        public bool Stopped => _originalMovement.Stopped;

        private Pose _smoothPose;
        private Pose _targetPose;

        public SmoothMovement(IMovement originalMovement, float smoothTime)
        {
            _originalMovement = originalMovement;
            _smoothTime = smoothTime;
            _smoothPose = originalMovement.Pose;
            _targetPose = _smoothPose;
        }

        public void MoveTo(Pose target)
        {
            _originalMovement.MoveTo(target);
            _targetPose = target;
        }

        public void UpdateTarget(Pose target)
        {
            _originalMovement.UpdateTarget(target);
            _targetPose = target;
        }

        public void StopAndSetPose(Pose pose)
        {
            _originalMovement.StopAndSetPose(pose);
            _smoothPose = pose;
            _targetPose = pose;
            _currentVelocity = Vector3.zero;
            _currentAngularVelocity = Vector3.zero;
        }

        public void Tick()
        {
            _originalMovement.Tick();
            _smoothPose.position = Vector3.SmoothDamp(_smoothPose.position, _targetPose.position, ref _currentVelocity, _smoothTime);
            _smoothPose.rotation = QuaternionUtil.SmoothDamp(_smoothPose.rotation, _targetPose.rotation, ref _currentAngularVelocity, _smoothTime);
        }
    }

    private static class QuaternionUtil
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
}