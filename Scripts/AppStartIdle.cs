using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStartIdle : MonoBehaviour
{
    public GameObject IdleObj;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "IdleScene")
        {
            IdleObj.SetActive(true);
        }
    }


}
