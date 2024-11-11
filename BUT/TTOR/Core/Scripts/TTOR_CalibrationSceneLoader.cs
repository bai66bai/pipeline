using UnityEngine;
using UnityEngine.SceneManagement;

namespace BUT.TTOR.Core.Utils
{
    public class TTOR_CalibrationSceneLoader : MonoBehaviour
    {
        private static string _previousSceneName;
        private const string CALIBRATION_SCENENAME = "TTOR_Calibrator";

        void Start()
        {
            if (!IsSceneAvailable(CALIBRATION_SCENENAME))
            {
                TTOR_Logger.LogWarning("Calibration scene will not load when pressing Ctrl+C. Please add the scene with name " + CALIBRATION_SCENENAME + " to the build settings to fix this.");
            }

            if (!IsSceneAvailable(SceneManager.GetActiveScene().name))
            {
                TTOR_Logger.LogWarning("Current scene (" + SceneManager.GetActiveScene().name + ") is not in the build settings, pressing the back button in the calibration scene will not work.");
            }

            _previousSceneName = SceneManager.GetActiveScene().name;
        }


        void Update()
        {
            // Check for keyboard input (Ctrl+C)
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKeyDown(KeyCode.C))
            {
                LoadCalibratorScene();
            }
        }

        public static void LoadPreviousScene()
        {
            // Check if a previous scene was recorded
            if (!string.IsNullOrEmpty(_previousSceneName))
            {
                // Load the previous scene
                SceneManager.LoadScene(_previousSceneName);
            }
            else
            {
                Debug.LogWarning("No previous scene recorded.");
            }
        }

        public void LoadCalibratorScene()
        {
            if (IsSceneAvailable(CALIBRATION_SCENENAME))
            {
                SceneManager.LoadScene(CALIBRATION_SCENENAME);
            }
            else
            {
                Debug.LogWarning("Scene 'TTOR_Calibrator' not in build settings. Unable to load.");
            }
        }

        private bool IsSceneAvailable(string sceneName)
        {
            // Check if the scene is in the build settings
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameInBuildSettings = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (sceneNameInBuildSettings == sceneName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}