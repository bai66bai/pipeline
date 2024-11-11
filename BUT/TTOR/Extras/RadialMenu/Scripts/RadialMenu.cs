using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BUT.TTOR.Core
{
    public class RadialMenu : MonoBehaviour
    {
        [SerializeField] private float _selectorAngle = -90;
        [SerializeField] [Range(-1, 1)] private float _selectionTreshold = 0.8f;

        [SerializeField] private Transform _selectorTransform;
        private Vector3 _selectorDirection;

        private List<RadialMenuItem> _menuItems = new List<RadialMenuItem>();
        private RadialMenuItem _selectedMenuItem;

        public UnityEvent<int> OnSelectedMenuItemChanged;

        public RadialMenuItem SelectedMenuItem 
        { 
            get => _selectedMenuItem;
            set
            {
                if (_selectedMenuItem == value) { return; }

                if(_selectedMenuItem) _selectedMenuItem.Deselect();

                _selectedMenuItem = value;
                
                _selectedMenuItem.Select();
                OnSelectedMenuItemChanged?.Invoke(_menuItems.IndexOf(_selectedMenuItem));
            }
        }

        public List<RadialMenuItem> MenuItems => _menuItems;

        private void Start()
        {
            _menuItems = GetComponentsInChildren<RadialMenuItem>(true).ToList();

            _selectorDirection = (Quaternion.Euler(new Vector3(0, 0, _selectorAngle)) * Vector3.up);
        }

        private void OnEnable()
        {
            _menuItems = GetComponentsInChildren<RadialMenuItem>(true).ToList();
        }

        private void Update()
        {
            float largestDot = float.MinValue;
            int closestMenuItemIndex = -1;

            for (int i = 0; i < _menuItems.Count; i++)
            {
                if (!_menuItems[i])
                {
                    return;
                }


                Vector3 menuItemDirection = (_menuItems[i].Transform.position - _selectorTransform.position);
                float dot = Vector3.Dot(_selectorDirection.normalized, menuItemDirection.normalized);


                if (dot > largestDot)
                {
                    largestDot = dot;
                    closestMenuItemIndex = i;
                }
            }

            if(closestMenuItemIndex != -1 && largestDot > _selectionTreshold)
            {
                SelectedMenuItem = _menuItems[closestMenuItemIndex];
            }
        }
    }
}
