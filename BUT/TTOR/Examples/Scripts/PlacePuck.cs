using BUT.TTOR.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BUT.TTOR.Examples
{
    public class PlacePuck : MonoBehaviour
    {
        public TMP_Text PlacePuckText;
        public float MinScalePulse = 3f;
        public float MaxScalePulse = 4f;
        public float SpeedPulse = 0.7f;

        private List<Puck> _activePucks = new List<Puck>();

        private Image _pulseRing;

        void Start()
        {
            _pulseRing = GetComponent<Image>();
        }
        public void OnPuckCreated(Puck puck)
        {
            if (_activePucks.Count > 0)
            {
                return;
            }
            else
            {
                PlacePuckText.text = "";
                _activePucks.Add(puck);
                CheckForActivePucks();
            }  
        }

        public void OnPuckRemoved(Puck puck)
        {
            if (_activePucks.Remove(puck))
            {
                PlacePuckText.text = "Place your puck";
                CheckForActivePucks();
            }
        }

        public void CheckForActivePucks()
        {
            if(_activePucks.Count == 0)
            {
                EnablePulseRing();
            } else
            {
                DisablePulseRing();
            }
        }

        public void DisablePulseRing()
        {
            _pulseRing.enabled = false;
        }

        public void EnablePulseRing()
        {
            _pulseRing.enabled = true;
        }

        void Update()
        {
            if (_pulseRing.enabled == true)
            {
                float scaleFactor = Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * SpeedPulse, 1));

                float scale = Mathf.Lerp(MinScalePulse, MaxScalePulse, scaleFactor);
                transform.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}
