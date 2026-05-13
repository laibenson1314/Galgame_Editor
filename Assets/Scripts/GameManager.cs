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
        Debug.Log($"[GameManager] StartStory {storyName}");
        for(int i = 0; i < CharactersFolder.childCount; i++)
            Destroy(CharactersFolder.GetChild(i).gameObject);
        storyManager.StartStory(
            File.ReadAllLines(
                Path.Combine(
                    Application.streamingAssetsPath, 
                    "Stories", 
                    $"{storyName}.txt"
                )
            ).Where(x=>x.Length > 0).ToArray()
        );
    }
}
