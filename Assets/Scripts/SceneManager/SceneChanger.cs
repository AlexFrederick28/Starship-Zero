using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

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
