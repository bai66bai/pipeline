using BUT.TTOR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace BUT.TTOR
{
    public class PuckInfo : MonoBehaviour, TTOR_IPuckContent
    {
        [Header("References")]
        public GameObject RotationPoint;
        public TMP_Text PuckInfoText;

        [Header("Settings")]
        public bool ShowDebugging = false;

        private Puck _activePuck;

        public void AssignPuck(Puck puck)
        {
            _activePuck = puck;
        }

        private void Update()
        {
            if (_activePuck != null && ShowDebugging == false) 
            {
                PuckInfoText.text = 
                    "Name: " + "<color=#EF1F79>" + _activePuck.Data.name + "</color>" + "<br>"
                    + "Velocity " + _activePuck.Triangle.Velocity + "<br>"
                    + "Rotation: " + _activePuck.GetRotation().eulerAngles.z.ToString("F2") + "° <br>";
            } 
            else if(_activePuck != null && ShowDebugging == true)
            {
                float differenceAngle = _activePuck.Data.KeyAngle - _activePuck.Triangle.GetKeyAngle();
                float differenceArea = _activePuck.Data.SurfaceArea - _activePuck.Triangle.GetSurfaceArea();

                PuckInfoText.text = 
                    "Name: " + "<color=#EF1F79>" + _activePuck.Data.name + "</color>" + "<br>"
                    + "Surface area: " + _activePuck.Data.SurfaceArea.ToString("F2") + " cm² +- " + _activePuck.Triangle.GetSurfaceArea().ToString("F2") + " cm² = " + differenceArea.ToString("F2") + " cm²" + "<br>"
                    + "Velocity " + _activePuck.Triangle.Velocity + "<br>"
                    + "Rotation: " + _activePuck.GetRotation().eulerAngles.z.ToString("F2") + "° <br>" 
                    + "Angle: " + _activePuck.Data.KeyAngle.ToString("F2") + "° +- " + _activePuck.Triangle.GetKeyAngle().ToString("F2") + "° = " + differenceAngle.ToString("F2") + "°";                              
            }
        }

        private void LateUpdate()
        {
            RotationPoint.transform.rotation = Quaternion.identity;
        }

        public void RemovePuck(Puck puck)
        {
            _activePuck = null;   
        }
    }
}
