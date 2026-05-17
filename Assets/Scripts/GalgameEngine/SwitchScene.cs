using UnityEngine;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StoryManager storyManager;

    public void Story()
    {
        gameManager.Story(storyManager.nextStory);
    }

    public void ActivityOff()
    {
        gameObject.SetActive(false);
    }
}