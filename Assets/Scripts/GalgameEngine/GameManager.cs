using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform CharactersFolder;
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private string startingStory = "StartingStory";

    private void Awake()
    {
        if (PlayerPrefs.HasKey("LoadStory")) Story(PlayerPrefs.GetString("LoadStory"));
        else Story(startingStory);
    }

    public void Story(string storyName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Stories", $"{storyName}.txt");
        if (!File.Exists(path))
        {
            Debug.LogError($"[GameManager] Story file not found: {path}");
            return;
        }

        for(int i = 0; i < CharactersFolder.childCount; i++)
            Destroy(CharactersFolder.GetChild(i).gameObject);
        storyManager.StartStory(path);
    }
}