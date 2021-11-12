using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpashScreenManager : MonoBehaviour
{
    [SerializeField] private float TimeToLoad = 5f;
    void Start()
    {
        Invoke("LoadMenuScene", TimeToLoad);
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

}
