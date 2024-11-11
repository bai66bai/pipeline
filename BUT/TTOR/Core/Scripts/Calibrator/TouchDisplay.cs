using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public class TouchDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _visualGO;
        private List<TouchLocation> _touchLocations = new List<TouchLocation>();

        private RenderMode _canvasRenderMode;
        private Canvas _myCanvas;

        private void Start()
        {
            _myCanvas = GetComponentInParent<Canvas>();
            if (_myCanvas != null)
            {
                _canvasRenderMode = _myCanvas.renderMode;
            }
        }

        private void Update()
        {
            int i = 0;
            while (i < Input.touchCount)
            {
                Touch t = Input.GetTouch(i);
                if(t.phase == TouchPhase.Began)
                {
                    _touchLocations.Add(new TouchLocation(t.fingerId, CreateVisual(t)));
                }
                else if(t.phase == TouchPhase.Ended)
                {
                    TouchLocation thisTouch = _touchLocations.Find(touchLocation => touchLocation.FingerId == t.fingerId);
                    Destroy(thisTouch.VisualGO);
                    _touchLocations.RemoveAt(_touchLocations.IndexOf(thisTouch));
                }
                else if (t.phase == TouchPhase.Moved)
                {
                    TouchLocation thisTouch = _touchLocations.Find(touchLocation => touchLocation.FingerId == t.fingerId);
                    thisTouch.VisualGO.transform.position = TouchPosition(t);
                }
                i++;
            }
        }

        private GameObject CreateVisual(Touch touch)
        {
            GameObject visualGO = Instantiate(_visualGO, transform);
            visualGO.name = "Touch " + touch.fingerId;
            visualGO.transform.position = TouchPosition(touch);
            visualGO.GetComponentInChildren<TextMeshProUGUI>().text = touch.fingerId.ToString();
            return visualGO;
        }

        private Vector3 TouchPosition(Touch touch)
        {
            if (_canvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                return touch.position;
            }
            
            return Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, _myCanvas.planeDistance));
        }
    }

    public class TouchLocation
    {
        public int FingerId;
        public GameObject VisualGO;

        public TouchLocation(int fingerId, GameObject visualGO)
        {
            FingerId = fingerId;
            VisualGO = visualGO;
        }
    }
}
