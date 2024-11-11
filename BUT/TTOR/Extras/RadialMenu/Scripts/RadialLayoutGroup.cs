using UnityEngine;
using UnityEngine.UI;

namespace BUT.TTOR.Core
{
    public class RadialLayoutGroup : LayoutGroup
    {
        [Header("Display Config")]
        public float Radius = 100;
        public float SpacingInDegrees = 45;
        public float StartingAngle = -90;
        public bool InvertDirection = true;

        protected override void OnEnable() 
        { 
            base.OnEnable(); 
        }
        public override void SetLayoutHorizontal()
        {
        }
        public override void SetLayoutVertical()
        {
        }
        public override void CalculateLayoutInputVertical()
        {
            CalculateRadial();
        }
        public override void CalculateLayoutInputHorizontal()
        {
            CalculateRadial();
        }
    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
    #endif

        private void CalculateRadial()
        {
            m_Tracker.Clear();
            if (transform.childCount == 0) { return; }

            float angleDelta = (InvertDirection ? -1 : 1) * SpacingInDegrees;
            float angle = StartingAngle;

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = (RectTransform)transform.GetChild(i);

                if (child != null)
                {
                    m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);

                    float xPos = Mathf.Sin(Mathf.Deg2Rad * angle);
                    float yPos = Mathf.Cos(Mathf.Deg2Rad * angle);

                    Vector2 childPos = new Vector2(xPos, yPos) * Radius;
                    child.localPosition = childPos;
                    child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);

                    angle += angleDelta;
                }
            }
        }
    }
}
