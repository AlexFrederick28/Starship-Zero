using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    public GameObject videoImage;
    public GameObject videoSource;
    private bool loadingGameScene;

    private void OnEnable()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (instance != null)
        {
            instance = null;
        }
    }

    public void LoadSceneByNumber(int sceneNum)
    {
        Debug.Log("Loading scene [" + sceneNum + "]");
        SceneManager.LoadScene(sceneNum); // change scene
    }

    public void LoadSceneByName(string sceneName)
    {
        Debug.Log("Loading [" + sceneName + "] scene");
        SceneManager.LoadScene(sceneName); // change scene
    }

    public IEnumerator LoadSceneAfterDelay()
    {
        loadingGameScene = true;

        yield return new WaitForSeconds(22f);

        LoadSceneByNumber(1);
    }

    public void StartIntroAnimationAndLoadGameScene()
    {
        if (loadingGameScene == true) { return; }

        videoImage.SetActive(true);
        videoSource.SetActive(true);

        StartCoroutine(LoadSceneAfterDelay());
    }

    public void QuitTheGame()
    {

#if UNITY_EDITOR
        Debug.Log("Quitting Editor");
        EditorApplication.isPlaying = false;

#if UNITY_STANDALONE_WIN
        Debug.Log("Quitting Windows");
        Application.Quit();
#elif UNITY_WEBGL
        Debug.Log("WebGL Build");
#else
        Debug.Log("Other");
#endif
#endif

    }
}
