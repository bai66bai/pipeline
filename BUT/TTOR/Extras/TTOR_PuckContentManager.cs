using System;
using System.Collections.Generic;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public class TTOR_PuckContentManager : MonoBehaviour
    {
        public PuckDataPrefab[] PuckDataPrefabs;
        public Transform PrefabParentTransform;

        [Header("Settings")]
        public bool ApplyPuckPositionOnPrefabSpawn = true;
        public bool ApplyPuckRotationOnPrefabSpawn = true;
        public bool DestroyPrefabOnPuckRemoved = true;

        private TTOR_PuckTracker _puckTracker;
        private List<PuckContent> _puckContentInstances = new List<PuckContent>();

        private void Start()
        {
            if (!PrefabParentTransform) { PrefabParentTransform = transform; }
        }

        private void OnEnable()
        {
            if (!_puckTracker) { _puckTracker = FindObjectOfType<TTOR_PuckTracker>(); }
            if (!_puckTracker) { return; }

            _puckTracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);
        }

        private void OnDisable()
        {
            if (_puckTracker) { _puckTracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated); }
        }

        private void OnPucksUpdated(List<Puck> pucks)
        {
            foreach (Puck puck in pucks)
            {
                if (puck.Phase == PuckPhase.Created) // Create new PuckContent
                {
                    foreach(PuckDataPrefab puckDataPrefab in PuckDataPrefabs)
                    {
                        if(puck.Data != puckDataPrefab.PuckData) { continue; }

                        if (!puckDataPrefab.Prefab) 
                        {
                            Debug.LogWarning("PuckContentManagers PuckDataPrefabs has an entry with no prefab assigned.", gameObject);
                            continue;
                        }

                        GameObject newPrefabInstance = Instantiate(puckDataPrefab.Prefab, PrefabParentTransform);

                        PuckContent newPuckContent = new PuckContent(newPrefabInstance, puck, newPrefabInstance.GetComponentsInChildren<TTOR_IPuckContent>(true));

                        foreach(TTOR_IPuckContent iPuckContent in newPuckContent.IPuckContents)
                        {
                            iPuckContent.AssignPuck(puck);
                        }

                        _puckContentInstances.Add(newPuckContent);

                        if (ApplyPuckRotationOnPrefabSpawn) { ApplyPuckRotation(newPrefabInstance, puck); }
                        if (ApplyPuckPositionOnPrefabSpawn) { ApplyPuckPosition(newPrefabInstance, puck); }

                    }
                }
                else if (puck.Phase == PuckPhase.Removed) // Remove PuckContent
                {
                    for (int i = 0; i < _puckContentInstances.Count; i++)
                    {
                        if(_puckContentInstances[i].Puck != puck) { continue; }

                        foreach (TTOR_IPuckContent iPuckContent in _puckContentInstances[i].IPuckContents)
                        {
                            iPuckContent.RemovePuck(puck);
                        }

                        if (DestroyPrefabOnPuckRemoved) { Destroy(_puckContentInstances[i].GameObject); }

                        _puckContentInstances.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void ApplyPuckRotation(GameObject gameObject, Puck puck)
        {
            gameObject.transform.rotation = puck.GetRotation();
        }

        private void ApplyPuckPosition(GameObject gameObject, Puck puck)
        {
            gameObject.transform.position = puck.GetPosition(_puckTracker.ProjectionDistance);

            // For gameobjects in a canvas
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 canvasPos = GetCanvasPosition(gameObject.transform, rectTransform, rectTransform.GetComponentInParent<Canvas>());
                rectTransform.anchoredPosition = canvasPos;
            }
        }

        private Vector2 GetCanvasPosition(Transform transform, RectTransform rectTransform, Canvas canvas)
        {
            Vector2 screenPos;
            Camera cam;
            if (canvas.worldCamera != null)
            {
                cam = canvas.worldCamera;
            }
            else
            {
                cam = Camera.main;
            }

            screenPos = cam.WorldToScreenPoint(transform.position);

            RectTransform parentRectTransform = null;
            if (canvas != null)
            {
                parentRectTransform = transform.parent.GetComponent<RectTransform>();
            }

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, out canvasPos);
            if (parentRectTransform.gameObject != canvas.gameObject)
            {
                rectTransform.anchorMax = parentRectTransform.anchorMax;
                rectTransform.anchorMin = parentRectTransform.anchorMin;
            }

            return canvasPos;
        }


        [Serializable]
        public struct PuckDataPrefab
        {
            public PuckData PuckData;
            public GameObject Prefab;
        }

        [Serializable]
        public struct PuckContent
        {
            public GameObject GameObject;
            public Puck Puck;
            public TTOR_IPuckContent[] IPuckContents;

            public PuckContent(GameObject gameObject, Puck puck, TTOR_IPuckContent[] iPuckContents = null)
            {
                GameObject = gameObject;
                Puck = puck;
                IPuckContents = iPuckContents;
            }
        }
    }
}
