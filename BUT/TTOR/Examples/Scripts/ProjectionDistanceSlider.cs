using BUT.TTOR.Core;
using TMPro;
using UnityEngine;

namespace BUT.TTOR.Examples
{
    public class ProjectionDistanceSlider : MonoBehaviour
    {
        public TTOR_PuckTracker PuckTracker;
        public TextMeshProUGUI TextMesh;
        
        public void OnSliderValueChanged(float newValue)
        {
            PuckTracker.ProjectionDistance = newValue;
            TextMesh.text = "Projection Distance: " + newValue.ToString("F1");
        }
    }
}
