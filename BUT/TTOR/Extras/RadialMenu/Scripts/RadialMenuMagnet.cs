using UnityEngine;

namespace BUT.TTOR.Core
{
    public class RadialMenuMagnet : MonoBehaviour
    {
        public float LerpSpeed = 7.5f;
        public float Radius = 10;
        [Range(0, 1)]
        public float AttractorStrength;
        public float RepulsorStrength;

        private Transform _transform;
        private RadialMenu _radialMenu;

        void Start()
        {
            _transform = transform;
            _radialMenu = GetComponentInParent<RadialMenu>();
        }

        void LateUpdate()
        {
            if (!_radialMenu) { return; }

            foreach (RadialMenuItem menuItem in _radialMenu.MenuItems)
            {
                Vector2 wantedPosition = menuItem.Transform.position;

                if (menuItem.IsSelected)
                {
                    wantedPosition = _transform.position + ((menuItem.Transform.position - _transform.position) * AttractorStrength);
                }
                else
                {
                    Vector3 dir = (menuItem.Transform.position - _transform.position).normalized;
                    float distanceStrengthModifier = 1 - (Mathf.Clamp(Vector2.Distance(menuItem.Transform.position, _transform.position), 0, Radius) / Radius);
                    wantedPosition = menuItem.Transform.position + (dir * RepulsorStrength * distanceStrengthModifier);
                }

                menuItem.VisualsTransform.position = Vector3.Lerp(menuItem.VisualsTransform.position, wantedPosition, LerpSpeed * Time.deltaTime);
            }
           
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
