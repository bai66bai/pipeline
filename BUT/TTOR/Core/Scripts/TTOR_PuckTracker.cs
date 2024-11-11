using BUT.TTOR.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BUT.TTOR.Core
{
    public class TTOR_PuckTracker : MonoBehaviour
    {
        private List<Puck> _pucks = new List<Puck>();

        private static List<Touch> _currentTouchPoints = new List<Touch>();

        private List<Triangle> _currentTriangles = new List<Triangle>();

        private PuckData[] _puckData;

        private int _initialTrianglePoolSize;
        private Stack<Triangle> _trianglePool = new Stack<Triangle>();

        [Tooltip("The camera used to position the pucks in worldspace.")]
        public Camera TargetCamera;

        [Tooltip("The default distance from the camera used to position graphics that follow pucks.")]
        public float ProjectionDistance = 1;


        [Space(10)]
        [Header("Tolerances")]

        [Tooltip("The maximum amount of touches that are considererd for finding Pucks")]
        public int MaxTouchCount = 20;

        [Tooltip("[Percentage difference] The length 2 edges of a triangle can differ before they're no longer considered the same.")]
        public float IsoseclesTriangleTolerance = 0.1f;

        [Tooltip("[degrees] The amount of degrees an angle can be larger or smaller to be considered the same as the pucks.")]
        public float AngleTolerance = 5f;

        [Tooltip("[cm²] The amount of square cm a triangle can be larger or smaller to be considered the same as the pucks.")]
        public float SurfaceTolerance = 10f;

        [Tooltip("[seconds] The time a puck needs to be present before it's created.")]
        public float StagingTime = 0.2f;

        [Tooltip("[seconds] The time a puck is allowed to be lost before it's removed")]
        public float LostPuckAllowedRecoveryTime = 1f;

        [Tooltip("[pixels] The distance a point can be from another point to be considered the same point.")]
        public float SamePointTolerance = 40f; // pixelspace

        [Tooltip("The maximum number of pucks that can be on the screen at the same time.")]
        public int MaxSimultaniousPucks = 12;

        [Space(10)]
        [Header("Events")]

        public UnityEvent<List<Puck>> OnPucksUpdatedEvent;


        [Space(10)]
        [Header("Other")]
        public TTOR_Logger.LogMask LogFilter;

        public static float ScreenDPI = 96;

        public List<Puck> Pucks { get => _pucks; }

        [Space(10)]
        [Header("Debug")]

        [Tooltip("a list of isoSelec triangles before filtering on valid pucks. (used for debugging)")]
        public List<Triangle> RawTriangles = new List<Triangle>();

        private void Awake()
        {
            TTOR_Logger.LogFilter = LogFilter;

            ScreenDPI = Screen.dpi;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            ScreenDPI = WindowsMonitorUtils.GetRawDPIForCurrentWindow();
            TTOR_Logger.Log("Raw DPI for the current window: " + ScreenDPI, TTOR_Logger.LogMask.Debug);
#endif


            if (!TargetCamera)
            {
                TargetCamera = Camera.main;
            }

            _puckData = PuckDataManager.GetPuckData();
        }

        private void OnValidate()
        {
            TTOR_Logger.LogFilter = LogFilter;
        }

        private void Start()
        {
            SetupTrianglePool();
        }

        private void OnApplicationQuit()
        {
            // we don't want our ScriptableOIbjects to have angles.. the calibration saves are the only truth.
            PuckData[] allPuckData = Resources.LoadAll<PuckData>("");
            for (int i = 0; i < allPuckData.Length; i++)
            {
                allPuckData[i].KeyAngle = 0;
                allPuckData[i].SurfaceArea = 0;
                allPuckData[i].ForwardDirectionOffset = 0;
                allPuckData[i].CenterOffset = Vector2.zero;
                allPuckData[i].IsCalibrated = false;
            }
        }

        private void SetupTrianglePool()
        {
            _initialTrianglePoolSize = (MaxTouchCount * (MaxTouchCount - 1) * (MaxTouchCount - 2)) / 6;
            for (int i = 0; i < _initialTrianglePoolSize; ++i)
            {
                _trianglePool.Push(new Triangle());
            }
            //Debug.Log("_initialTrianglePoolSize: " + _trianglePool.Count);
        }

        private Triangle GetTriangleFromPool()
        {
            //Debug.Log("GET triangle from pool: " + _trianglePool.Count);

            while (_trianglePool.Count > 0)
            {
                Triangle triangle = _trianglePool.Pop();
                triangle.ResetTriangle();

                if (triangle != null)
                {
                    return triangle;
                }
                else
                {
                    TTOR_Logger.LogWarning("Found a null object in the pool. Has some code outside the pool destroyed it?");
                }
            }

            TTOR_Logger.LogWarning("Pool is empty, extending capacity by creating new Triangle");
            return new Triangle();
        }

        private void ReturnTriangleToPool(Triangle triangle)
        {
            if (triangle != null)
            {
                triangle.ResetTriangle();
                _trianglePool.Push(triangle);
                //Debug.Log("Returning triangle to pool: " + _trianglePool.Count + ":  " + triangle.Points[0].fingerId + " :" + triangle.Points[1].fingerId + " :" + triangle.Points[2].fingerId);
            }
        }

        private void Update()
        {
            // 获取屏幕上所有触点
            GetAllTouches();

            // 移除所有结束的pucks（移除所有消失了很长时间都没再出现的pucks）
            RemoveEndedPucks();
            // 更新所有存在的Puck的位置等信息
            UpdateExistingPucks();
            // 寻找有没有和触点匹配的新puck
            FindPucks();
            // 更新所有puck的状态
            SetPuckTouchPhase();

            // 触发所有puck身上被挂载的所有Update事件
            OnPucksUpdatedEvent?.Invoke(_pucks);
        }

        private void GetAllTouches()
        {
            _currentTouchPoints.Clear();
            int touchCount = Input.touchCount;

            for (int i = 0; i < touchCount; i++)
            {
                if (i > MaxTouchCount - 1) { return; }
                _currentTouchPoints.Add(Input.GetTouch(i));
            }

            SimulatedTouch[] _simulatedTouches = GetComponentsInChildren<SimulatedTouch>();
            for (int y = 0; y < _simulatedTouches.Length; y++)
            {
                _currentTouchPoints.Add(_simulatedTouches[y].GetTouch());
            }
        }

        private void RemoveEndedPucks()
        {
            for (int i = 0; i < _pucks.Count; i++)
            {
                Puck p = _pucks[i];

                if (p.Phase == PuckPhase.FoundAgain)
                {
                    p.Phase = PuckPhase.Moved;
                }

                if (p.Phase == PuckPhase.Removed)
                {
                    ReturnTriangleToPool(_pucks[i].Triangle);
                    _currentTriangles.Remove(_pucks[i].Triangle);

                    _pucks.Remove(p);
                    TTOR_Logger.Log("<color=red><b>Puck Removed: </b></color>" + p.Data.name, TTOR_Logger.LogMask.PuckPhaseChanges);
                }
            }
        }

        private void UpdateExistingPucks()
        {
            for (int i = 0; i < _pucks.Count; i++)
            {
                int pointIndex = 0;
                Touch[] puckTrianglePointsToUpdate = new Touch[3];

                for (int j = 0; j < puckTrianglePointsToUpdate.Length; j++)
                {
                    puckTrianglePointsToUpdate[j].fingerId = -j - 2;
                }


                for (int j = 0; j < _currentTouchPoints.Count; j++)
                {
                    if (_pucks[i].Triangle.ContainsTouch(_currentTouchPoints[j]))
                    {
                        puckTrianglePointsToUpdate[pointIndex] = _currentTouchPoints[j];
                        pointIndex++;
                    }
                }



                // Introduce 1 ghost touch here if only 1 point is not found?
                if (pointIndex == 2) // 2 existing points found
                {
                    // find the other 2 correct points
                    int missingPointIndex = -1;

                    // this method updates the positions when 2 points are still the same fingerId.
                    Touch missingTouch = _pucks[i].Triangle.UpdatePoints(puckTrianglePointsToUpdate[0], puckTrianglePointsToUpdate[1], ref missingPointIndex);

                    // at this time the 3 points of this pucks' triangle are in the new position

                    // try to find a new touchpoint that has almost the same location as the old missing one
                    if (_currentTouchPoints.Count > 2 && missingPointIndex != -1)
                    {
                        // find the closest distnace to our missingTouch point in the list of touches
                        _currentTouchPoints.Sort((t, y) => (t.position - missingTouch.position).magnitude.CompareTo((y.position - missingTouch.position).magnitude));
                        float dist = (_currentTouchPoints[0].position - missingTouch.position).magnitude;
                        if (dist < SamePointTolerance)
                        {
                            puckTrianglePointsToUpdate[2] = _currentTouchPoints[0];

                            // if you find such a new point add it as puckTrianglePointsToUpdate[2] and change pointIndex to 3
                            _pucks[i].Triangle.Points[missingPointIndex] = _currentTouchPoints[0];
                            _pucks[i].Triangle.UpdatePoints(puckTrianglePointsToUpdate[0], puckTrianglePointsToUpdate[1], puckTrianglePointsToUpdate[2]);
                        }
                    }
                }

                if (pointIndex == 3) // 3 existing points found
                {
                    _pucks[i].Triangle.UpdatePoints(puckTrianglePointsToUpdate[0], puckTrianglePointsToUpdate[1], puckTrianglePointsToUpdate[2]);
                }


                if (pointIndex >= 2 && _pucks[i].Phase != PuckPhase.Lost && _pucks[i].Phase != PuckPhase.Removed)
                {
                    if (_pucks[i].Phase != PuckPhase.Staged)
                    {
                        _pucks[i].Phase = PuckPhase.Moved;
                    }

                    if (PuckStillValid(_pucks[i]))
                    {
                        _pucks[i].lastSeen = Time.time;
                    }

                    for (int k = 0; k < puckTrianglePointsToUpdate.Length; k++)
                    {
                        _currentTouchPoints.Remove(puckTrianglePointsToUpdate[k]);
                    }
                }

            }
        }

        private bool PuckStillValid(Puck puck)
        {
            puck.lastChecked = Time.time;

            if (!puck.Triangle.IsIsoseclesTriangle(IsoseclesTriangleTolerance))
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed IsIsoseclesTriangle test", TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }

            if (puck.Triangle.GetSurfaceArea() > (puck.Data.SurfaceArea + SurfaceTolerance))
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed surface area test, larger than the tolerance allows: " + puck.Triangle.GetSurfaceArea() + " > " + (puck.Data.SurfaceArea + SurfaceTolerance), TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }

            if (puck.Triangle.GetSurfaceArea() < (puck.Data.SurfaceArea - SurfaceTolerance))
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed surface area test, smaller than the tolerance allows: " + puck.Triangle.GetSurfaceArea() + " < " + (puck.Data.SurfaceArea - SurfaceTolerance), TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }

            if (puck.Triangle.GetKeyAngle() > puck.Data.KeyAngle + AngleTolerance)
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed keyAngle test, larger than the tolerance allows: " + puck.Triangle.GetKeyAngle() + " > " + (puck.Data.KeyAngle + AngleTolerance), TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }

            if (puck.Triangle.GetKeyAngle() < puck.Data.KeyAngle - AngleTolerance)
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed keyAngle test, smaller than the tolerance allows: " + puck.Triangle.GetKeyAngle() + " < " + (puck.Data.KeyAngle - AngleTolerance), TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }

            // check if there isn't another puckData that is actually closer to the current values.

            float closestAngleDifference = float.MaxValue;
            PuckData closestPuckDataForAngle = null;

            foreach (var puckData in _puckData)
            {
                float angleDiff = Math.Abs(puckData.KeyAngle - puck.Triangle.GetKeyAngle());

                if (angleDiff < closestAngleDifference)
                {
                    closestPuckDataForAngle = puckData;
                    closestAngleDifference = angleDiff;
                }
            }

            if (closestPuckDataForAngle != puck.Data)
            {
                TTOR_Logger.Log("Puck " + puck.Data.name + " failed isClosestPuckDataForThisAngle test, another puckData (" + closestPuckDataForAngle.name + ") has a keyAngle that is closer to the one currently measured.", TTOR_Logger.LogMask.ReasonsPuckIsLost);
                return false;
            }


            return true;
        }

        /*寻找所有触点匹配的所有Puck*/
        private void FindPucks()
        {
            // not lost pucks count
            List<Puck> allNotLostPucks = _pucks.FindAll((p) => p.Phase != PuckPhase.Lost);
            if (allNotLostPucks.Count >= MaxSimultaniousPucks)
            {
                return;
            }

            // together with the detected Pucks and the _currentTriangles we can export all Isosecles triangles that are being detected before filtering on calibration data
            // this list could be usefull for debugging and calibration purposes
            RawTriangles.Clear();
            _pucks.ForEach((p) =>
            {
                RawTriangles.Add(p.Triangle);
            });

            if (_currentTouchPoints.Count < 3)
            {
                return;
            }

            GetAllPossibleTriangles();
            FilterIsoseclesTriangles();

            RawTriangles.AddRange(_currentTriangles);


            FilterOnKeyAngleAndSurface();


            if (_currentTriangles.Count <= 0) { return; }


            foreach (Triangle triangle in _currentTriangles)
            {
                float closestAngleAndSurfaceDifference = float.MaxValue;
                PuckData closestPuckData = null;

                foreach (var puckData in _puckData)
                {
                    float angleDiff = Math.Abs(puckData.KeyAngle - triangle.GetKeyAngle());
                    float surfaceDiff = Math.Abs(puckData.SurfaceArea - triangle.GetSurfaceArea());

                    float delta = angleDiff + surfaceDiff;

                    if (delta < closestAngleAndSurfaceDifference)
                    {
                        closestPuckData = puckData;
                        closestAngleAndSurfaceDifference = delta;
                    }
                }

                if (closestPuckData != null)
                {
                    if ((triangle.GetKeyAngle() < closestPuckData.KeyAngle + AngleTolerance && triangle.GetKeyAngle() > closestPuckData.KeyAngle - AngleTolerance))
                    {
                        // check the lost pucks first
                        Puck lostPuck = FindLostPuckWithMatchingDataAndFuzzyPosition(closestPuckData, triangle);
                        if (lostPuck != null)
                        {
                            TTOR_Logger.Log("<color=lime><b>Lost Puck is found again: </b></color>" + closestPuckData.name, TTOR_Logger.LogMask.PuckPhaseChanges);
                            ReturnTriangleToPool(lostPuck.Triangle);
                            lostPuck.Triangle = triangle;
                            lostPuck.Triangle.puck = lostPuck;
                            lostPuck.Phase = PuckPhase.FoundAgain;
                            lostPuck.Triangle.GetKeyAngle();
                            lostPuck.lastSeen = Time.time;
                        }
                        else
                        {
                            if (_pucks.Count < MaxSimultaniousPucks)
                            {
                                TTOR_Logger.Log("<color=orange><b>New Puck Staged: </b></color>" + closestPuckData.name, TTOR_Logger.LogMask.PuckPhaseChanges);
                                Puck puck = new Puck(this);
                                puck.Triangle = triangle;
                                puck.Triangle.puck = puck;
                                puck.Data = closestPuckData;
                                puck.Phase = PuckPhase.Staged;
                                puck.Triangle.GetKeyAngle();
                                _pucks.Add(puck);
                            }
                        }

                        _currentTouchPoints.Remove(triangle.Points[0]);
                        _currentTouchPoints.Remove(triangle.Points[1]);
                        _currentTouchPoints.Remove(triangle.Points[2]);

                        FindPucks();

                        return;
                    }
                }
            }
        }

        private Puck FindLostPuckWithMatchingDataAndFuzzyPosition(PuckData puckData, Triangle closestTriangle)
        {
            List<Puck> possibleReFoundPucks = _pucks.FindAll((p) => p.Data == puckData && p.Phase == PuckPhase.Lost);
            possibleReFoundPucks.Sort((x, y) => ((x.Triangle.Center - closestTriangle.Center).magnitude).CompareTo((y.Triangle.Center - closestTriangle.Center).magnitude));

            if (possibleReFoundPucks.Count == 0)
            {
                return null;
            }

            return possibleReFoundPucks[0];
        }

        private void GetAllPossibleTriangles()
        {

            // Don't return a triangle to the pool if a puck uses it
            for (int i = 0; i < _currentTriangles.Count; i++)
            {
                bool returnTriangleToPool = true;
                for (int j = 0; j < _pucks.Count; j++)
                {
                    if (_pucks[j].Triangle == _currentTriangles[i])
                    {
                        returnTriangleToPool = false;
                        break;
                    }
                }
                if (returnTriangleToPool) { ReturnTriangleToPool(_currentTriangles[i]); }
            }


            _currentTriangles.Clear();

            IEnumerable<Touch[]> touchCombinations = _currentTouchPoints.Combinations(3);
            foreach (Touch[] combination in touchCombinations)
            {
                // Don't take a triangle from the pool if a puck uses it
                bool combinationUsedInPuck = false;
                for (int i = 0; i < _pucks.Count; i++)
                {
                    if (_pucks[i].Triangle.IsComparable(combination))
                    {
                        combinationUsedInPuck = true;
                        break;
                    }
                }
                if (!combinationUsedInPuck)
                {
                    Triangle triangle = GetTriangleFromPool();
                    triangle.OverridePoints(combination[0], combination[1], combination[2]);
                    _currentTriangles.Add(triangle);
                }
            }
        }

        private void FilterIsoseclesTriangles()
        {
            for (int i = 0; i < _currentTriangles.Count; i++)
            {
                if (!_currentTriangles[i].IsIsoseclesTriangle(IsoseclesTriangleTolerance))
                {
                    ReturnTriangleToPool(_currentTriangles[i]);
                    _currentTriangles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterOnKeyAngleAndSurface()
        {
            bool filterTriangle;
            for (int i = 0; i < _currentTriangles.Count; i++)
            {
                filterTriangle = true;
                float keyAngle = _currentTriangles[i].GetKeyAngle();

                for (int j = 0; j < _puckData.Length; j++)
                {
                    if (keyAngle < _puckData[j].KeyAngle + AngleTolerance &&
                    keyAngle > _puckData[j].KeyAngle - AngleTolerance)
                    {
                        float surfaceArea = _currentTriangles[i].GetSurfaceArea();
                        if (surfaceArea < _puckData[j].SurfaceArea + SurfaceTolerance &&
                    surfaceArea > _puckData[j].SurfaceArea - SurfaceTolerance)
                        {
                            filterTriangle = false;
                            break;
                        }
                    }
                }
                if (filterTriangle)
                {
                    ReturnTriangleToPool(_currentTriangles[i]);
                    _currentTriangles.RemoveAt(i);
                    i--;
                }
            }
        }

        private void SetPuckTouchPhase()
        {
            List<Puck> pucksToRemove = new List<Puck>();

            for (int i = 0; i < _pucks.Count; i++)
            {
                Puck puck = _pucks[i];

                // lost it for x seconds
                float timePuckWasNotSeenAnymore = Time.time - puck.lastSeen;
                if (timePuckWasNotSeenAnymore > Time.deltaTime * 4)
                {
                    if (puck.Phase == PuckPhase.Staged)
                    {
                        pucksToRemove.Add(puck);
                    }
                    else
                    {
                        if (puck.Phase != PuckPhase.Lost && puck.Phase != PuckPhase.Removed)
                        {
                            TTOR_Logger.Log("<color=red><b>Puck is lost: </b></color>" + puck.Data.name, TTOR_Logger.LogMask.PuckPhaseChanges);
                            puck.Triangle.InvalidateFingerIds();

                            puck.Phase = PuckPhase.Lost;
                        }
                    }

                }

                if (puck.Phase == PuckPhase.Lost &&
                    Time.time - puck.lastSeen > LostPuckAllowedRecoveryTime) // lost it for x seconds, now remove
                {
                    puck.Phase = PuckPhase.Removed;
                }

                if (puck.Phase == PuckPhase.Staged &&
                    puck.Triangle.LifeTime > StagingTime) // puck existed for longer than STAGING_TIMEOUT, create it
                {
                    TTOR_Logger.Log("<color=lime><b>New Puck Created: </b></color>" + puck.Data.name, TTOR_Logger.LogMask.PuckPhaseChanges);
                    puck.Phase = PuckPhase.Created;
                }
            }

            pucksToRemove.ForEach((puck) =>
            {
                puck.Triangle.InvalidateFingerIds();
                puck.Phase = PuckPhase.Removed;
                _currentTriangles.Remove(puck.Triangle);
                ReturnTriangleToPool(puck.Triangle);
                _pucks.Remove(puck);
                TTOR_Logger.Log("<color=orange><b>Staged Puck is Removed: </b></color>" + puck.Data.name, TTOR_Logger.LogMask.PuckPhaseChanges);
            });
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < _pucks.Count; i++)
            {
                Gizmos.color = Color.white;
                for (int j = 0; j < _pucks[i].Triangle.Edges.Length; j++)
                {
                    Gizmos.DrawLine(TargetCamera.ScreenToWorldPoint(new Vector3(_pucks[i].Triangle.Edges[j].P1.x, _pucks[i].Triangle.Edges[j].P1.y, ProjectionDistance)),
                    TargetCamera.ScreenToWorldPoint(new Vector3(_pucks[i].Triangle.Edges[j].P2.x, _pucks[i].Triangle.Edges[j].P2.y, ProjectionDistance)));
                }

                Gizmos.color = Color.cyan;

                Gizmos.DrawRay(_pucks[i].GetPosition(ProjectionDistance), _pucks[i].GetRotation() * Vector3.up);
            }
        }
    }
}
