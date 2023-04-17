using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadUp : MonoBehaviour
{
    public string sceneToLoad = "MenuScene";//Get a scene to load in to
    public float loadSpeed = 0.5f;//assign a load in Speed
    bool startScene;
    float a;//Value for Loading Indicator...

    void OnEnable() 
    {
        startScene = false;//Set to false on start
        a = 0;
    }

    void Update()
    {
        a += Time.deltaTime * loadSpeed;
        
        if(a >= 1f && !startScene) // When the colour gets to the value 0.1...
        {
            // ...Set the Load Bar also active
            StartCoroutine(LoadAsynchrously(sceneToLoad.ToString()));
            startScene = true;
        }
    }

    IEnumerator LoadAsynchrously (string _scene)//Load In New Scene alongside the load Up Screen
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_scene);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }
}
