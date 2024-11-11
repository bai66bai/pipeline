using System.Collections.Generic;
using UnityEngine;
using BUT.TTOR.Core;
 
namespace BUT.TTOR.Calibration
{
    public class RawTriangleVisualiser : MonoBehaviour
    {
        public TTOR_PuckTracker puckTracker;

        public GameObject RawTriangleDebugInfoPrefab;
        private Stack<RawTriangleVisualiserUI> _triangleUIPool = new Stack<RawTriangleVisualiserUI>();

        private List<RawTriangleVisualiserUI> _tempVisualisers = new List<RawTriangleVisualiserUI>();

        private void OnEnable()
        {
            puckTracker = FindFirstObjectByType<TTOR_PuckTracker>();
            if (!puckTracker)
            {
                Debug.LogError("Could not find a puckTracker");
            }

            if (puckTracker)
            {
                puckTracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);
            }
        }

        private void OnDisable()
        {
            if (puckTracker)
            {
                puckTracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated);
            }
        }

        private void OnPucksUpdated(List<Puck> pucks)
        {
            foreach (RawTriangleVisualiserUI rawTriangleVisualiserUI in _triangleUIPool) {
                rawTriangleVisualiserUI.gameObject.SetActive(false);
            }

            _tempVisualisers.Clear();

            puckTracker.RawTriangles.ForEach((Triangle t) =>
            {
                //Debug.Log("t: " + t.GetKeyAngle());

                RawTriangleVisualiserUI rawTriangleVisualiserUI = GetTriangleUIFromPool();
                rawTriangleVisualiserUI.gameObject.SetActive(true);
                rawTriangleVisualiserUI.SetTriangle(t);

                _tempVisualisers.Add(rawTriangleVisualiserUI);
            });

            _tempVisualisers.ForEach((rawTriangleVisualiserUI) => { 
                ReturnTriangleUIToPool(rawTriangleVisualiserUI);        
            });
        }


        private RawTriangleVisualiserUI GetTriangleUIFromPool()
        {
            while (_triangleUIPool.Count > 0)
            {
                RawTriangleVisualiserUI triangleUI = _triangleUIPool.Pop();

                if (triangleUI != null)
                {
                    return triangleUI;
                }
                else
                {
                    Debug.LogWarning("Found a null object in the pool. Has some code outside the pool destroyed it?");
                }
            }

            //Debug.LogWarning("Pool is empty, extending capacity by creating new TriangleUI");
            return Instantiate(RawTriangleDebugInfoPrefab, transform).GetComponent<RawTriangleVisualiserUI>();
        }

        private void ReturnTriangleUIToPool(RawTriangleVisualiserUI triangle)
        {
            if (triangle != null)
            {
                _triangleUIPool.Push(triangle);
            }
        }
    }
}
