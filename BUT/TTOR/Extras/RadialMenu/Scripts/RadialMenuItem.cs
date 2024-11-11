using UnityEngine;
using UnityEngine.Events;

namespace BUT.TTOR.Core
{
    public class RadialMenuItem : MonoBehaviour
    {
        public Transform VisualsTransform;
        public bool KeepUpright = true;

        private Transform _transform;
        public Transform Transform => _transform;

        private bool _isSelected = false;
        public bool IsSelected => _isSelected;

        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;

        private void Awake()
        {
            _transform = transform;
        }

        public void Select()
        {
            _isSelected = true;
            OnSelected?.Invoke();
        }

        public void Deselect()
        {
            _isSelected = false;
            OnDeselected?.Invoke();
        }

        private void LateUpdate()
        {
            if (KeepUpright) { _transform.eulerAngles = Vector3.zero; }
        }
    }
}
