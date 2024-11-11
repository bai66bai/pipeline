using System;
using System.Collections.Generic;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public class Triangle
    {
        private Touch[] _points = new Touch[3];
        private Edge[] _edges = new Edge[3];

        private Vector2 _forwardVector;

        private float[] _angles = new float[3];
        private float[] _angleDifferences = new float[3];
        private bool[] _equalEdgeDistances = new bool[3];
        private List<int> _pointAcrossIDs = new List<int>();
        private Touch _nonTouch = new Touch();

        private float _cachedSurfaceArea = 0;
        private float _cachedKeyAngle = 0;
        private int _keyPointId = -1;

        public bool HasMissingPoint = false;
        public Puck puck;

        public Touch[] Points => _points;
        public Edge[] Edges => _edges;

        private float _resetTime;
        private Queue<Vector2> _historicPositions = new Queue<Vector2>();

        public Vector2 ForwardVector
        {
            get
            {
                if (_forwardVector == null || _keyPointId == -1) 
                {
                    GetKeyAngle();
                    CalculateForwardVector(_keyPointId);
                    Debug.LogWarning("The forward vector is not calculated, call GetKeyAngle() first before acessing ForwardVector"); 
                }
                else if(_forwardVector == Vector2.zero && _keyPointId != -1)
                {
                    CalculateForwardVector(_keyPointId);
                }
                return _forwardVector;
            }
        }
        public Vector3 Center {
            get {
                return (_points[0].position + _points[1].position + _points[2].position) / 3f;
            }
        }

        public Vector2 Velocity { 
            get {
                Vector2 vel = Vector2.zero;
                Vector2 prevPos = Vector2.zero;

                if (_historicPositions.Count == 0)
                {
                    return Vector2.zero;
                }

                foreach (Vector2 p in _historicPositions)
                {
                    if (prevPos != Vector2.zero)
                    {
                        vel += prevPos - p;
                    }
                    prevPos = p;
                }
                return vel / _historicPositions.Count;
            }
        }

        public float LifeTime { 
            get {
                return Time.time - _resetTime;
            }
        }

        public Triangle()
        {
            _nonTouch.fingerId = -1;
        }

        public Triangle(Touch p1, Touch p2, Touch p3)
        {
            _nonTouch.fingerId = -1;

            _points[0] = p1;
            _points[1] = p2;
            _points[2] = p3;

            UpdateEdges();
        }

        public void OverridePoints(Touch p1, Touch p2, Touch p3)
        {
            //ResetTriangle();
            _forwardVector = Vector2.zero; // Reset the forward vector so it is recalculated

            _points[0] = p1;
            _points[1] = p2;
            _points[2] = p3;

            UpdateEdges();
        }

        public void UpdatePoints(Touch p1, Touch p2, Touch p3)
        {
            HasMissingPoint = false;
            //ResetTriangle();
            _forwardVector = Vector2.zero; // Reset the forward vector so it is recalculated

            for (int i = 0; i < 3; i++)
            {
                if (p1.fingerId == _points[i].fingerId)
                {
                    _points[i] = p1;
                }
                if (p2.fingerId == _points[i].fingerId)
                {
                    _points[i] = p2;
                }
                if (p3.fingerId == _points[i].fingerId)
                {
                    _points[i] = p3;
                }
            }

            _historicPositions.Enqueue(Center);
            if (_historicPositions.Count > 5) // average over 5 frames for a smoothed velocity
            {
                _historicPositions.Dequeue();
            }

            //Debug.Log("velocity: " + Velocity.magnitude);

            if (Velocity.magnitude < 0.1f)
            {
            }
               
            InvalidateCache();

            /*_points[0] = p1;
            _points[1] = p2;
            _points[2] = p3;*/
            UpdateEdges();
        }

        // when only 2 points are still known, update those and calc the new location of the missing point
        public Touch UpdatePoints(Touch p1, Touch p2, ref int missingPointIndex)
        {
            HasMissingPoint = true;
            _forwardVector = Vector2.zero;

            bool foundKeyAnglePoint = false;
            Touch knownNonKeyTouch = _nonTouch;
            for (int i = 0; i < 3; i++)
            {
                bool foundPoint = false;
                if (p1.fingerId == _points[i].fingerId)
                {
                    if (_keyPointId == i) {
                        knownNonKeyTouch = p2;
                    }
                    foundPoint = true;
                    foundKeyAnglePoint = foundKeyAnglePoint || _keyPointId == i;
                    _points[i] = p1;
                }
                if (p2.fingerId == _points[i].fingerId)
                {
                    if (_keyPointId == i)
                    {
                        knownNonKeyTouch = p1;
                    }
                    foundPoint = true;
                    foundKeyAnglePoint = foundKeyAnglePoint || _keyPointId == i;
                    _points[i] = p2;
                }
                if (!foundPoint)
                {
                    missingPointIndex = i;
                }
            }

            bool missingPointIsTheKeyAnglePoint = !foundKeyAnglePoint;


            //Debug.Log("missingPointIndex= " + missingPointIndex + " fingerID:" + Points[missingPointIndex].fingerId);
            if (missingPointIndex > Points.Length - 1 || missingPointIndex < 0) // Catch index out of range
            {
                return _points[0];
            }

            Points[missingPointIndex].fingerId = -1;

            // calculate missing point location when it's the keyAngle
            if (missingPointIsTheKeyAnglePoint)
            {
                // dont use "knownNonKeyTouch" here.. only when missingPointIsTheKeyAnglePoint is false

                float height = (MathF.Tan((180f-_cachedKeyAngle)/2 * 0.0174533f)) * (Vector2.Distance(p1.position, p2.position)/2f);
                Vector2 centerPos = p1.position + ((p2.position - p1.position) * 0.5f);

                // we need to find 2 angles.. the points would change order causing the forward direction to flip.
                // now we calculate what is closest to the previous point to correct for this.
                // we can't just use the previous forward.. we need to calculate a new one based on the position of the known points.
                Vector2 v = Quaternion.AngleAxis(90, Vector3.forward) * (p2.position - p1.position).normalized;
                Vector2 v2 = Quaternion.AngleAxis(-90, Vector3.forward) * (p2.position - p1.position).normalized;

                Vector2 newPos = centerPos + (v * height);
                Vector2 newPos2 = centerPos + (v2 * height);

                if ((_points[missingPointIndex].position - newPos).magnitude < (_points[missingPointIndex].position - newPos2).magnitude)
                {
                    _points[missingPointIndex].position = newPos;
                }
                else
                {
                    _points[missingPointIndex].position = newPos2;
                }

                //_keyPointId = missingPointIndex;
                CalculateForwardVector(missingPointIndex);
                //Debug.Log("missingPointIsTheKeyAnglePoint: " + _points[missingPointIndex].fingerId + " : " + _points[missingPointIndex].position);
            }
            else
            {

                float baseLength = 2 * (MathF.Sin(_cachedKeyAngle / 2f * 0.0174533f) * Vector3.Distance(p1.position, p2.position));
                Vector3 directionBaseAngleToKeyPoint = _points[_keyPointId].position - knownNonKeyTouch.position;
                
                // rotate direction with the baseAngle
                Vector2 v = Quaternion.AngleAxis((180f - _cachedKeyAngle) / 2, Vector3.forward) * directionBaseAngleToKeyPoint.normalized;
                Vector2 v2 = Quaternion.AngleAxis(-(180f - _cachedKeyAngle) / 2, Vector3.forward) * directionBaseAngleToKeyPoint.normalized;
                
                Vector2 newPos = knownNonKeyTouch.position + (v * baseLength);
                Vector2 newPos2 = knownNonKeyTouch.position + (v2 * baseLength);

                if ((_points[missingPointIndex].position - newPos).magnitude < (_points[missingPointIndex].position - newPos2).magnitude)
                {
                    _points[missingPointIndex].position = newPos;
                }
                else
                {
                    _points[missingPointIndex].position = newPos2;
                }

                //Debug.Log("Missing point is a leg point, other leg is still known: " + knownNonKeyTouch.fingerId + " : " + _points[_keyPointId].fingerId);
            }

            UpdateEdges();

            return _points[missingPointIndex];
        }

        public void ResetTriangle()
        {
            _resetTime = Time.time;
            _historicPositions.Clear();
            puck = null;
            HasMissingPoint = false; 
            InvalidateCache();
             _keyPointId = -1;
            InvalidateFingerIds();
            //_forwardVector = Vector2.zero;
        }

        public void InvalidateCache()
        {
            _cachedSurfaceArea = 0;
            _cachedKeyAngle = 0;
        }

        private void UpdateEdges()
        {
            _edges[0] = new Edge(_points[0].position, _points[1].position);
            _edges[1] = new Edge(_points[0].position, _points[2].position);
            _edges[2] = new Edge(_points[1].position, _points[2].position);
        }

        public bool IsIsoseclesTriangle(float lengthTolerance)
        {
            _equalEdgeDistances[0] = IsEqualDistance(_edges[0].GetDistance(), _edges[1].GetDistance(), lengthTolerance);
            _equalEdgeDistances[1] = IsEqualDistance(_edges[0].GetDistance(), _edges[2].GetDistance(), lengthTolerance);
            _equalEdgeDistances[2] = IsEqualDistance(_edges[1].GetDistance(), _edges[2].GetDistance(), lengthTolerance);

            int amountOfEqualDistances = 0;
            for (int i = 0; i < _equalEdgeDistances.Length; i++)
            {
                if (_equalEdgeDistances[i])
                {
                    amountOfEqualDistances++;    
                }
            }

            if(amountOfEqualDistances >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsComparable(Triangle triangle)
        {
            int pointsTheSame = 0;
            foreach (Touch t in triangle.Points)
            {
                if (Points[0].fingerId == t.fingerId || Points[1].fingerId == t.fingerId || Points[2].fingerId == t.fingerId)
                {
                    pointsTheSame++;
                }
            }

            // we want at least 2 points to be the same
            return pointsTheSame >= 2;
        }

        public bool IsComparable(Touch[] touches)
        {
            int pointsTheSame = 0;
            foreach (Touch t in touches)
            {
                if (Points[0].fingerId == t.fingerId || Points[1].fingerId == t.fingerId || Points[2].fingerId == t.fingerId)
                {
                    pointsTheSame++;
                }
            }

            // we want at least 2 points to be the same
            return pointsTheSame >= 2;
        }

        public bool ContainsTouch(Touch touch)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                // todo: add sensibility check! distance can't jump to high distance?
                // what if the finderId changes, because it gets lifted real quick, if the position is very similar, keep it also
                //bool similarPoint = (_points[i].position - touch.position).magnitude < 40;
                /*if (similarPoint && _points[i].fingerId != touch.fingerId)
                {
                    Debug.Log("fingerID's don't match, but position indicates this is the same point");
                    _points[i].fingerId = touch.fingerId;
                }*/

                if (_points[i].fingerId == touch.fingerId)  //  || similarPoint
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsEqualDistance(float distance1, float distance2, float tolerance)
        {
            float avgLength = (distance1 + distance2) / 2;
            float diff = Mathf.Abs(distance1 - distance2);
            //Debug.Log("dif: " + (diff / avgLength));
            return (diff / avgLength) <= tolerance;
        }

        public float GetSurfaceArea()
        {
            if(_cachedSurfaceArea != 0)
            {
                return _cachedSurfaceArea;
            }

            Vector3 Cross = Vector3.Cross((_points[0].position - _points[1].position), (_points[0].position - _points[2].position));
            float area = Mathf.Abs(Cross.z) * 0.5f;
            area = (area / (TTOR_PuckTracker.ScreenDPI * TTOR_PuckTracker.ScreenDPI)) * 6.4516f; //Why convert it? Let's not? //convert to square cm (1 square inch is 6.4516 square cm.
            _cachedSurfaceArea = area;

            return area;
        }

        public float GetKeyAngle()
        {
            if (_cachedKeyAngle != 0)
            {
                return _cachedKeyAngle;
            }

            _angles[0] = Vector2.Angle((_points[1].position - _points[0].position), (_points[2].position - _points[0].position));
            _angles[1] = Vector2.Angle((_points[0].position - _points[1].position), (_points[2].position - _points[1].position));
            _angles[2] = Vector2.Angle((_points[0].position - _points[2].position), (_points[1].position - _points[2].position));

            _angleDifferences[0] = Mathf.Abs(_angles[0] - _angles[1]);
            _angleDifferences[1] = Mathf.Abs(_angles[0] - _angles[2]);
            _angleDifferences[2] = Mathf.Abs(_angles[1] - _angles[2]);

            float smallestDifference = float.MaxValue;
            int smallestDifferenceId = -1;
            for (int i = 0; i < _angleDifferences.Length; i++)
            {
                if(_angleDifferences[i] < smallestDifference)
                {
                    smallestDifference = _angleDifferences[i];
                    smallestDifferenceId = i;
                }
            }

            switch (smallestDifferenceId)
            {
                case 0: // Smallest difference is between angle 0 and 1, so that makes angle 2 the most unique
                    _keyPointId = 2;
                    _cachedKeyAngle = _angles[2];
                    //CalculateForwardVector(2);
                    return _angles[2];
                case 1: // Smallest difference is between angle 0 and 2, so that makes angle 1 the most unique
                    _keyPointId = 1;
                    _cachedKeyAngle = _angles[1];
                    //CalculateForwardVector(1);
                    return _angles[1];
                case 2: // Smallest difference is between angle 1 and 2, so that makes angle 0 the most unique
                    _keyPointId = 0;
                    _cachedKeyAngle = _angles[0];
                    //CalculateForwardVector(0);
                    return _angles[0];
            }
            return 0;
        }

        private void CalculateForwardVector(int keyPointID)
        {
            _pointAcrossIDs.Clear();
            for (int i = 0; i < _points.Length; i++)
            {
                if(i != keyPointID)
                {
                    _pointAcrossIDs.Add(i);
                }
            }

            Vector2 centerPointAcross = (_points[_pointAcrossIDs[0]].position + _points[_pointAcrossIDs[1]].position) * 0.5f;
            _forwardVector = (_points[keyPointID].position - centerPointAcross).normalized;
        }

        public void InvalidateFingerIds()
        {
            Points[0].fingerId = -1;
            Points[1].fingerId = -1;
            Points[2].fingerId = -1;
        }
    }

    public struct Edge
    {
        public Vector2 P1;
        public Vector2 P2;

        public Edge(Vector2 p1, Vector2 p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public float GetDistance()
        {
            return (P2 - P1).magnitude;
        }
    }
}

