using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BUT.TTOR.Core
{
    public class TTOR_PuckEvents : MonoBehaviour, TTOR_IPuckContent
    {
        [Tooltip("Listen only to events from these pucks, leave empty to listen to events from all pucks")]
        public List<PuckData> PucksToListenTo;


        [Space(10)]
        [Header("Puck Events")]

        [Tooltip("The Puck got created.")]
        public UnityEvent<Puck> OnPuckCreatedEvent;

        [Tooltip("The Puck is moving.")]
        public UnityEvent<Puck> OnPuckMovedEvent;

        [Tooltip("The Puck is lost.")]
        public UnityEvent<Puck> OnPuckLostEvent;

        [Tooltip("The lost Puck got found again.")]
        public UnityEvent<Puck> OnPuckFoundAgainEvent;

        [Tooltip("The Puck was removed after being lost for too long.")]
        public UnityEvent<Puck> OnPuckRemovedEvent;


        [Space(10)]
        [Header("Other Events")]
        [Tooltip("Triggered when this component gets enabled.")]
        public UnityEvent OnEnabledEvent;

        private List<Puck> _pucksLost = new List<Puck>();

        private TTOR_PuckTracker _tracker;
        private Puck _assignedPuck;


        private void OnEnable()
        {
            _tracker = GetComponentInParent<TTOR_PuckTracker>();
            if (_tracker == null)
            {
                _tracker = FindObjectOfType<TTOR_PuckTracker>();
            }

            if (_tracker == null)
            {
                Debug.LogError("Could not find a PuckTracker in the scene.");
                return;
            }

            _tracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);

            OnEnabledEvent.Invoke();
        }

        private void OnDisable()
        {
            if (_tracker)
            {
                _tracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated);
            }
        }

        private void OnPucksUpdated(List<Puck> trackedPucks)
        {
            // IF we don't specify certain puckData's to listen too
            // AND we've gotten assigned a puck by a TTOR_PuckContentManager
            // THEN only do the events for the _assignedPuck.
            if (PucksToListenTo.Count == 0 && _assignedPuck != null)
            {
                InvokePuckEvents(_assignedPuck);
                return;
            }

            foreach (Puck puck in trackedPucks)
            {
                if (PucksToListenTo.Count == 0 || PucksToListenTo.Find(x => x.name == puck.Data.name) != null)
                {
                    InvokePuckEvents(puck);
                }
            }
        }

        private void InvokePuckEvents(Puck puck)
        {
            if (puck.Phase == PuckPhase.Created)
            {
                OnPuckCreatedEvent?.Invoke(puck);
            }

            if (puck.Phase == PuckPhase.Moved)
            {
                OnPuckMovedEvent?.Invoke(puck);
            }

            if (puck.Phase == PuckPhase.Lost)
            {
                if (!_pucksLost.Contains(puck))
                {
                    _pucksLost.Add(puck);

                    //Debug.Log("ONPUCKLOST: " + puck.Data.name);                    
                    OnPuckLostEvent?.Invoke(puck);
                }
            }

            if (puck.Phase == PuckPhase.FoundAgain)
            {
                _pucksLost.Remove(puck);
                OnPuckFoundAgainEvent?.Invoke(puck);
            }

            if (puck.Phase == PuckPhase.Removed)
            {
                OnPuckRemovedEvent?.Invoke(puck);
            }
        }

        public void AssignPuck(Puck puck)
        {
            _assignedPuck = puck;
        }

        public void RemovePuck(Puck puck)
        {
            _assignedPuck = null;
        }
    }
}