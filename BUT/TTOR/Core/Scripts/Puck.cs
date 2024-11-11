using System;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public enum PuckPhase
    {
        Staged,
        Created,
        Moved,
        Lost,
        FoundAgain,
        Removed
    }

    [Serializable]
    public class Puck
    {
        public PuckPhase Phase;

        public PuckData Data;
        public Triangle Triangle;
        private TTOR_PuckTracker _puckTracker;

        // when was my triangle last updated
        public float lastSeen = 0;

        // when was my keyangle and surface last checked for being valid
        public float lastChecked = 0;

        private Vector3 _lerpedForward;
        private bool _lerpedForwardOnceSet = false;

        public TTOR_PuckTracker PuckTracker { get => _puckTracker; }

        public Puck(TTOR_PuckTracker puckTracker)
        {
            lastSeen = Time.time;
            lastChecked = Time.time;
            _puckTracker = puckTracker;

        }

        public Vector3 GetPosition(float distanceFromCamera)
        {
            Vector2 v = Quaternion.AngleAxis(Data.ForwardDirectionOffset, Vector3.forward) * Triangle.ForwardVector;
            Quaternion rot = Quaternion.LookRotation(v, Vector3.forward) * Quaternion.Euler(-90, 0, 180);

            Vector2 offset = rot * Data.CenterOffset.ToVector2();
            //Debug.Log("offset: " + Data.CenterOffset.ToVector2() + " : "+ offset);
            Vector3 p = (Vector2)Triangle.Center - offset;
            p.z = distanceFromCamera;
            return _puckTracker.TargetCamera.ScreenToWorldPoint(p);
        }

        public Vector2 GetScreenPosition()
        {
            Vector3 p = Triangle.Center;
            return p;
        }

        public Quaternion GetRotation()
        {
            Vector2 v = Quaternion.AngleAxis(Data.ForwardDirectionOffset, Vector3.forward) * Triangle.ForwardVector;
            Quaternion rot = Quaternion.LookRotation(v, Vector3.forward) * Quaternion.Euler(-90, 0, 180);
            rot *= _puckTracker.TargetCamera.transform.rotation;
            return rot;
        }


        internal Quaternion GetLerpedRotation(float t = 7.5f)
        {
            if (Triangle != null && !_lerpedForwardOnceSet)
            {
                _lerpedForward = Triangle.ForwardVector;
                _lerpedForwardOnceSet = true;
            }
            _lerpedForward = Vector3.LerpUnclamped(_lerpedForward, Triangle.ForwardVector, t);
            Vector2 v = Quaternion.AngleAxis(Data.ForwardDirectionOffset, Vector3.forward) * _lerpedForward;

            Quaternion rot = Quaternion.LookRotation(v, Vector3.forward) * Quaternion.Euler(-90, 0, 180);
            rot *= _puckTracker.TargetCamera.transform.rotation;

            return rot;

        }
    }
}
