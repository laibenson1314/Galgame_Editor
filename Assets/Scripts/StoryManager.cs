using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Background;
    [SerializeField] private Transform CharactersFolder;
    [SerializeField] private Transform ButtonsFolder;
    [SerializeField] private ContentShowing contentShowing;
    [SerializeField] private GameObject switchScene;
    [SerializeField] private Button buttonPrefab;

    public string nextStory;

    string[] _scripts;
    int currentIndex;

    [HideInInspector] public bool isChoising;

    public void NextStep()
    {
        if(currentIndex >= _scripts.Length)
        {
            switchScene.SetActive(true);
            return;
        }
        SetUp();
        string[] parts = _scripts[currentIndex].Split(": ");
        contentShowing.ShowText(parts[0], parts[1]);
        currentIndex++;
    }

    public void StartStory(string[] scripts)
    {
        _scripts = scripts;
        currentIndex = 0;
        NextStep();
    }

    private void SetUp()
    {
        bool loopping = true;
        do
        {
            string currentLine = _scripts[currentIndex];
            if (currentLine.StartsWith("#"))
            {
                string[] parts = currentLine[1..].Split(' ');
                string cmd = parts[0];
                string[] arg = parts[1..];

                switch (cmd)
                {
                    case "bg": SwitchBackground(arg[0]); break;
                    case "enter": CharacterEnter(arg[0], arg[1]); break;
                    case "exit": CharacterExit(arg[0]); break;
                    case "change": CharacterChangeSprite(arg[0], arg[1]); break;
                    case "jump": JumpToPart(arg[0]); break;
                    case "choice": SpawnChoiceButton(); break;
                }

                currentIndex++;
            }
            else if (currentLine.StartsWith("*"))
            {
                string line;
                do
                {
                    line = _scripts[currentIndex];
                    currentIndex++;
                } while (line.StartsWith("*"));
            }
            else loopping = false;
        } while (loopping);
    }
    private void SwitchBackground(string backgroundName)
    {
        Background.sprite = LoadBackground(backgroundName);
    }
    private void CharacterEnter(string characterName, string characterStatus)
    {
        Debug.Log($"[StoryManager] #enter: {characterName}");
        GameObject character = new GameObject(characterName);
        character.AddComponent<SpriteRenderer>().sprite = LoadCharacter(characterName, characterStatus);
        character.GetComponent<SpriteRenderer>().sortingOrder = 10;
        character.transform.localScale = new Vector2(1.57f, 1.57f);
        character.transform.position = new Vector2(0f, -1.22f);
        character.transform.parent = CharactersFolder;
    }
    private void CharacterExit(string characterName)
    {
        Debug.Log($"[StoryManager] #exit: {characterName}");
        Destroy(CharactersFolder.Find(characterName).gameObject);
    }
    private void CharacterChangeSprite(string characterName, string characterStatus)
    {
        Debug.Log($"[StoryManager] #change: {characterName}");
        GameObject character = CharactersFolder.Find(characterName).gameObject;
        character.GetComponent<SpriteRenderer>().sprite = LoadCharacter(characterName, characterStatus);
    }
    private void JumpToPart(string part)
    {
        contentShowing.pausing = true;
        nextStory = part;
        switchScene.SetActive(true);
        Invoke(nameof(stopContentPausing), 1f);
    } private void stopContentPausing() { contentShowing.pausing = false; }
    private void SpawnChoiceButton()
    {
        contentShowing.pausing = true;
        currentIndex++;
        do
        {
            string[] arg = _scripts[currentIndex].Split("=>");

            var btn = Instantiate(buttonPrefab, ButtonsFolder);
            btn.GetComponentInChildren<Text>().text = arg[0];
            btn.onClick.AddListener(() =>
            {
                currentIndex = _scripts.ToList().IndexOf($"*{arg[1].Trim()}") + 1;
                contentShowing.pausing = false;
                foreach (Transform child in ButtonsFolder) Destroy(child.gameObject);
                NextStep();
            });

            currentIndex++;
        } while (!_scripts[currentIndex].StartsWith("#"));
    }

    Sprite LoadCharacter(string characterName, string characterStatus)
    {
        return Resources.Load<Sprite>($"Characters/{characterName}/{characterStatus}");
    }
    Sprite LoadBackground(string backgroundName)
    {
        Debug.Log($"[StoryManager] #bg: {backgroundName}");
        return Resources.Load<Sprite>($"Backgrounds/{backgroundName}");
    }
}