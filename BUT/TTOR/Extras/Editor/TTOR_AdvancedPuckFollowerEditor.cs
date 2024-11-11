#if UNITY_EDITOR

using UnityEditor;

namespace BUT.TTOR.Core
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TTOR_AdvancedPuckFollower))]
    public class TTOR_AdvancedPuckFollowerEditor : Editor
    {
        private SerializedProperty _pucksToFollow;

        private SerializedProperty _followPosition;
        private SerializedProperty _resetPositionOnDisable;
        private SerializedProperty _positionFollowType;
        private SerializedProperty _positionSmoothFollowSpeed;
        private SerializedProperty _projectionDistanceOffset;

        private SerializedProperty _followRotation;
        private SerializedProperty _resetRotationOnDisable;
        private SerializedProperty _rotationFollowType;
        private SerializedProperty _rotationSmoothFollowSpeed;
        private SerializedProperty _puckVelocityBlocksRotation;
        private SerializedProperty _puckVelocityTreshold;
        private SerializedProperty _secondsToHoldRotationBlock;
        private SerializedProperty _keepInitialRotation;
        private SerializedProperty _limitRotation;
        private SerializedProperty _minRotation;
        private SerializedProperty _maxRotation;

        void OnEnable()
        {
            _pucksToFollow = serializedObject.FindProperty("PucksToFollow");

            _followPosition = serializedObject.FindProperty("FollowPosition");
            _resetPositionOnDisable = serializedObject.FindProperty("ResetPositionOnDisable");
            _positionFollowType = serializedObject.FindProperty("PositionFollowType");
            _positionSmoothFollowSpeed = serializedObject.FindProperty("PositionSmoothFollowSpeed");
            _projectionDistanceOffset = serializedObject.FindProperty("ProjectionDistanceOffset");


            _followRotation = serializedObject.FindProperty("FollowRotation");
            _resetRotationOnDisable = serializedObject.FindProperty("ResetRotationOnDisable");
            _rotationFollowType = serializedObject.FindProperty("RotationFollowType");
            _rotationSmoothFollowSpeed = serializedObject.FindProperty("RotationSmoothFollowSpeed");
            _puckVelocityBlocksRotation = serializedObject.FindProperty("PuckVelocityBlocksRotation");
            _puckVelocityTreshold = serializedObject.FindProperty("VelocityTreshold");
            _secondsToHoldRotationBlock = serializedObject.FindProperty("SecondsToHoldRotationBlock");
            _keepInitialRotation = serializedObject.FindProperty("KeepInitialRotation");
            _limitRotation = serializedObject.FindProperty("LimitRotation");
            _minRotation = serializedObject.FindProperty("MinRotation");
            _maxRotation = serializedObject.FindProperty("MaxRotation");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_pucksToFollow);

            // Position
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_followPosition);
            EditorGUILayout.Space(10);

            if (_followPosition.boolValue == true)
            {
                EditorGUILayout.PropertyField(_resetPositionOnDisable);

                EditorGUILayout.PropertyField(_positionFollowType);
                if (_positionFollowType.enumValueIndex == 1) // Smoothed
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_positionSmoothFollowSpeed);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(_projectionDistanceOffset);
            }

            // Rotation
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_followRotation);
            EditorGUILayout.Space(10);
            if (_followRotation.boolValue == true)
            {
                EditorGUILayout.PropertyField(_resetRotationOnDisable);

                EditorGUILayout.PropertyField(_rotationFollowType);
                if (_rotationFollowType.enumValueIndex == 1) // Smoothed
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_rotationSmoothFollowSpeed);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space(-20);
                EditorGUILayout.PropertyField(_puckVelocityBlocksRotation);
                if (_puckVelocityBlocksRotation.boolValue == true)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_puckVelocityTreshold);
                    EditorGUILayout.PropertyField(_secondsToHoldRotationBlock);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(_keepInitialRotation);
                if (_keepInitialRotation.boolValue == true)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(-10);
                    EditorGUILayout.PropertyField(_limitRotation);
                    if (_limitRotation.boolValue == true)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(_minRotation);
                        EditorGUILayout.PropertyField(_maxRotation);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
