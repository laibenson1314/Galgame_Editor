using UnityEngine;
using UnityEngine.UI;

public class ContentShowing : MonoBehaviour
{
    [SerializeField] private float TextShowSpeed = .05f;
    [SerializeField] private Text Title, Content;
    [SerializeField] private StoryManager StoryManager;

    [HideInInspector] public bool pausing;
    
    bool finished;
    string originalText = "", currentText = "";
    int currentTextIndex;
    float textShowSpeedTimer;

    void Update()
    {
        if(pausing) return;

        textShowSpeedTimer += Time.deltaTime;
        Content.text = currentText;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!finished)
            {
                currentText = originalText;
                finished = true;
            }
            else StoryManager.NextStep();
        }

        if (!finished && textShowSpeedTimer >= TextShowSpeed)
        {
            if (currentTextIndex >= originalText.Length)
            {
                finished = true;
                return;
            }
            currentText += originalText[currentTextIndex];
            currentTextIndex++;
            textShowSpeedTimer = 0f;
            if (currentTextIndex >= originalText.Length) finished = true;
        }
    }

    public void ShowText(string title, string content)
    {
        finished = true;
        Debug.Log($"[ContentShowing] title: {title}, content: {content}");
        Title.text = title;
        originalText = content;
        currentText = "";
        currentTextIndex = 0;
        finished = false;
    }
}
