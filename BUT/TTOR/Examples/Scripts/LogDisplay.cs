using System.Collections.Generic;
using UnityEngine;
using BUT.TTOR.Core.Utils;
using TMPro;
using BUT.TTOR.Core;
using System;

namespace BUT.TTOR.Examples
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LogDisplay : MonoBehaviour
    {
        [SerializeField] private int maxLogCount = 5;

        private TTOR_PuckTracker _puckTracker;

        private TextMeshProUGUI _textMesh;
        private Queue<string> _logsQueue = new Queue<string>();

        void Start()
        {
            _puckTracker = FindObjectOfType<TTOR_PuckTracker>();
            _textMesh = GetComponent<TextMeshProUGUI>();

            _puckTracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);
        }

        private void OnPucksUpdated(List<Puck> updatedPucks)
        {
            foreach (Puck puck in updatedPucks)
            {
                switch (puck.Phase)
                {
                    case PuckPhase.Created:
                        AddLog("<color=#00FF00><b>Puck Created: </b></color> " + puck.Data.name);
                        break;
                    case PuckPhase.Removed:
                        AddLog("<color=red><b>Puck Removed: </b></color> " + puck.Data.name);
                        break;
                }
            }
        }

        private void AddLog(string msg)
        {
            msg = msg.Replace("color=lime", "color=#00ff00");

            _logsQueue.Enqueue(msg);
            if(_logsQueue.Count > maxLogCount)
            {
                int amountToRemove = _logsQueue.Count - maxLogCount;
                for (int i = 0; i < amountToRemove; i++)
                {
                    _logsQueue.Dequeue();
                }
            }

            string logsText = "";
            foreach (string log in _logsQueue)
            {
                logsText = string.Concat(logsText, log, "\n");
            }

            _textMesh.text = logsText;
        }
    }
}
