using UnityEngine;

namespace BUT.TTOR.Core.Utils
{
    public class TTOR_ToggleSimulatedPucksEnabled : MonoBehaviour
    {
        [SerializeField] private GameObject simulatedPucks;

        void Update()
        {
            // Check for keyboard input (Ctrl+X)
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKeyDown(KeyCode.X))
            {
                ToggleSimulatedPucks();
            }
        }

        public void ToggleSimulatedPucks()
        {
            simulatedPucks.SetActive(!simulatedPucks.activeSelf);
        }
    }
}
