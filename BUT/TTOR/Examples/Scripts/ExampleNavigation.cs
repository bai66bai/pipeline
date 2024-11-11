using BUT.TTOR.Core.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BUT.TTOR.Examples
{
    public class ExampleNavigation : MonoBehaviour
    {
        public GameObject Menu;
        public Button btnNextScene;
        public Button btnPreviousScene;

        private TTOR_CalibrationSceneLoader _calibrationSceneLoader;
        private TTOR_ToggleSimulatedPucksEnabled _toggleSimulatedPucks;

        private void Start()
        {
            _calibrationSceneLoader = FindObjectOfType<TTOR_CalibrationSceneLoader>();
            _toggleSimulatedPucks = FindObjectOfType<TTOR_ToggleSimulatedPucksEnabled>();

            SetButtonsInteractable();
        }

        public void NextScene()
        {
            int targetBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (SkipScene(targetBuildIndex) && !(SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 2)) { targetBuildIndex++; }

            SceneManager.LoadScene(targetBuildIndex);
        }

        public void PreviousScene()
        {
            int targetBuildIndex = SceneManager.GetActiveScene().buildIndex - 1;
            if (SkipScene(targetBuildIndex) && SceneManager.GetActiveScene().buildIndex != 0) { targetBuildIndex--; }

            SceneManager.LoadScene(targetBuildIndex);
        }

        public bool SkipScene(int buildIndex)
        {
            bool skip = false;

            if(buildIndex > SceneManager.sceneCountInBuildSettings -1 || buildIndex < 0) { return skip; }

            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            string sceneName = Path.GetFileName(scenePath);

            if (sceneName.Contains("TTOR_Calibrator"))
            {
                skip = true;
            }

            return skip;
        }

        private void SetButtonsInteractable()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if(btnPreviousScene != null)
                btnPreviousScene.interactable = !SceneManager.GetActiveScene().name.Contains("TTOR_00") && activeScene.buildIndex > 0;
            if(btnNextScene != null)
                btnNextScene.interactable = activeScene.buildIndex < SceneManager.sceneCountInBuildSettings - 1;
        }

        public void ToggleMenu()
        {
            Menu.SetActive(!Menu.activeSelf);
        }

        public void LoadCalibrationScene()
        {
            if (!_calibrationSceneLoader) { return; }

            _calibrationSceneLoader.LoadCalibratorScene();
        }

        public void ToggleSimulatedPucks()
        {
            if (!_toggleSimulatedPucks) { return; }

            _toggleSimulatedPucks.ToggleSimulatedPucks();
        }
    }
}
