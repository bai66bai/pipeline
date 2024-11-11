#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BUT.TTOR.Core.Utils;

namespace BUT.TTOR.Core
{
    [InitializeOnLoad]
    [CustomEditor(typeof(SimulatedPuckController))]
    public class SimulatedPuckControllerEditor : Editor
    {
        private static SimulatedPuckController _debugPuckController;

        static SimulatedPuckControllerEditor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredPlayMode)
            {
                if (!_debugPuckController) { _debugPuckController = GameObject.FindObjectOfType<SimulatedPuckController>(); }

                if (_debugPuckController && !GameViewUtility.IsSelectedSizeTheSame(_debugPuckController.TargetDisplayResolution))
                {
                    TTOR_Logger.LogWarning("The game windows' resolution does not match the Target Display Resolution.\nPlease change the game windows' resolution to corrctly display the Debug Pucks", _debugPuckController.gameObject);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            _debugPuckController = target as SimulatedPuckController;

            if (!GameViewUtility.IsSelectedSizeTheSame(_debugPuckController.TargetDisplayResolution))
            {
                EditorGUILayout.HelpBox("The game windows' resolution does not match the Target Display Resolution.\nPlease change the game windows' resolution to corrctly display the Debug Pucks", MessageType.Warning);
            }
        }
    }
}
#endif
