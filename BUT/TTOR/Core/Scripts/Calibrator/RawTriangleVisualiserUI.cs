using TMPro;
using UnityEngine;
using BUT.TTOR.Core;

namespace BUT.TTOR.Calibration
{
    public class RawTriangleVisualiserUI : MonoBehaviour
    {
        public TextMeshProUGUI txtAngle;
        public TextMeshProUGUI txtSurface;
        public TextMeshProUGUI txtPuckInfo;
        public void SetTriangle(Triangle t)
        {
            txtAngle.SetText("" + Mathf.FloorToInt(t.GetKeyAngle() * 100f) / 100f + "°");
            txtSurface.SetText("" + Mathf.FloorToInt(t.GetSurfaceArea() * 100f) / 100f + "cm²");
            txtPuckInfo.SetText("");
            if (t.puck != null && (t.puck.Phase == PuckPhase.Moved || t.puck.Phase == PuckPhase.Created))
            {
                txtPuckInfo.SetText("" + t.puck.Data.name);
            }
            transform.position = t.Center;
        }
    }
}
