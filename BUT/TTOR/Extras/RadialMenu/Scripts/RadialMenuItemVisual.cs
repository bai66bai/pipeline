using UnityEngine;

namespace BUT.TTOR.Core
{
    public class RadialMenuItemVisual : MonoBehaviour
    {
        public float SelectedScaleMultiplier = 1.2f;

        public void IncreaseScale()
        {
            transform.localScale = Vector3.one * SelectedScaleMultiplier;
        }

        public void ResetScale()
        {
            transform.localScale = Vector3.one;
        }
    }
}
