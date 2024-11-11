using UnityEngine;

namespace BUT.TTOR.Core.Utils
{
    public static class TTOR_Logger
    {
        private const string PREFIX = "<b><color=#07c2a3>[TTOR]</color></b> ";

        public delegate void OnLog(string msg);
        public static event OnLog OnLogEvent;

        public static LogMask LogFilter;

        [System.Flags]
        public enum LogMask
        {
            None = 0,
            PuckPhaseChanges = 1,
            ReasonsPuckIsLost = 2,
            CalibrationEvents = 4,
            Debug = 8,
        }

        public static void Log(string msg, LogMask logType)
        {
            if ((LogFilter & logType) != 0)
            {
                OnLogEvent?.Invoke(msg);
                Debug.Log(string.Concat(PREFIX, msg));
            }
        }

        public static void Log(string msg, LogMask logType, GameObject gameObject)
        {
            OnLogEvent?.Invoke(msg);

            Debug.Log(string.Concat(PREFIX, msg), gameObject);
        }

        public static void LogWarning(string msg)
        {
            OnLogEvent?.Invoke(msg);

            Debug.LogWarning(string.Concat(PREFIX, msg));
        }

        public static void LogWarning(string msg, GameObject gameObject)
        {
            OnLogEvent?.Invoke(msg);

            Debug.LogWarning(string.Concat(PREFIX, msg), gameObject);
        }

        public static void LogError(string msg)
        {
            OnLogEvent?.Invoke(msg);

            Debug.LogWarning(string.Concat(PREFIX, msg));
        }

        public static void LogError(string msg, GameObject gameObject)
        {
            OnLogEvent?.Invoke(msg);

            Debug.LogError(string.Concat(PREFIX, msg), gameObject);
        }
    }
}
