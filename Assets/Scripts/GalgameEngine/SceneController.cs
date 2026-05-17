using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private string switchSceneName;

    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SwitchScene(string sceneName)
    {
        switchSceneName = sceneName;
        foreach(var item in SceneManager.GetActiveScene().GetRootGameObjects()) item.SetActive(false);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    public async void ReturnScene()
    {
        await SceneManager.UnloadSceneAsync(switchSceneName);
        switchSceneName = null;
        foreach (var item in SceneManager.GetActiveScene().GetRootGameObjects()) item.SetActive(true);
    }
}
