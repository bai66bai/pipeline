using UnityEngine;

namespace BUT.TTOR.Core
{
    public class SimulatedTouch : MonoBehaviour
    {
        private bool _touchBegan = false;

        private Touch _myTouch;

        private int _fingerId = 0;

        //private int _previousFingerId = 0;

        public void SetFingerId(int id)
        {
            //_previousFingerId = _fingerId;
            _fingerId = id;
        }

        public Touch GetTouch()
        {
            _myTouch = new Touch();
            _myTouch.position = transform.position;
            _myTouch.fingerId = _fingerId;
            if (!_touchBegan)
            {
                _myTouch.phase = TouchPhase.Began;
                _touchBegan = true;
            }
            else
            {
                _myTouch.phase = TouchPhase.Moved;
            }

            //CheckFingerID();

            return _myTouch;
        }

        /*private void CheckFingerID()
        {
            if (_previousFingerId != _myTouch.fingerId)
            {
                Debug.Log("FINGERID GOT CHANGED! from: " + _previousFingerId + " to: " + _fingerId, gameObject);
                _previousFingerId = _myTouch.fingerId;
            } 
        }*/

        private void OnDisable()
        {
            _myTouch.phase = TouchPhase.Ended;
            _touchBegan = false;
        }
    }
}

