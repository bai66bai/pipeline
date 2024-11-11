using UnityEngine;

namespace BUT.TTOR.Core
{
    [RequireComponent(typeof(RadialLayoutGroup))]
    [RequireComponent(typeof(TTOR_AdvancedPuckFollower))]
    public class SetRotationLimits : MonoBehaviour
    {
        private Transform _transform;
        private RadialLayoutGroup _radialLayoutGroup;
        private TTOR_AdvancedPuckFollower _puckFollower;

        private int _childCount;

        private float _startAngle = 0;
        private float _maxAngle = 0;
        private float _currentAngle;

        public float CurrentAngle  => _currentAngle;
        public float StartAngle => _startAngle;

        private void Awake()
        {
            _transform = transform;
            _radialLayoutGroup = GetComponent<RadialLayoutGroup>();
            _childCount = _transform.childCount;
            _puckFollower = GetComponent<TTOR_AdvancedPuckFollower>();

            _startAngle = _transform.localRotation.z;

            CalculateMaxAngle();
        }

        private void CalculateMaxAngle()
        {
            _maxAngle = _startAngle + (_radialLayoutGroup.SpacingInDegrees * (_transform.childCount - 1) * (_radialLayoutGroup.InvertDirection ? -1 : 1));

            _puckFollower.KeepInitialRotation = true;
            _puckFollower.LimitRotation = true;

            if (!_radialLayoutGroup.InvertDirection)
            {
                _puckFollower.MaxRotation = 0;
                _puckFollower.MinRotation = -Mathf.Abs(_maxAngle);
            }
            else
            {
                _puckFollower.MinRotation = 0;
                _puckFollower.MaxRotation = Mathf.Abs(_maxAngle);
            }
        }

        private void LateUpdate()
        {
            if (_childCount != _transform.childCount)
            {
                CalculateMaxAngle();
                _childCount = _transform.childCount;
            }
        }
    }
}
