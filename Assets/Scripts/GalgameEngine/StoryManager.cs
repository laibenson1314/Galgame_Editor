using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

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
    string _path;
    int _currentIndex;

    [HideInInspector] public bool isChoising;

    public void NextStep()
    {
        SetUp();
        if(_currentIndex >= _scripts.Length)
        {
            switchScene.SetActive(true);
            return;
        }
        string[] parts = _scripts[_currentIndex].Split(": ");
        contentShowing.ShowText(parts[0], parts[1]);
        _currentIndex++;
    }

    public void StartStory(string path)
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("sceneChangeTransport")))
        {
            _path = path;
            _scripts = File.ReadAllLines(path);
            _currentIndex = 0;
        }
        else
        {
            var json = JsonUtility.FromJson<SceneChangeTransport>(PlayerPrefs.GetString("sceneChangeTransport"));
            _path = json.path;
            _scripts = File.ReadAllLines(json.path);
            SetUp();
            _currentIndex = json.currentIndex;

            foreach (Transform child in CharactersFolder) Destroy(child.gameObject);

            for (int i = 0; i <= _currentIndex; i++)
            {
                if (_scripts[i].StartsWith("#"))
                {
                    string[] parts = _scripts[i][1..].Split(' ');
                    Compiler(parts[0], parts[1..]);
                }
            }

            PlayerPrefs.DeleteKey("sceneChangeTransport");
        }

        _scripts = _scripts
            .ToList()
            .Where(x => x.Length > 0)
            .ToArray();
        NextStep();
    }

    private void SetUp()
    {
        bool loopping = true;
        do
        {
            if(_currentIndex >= _scripts.Length) break;
            string currentLine = _scripts[_currentIndex];
            if (currentLine.StartsWith("//"))
            {
                _currentIndex++;
                continue;
            }
            else if (currentLine.StartsWith("#"))
            {
                string[] parts = currentLine[1..].Split(' ');
                Compiler(parts[0], parts[1..]);
                if (parts[0] == "scene")
                {
                    if (CheckArgs("SwitchBackground", parts[1..].Length, parts[0])) return;
                    ChangeScene(parts[1]);
                }
                _currentIndex++;
            }
            else if (currentLine.StartsWith("*"))
            {
                string line;
                do
                {
                    line = _scripts[_currentIndex];
                    _currentIndex++;
                } while (line.StartsWith("*"));
            }
            else loopping = false;
        } while (loopping);
    }

    private void Compiler(string cmd, string[] arg)
    {
        switch (cmd)
        {
            case "bg":
                if(CheckArgs("SwitchBackground", arg.Length, cmd)) return;
                SwitchBackground(arg[0]);
                break;
            case "enter":
                if(CheckArgs("CharacterEnter", arg.Length, cmd)) return;
                CharacterEnter(arg[0], arg[1]);
                break;
            case "exit":
                if(CheckArgs("CharacterExit", arg.Length, cmd)) return;
                CharacterExit(arg[0]);
                break;
            case "change":
                if(CheckArgs("CharacterChangeSprite", arg.Length, cmd)) return;
                CharacterChangeSprite(arg[0], arg[1]);
                break;
            case "jump":
                if(CheckArgs("JumpToPart", arg.Length, cmd)) return;
                JumpToPart(arg[0]);
                break;
            case "choice":
                SpawnChoiceButton();
                break;
        }
    }
    private void SwitchBackground(string backgroundName)
    {
        Background.sprite = LoadBackground(backgroundName);
    }
    private void CharacterEnter(string characterName, string characterStatus)
    {
        GameObject character = new GameObject(characterName);
        character.AddComponent<SpriteRenderer>().sprite = LoadCharacter(characterName, characterStatus);
        character.GetComponent<SpriteRenderer>().sortingOrder = 10;
        character.transform.localScale = new Vector2(1.57f, 1.57f);
        character.transform.position = new Vector2(0f, -1.22f);
        character.transform.parent = CharactersFolder;
    }
    private void CharacterExit(string characterName)
    {
        Destroy(CharactersFolder.Find(characterName).gameObject);
    }
    private void CharacterChangeSprite(string characterName, string characterStatus)
    {
        GameObject character = CharactersFolder.Find(characterName).gameObject;
        character.GetComponent<SpriteRenderer>().sprite = LoadCharacter(characterName, characterStatus);
    }
    private void JumpToPart(string part)
    {
        nextStory = part;
        contentShowing.pausing = true;
        switchScene.SetActive(true);
        Invoke(nameof(stopContentPausing), 1f);
    }private void stopContentPausing() { contentShowing.pausing = false; }
    private void SpawnChoiceButton()
    {
        contentShowing.pausing = true;
        _currentIndex++;
        do
        {
            if(!TryParseChoise(_scripts[_currentIndex], out string left, out string right))
            {
                _currentIndex++;
                continue;
            }

            var btn = Instantiate(buttonPrefab, ButtonsFolder);
            btn.GetComponentInChildren<Text>().text = left;
            btn.onClick.AddListener(() =>
            {
                _currentIndex = _scripts.ToList().IndexOf($"*{right}") + 1;
                contentShowing.pausing = false;
                foreach (Transform child in ButtonsFolder) Destroy(child.gameObject);
                NextStep();
            });

            _currentIndex++;
        } while (_scripts[_currentIndex] != "#choice");
    } private bool TryParseChoise(string input, out string left, out string right)
    {
        string[] args = input.Split("=>");
        if (args.Length != 2)
        {
            Debug.LogError($"[StoryManager] Invalid choice command format at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}." +
                $"\r\n Expected format: ButtonText => TargetPart");
            left = right = null;
            return false;
        }
        if(args[1].TrimStart() != args[1].Trim())
        {
            Debug.LogError($"[StoryManager] Invalid part format at story {Path.GetFileNameWithoutExtension(_path)} line {_currentIndex + 1}. " +
                $"\r\n Expected format: my_part");
            left = right = null;
            return false;
        }

        left = args[0];
        right = args[1].Trim();
        return true;
    }
    private void ChangeScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) == null)
        {
            Debug.LogError($"Scene {sceneName} not found! Make sure it's added to the build settings.");
            return;
        }

        PlayerPrefs.SetString(
            "sceneChangeTransport",
            JsonUtility.ToJson(new SceneChangeTransport
            {
                path = _path,
                currentIndex = _currentIndex + 1,
            })
        );
        SceneManager.LoadScene(sceneName);
    }
    private bool CheckArgs(string methodName, int argCount, string cmd)
    {
        var method = typeof(StoryManager).GetMethod(
            methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (method == null)
        {
            Debug.LogError($"[StoryManager] Method '{methodName}' not found!");
            return true;
        }

        int expectedCount = method.GetParameters().Length;
        if (expectedCount != argCount)
        {
            Debug.LogError($"[StoryManager] Command '{cmd}' requires {expectedCount} arguments, but got {argCount}!");
            return true;
        }

        return false;
    }

    Sprite LoadCharacter(string characterName, string characterStatus)
    {
        return Resources.Load<Sprite>($"Characters/{characterName}/{characterStatus}");
    }
    Sprite LoadBackground(string backgroundName)
    {
        return Resources.Load<Sprite>($"Backgrounds/{backgroundName}");
    }
}

internal class SceneChangeTransport
{
    public string path;
    public int currentIndex;
}