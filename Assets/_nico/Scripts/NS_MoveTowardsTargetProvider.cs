using UnityEngine;
using Oculus.Interaction;

public class NS_MoveTowardsTargetProvider : MonoBehaviour, IMovementProvider
{
    [SerializeField]
    private Transform _controllerTransform; // The transform of the controller
    [SerializeField]
    private LayerMask _groundLayer; // Layer mask for the ground
    [SerializeField]
    private float _maxRaycastDistance = 100f; // Maximum distance for the raycast
    [SerializeField]
    private float _dragHeight = 0.1f; // Height above the ground to drag the object
    [SerializeField]
    private PoseTravelData _travellingData = PoseTravelData.FAST;

    private RayInteractor _rayInteractor;
    private RaycastHit _hitInfo;
    private bool _isDragging;
    private IMovement _movement;

    private void Awake()
    {
        _rayInteractor = GetComponent<RayInteractor>();
    }

    private void Update()
    {
        if (_rayInteractor == null || _controllerTransform == null)
        {
            return;
        }

        if (_rayInteractor.HasSelectedInteractable)
        {
            if (!_isDragging)
            {
                StartDragging(_rayInteractor.SelectedInteractable.transform);
            }

            UpdateDragging();
        }
        else if (_isDragging)
        {
            StopDragging();
        }
    }

    private void StartDragging(Transform objectToDrag)
    {
        _movement = CreateMovement();
        _movement.StopAndSetPose(new Pose(objectToDrag.position, objectToDrag.rotation));
        _isDragging = true;
    }

    private void UpdateDragging()
    {
        if (Physics.Raycast(_controllerTransform.position, _controllerTransform.forward, out _hitInfo, _maxRaycastDistance, _groundLayer))
        {
            Vector3 targetPosition = _hitInfo.point;
            targetPosition.y += _dragHeight; // Adjust height above the ground
            Pose targetPose = new Pose(targetPosition, _movement.Pose.rotation);
            _movement.MoveTo(targetPose);
            _movement.Tick();
        }
    }

    private void StopDragging()
    {
        _isDragging = false;
        _movement = null;
    }

    public IMovement CreateMovement()
    {
        return new NS_MoveTowardsTarget(_travellingData, new Vector3(1, 0, 1)); // Constrain to XZ plane
    }
}

public class NS_MoveTowardsTarget : IMovement
{
    private PoseTravelData _travellingData;
    private Vector3 _axisConstraint;
    private Pose _source;
    private Pose _target;
    private Tween _tween;

    public Pose Pose => _tween.Pose;
    public bool Stopped => _tween != null && _tween.Stopped;

    public NS_MoveTowardsTarget(PoseTravelData travellingData, Vector3 axisConstraint)
    {
        _travellingData = travellingData;
        _axisConstraint = axisConstraint;
    }

    public void MoveTo(Pose target)
    {
        _target = ApplyAxisConstraint(target);
        _tween = _travellingData.CreateTween(_source, _target);
    }

    public void UpdateTarget(Pose target)
    {
        _target = ApplyAxisConstraint(target);
        _tween?.UpdateTarget(_target);
    }

    public void StopAndSetPose(Pose source)
    {
        _source = ApplyAxisConstraint(source);
        _tween?.StopAndSetPose(_source);
    }

    public void Tick()
    {
        _tween?.Tick();
    }

    private Pose ApplyAxisConstraint(Pose pose)
    {
        Vector3 constrainedPosition = new Vector3(
            _axisConstraint.x != 0 ? pose.position.x : _source.position.x,
            _axisConstraint.y != 0 ? _source.position.y : pose.position.y, // Keep Y constant
            _axisConstraint.z != 0 ? pose.position.z : _source.position.z
        );

        return new Pose(constrainedPosition, pose.rotation);
    }
}