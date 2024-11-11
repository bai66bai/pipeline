using UnityEngine;

namespace BUT.TTOR.Examples
{
    public class SetFrameRate : MonoBehaviour
    {
        void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}
