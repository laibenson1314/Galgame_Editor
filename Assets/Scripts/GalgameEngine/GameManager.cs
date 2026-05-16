using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform CharactersFolder;
    [SerializeField] private StoryManager storyManager;

    private void Awake()
    {
        Story("StartingStory");
    }

    public void Story(string storyName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Stories", $"{storyName}.txt");
        if (!File.Exists(path))
        {
            Debug.LogError($"[GameManager] Story file not found: {path}");
            return;
        }

        Debug.Log($"[GameManager] Start story {storyName}");
        for(int i = 0; i < CharactersFolder.childCount; i++)
            Destroy(CharactersFolder.GetChild(i).gameObject);
        storyManager.StartStory(path);
    }
}
